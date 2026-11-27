using System;

// UML v2 dùng dữ liệu phân bón thay cho hai thuật toán Strategy riêng.
public sealed class FertilizerDefinition
{
    public ItemType Item { get; }
    public FertilizerType Type { get; }
    public float YieldBonus { get; }
    public float TimeMultiplier { get; }
    public int RequiredQuantity { get; }

    public FertilizerDefinition(ItemType item,FertilizerType type,float yieldBonus,float timeMultiplier,int requiredQuantity=1)
    {
        if(type!=FertilizerType.YieldBoost && type!=FertilizerType.GrowthBoost)throw new ArgumentOutOfRangeException(nameof(type));
        if((type==FertilizerType.YieldBoost && item!=ItemType.YieldBoostFertilizer)
            || (type==FertilizerType.GrowthBoost && item!=ItemType.GrowthBoostFertilizer))throw new ArgumentException("Fertilizer item/type mismatch.");
        if(!PlantTime.IsValid(yieldBonus) || yieldBonus>1 || !PlantTime.IsValid(timeMultiplier)
            || timeMultiplier<=0 || timeMultiplier>1 || requiredQuantity<1)throw new ArgumentException("Invalid fertilizer configuration.");
        Item=item;Type=type;YieldBonus=yieldBonus;TimeMultiplier=timeMultiplier;RequiredQuantity=requiredQuantity;
    }

    public static FertilizerDefinition[] CreateDefaults() => new[] {
        // 0,5 tương ứng tăng 50% năng suất; 0,75 tương ứng giảm 25% thời gian.
        new FertilizerDefinition(ItemType.YieldBoostFertilizer,FertilizerType.YieldBoost,.5f,1f),
        new FertilizerDefinition(ItemType.GrowthBoostFertilizer,FertilizerType.GrowthBoost,0f,.75f)
    };
}
