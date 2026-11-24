public sealed class GrowthBoostStrategy : IFertilizerStrategy
{
    public FertilizerType Type => FertilizerType.GrowthBoost;
    public float GetYieldBonus() => 0;
    public float AdjustRemainingTime(float remainingTime) => remainingTime * 0.75f;
}
