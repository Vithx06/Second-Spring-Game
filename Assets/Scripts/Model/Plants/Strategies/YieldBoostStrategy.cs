public sealed class YieldBoostStrategy : IFertilizerStrategy
{
    public FertilizerType Type => FertilizerType.YieldBoost;
    public float GetYieldBonus() => 0.5f;
    public float AdjustRemainingTime(float remainingTime) => remainingTime;
}
