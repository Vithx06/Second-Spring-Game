using System.Collections.Generic;

// Hợp đồng kho chung: mọi nghiệp vụ dùng enum thay vì tự ghép mã vật phẩm.
public interface IInventory
{
    int GetQuantity(ItemType type);
    IReadOnlyList<ItemStack> GetItems();
    bool CanAdd(ItemType type,int quantity);
    bool TryAdd(ItemType type,int quantity);
    bool TryRemove(ItemType type,int quantity);
}
