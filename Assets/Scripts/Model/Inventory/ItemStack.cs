using System;

// Một dòng kết quả đọc kho; sửa Quantity ở View không thể sửa dữ liệu gốc trong Inventory.
public sealed class ItemStack
{
    public ItemDefinition Item { get; }
    public int Quantity { get; }
    public ItemStack(ItemDefinition item,int quantity)
    {
        if(item==null || quantity<0)throw new ArgumentException("Invalid stack.");
        Item=item;Quantity=quantity;
    }
}
