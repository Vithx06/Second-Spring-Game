using System;
using System.Collections.Generic;

internal static class UmlV2Checks
{
    private sealed class Recorder : IFarmEventListener
    {
        internal readonly List<FarmEvent> Events=new List<FarmEvent>();
        public void OnFarmEvent(FarmEvent evt) { Events.Add(evt); }
    }
    private sealed class NullCreator : ICropCreator
    {
        public bool CanCreate(CropDefinition definition)=>true;
        public Crop CreateCrop(CropDefinition definition)=>null;
    }
    internal static int Run()
    {
        int checks=0;
        void Check(bool ok,string name) { if(!ok)throw new Exception(name);checks++;Console.WriteLine("PASS UML v2: "+name); }
        var definitions=CropGameRules.CreateDefinitions(86400);
        var types=new HashSet<CropType>();
        foreach(var definition in definitions)types.Add(definition.Type);
        Check(types.Count==8 && Enum.GetValues(typeof(CropType)).Length==8,"eight distinct typed crops");
        Check(definitions[3].Type==CropType.Strawberry && definitions[6].Type==CropType.Rose,"UI ordering independent of enum values");
        Check(definitions[0].SeedItem==ItemType.RiceSeed && definitions[0].ProductItem==ItemType.Rice,"typed seed and produce");
        Check(definitions[0].SeedItemId=="rice.seed","legacy ID conversion only at boundary");
        var def=new CropDefinition(CropType.Rice,"Lúa",ItemType.RiceSeed,ItemType.Rice,3,10,20,5,15,8);
        var inv=new PlantDemoInventory();var seed=new ItemDefinition("rice.seed","Giống lúa");var product=new ItemDefinition("rice.product","Lúa");
        var fertilizerItem=new ItemDefinition("fertilizer.yield","Phân năng suất");
        inv.Register(seed);inv.Register(product);inv.Register(fertilizerItem);inv.TryAdd(seed,3);inv.TryAdd(fertilizerItem,1);
        var plot=new FarmPlot(7,2);var hub=new FarmEventHub();var recorder=new Recorder();hub.Subscribe(recorder);hub.Subscribe(recorder);
        var fertilizer=new FertilizerDefinition(ItemType.YieldBoostFertilizer,FertilizerType.YieldBoost,.5f,1f,2);
        var service=new FarmingService(inv,new[]{plot},inv,new ICropCreator[]{new SingleHarvestCropCreator()},new[]{fertilizer},hub);
        Check(plot.Id==7,"explicit plot identity");
        Check(!service.Plant(plot,def) && recorder.Events.Count==0,"failed planting emits no event");
        service.Till(plot);service.OnTimeElapsed(2);service.OnDayChanged(3);service.Plant(plot,def);
        Check(recorder.Events.Count==1 && recorder.Events[0].Kind==FarmEventKind.SeedPlanted && recorder.Events[0].Day==3,"one typed planting event with game day");
        Check(inv.GetQuantity(ItemType.RiceSeed)==2,"typed seed consumption");
        Check(!service.Fertilize(plot,ItemType.YieldBoostFertilizer) && inv.GetQuantity(ItemType.YieldBoostFertilizer)==1 && plot.CurrentCrop.CanFertilize(),"required fertilizer quantity checked atomically");
        inv.TryAdd(fertilizerItem,1);Check(service.Fertilize(plot,ItemType.YieldBoostFertilizer) && inv.GetQuantity(ItemType.YieldBoostFertilizer)==0,"definition controls fertilizer amount");
        service.Water(plot);service.OnTimeElapsed(10);service.Water(plot);service.OnTimeElapsed(20);
        Check(recorder.Events.Count==3 && recorder.Events[1].Metric==StatType.CropsWatered,"watering events do not double subscribe");
        inv.SimulateFull=true;Check(!service.Harvest(plot) && recorder.Events.Count==3 && plot.CurrentCrop.CanHarvest(),"full inventory emits no harvest event");
        inv.SimulateFull=false;service.Harvest(plot);
        Check(recorder.Events.Count==4 && recorder.Events[3].Kind==FarmEventKind.CropHarvested && recorder.Events[3].Quantity==inv.GetQuantity(ItemType.Rice),"harvest event records actual randomized yield");
        Check(!service.Harvest(plot) && recorder.Events.Count==4,"repeat harvest cannot emit or reward again");
        var repeatDef=new CropDefinition(CropType.Strawberry,"Dâu",ItemType.StrawberrySeed,ItemType.Strawberry,3,10,20,5,15,8,harvestType:HarvestType.Regrowing,regrowthDuration:6);
        var repeat=new RegrowingCrop(repeatDef);repeat.Water();repeat.UpdateGrowth(10);repeat.Water();repeat.UpdateGrowth(20);repeat.Harvest();
        Check(repeat.State==CropState.Regrowing && repeat.CanWater && !repeat.CanHarvest(),"explicit regrowing state needs water");
        repeat.Water();repeat.UpdateGrowth(6);Check(repeat.State==CropState.Ripe,"regrowth completes without repeating stage one");
        var badPlot=new FarmPlot(8,1);var badService=new FarmingService(inv,new[]{badPlot},inv,new ICropCreator[]{new NullCreator()},new FertilizerDefinition[0]);
        badService.Till(badPlot);badService.OnTimeElapsed(1);int before=inv.GetQuantity(ItemType.RiceSeed);
        Check(!badService.Plant(badPlot,def) && inv.GetQuantity(ItemType.RiceSeed)==before,"bad creator cannot consume seed");
        bool rejected=false;try { new FarmingService(inv,new[]{new FarmPlot(9,1),new FarmPlot(9,1)},inv,new ICropCreator[0],new FertilizerDefinition[0]); }catch(ArgumentException){rejected=true;}
        Check(rejected,"duplicate explicit plot IDs rejected");
        rejected=false;try { new FertilizerDefinition(ItemType.RiceSeed,FertilizerType.YieldBoost,.5f,1); }catch(ArgumentException){rejected=true;}
        Check(rejected,"seed cannot be used as fertilizer definition");
        hub.Unsubscribe(recorder);int prior=recorder.Events.Count;hub.Publish(new FarmEvent(FarmEventKind.SeedPlanted,StatType.SeedsPlanted,1,3));
        Check(recorder.Events.Count==prior,"event listener can unsubscribe");
        return checks;
    }
}
