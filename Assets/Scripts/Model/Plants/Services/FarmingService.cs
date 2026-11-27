using System;
using System.Collections.Generic;

// Điều phối trồng trọt: ô đất giữ trạng thái, kho giữ vật phẩm, service nối hai bên.
public sealed class FarmingService : IGameTimeListener
{
    private readonly IInventory _inventory;
    private readonly List<FarmPlot> _plots;
    private readonly List<ICropCreator> _creators;
    private readonly Dictionary<ItemType,FertilizerDefinition> _fertilizers;
    private readonly IItemCatalog _items;
    private readonly FarmEventHub _events;
    private int _day=1;
    public FarmEventHub Events => _events;

    public FarmingService(IInventory inventory,IEnumerable<FarmPlot> plots,IItemCatalog items,
        IEnumerable<ICropCreator> creators,IEnumerable<FertilizerDefinition> fertilizers,FarmEventHub events=null)
    {
        _inventory=inventory??throw new ArgumentNullException(nameof(inventory));
        _items=items??throw new ArgumentNullException(nameof(items));
        _plots=new List<FarmPlot>(plots??throw new ArgumentNullException(nameof(plots)));
        _creators=new List<ICropCreator>(creators??throw new ArgumentNullException(nameof(creators)));
        _events=events??new FarmEventHub();
        if(_plots.Exists(p=>p==null) || new HashSet<FarmPlot>(_plots).Count!=_plots.Count || _creators.Exists(c=>c==null))
            throw new ArgumentException("Invalid registered farming objects.");
        var ids=new HashSet<int>();
        foreach(var plot in _plots)
            // ID 0 dành cho prototype cũ; các ô được đánh số phải có ID riêng.
            if(plot.Id>0 && !ids.Add(plot.Id))throw new ArgumentException("Duplicate plot ID.");
        _fertilizers=new Dictionary<ItemType,FertilizerDefinition>();
        foreach(var fertilizer in fertilizers??throw new ArgumentNullException(nameof(fertilizers)))
        {
            if(fertilizer==null)throw new ArgumentException("Null fertilizer definition.");
            _fertilizers.Add(fertilizer.Item,fertilizer);
        }
    }

    private bool Owns(FarmPlot plot) => plot!=null && _plots.Contains(plot);
    public bool Till(FarmPlot plot) => Owns(plot) && plot.StartTilling();

    public bool Plant(FarmPlot plot,CropDefinition definition)
    {
        if(!Owns(plot) || !plot.CanPlant() || definition==null)return false;
        ItemDefinition seed=_items.FindItem(definition.SeedItem);
        if(seed==null || _items.FindItem(definition.ProductItem)==null || _inventory.GetQuantity(definition.SeedItem)<1)return false;
        ICropCreator creator=_creators.Find(c=>c.CanCreate(definition));
        if(creator==null)return false;
        Crop crop=creator.CreateCrop(definition);
        // Kiểm tra cây trước khi trừ hạt. Creator lỗi không được làm mất hạt giống.
        if(crop==null || crop.Owner!=null || crop.State!=CropState.Stage1 || crop.Definition!=definition)return false;
        if(!_inventory.TryRemove(definition.SeedItem,1))return false;
        if(!plot.Plant(crop))
        {
            if(!_inventory.TryAdd(definition.SeedItem,1))throw new InvalidOperationException("Seed rollback failed.");
            return false;
        }
        Publish(FarmEventKind.SeedPlanted,StatType.SeedsPlanted,1);
        return true;
    }

    public bool Water(FarmPlot plot)
    {
        if(!Owns(plot) || plot.CurrentCrop==null)return false;
        bool result=plot.CurrentCrop.Water();
        RemoveDeadCrop(plot);
        if(result)Publish(FarmEventKind.CropWatered,StatType.CropsWatered,1);
        return result;
    }

    public bool Fertilize(FarmPlot plot,ItemType fertilizerItem)
    {
        if(!Owns(plot) || plot.CurrentCrop==null || !plot.CurrentCrop.CanFertilize()
            || !_fertilizers.TryGetValue(fertilizerItem,out var fertilizer))return false;
        ItemDefinition item=_items.FindItem(fertilizerItem);
        if(item==null || !_inventory.TryRemove(fertilizerItem,fertilizer.RequiredQuantity))return false;
        if(plot.CurrentCrop.Fertilize(fertilizer))return true;
        // Nếu bón không thành công thì hoàn lại đúng số lượng phân đã trừ.
        if(!_inventory.TryAdd(fertilizerItem,fertilizer.RequiredQuantity))throw new InvalidOperationException("Fertilizer rollback failed.");
        return false;
    }

    public bool Harvest(FarmPlot plot)
    {
        if(!Owns(plot) || plot.CurrentCrop==null || !plot.CurrentCrop.CanHarvest())return false;
        Crop crop=plot.CurrentCrop;
        ItemDefinition item=_items.FindItem(crop.Definition.ProductItem);
        if(item==null)return false;
        // Chỉ lấy kết quả làm tròn một lần; kiểm tra kho và cộng kho dùng cùng con số.
        int amount=crop.CalculateYield();
        if(!_inventory.CanAdd(crop.Definition.ProductItem,amount) || !_inventory.TryAdd(crop.Definition.ProductItem,amount))return false;
        if(!crop.Harvest())
        {
            if(!_inventory.TryRemove(crop.Definition.ProductItem,amount))throw new InvalidOperationException("Harvest rollback failed.");
            return false;
        }
        if(crop.State==CropState.Harvested)plot.RemoveCrop(ResidueType.CropRemains);
        // Chỉ phát sự kiện khi thu hoạch đã hoàn tất; kho đầy không tạo thành tích giả.
        Publish(FarmEventKind.CropHarvested,StatType.CropsHarvested,amount);
        return true;
    }

    public void OnTimeElapsed(float deltaTime)
    {
        PlantTime.ValidateDelta(deltaTime);
        foreach(FarmPlot plot in _plots)
        {
            plot.UpdateTilling(deltaTime);
            if(plot.CurrentCrop==null)continue;
            plot.CurrentCrop.UpdateGrowth(deltaTime);
            RemoveDeadCrop(plot);
        }
    }
    private static void RemoveDeadCrop(FarmPlot plot)
    {
        if(plot.CurrentCrop!=null && plot.CurrentCrop.State==CropState.Dead)plot.RemoveCrop(ResidueType.RottenCrop);
    }
    private void Publish(FarmEventKind kind,StatType metric,int quantity) => _events.Publish(new FarmEvent(kind,metric,quantity,_day));
    public void OnHourChanged(int day,int hour) { }
    public void OnDayChanged(int day)
    {
        if(day<1)throw new ArgumentOutOfRangeException(nameof(day));
        _day=day;
    }
}
