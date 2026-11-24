using System;
using System.Collections.Generic;

static class Program
{
    static int _passed;
    static void Check(bool value, string name) { if (!value) throw new Exception(name); _passed++; Console.WriteLine("PASS " + name); }
    static CropDefinition Def(HarvestType type=HarvestType.SingleHarvest, int yield=2)
        => new CropDefinition("rice","Rice","seed","product",yield,10,20,5,15,8,harvestType:type,regrowthDuration:6);
    static void Ripe(Crop crop) { crop.Water(); crop.UpdateGrowth(10); crop.Water(); crop.UpdateGrowth(20); }
    static void Throws(Action action, string name) { try { action(); } catch (ArgumentException) { Check(true,name); return; } throw new Exception(name); }
    static void Main()
    {
        var plot=new FarmPlot(3);
        Check(plot.CanTill() && plot.Status==FarmPlotStatus.Untilled,"initial soil");
        Check(plot.StartTilling() && !plot.StartTilling(),"reject repeat till");
        plot.UpdateTilling(1);Check(plot.TillingTimeRemaining==2,"till timer unchanged by rejected click");
        plot.UpdateTilling(99);Check(plot.CanPlant() && plot.TillingTimeRemaining==0,"till completion clamps");
        Check(!plot.Plant(null),"reject null crop");
        var crop=new SingleHarvestCrop(Def(),()=>0);
        Check(plot.Plant(crop) && !plot.CanTill() && !plot.Plant(crop),"plant occupies soil");
        var secondPlot = new FarmPlot(1); secondPlot.StartTilling(); secondPlot.UpdateTilling(1);
        Check(!secondPlot.Plant(crop),"one crop belongs to only one plot");
        crop.UpdateGrowth(5);Check(crop.HasDroughtPenalty && crop.GrowthTimeRemaining==10,"wait for watering and apply penalty");
        crop.Water();Check(crop.WaterDeprivationElapsedTime==0 && crop.HasDroughtPenalty,"watering resets drought timer only");
        crop.UpdateGrowth(10);Check(crop.State==CropState.Stage2 && crop.WateringCount==0,"stage transition needs new watering");
        crop.Water();crop.UpdateGrowth(20);Check(crop.CanHarvest() && !crop.Water() && !crop.CanFertilize(),"ripe restrictions");
        Check(crop.CalculateYield()==1,"half rounding down");
        Check(crop.Harvest() && !crop.Harvest(),"single harvest once");
        plot.RemoveCrop(ResidueType.CropRemains);Check(plot.CurrentCrop==null && plot.ResidueType==ResidueType.CropRemains,"harvest residue");
        plot.StartTilling();plot.UpdateTilling(3);Check(plot.ResidueType==ResidueType.None,"retill removes residue");
        crop=new SingleHarvestCrop(Def());for(int i=0;i<5;i++)crop.Water();Check(crop.State==CropState.Dead,"fifth watering kills");
        crop=new SingleHarvestCrop(Def());crop.UpdateGrowth(15);Check(crop.State==CropState.Dead,"drought death");
        crop=new SingleHarvestCrop(Def());Ripe(crop);crop.UpdateGrowth(8);Check(crop.State==CropState.Dead,"ripe rot");
        crop=new SingleHarvestCrop(Def());crop.Water();crop.UpdateGrowth(25);Check(crop.State==CropState.Dead,"large tick handles next-stage drought");
        crop=new SingleHarvestCrop(Def());crop.Water();crop.UpdateGrowth(10);crop.Water();crop.UpdateGrowth(28);Check(crop.State==CropState.Dead,"large tick handles rot overflow");
        crop=new SingleHarvestCrop(Def());Check(crop.Fertilize(new YieldBoostStrategy()) && !crop.Fertilize(new GrowthBoostStrategy()),"one fertilizer per lifetime");
        Check(crop.CalculateYield()==3,"yield boost fifty percent");
        crop.UpdateGrowth(5);Check(crop.CalculateYield()>=2 && crop.CalculateYield()<=3,"bonus and drought use base yield");
        crop=new SingleHarvestCrop(Def());crop.Fertilize(new GrowthBoostStrategy());Check(crop.GrowthTimeRemaining==7.5f,"growth fertilizer current stage");
        crop.UpdateGrowth(1);Check(crop.GrowthTimeRemaining==7.5f,"fertilizer does not bypass water");
        crop.Water();crop.UpdateGrowth(7.5f);Check(crop.GrowthTimeRemaining==15,"future stage acceleration");
        var repeat=new RegrowingCrop(Def(HarvestType.Regrowing));Ripe(repeat);repeat.Harvest();Check(repeat.State==CropState.Stage2 && repeat.WateringCount==0 && repeat.GrowthTimeRemaining==6,"regrowth resets stage water");
        var boosted=new RegrowingCrop(Def(HarvestType.Regrowing));boosted.Fertilize(new GrowthBoostStrategy());boosted.Water();boosted.UpdateGrowth(7.5f);boosted.Water();boosted.UpdateGrowth(15);boosted.Harvest();Check(boosted.GrowthTimeRemaining==4.5f && !boosted.CanFertilize(),"regrowth retains lifetime fertilizer");
        var up=new SingleHarvestCrop(Def(),()=>1);up.UpdateGrowth(5);Check(up.CalculateYield()==2,"half rounding up");
        Check(new SingleHarvestCrop(Def(yield:3)).CalculateYield()==3,"integer yield unchanged");
        Check(new SingleHarvestCropCreator().PrepareCrop(Def()) is SingleHarvestCrop,"single factory");
        Check(new RegrowingCropCreator().PrepareCrop(Def(HarvestType.Regrowing)) is RegrowingCrop,"regrowing factory");
        Throws(()=>new FarmPlot(-1),"invalid till duration");
        Throws(()=>new FarmPlot(float.NaN),"NaN duration");
        Throws(()=>plot.UpdateTilling(-1),"negative delta");
        Throws(()=>crop.UpdateGrowth(float.PositiveInfinity),"infinite delta");
        Throws(()=>new SingleHarvestCropCreator().PrepareCrop(Def(HarvestType.Regrowing)),"factory rejects mismatch");
        ServiceTests();
        Console.WriteLine("TOTAL PASS: "+_passed);
    }
    static void ServiceTests()
    {
        var inv=new PlantDemoInventory();
        var seed=new ItemDefinition("seed","Seed");var product=new ItemDefinition("product","Rice");var fert=new ItemDefinition("fert","Fertilizer");
        inv.Register(seed);inv.Register(product);inv.Register(fert);
        inv.TryAdd(seed,2);inv.TryAdd(fert,2);
        var plot=new FarmPlot(1);var other=new FarmPlot(1);
        var service=new FarmingService(inv,new[]{plot},inv,new CropCreator[]{new SingleHarvestCropCreator(),new RegrowingCropCreator()},new Dictionary<string,IFertilizerStrategy>{{"fert",new YieldBoostStrategy()}});
        Check(!service.Till(other),"unregistered plot rejected");
        Check(!service.Plant(plot,Def()) && inv.GetQuantity("seed")==2,"invalid soil keeps seed");
        service.Till(plot);service.OnTimeElapsed(1);Check(service.Plant(plot,Def()) && inv.GetQuantity("seed")==1,"plant transaction consumes one seed");
        Check(!service.Plant(plot,Def()) && inv.GetQuantity("seed")==1,"occupied soil keeps seed");
        Check(service.Fertilize(plot,"fert") && inv.GetQuantity("fert")==1,"fertilizer transaction");
        Check(!service.Fertilize(plot,"fert") && inv.GetQuantity("fert")==1,"repeat fertilizer keeps item");
        service.Water(plot);service.OnTimeElapsed(10);service.Water(plot);service.OnTimeElapsed(20);
        inv.Capacity=0;Check(!service.Harvest(plot) && plot.CurrentCrop.CanHarvest() && inv.GetQuantity("product")==0,"full inventory preserves ripe crop");
        inv.Capacity=100;Check(service.Harvest(plot) && inv.GetQuantity("product")==3 && plot.Status==FarmPlotStatus.HaveResidue,"harvest adds yield and residue");
        Check(!service.Harvest(plot) && inv.GetQuantity("product")==3,"no duplicate harvest");
        service.Till(plot);service.OnTimeElapsed(1);service.Plant(plot,Def());service.OnTimeElapsed(15);
        Check(plot.CurrentCrop==null && plot.ResidueType==ResidueType.RottenCrop,"service cleans dead crop");
        service.Till(plot);service.OnTimeElapsed(1);Check(!service.Plant(plot,Def()),"no seed rejects planting");
        inv.Capacity=1;inv.TryRemove("fert",1);Check(inv.TryAdd(product,47) && !inv.TryAdd(product,1),"stack limit affects capacity");
        Throws(()=>new FarmingService(inv,new[]{plot,plot},inv,new CropCreator[0],new Dictionary<string,IFertilizerStrategy>()),"duplicate plots rejected");
    }
}
