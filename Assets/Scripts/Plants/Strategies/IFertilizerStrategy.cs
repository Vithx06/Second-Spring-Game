public interface IFertilizerStrategy
{
    FertilizerType Type { get; }
    float GetYieldBonus();
    float AdjustRemainingTime(float remainingTime);
}
