using System;
public sealed class SingleHarvestCrop : Crop
{
    public SingleHarvestCrop(CropDefinition definition, Func<double> random = null) : base(definition, random) { }
    // Cây thu hoạch một lần: thu xong chuyển Harvested để ô đất dọn cây.
    public override bool Harvest()
    {
        if (!CanHarvest()) return false;
        MarkHarvested();
        return true;
    }
}
