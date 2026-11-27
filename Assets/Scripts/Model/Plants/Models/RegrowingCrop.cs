using System;
public sealed class RegrowingCrop : Crop
{
    public RegrowingCrop(CropDefinition definition, Func<double> random = null) : base(definition, random) { }
    // Cây thu hoạch nhiều lần: giữ cây trên đất và bắt đầu thời gian tái sinh.
    public override bool Harvest()
    {
        if (!CanHarvest()) return false;
        BeginRegrowth();
        return true;
    }
}
