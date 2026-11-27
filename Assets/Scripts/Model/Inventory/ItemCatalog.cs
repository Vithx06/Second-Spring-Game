using System;
using System.Collections.Generic;

public sealed class ItemCatalog : IItemCatalog
{
    private readonly Dictionary<ItemType,ItemDefinition> _items=new Dictionary<ItemType,ItemDefinition>();
    public ItemCatalog(IEnumerable<ItemDefinition> items)
    {
        if(items==null)throw new ArgumentNullException(nameof(items));
        foreach(var item in items)Register(item);
    }
    internal void Register(ItemDefinition item)
    {
        if(item==null)throw new ArgumentNullException(nameof(item));
        _items.Add(item.Type,item);
    }
    public ItemDefinition FindItem(ItemType type) => _items.TryGetValue(type,out var item)?item:null;

    public static ItemCategory CategoryOf(ItemType type)
    {
        if(!Enum.IsDefined(typeof(ItemType),type))throw new ArgumentOutOfRangeException(nameof(type));
        if(type<=ItemType.OrangeSeed)return ItemCategory.Seed;
        if(type<=ItemType.Orange)return ItemCategory.CropProduct;
        if(type<=ItemType.Sturgeon)return ItemCategory.AnimalProduct;
        if(type<=ItemType.FishFeed)return ItemCategory.AnimalFeed;
        if(type<=ItemType.GrowthBoostFertilizer)return ItemCategory.Fertilizer;
        return type==ItemType.Manure?ItemCategory.Manure:ItemCategory.Tool;
    }
}
