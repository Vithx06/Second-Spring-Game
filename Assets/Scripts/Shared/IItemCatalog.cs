// Danh mục chỉ giữ cấu hình vật phẩm, không giữ số lượng đang sở hữu.
public interface IItemCatalog
{
    ItemDefinition FindItem(ItemType type);
}
