using System;
using System.Collections.Generic;

public enum FarmEventKind
{
    SeedPlanted, // Đã gieo hạt
    CropWatered, // Đã tưới cây
    CropHarvested, // Đã thu hoạch cây trồng
    AnimalBought, // Đã mua vật nuôi
    PenFed, // Đã cho khu nuôi ăn
    ProductCollected, // Đã thu sản phẩm vật nuôi
    AnimalHarvested, // Đã thu hoạch vật nuôi
    ItemBought, // Đã mua vật phẩm
    ItemSold, // Đã bán vật phẩm
    MinigameWon, // Đã thắng trò chơi nhỏ
    BalanceChanged, // Số tiền thay đổi
    AchievementClaimed // Đã nhận thành tựu
}

public enum StatType
{
    SeedsPlanted, // Số hạt đã gieo
    CropsWatered, // Số lần tưới cây
    CropsHarvested, // Số sản phẩm cây đã thu
    AnimalsBought, // Số vật nuôi đã mua
    PensFed, // Số lần cho khu nuôi ăn
    ChickenEggsCollected, // Số trứng gà đã thu
    DuckEggsCollected, // Số trứng vịt đã thu
    MilkCollected, // Số đơn vị sữa đã thu
    FishHarvested, // Số cá đã thu hoạch
    MeatCollected, // Số đơn vị thịt đã thu
    MoleWins, // Số lượt thắng đập chuột
    GrassCuttingWins, // Số lượt thắng cắt cỏ
    ItemsBought, // Số vật phẩm đã mua
    ItemsSold, // Số vật phẩm đã bán
    ChickensOwned, // Số gà đang nuôi
    DucksOwned, // Số vịt đang nuôi
    CowsOwned, // Số bò đang nuôi
    PigsOwned, // Số lợn đang nuôi
    FishOwned, // Số cá đang nuôi
    ChickenPenFed, // Đã cho chuồng gà ăn
    DuckPenFed, // Đã cho chuồng vịt ăn
    CowPenFed, // Đã cho chuồng bò ăn
    PigPenFed, // Đã cho chuồng lợn ăn
    FishPenFed // Đã cho ao cá ăn
}

// Sự kiện chỉ mô tả việc đã thành công; không tự sửa kho hay trạng thái cây.
public sealed class FarmEvent
{
    public FarmEventKind Kind { get; }
    public StatType? Metric { get; }
    public int Quantity { get; }
    public int Day { get; }
    public int CurrentCount { get; }
    public FarmEvent(FarmEventKind kind,StatType? metric,int quantity,int day,int currentCount=0)
    {
        if(!Enum.IsDefined(typeof(FarmEventKind),kind) || (metric.HasValue && !Enum.IsDefined(typeof(StatType),metric.Value)))
            throw new ArgumentException("Unknown farm event.");
        if(quantity<0 || day<1 || currentCount<0)throw new ArgumentOutOfRangeException(nameof(quantity));
        Kind=kind;Metric=metric;Quantity=quantity;Day=day;CurrentCount=currentCount;
    }
}

public interface IFarmEventListener { void OnFarmEvent(FarmEvent evt); }

public sealed class FarmEventHub
{
    private readonly List<IFarmEventListener> _listeners=new List<IFarmEventListener>();
    public void Subscribe(IFarmEventListener listener)
    {
        if(listener==null)throw new ArgumentNullException(nameof(listener));
        if(!_listeners.Contains(listener))_listeners.Add(listener);
    }
    public void Unsubscribe(IFarmEventListener listener) { _listeners.Remove(listener); }
    public void Publish(FarmEvent evt)
    {
        if(evt==null)throw new ArgumentNullException(nameof(evt));
        // Chụp danh sách để listener có thể đăng ký/hủy đăng ký trong lúc nhận sự kiện.
        foreach(var listener in _listeners.ToArray())listener.OnFarmEvent(evt);
    }
}
