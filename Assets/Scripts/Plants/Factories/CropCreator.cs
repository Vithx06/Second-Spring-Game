using System;
public abstract class CropCreator
{
    public abstract bool CanCreate(CropDefinition definition);
    public Crop PrepareCrop(CropDefinition definition)
    {
        if (!CanCreate(definition)) throw new ArgumentException("Unsupported crop definition.");
        return CreateCrop(definition);
    }
    public abstract Crop CreateCrop(CropDefinition definition);
}
