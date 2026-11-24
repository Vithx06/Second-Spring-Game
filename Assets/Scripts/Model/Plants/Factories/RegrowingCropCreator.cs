public sealed class RegrowingCropCreator : CropCreator
{
    public override bool CanCreate(CropDefinition definition) => definition != null && definition.HarvestType == HarvestType.Regrowing;
    public override Crop CreateCrop(CropDefinition definition)
    {
        if (!CanCreate(definition)) throw new System.ArgumentException("Unsupported harvest type.");
        return new RegrowingCrop(definition);
    }
}
