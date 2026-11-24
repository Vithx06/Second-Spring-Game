using System;
public sealed class RegrowingCrop : Crop
{
    public RegrowingCrop(CropDefinition definition, Func<double> random = null) : base(definition, random) { }
    public override bool Harvest()
    {
        if (!CanHarvest()) return false;
        BeginRegrowth();
        return true;
    }
}
