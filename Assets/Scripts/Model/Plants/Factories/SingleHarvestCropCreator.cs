// Tạo cây thu một lần khi cấu hình HarvestType phù hợp.
public sealed class SingleHarvestCropCreator : ICropCreator
{
    public bool CanCreate(CropDefinition definition) => definition != null && definition.HarvestType == HarvestType.SingleHarvest;
    public Crop CreateCrop(CropDefinition definition)
    {
        if (!CanCreate(definition)) throw new System.ArgumentException("Unsupported harvest type.");
        return new SingleHarvestCrop(definition);
    }
}
