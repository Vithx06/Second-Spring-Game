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
        public const double DurationSeconds = 10;
        private readonly HashSet<FishingCatch> _available;
        private readonly double _roll;
        private double _position;
        private FishingCatch _selectedCatch;
        private FishingOutcome _outcome;
        private bool _timedOut;
        public double ElapsedSeconds { get; private set; }
        public FishingCatch SelectedCatch => IsStarted ? _selectedCatch : throw new InvalidOperationException("This attempt has not started.");
        public double ZoneWidth => SelectedCatch == FishingCatch.Sturgeon ? .15 : SelectedCatch == FishingCatch.Carp ? .22 : .30;
        public double SecondsPerDirection => SelectedCatch == FishingCatch.Sturgeon ? 1 : SelectedCatch == FishingCatch.Carp ? 1.5 : 2;
        public double ZoneStart => (1 - ZoneWidth) / 2;
        public double ZoneEnd => (1 + ZoneWidth) / 2;
        public double MarkerPosition
        {
            get { double phase = (ElapsedSeconds / SecondsPerDirection) % 2; return phase <= 1 ? phase : 2 - phase; }
        }

        // roll là số ngẫu nhiên phân bố đều trong [0, 1).
        public FishingMinigame(IEnumerable<FishingCatch> adultFish, double roll)
        {
            if (adultFish == null) throw new ArgumentNullException(nameof(adultFish));
            if (double.IsNaN(roll) || roll < 0 || roll >= 1) throw new ArgumentOutOfRangeException(nameof(roll));
            _available = new HashSet<FishingCatch>();
            foreach (var fish in adultFish)
            {
                if (fish != FishingCatch.Perch && fish != FishingCatch.Carp && fish != FishingCatch.Sturgeon)
                    throw new ArgumentException("Only fish species are allowed.", nameof(adultFish));
                _available.Add(fish);
            }
            _roll = roll;
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
                if (selection < 0) { _selectedCatch = Catches[i]; break; }
            }
        }

        // Lượt chơi chờ thao tác giật cần; Advance cập nhật thời gian thực.
        protected override void PlayCore() { }

        public override void Advance(double realSeconds)
        {
            if (!IsStarted || IsFinished) throw new InvalidOperationException("This attempt is not active.");
            if (double.IsNaN(realSeconds) || double.IsInfinity(realSeconds) || realSeconds < 0) throw new ArgumentOutOfRangeException(nameof(realSeconds));
            ElapsedSeconds = Math.Min(DurationSeconds, ElapsedSeconds + realSeconds);
            if (ElapsedSeconds >= DurationSeconds) { _timedOut = true; Complete(); }
        }

        public FishingResult Pull()
        {
            if (!IsStarted || IsFinished) throw new InvalidOperationException("This attempt is not active.");
            _position = MarkerPosition;
            return Complete();
        }

        protected override void EvaluateResult()
        {
            if (_timedOut || _position < ZoneStart || _position > ZoneEnd) { _outcome = FishingOutcome.Miss; return; }
            bool fish = SelectedCatch == FishingCatch.Perch || SelectedCatch == FishingCatch.Carp || SelectedCatch == FishingCatch.Sturgeon;
            _outcome = fish ? FishingOutcome.Fish : FishingOutcome.Trash;
        }

        protected override FishingResult BuildReward() =>
            new FishingResult(_outcome, _outcome == FishingOutcome.Miss ? (FishingCatch?)null : SelectedCatch);
    }
}
