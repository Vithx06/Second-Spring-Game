using System;

public sealed class CropDefinition
{
    // Enum xác định loại cây; Name chỉ dùng để hiển thị tiếng Việt.
    public CropType Type { get; }
    public ItemType SeedItem { get; }
    public ItemType ProductItem { get; }

    // Cầu nối cho View và bản lưu cũ; logic mới dùng Type, SeedItem và ProductItem.
    public string Id => CropIdentifiers.GetId(Type);
    public string Name { get; }
    public string SeedItemId => ItemIdentifiers.GetId(SeedItem);
    public string ProductItemId => ItemIdentifiers.GetId(ProductItem);
    public int BaseYield { get; }
    public float Stage1Duration { get; }
    public float Stage2Duration { get; }
    public float DroughtPenaltyThreshold { get; }
    public float DroughtDeathThreshold { get; }
    public float RotThreshold { get; }
    public CropCategory Category { get; }
    public HarvestType HarvestType { get; }
    public float RegrowthDuration { get; }

    // Các khoảng thời gian đều tính bằng giây trong game, không phải giây thực.
    public CropDefinition(CropType type, string name, ItemType seedItem, ItemType productItem,
        int baseYield, float stage1Duration, float stage2Duration,
        float droughtPenaltyThreshold, float droughtDeathThreshold, float rotThreshold,
        CropCategory category = CropCategory.FieldCrop,
        HarvestType harvestType = HarvestType.SingleHarvest, float regrowthDuration = 1f)
    {
        if (!Enum.IsDefined(typeof(CropType),type) || string.IsNullOrWhiteSpace(name)
            || !Enum.IsDefined(typeof(ItemType),seedItem) || !Enum.IsDefined(typeof(ItemType),productItem))
            throw new ArgumentException("Crop identifiers and name are required.");
        if (baseYield < 1) throw new ArgumentOutOfRangeException(nameof(baseYield));
        PlantTime.RequirePositive(stage1Duration, nameof(stage1Duration));
        PlantTime.RequirePositive(stage2Duration, nameof(stage2Duration));
        PlantTime.RequirePositive(droughtPenaltyThreshold, nameof(droughtPenaltyThreshold));
        PlantTime.RequirePositive(droughtDeathThreshold, nameof(droughtDeathThreshold));
        PlantTime.RequirePositive(rotThreshold, nameof(rotThreshold));
        PlantTime.RequirePositive(regrowthDuration, nameof(regrowthDuration));
        if (droughtDeathThreshold <= droughtPenaltyThreshold)
            throw new ArgumentException("Death threshold must exceed penalty threshold.");
        if (!Enum.IsDefined(typeof(CropCategory), category) || !Enum.IsDefined(typeof(HarvestType), harvestType))
            throw new ArgumentException("Unknown crop classification.");
        Type=type; Name=name; SeedItem=seedItem; ProductItem=productItem;
        BaseYield=baseYield; Stage1Duration=stage1Duration; Stage2Duration=stage2Duration;
        DroughtPenaltyThreshold=droughtPenaltyThreshold; DroughtDeathThreshold=droughtDeathThreshold;
        RotThreshold=rotThreshold; Category=category; HarvestType=harvestType; RegrowthDuration=regrowthDuration;
    }

    public float GetStageDuration(CropState state)
    {
        switch (state)
        {
            case CropState.Stage1: return Stage1Duration;
            case CropState.Stage2: return Stage2Duration;
            case CropState.Regrowing: return RegrowthDuration;
            default: throw new ArgumentOutOfRangeException(nameof(state));
        }
    }
}

internal static class PlantTime
{
    internal static bool IsValid(float value) => !float.IsNaN(value) && !float.IsInfinity(value) && value >= 0;
    internal static void RequirePositive(float value, string name)
    {
        if (!IsValid(value) || value <= 0) throw new ArgumentOutOfRangeException(name);
    }
    internal static void ValidateDelta(float value)
    {
        if (!IsValid(value)) throw new ArgumentOutOfRangeException(nameof(value));
    }
}
