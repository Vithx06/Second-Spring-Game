public partial class FarmPlot
{
    private readonly int _id;
    private FarmPlotStatus _status;
    private Crop _currentCrop;
    private readonly float _tillingDuration;
    private float _tillingTimeRemaining;
    private ResidueType _residueType;

    // Constructor cũ dành cho prototype; khi nối scene mới, truyền ID riêng cho mỗi ô.
    public FarmPlot(float tillingDuration) : this(0,tillingDuration) { }

    public FarmPlot(int id,float tillingDuration)
    {
        if(id<0)throw new System.ArgumentOutOfRangeException(nameof(id));
        _id=id;
        PlantTime.RequirePositive(tillingDuration, nameof(tillingDuration));
        _tillingDuration = tillingDuration;
        _status = FarmPlotStatus.Untilled;
        _residueType = ResidueType.None;
    }
    public int Id => _id;
    public FarmPlotStatus Status => _status;
    public Crop CurrentCrop => _currentCrop;
    public ResidueType ResidueType => _residueType;
    public float TillingTimeRemaining => _tillingTimeRemaining;
    // Chỉ đất chưa xới hoặc còn tàn dư mới nhận yêu cầu xới.
    public bool CanTill() => _status == FarmPlotStatus.Untilled || _status == FarmPlotStatus.HaveResidue;
    public bool StartTilling()
    {
        if (!CanTill()) return false;
        _tillingTimeRemaining = _tillingDuration;
        _status = FarmPlotStatus.Tilling;
        return true;
    }
    public void UpdateTilling(float deltaTime)
    {
        PlantTime.ValidateDelta(deltaTime);
        if (_status != FarmPlotStatus.Tilling) return;
        // Chặn thời gian ở 0; hoàn tất mới dọn tàn dư và cho phép gieo.
        _tillingTimeRemaining = System.Math.Max(0, _tillingTimeRemaining - deltaTime);
        if (_tillingTimeRemaining > 0) return;
        _residueType = ResidueType.None;
        _status = FarmPlotStatus.ReadyToSow;
    }
    public bool CanPlant() => _status == FarmPlotStatus.ReadyToSow && _currentCrop == null;
    public bool Plant(Crop crop)
    {
        if (!CanPlant() || crop == null || crop.Owner != null || crop.State != CropState.Stage1) return false;
        // Một cây chỉ được thuộc về một ô đất.
        _currentCrop = crop;
        crop.Owner = this;
        _status = FarmPlotStatus.HavePlant;
        return true;
    }
    public void RemoveCrop(ResidueType residueType)
    {
        if (_currentCrop == null) return;
        bool harvested = _currentCrop.State == CropState.Harvested && residueType == ResidueType.CropRemains;
        bool dead = _currentCrop.State == CropState.Dead && residueType == ResidueType.RottenCrop;
        if (!harvested && !dead) throw new System.InvalidOperationException("Only finished crops can be removed with matching residue.");
        // Chỉ cây đã thu hoạch xong hoặc đã chết mới được rời ô đất.
        _currentCrop.Owner = null;
        _currentCrop = null;
        _residueType = residueType;
        _status = FarmPlotStatus.HaveResidue;
    }
}
