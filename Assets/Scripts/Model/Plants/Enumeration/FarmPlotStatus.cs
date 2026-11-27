public enum FarmPlotStatus
{
    Untilled = 0, // Đất chưa xới
    Tilling = 1, // Đất đang được xới
    ReadyToSow = 2, // Đất sẵn sàng gieo hạt
    HavePlant = 3, // Đất đang có cây
    HaveResidue = 4, // Đất còn tàn dư sau thu hoạch hoặc cây chết
}
