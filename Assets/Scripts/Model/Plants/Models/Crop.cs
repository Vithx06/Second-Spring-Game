using System;

public abstract partial class Crop
{
    private readonly CropDefinition _definition;
    private CropState _state = CropState.Stage1;
    // Nhóm dữ liệu của giai đoạn đang lớn: lượt tưới, thời gian lớn và thời gian chịu hạn.
    private int _wateringCount;
    private float _growthTimeRemaining;
    private float _waterDeprivationElapsedTime;
    private bool _hasDroughtPenalty;
    // Hiệu quả phân bón tồn tại suốt đời cây; hệ số thời gian áp dụng cả lúc tái sinh.
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
    // Cây tái sinh cũng cần tưới như hai giai đoạn sinh trưởng đầu.
    public bool CanWater => _state==CropState.Stage1 || _state==CropState.Stage2 || _state==CropState.Regrowing;

    protected Crop(CropDefinition definition, Func<double> random = null)
    {
        _definition = definition ?? throw new ArgumentNullException(nameof(definition));
        _growthTimeRemaining = definition.Stage1Duration;
        _random = random ?? (() => Random.NextDouble());
    }

    public bool Water()
    {
        if (!CanWater) return false;
        // Mỗi giai đoạn cần tưới lại; tưới đến lần thứ năm làm cây chết.
        _wateringCount++;
        _waterDeprivationElapsedTime = 0;
        if (_wateringCount >= 5) _state = CropState.Dead;
        return true;
    }

    public bool CanFertilize() => CanWater
        && _fertilizerType == FertilizerType.None;

    public bool Fertilize(FertilizerDefinition fertilizer)
    {
        if(!CanFertilize() || fertilizer==null)return false;
        float remaining=_growthTimeRemaining*fertilizer.TimeMultiplier;
        if(!PlantTime.IsValid(remaining) || remaining<=0)return false;
        // Chỉ nhận một loại phân trong cả đời cây, kể cả cây thu hoạch nhiều lần.
        _fertilizerType=fertilizer.Type;
        _yieldBonus=fertilizer.YieldBonus;
        _growthTimeRemaining=remaining;
        _pendingStageTimeMultiplier=fertilizer.TimeMultiplier;
        return true;
    }

    public void UpdateGrowth(float deltaTime)
    {
        PlantTime.ValidateDelta(deltaTime);
        // Tiêu thụ cả phần thời gian dư ở trạng thái kế tiếp để không bỏ qua hạn hoặc thối cây.
        while (deltaTime > 0)
        {
            if (_state == CropState.Dead || _state == CropState.Harvested) return;
            if (_state == CropState.Ripe)
            {
                _ripeElapsedTime += deltaTime;
                if (_ripeElapsedTime >= _definition.RotThreshold) _state = CropState.Dead;
                return;
            }
            // Chưa tưới thì cây dừng lớn, nhưng thời gian chịu hạn vẫn tăng.
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
        // Tăng/giảm đều tính trên năng suất gốc. Đuôi 0,5 được làm tròn ngẫu nhiên.
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
        _state = CropState.Regrowing;
        _wateringCount = 0;
        _waterDeprivationElapsedTime = 0;
        _ripeElapsedTime = 0;
        _growthTimeRemaining = _definition.RegrowthDuration * _pendingStageTimeMultiplier;
        // Phân bón và hình phạt thiếu nước theo suốt đời cây, không được làm mới mỗi lần hái.
    }
}
