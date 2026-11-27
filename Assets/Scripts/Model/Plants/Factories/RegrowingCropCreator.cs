// Tạo cây tái sinh khi cấu hình HarvestType phù hợp.
public sealed class RegrowingCropCreator : ICropCreator
{
    public bool CanCreate(CropDefinition definition) => definition != null && definition.HarvestType == HarvestType.Regrowing;
    public Crop CreateCrop(CropDefinition definition)
    {
        if (!CanCreate(definition)) throw new System.ArgumentException("Unsupported harvest type.");
        return new RegrowingCrop(definition);
    }
}
