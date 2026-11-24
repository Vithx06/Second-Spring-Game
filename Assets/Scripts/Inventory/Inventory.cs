using System;
using System.Collections.Generic;

// Shared inventory data used by farming and trading.
public class Inventory : IInventory, IItemCatalog
{
    private readonly Dictionary<string, ItemDefinition> _items = new Dictionary<string, ItemDefinition>();
    private readonly Dictionary<string, int> _quantities = new Dictionary<string, int>();
    public int Capacity { get; set; } = 100;
    public void Register(ItemDefinition item) { _items.Add(item.Id, item); }
    public ItemDefinition FindItem(string itemId) => itemId != null && _items.TryGetValue(itemId, out var item) ? item : null;
    public int GetQuantity(string itemId) => itemId != null && _quantities.TryGetValue(itemId, out int amount) ? amount : 0;
    public IReadOnlyList<ItemStack> GetItems()
    {
        var result = new List<ItemStack>();
        foreach (var pair in _quantities)
            if (pair.Value > 0) result.Add(new ItemStack(_items[pair.Key], pair.Value));
        return result.AsReadOnly();
    }
    public bool CanAdd(ItemDefinition item, int quantity)
    {
        if (item == null || quantity <= 0 || FindItem(item.Id) != item) return false;
        long used = 0;
        foreach (var pair in _quantities)
        {
            long amount = pair.Value;
            if (pair.Key == item.Id) amount += quantity;
            used += (amount + _items[pair.Key].MaxStackSize - 1) / _items[pair.Key].MaxStackSize;
        }
        if (!_quantities.ContainsKey(item.Id)) used += ((long)quantity + item.MaxStackSize - 1) / item.MaxStackSize;
        return used <= Capacity && (long)GetQuantity(item.Id) + quantity <= int.MaxValue;
    }
    public bool TryAdd(ItemDefinition item, int quantity)
    {
        if (!CanAdd(item, quantity)) return false;
        _quantities[item.Id] = GetQuantity(item.Id) + quantity;
        return true;
    }
    public bool TryRemove(string itemId, int quantity)
    {
        if (quantity <= 0 || GetQuantity(itemId) < quantity) return false;
        int remaining = _quantities[itemId] - quantity;
        if (remaining == 0) _quantities.Remove(itemId);
        else _quantities[itemId] = remaining;
        return true;
    }
}
