// Chỉ bản chơi thử có công tắc giả lập kho đầy để kiểm tra thất bại giao dịch.
public sealed class PlantDemoInventory : Inventory
{
    public bool SimulateFull { get; set; }
    public override bool CanAdd(ItemType type,int quantity) => !SimulateFull && base.CanAdd(type,quantity);
}
