// Hợp đồng tạo cây. FarmingService chọn creator phù hợp với kiểu thu hoạch.
public interface ICropCreator
{
    bool CanCreate(CropDefinition definition);
    Crop CreateCrop(CropDefinition definition);
}
