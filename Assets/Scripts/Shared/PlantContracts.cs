using System.Collections.Generic;

public interface IGameTimeListener
{
    void OnTimeElapsed(float deltaTime);
    void OnHourChanged(int day, int hour);
    void OnDayChanged(int day);
}

public interface IInventory
{
    int GetQuantity(string itemId);
    IReadOnlyList<ItemStack> GetItems();
    bool CanAdd(ItemDefinition item, int quantity);
    bool TryAdd(ItemDefinition item, int quantity);
    bool TryRemove(string itemId, int quantity);
}

public interface IItemCatalog { ItemDefinition FindItem(string itemId); }

public sealed class ItemDefinition
{
    public string Id { get; }
    public string Name { get; }
    public int MaxStackSize { get; }
    public ItemDefinition(string id, string name, int maxStackSize = 50)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name) || maxStackSize < 1)
            throw new System.ArgumentException("Invalid item definition.");
        Id=id; Name=name; MaxStackSize=maxStackSize;
    }
}

public sealed class ItemStack
{
    public ItemDefinition Item { get; }
    public int Quantity { get; }
    public ItemStack(ItemDefinition item, int quantity)
    {
        if (item == null || quantity < 0) throw new System.ArgumentException("Invalid stack.");
        Item=item; Quantity=quantity;
    }
}
