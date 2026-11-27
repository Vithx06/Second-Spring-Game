// Giữ số của trạng thái cũ để bản lưu không bị hiểu nhầm khi thêm Regrowing.
public enum CropState
{
    Stage1 = 0, // Giai đoạn sinh trưởng đầu
    Stage2 = 1, // Giai đoạn sinh trưởng thứ hai
    Ripe = 2, // Đã chín, có thể thu hoạch
    Dead = 3, // Cây đã chết
    Harvested = 4, // Cây đã thu hoạch xong
    Regrowing = 5, // Cây đang tái sinh sau một lần thu hoạch
}
