using System;
using System.Collections.Generic;

public sealed class FarmingService : IGameTimeListener
{
    private readonly IInventory _inventory;
    private readonly List<FarmPlot> _plots;
    private readonly List<CropCreator> _creators;
    private readonly Dictionary<string, IFertilizerStrategy> _fertilizers;
    private readonly IItemCatalog _items;

    public FarmingService(IInventory inventory, IEnumerable<FarmPlot> plots, IItemCatalog items,
        IEnumerable<CropCreator> creators, IDictionary<string, IFertilizerStrategy> fertilizers)
    {
        _inventory=inventory ?? throw new ArgumentNullException(nameof(inventory));
        _items=items ?? throw new ArgumentNullException(nameof(items));
        _plots=new List<FarmPlot>(plots ?? throw new ArgumentNullException(nameof(plots)));
        _creators=new List<CropCreator>(creators ?? throw new ArgumentNullException(nameof(creators)));
        _fertilizers=new Dictionary<string, IFertilizerStrategy>(fertilizers ?? throw new ArgumentNullException(nameof(fertilizers)));
        if (_plots.Exists(p => p == null) || new HashSet<FarmPlot>(_plots).Count != _plots.Count
            || _creators.Exists(c => c == null)) throw new ArgumentException("Invalid registered objects.");
    }
    private bool Owns(FarmPlot plot) => plot != null && _plots.Contains(plot);
    public bool Till(FarmPlot plot) => Owns(plot) && plot.StartTilling();
    public bool Plant(FarmPlot plot, CropDefinition definition)
    {
        if (!Owns(plot) || !plot.CanPlant() || definition == null) return false;
        ItemDefinition seed = _items.FindItem(definition.SeedItemId);
        if (seed == null || _items.FindItem(definition.ProductItemId) == null
            || _inventory.GetQuantity(seed.Id) < 1) return false;
        CropCreator creator = _creators.Find(c => c.CanCreate(definition));
        if (creator == null) return false;
        Crop crop = creator.PrepareCrop(definition);
        if (!_inventory.TryRemove(seed.Id, 1)) return false;
        if (plot.Plant(crop)) return true;
        if (!_inventory.TryAdd(seed, 1)) throw new InvalidOperationException("Seed rollback failed.");
        return false;
    }
    public bool Water(FarmPlot plot) => Owns(plot) && plot.CurrentCrop != null && WaterCrop(plot);
    private bool WaterCrop(FarmPlot plot)
    {
        bool result = plot.CurrentCrop.Water();
        RemoveDeadCrop(plot);
        return result;
    }
    public bool Fertilize(FarmPlot plot, string fertilizerItemId)
    {
        if (!Owns(plot) || plot.CurrentCrop == null || !plot.CurrentCrop.CanFertilize()
            || fertilizerItemId == null || !_fertilizers.TryGetValue(fertilizerItemId, out var strategy)) return false;
        ItemDefinition item = _items.FindItem(fertilizerItemId);
        if (item == null || !_inventory.TryRemove(item.Id, 1)) return false;
        if (plot.CurrentCrop.Fertilize(strategy)) return true;
        if (!_inventory.TryAdd(item, 1)) throw new InvalidOperationException("Fertilizer rollback failed.");
        return false;
    }
    public bool Harvest(FarmPlot plot)
    {
        if (!Owns(plot) || plot.CurrentCrop == null || !plot.CurrentCrop.CanHarvest()) return false;
        Crop crop = plot.CurrentCrop;
        ItemDefinition item = _items.FindItem(crop.Definition.ProductItemId);
        if (item == null) return false;
        // Sample random rounding once; use the same amount for capacity, commit and rollback.
        int amount = crop.CalculateYield();
        if (!_inventory.CanAdd(item, amount) || !_inventory.TryAdd(item, amount)) return false;
        if (!crop.Harvest())
        {
            if (!_inventory.TryRemove(item.Id, amount)) throw new InvalidOperationException("Harvest rollback failed.");
            return false;
        }
        if (crop.State == CropState.Harvested) plot.RemoveCrop(ResidueType.CropRemains);
        return true;
    }
    public void OnTimeElapsed(float deltaTime)
    {
        PlantTime.ValidateDelta(deltaTime);
        foreach (FarmPlot plot in _plots)
        {
            plot.UpdateTilling(deltaTime);
            if (plot.CurrentCrop == null) continue;
            plot.CurrentCrop.UpdateGrowth(deltaTime);
            RemoveDeadCrop(plot);
        }
    }
    private static void RemoveDeadCrop(FarmPlot plot)
    {
        if (plot.CurrentCrop != null && plot.CurrentCrop.State == CropState.Dead)
            plot.RemoveCrop(ResidueType.RottenCrop);
    }
    public void OnHourChanged(int day, int hour) { }
    public void OnDayChanged(int day) { }
}
