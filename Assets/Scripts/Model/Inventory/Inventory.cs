using System;
using System.Collections.Generic;

public class Inventory : IInventory,IItemCatalog
{
    private readonly Dictionary<ItemType,int> _items=new Dictionary<ItemType,int>();
    private readonly IItemCatalog _catalog;
    public int Capacity { get; }
    public int UsedSlots => _items.Count;

    public Inventory(IItemCatalog catalog,int capacity=100)
    {
        _catalog=catalog??throw new ArgumentNullException(nameof(catalog));
        if(capacity<0)throw new ArgumentOutOfRangeException(nameof(capacity));
        Capacity=capacity;
    }

    // Constructor/Register chỉ phục vụ các prototype cũ đang khai báo danh mục từng bước.
    public Inventory(int capacity=100):this(new ItemCatalog(new ItemDefinition[0]),capacity) { }
    public void Register(ItemDefinition item)
    {
        if(!(_catalog is ItemCatalog catalog))throw new InvalidOperationException("Catalog is not mutable.");
        catalog.Register(item);
    }
    public ItemDefinition FindItem(ItemType type) => _catalog.FindItem(type);
    public int GetQuantity(ItemType type) => _items.TryGetValue(type,out int quantity)?quantity:0;
    public IReadOnlyList<ItemStack> GetItems()
    {
        var result=new List<ItemStack>();
        foreach(var pair in _items)result.Add(new ItemStack(_catalog.FindItem(pair.Key),pair.Value));
        return result.AsReadOnly();
    }
    public virtual bool CanAdd(ItemType type,int quantity)
    {
        if(quantity<=0 || _catalog.FindItem(type)==null)return false;
        if((long)GetQuantity(type)+quantity>int.MaxValue)return false;
        // Theo luật kho: một ô giữ một loại; cộng thêm cùng loại không cần ô mới.
        return _items.ContainsKey(type) || _items.Count<Capacity;
    }
    public bool TryAdd(ItemType type,int quantity)
    {
        if(!CanAdd(type,quantity))return false;
        _items[type]=GetQuantity(type)+quantity;
        return true;
    }
    public bool TryRemove(ItemType type,int quantity)
    {
        if(quantity<=0 || GetQuantity(type)<quantity)return false;
        int remaining=_items[type]-quantity;
        if(remaining==0)_items.Remove(type);else _items[type]=remaining;
        return true;
    }
}
