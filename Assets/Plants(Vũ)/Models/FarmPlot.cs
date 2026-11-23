public class FarmPlot
{
    // Thuộc tính - Trạng thái của đất
    private FarmPlotStatus _status;
    private ResidueType _residueType;
    private float _tillingDuration;
    private float _tillingTimeRemaining;

    // Constructor
    public FarmPlot(float tillingDuration)
    {
        _status = FarmPlotStatus.Untilled;
        _residueType = ResidueType.None;
        _tillingDuration = tillingDuration;
        _tillingTimeRemaining = 0;
    }

    // Kiểm tra xem mảnh đất có thể xới hay không??
    public bool CanTill()
    {
        return _status == FarmPlotStatus.Untilled
            || _status == FarmPlotStatus.HaveResidue;
    }

    // Bắt đầu xới đất
 
    public bool StartTilling()
    {
        if (!CanTill())
        {
            return false;
        }

        _tillingTimeRemaining = _tillingDuration;
        _status = FarmPlotStatus.Tilling;

        return true;
    }

    // Cập nhật trạng thái xới đất
    public void UpdateTilling(float deltaTime)
    {
        if (_status != FarmPlotStatus.Tilling)
        {
            return;
        }

        _tillingTimeRemaining -= deltaTime;

        if (_tillingTimeRemaining <= 0)
        {
            _tillingTimeRemaining = 0;
            _residueType = ResidueType.None;
            _status = FarmPlotStatus.ReadyToSow;
        }
    }

    public FarmPlotStatus Status => _status;
}


