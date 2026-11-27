using System;

// Tên cũ chỉ được giữ ở biên giao diện/lưu game để các prototype đang có vẫn mở được.
public static class CropIdentifiers
{
    public static string GetId(CropType type)
    {
        switch(type)
        {
            case CropType.Rice: return "rice";
            case CropType.Corn: return "corn";
            case CropType.Sugarcane: return "sugarcane";
            case CropType.Rose: return "rose";
            case CropType.Orchid: return "orchid";
            case CropType.Strawberry: return "strawberry";
            case CropType.Apple: return "apple";
            case CropType.Orange: return "orange";
            default: throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}
