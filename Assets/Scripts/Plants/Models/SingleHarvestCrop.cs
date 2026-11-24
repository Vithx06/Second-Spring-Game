using System;
public sealed class SingleHarvestCrop : Crop
{
    public SingleHarvestCrop(CropDefinition definition, Func<double> random = null) : base(definition, random) { }
    public override bool Harvest()
    {
        if (!CanHarvest()) return false;
        MarkHarvested();
        return true;
    }
}
