public sealed class SingleHarvestCropCreator : CropCreator
{
    public override bool CanCreate(CropDefinition definition) => definition != null && definition.HarvestType == HarvestType.SingleHarvest;
    public override Crop CreateCrop(CropDefinition definition)
    {
        if (!CanCreate(definition)) throw new System.ArgumentException("Unsupported harvest type.");
        return new SingleHarvestCrop(definition);
    }
}
