using System;
using System.Collections.Generic;

namespace SecondSpring.Minigames
{
    public enum FishingCatch { Perch, Carp, Sturgeon, PlasticCan, OldShoe, TornClothes }
    public enum FishingOutcome { Miss, Fish, Trash }

    public sealed class FishingResult
    {
        public FishingOutcome Outcome { get; }
        public FishingCatch? Catch { get; }

        internal FishingResult(FishingOutcome outcome, FishingCatch? catchType)
        {
            Outcome = outcome;
            Catch = catchType;
        }
    }

    // Mỗi đối tượng tương ứng một lượt câu. Kết quả được chọn trước;
    // thao tác giật cần chỉ quyết định người chơi có nhận được kết quả đó không.
    public sealed class FishingMinigame : Minigame<FishingResult>
    {
        private static readonly FishingCatch[] Catches = {
            FishingCatch.Perch, FishingCatch.Carp, FishingCatch.Sturgeon,
            FishingCatch.PlasticCan, FishingCatch.OldShoe, FishingCatch.TornClothes
        };
        private static readonly int[] Weights = { 35, 25, 10, 10, 10, 10 };
        private readonly double _zoneStart;
        private readonly double _zoneEnd;
        private readonly HashSet<FishingCatch> _available;
        private readonly double _roll;
        private double _position;
        private FishingOutcome _outcome;
        public FishingCatch SelectedCatch { get; private set; }

        // roll là số ngẫu nhiên phân bố đều trong [0, 1). Vị trí và hai đầu
        // vùng chính xác được chuẩn hóa từ 0 đến 1 trên thanh câu.
        public FishingMinigame(IEnumerable<FishingCatch> adultFish, double roll, double zoneStart, double zoneEnd)
        {
            if (adultFish == null) throw new ArgumentNullException(nameof(adultFish));
            if (double.IsNaN(roll) || roll < 0 || roll >= 1) throw new ArgumentOutOfRangeException(nameof(roll));
            if (double.IsNaN(zoneStart) || double.IsNaN(zoneEnd) || zoneStart < 0 || zoneEnd > 1 || zoneStart >= zoneEnd)
                throw new ArgumentOutOfRangeException(nameof(zoneStart));
            _available = new HashSet<FishingCatch>();
            foreach (var fish in adultFish)
            {
                if (fish != FishingCatch.Perch && fish != FishingCatch.Carp && fish != FishingCatch.Sturgeon)
                    throw new ArgumentException("Only fish species are allowed.", nameof(adultFish));
                _available.Add(fish);
            }
            _roll = roll;
            _zoneStart = zoneStart;
            _zoneEnd = zoneEnd;
        }

        protected override void Setup()
        {
            // Ba loại rác luôn có tổng trọng số 30. Chỉ cộng trọng số của
            // những loài có cá trưởng thành, sau đó chọn trong tổng còn lại;
            // nhờ vậy tỷ lệ giữa các kết quả hợp lệ vẫn được giữ nguyên.
            int total = 30;
            for (int i = 0; i < 3; i++) if (_available.Contains(Catches[i])) total += Weights[i];
            double selection = _roll * total;
            for (int i = 0; i < Catches.Length; i++)
            {
                if (i < 3 && !_available.Contains(Catches[i])) continue;
                selection -= Weights[i];
                if (selection < 0) { SelectedCatch = Catches[i]; break; }
            }
        }

        // Câu cá chờ người chơi giật cần; chuyển động thanh do tầng giao diện cung cấp.
        protected override void PlayCore() { }

        public FishingResult Pull(double position)
        {
            if (!IsStarted || IsFinished) throw new InvalidOperationException("This attempt is not active.");
            if (double.IsNaN(position) || position < 0 || position > 1) throw new ArgumentOutOfRangeException(nameof(position));
            _position = position;
            return Complete();
        }

        protected override void EvaluateResult()
        {
            if (_position < _zoneStart || _position > _zoneEnd) { _outcome = FishingOutcome.Miss; return; }
            bool fish = SelectedCatch == FishingCatch.Perch || SelectedCatch == FishingCatch.Carp || SelectedCatch == FishingCatch.Sturgeon;
            _outcome = fish ? FishingOutcome.Fish : FishingOutcome.Trash;
        }

        protected override FishingResult BuildReward() =>
            new FishingResult(_outcome, _outcome == FishingOutcome.Miss ? (FishingCatch?)null : SelectedCatch);
    }
}
