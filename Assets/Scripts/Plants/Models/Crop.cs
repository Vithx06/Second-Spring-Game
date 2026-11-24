using System;

public abstract class Crop
{
    private readonly CropDefinition _definition;
    private CropState _state = CropState.Stage1;
    private int _wateringCount;
    private float _growthTimeRemaining;
    private float _waterDeprivationElapsedTime;
    private bool _hasDroughtPenalty;
    private FertilizerType _fertilizerType;
    private float _yieldBonus;
    private float _ripeElapsedTime;
    private float _pendingStageTimeMultiplier = 1f;
    private readonly Func<double> _random;
    private static readonly Random Random = new Random();
    internal FarmPlot Owner { get; set; }

    public CropDefinition Definition => _definition;
    public CropState State => _state;
    public int WateringCount => _wateringCount;
    public float GrowthTimeRemaining => _growthTimeRemaining;
    public float WaterDeprivationElapsedTime => _waterDeprivationElapsedTime;
    public bool HasDroughtPenalty => _hasDroughtPenalty;
    public FertilizerType FertilizerType => _fertilizerType;
    public float RipeElapsedTime => _ripeElapsedTime;

    protected Crop(CropDefinition definition, Func<double> random = null)
    {
        _definition = definition ?? throw new ArgumentNullException(nameof(definition));
        _growthTimeRemaining = definition.Stage1Duration;
        _random = random ?? (() => Random.NextDouble());
    }

    public bool Water()
    {
        if (_state != CropState.Stage1 && _state != CropState.Stage2) return false;
        _wateringCount++;
        _waterDeprivationElapsedTime = 0;
        if (_wateringCount >= 5) _state = CropState.Dead;
        return true;
    }

    public bool CanFertilize() => (_state == CropState.Stage1 || _state == CropState.Stage2)
        && _fertilizerType == FertilizerType.None;

    public bool Fertilize(IFertilizerStrategy strategy)
    {
        if (!CanFertilize() || strategy == null || strategy.Type == FertilizerType.None) return false;
        float bonus = strategy.GetYieldBonus();
        float remaining = strategy.AdjustRemainingTime(_growthTimeRemaining);
        float multiplier = strategy.AdjustRemainingTime(1f);
        if (!PlantTime.IsValid(bonus) || !PlantTime.IsValid(remaining) || remaining <= 0
            || !PlantTime.IsValid(multiplier) || multiplier <= 0 || multiplier > 1
            || (strategy.Type != FertilizerType.YieldBoost && strategy.Type != FertilizerType.GrowthBoost))
            return false;
        _fertilizerType = strategy.Type;
        _yieldBonus = bonus;
        _growthTimeRemaining = remaining;
        _pendingStageTimeMultiplier = multiplier;
        return true;
    }

    public void UpdateGrowth(float deltaTime)
    {
        PlantTime.ValidateDelta(deltaTime);
        // Consume overflow in the next state, so a long tick cannot skip drought or rot.
        while (deltaTime > 0)
        {
            if (_state == CropState.Dead || _state == CropState.Harvested) return;
            if (_state == CropState.Ripe)
            {
                _ripeElapsedTime += deltaTime;
                if (_ripeElapsedTime >= _definition.RotThreshold) _state = CropState.Dead;
                return;
            }
            if (_wateringCount == 0)
            {
                _waterDeprivationElapsedTime += deltaTime;
                if (_waterDeprivationElapsedTime >= _definition.DroughtPenaltyThreshold) _hasDroughtPenalty = true;
                if (_waterDeprivationElapsedTime >= _definition.DroughtDeathThreshold) _state = CropState.Dead;
                return;
            }
            if (deltaTime < _growthTimeRemaining)
            {
                _growthTimeRemaining -= deltaTime;
                return;
            }
            deltaTime -= _growthTimeRemaining;
            _wateringCount = 0;
            _waterDeprivationElapsedTime = 0;
            if (_state == CropState.Stage1)
            {
                _state = CropState.Stage2;
                _growthTimeRemaining = _definition.Stage2Duration * _pendingStageTimeMultiplier;
            }
            else
            {
                _state = CropState.Ripe;
                _growthTimeRemaining = 0;
                _ripeElapsedTime = 0;
            }
        }
    }

    public bool CanHarvest() => _state == CropState.Ripe;

    public int CalculateYield()
    {
        double amount = _definition.BaseYield * (1d + _yieldBonus - (_hasDroughtPenalty ? 0.25d : 0));
        if (amount > int.MaxValue) throw new OverflowException("Crop yield exceeds item capacity.");
        int lower = (int)Math.Floor(amount);
        double fraction = amount - lower;
        if (Math.Abs(fraction - 0.5d) < 0.000001d) return lower + (_random() < 0.5d ? 0 : 1);
        return fraction < 0.5d ? lower : lower + 1;
    }

    public abstract bool Harvest();
    protected void MarkHarvested() { _state = CropState.Harvested; }
    protected void BeginRegrowth()
    {
        _state = CropState.Stage2;
        _wateringCount = 0;
        _waterDeprivationElapsedTime = 0;
        _ripeElapsedTime = 0;
        _growthTimeRemaining = _definition.RegrowthDuration * _pendingStageTimeMultiplier;
        // Fertilizer and drought penalty belong to this plant's whole lifetime.
    }
}
