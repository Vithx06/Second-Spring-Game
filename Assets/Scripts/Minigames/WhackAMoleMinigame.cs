using System;

namespace SecondSpring.Minigames
{
    public enum MoleFertilizer { YieldBoost, GrowthBoost }

    public sealed class WhackAMoleResult
    {
        public bool IsWon { get; }
        public int Hits { get; }
        public MoleFertilizer? Fertilizer { get; }
        public int Quantity => IsWon ? 3 : 0;

        internal WhackAMoleResult(bool isWon, int hits, MoleFertilizer? fertilizer)
        {
            IsWon = isWon;
            Hits = hits;
            Fertilizer = fertilizer;
        }
    }

    public sealed class WhackAMoleMinigame : Minigame<WhackAMoleResult>
    {
        public const double DurationSeconds = 30;
        public const double MoleIntervalSeconds = DurationSeconds / 22;
        public const int TargetHits = 20;
        private readonly Func<int, int> _randomIndex;
        private double _slotElapsed;
        private bool _won;

        public double ElapsedSeconds { get; private set; }
        public int Lives { get; private set; }
        public int Hits { get; private set; }
        public int? ActiveHole { get; private set; }

        // Hàm ngẫu nhiên nhận số lựa chọn và trả về chỉ số từ 0 đến nhỏ hơn số đó.
        public WhackAMoleMinigame(Func<int, int> randomIndex)
        {
            _randomIndex = randomIndex ?? throw new ArgumentNullException(nameof(randomIndex));
        }

        protected override void Setup() { Lives = 3; }
        protected override void PlayCore() { Spawn(); }

        public bool Hit(int hole)
        {
            EnsureActive();
            if (hole < 0 || hole >= 9) throw new ArgumentOutOfRangeException(nameof(hole));
            if (ActiveHole != hole) return false;
            ActiveHole = null;
            Hits++;
            return true;
        }

        public override void Advance(double realSeconds)
        {
            EnsureActive();
            if (double.IsNaN(realSeconds) || double.IsInfinity(realSeconds) || realSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(realSeconds));

            // Chia thời gian tại từng mốc chuột chui xuống để không bỏ sót mạng
            // khi một lần cập nhật kéo dài qua nhiều lượt xuất hiện.
            double remaining = Math.Min(realSeconds, DurationSeconds - ElapsedSeconds);
            while (remaining > 0 && !IsFinished)
            {
                double toBoundary = Math.Min(MoleIntervalSeconds - _slotElapsed, DurationSeconds - ElapsedSeconds);
                // Bù sai số số thực rất nhỏ khi cộng nhiều khoảng thời gian.
                bool boundary = remaining + 1e-9 >= toBoundary;
                double step = boundary ? toBoundary : remaining;
                ElapsedSeconds += step;
                _slotElapsed += step;
                remaining = Math.Max(0, remaining - step);
                if (!boundary) continue;
                if (DurationSeconds - ElapsedSeconds < 1e-9) ElapsedSeconds = DurationSeconds;
                if (ActiveHole.HasValue) { ActiveHole = null; Lives--; }
                if (Lives == 0 || ElapsedSeconds >= DurationSeconds)
                {
                    Complete();
                    return;
                }
                _slotElapsed = 0;
                Spawn();
            }
        }

        protected override void EvaluateResult() { _won = Hits >= TargetHits && Lives > 0 && ElapsedSeconds >= DurationSeconds; }

        protected override WhackAMoleResult BuildReward()
        {
            MoleFertilizer? fertilizer = _won ? (MoleFertilizer?)NextIndex(2) : null;
            return new WhackAMoleResult(_won, Hits, fertilizer);
        }

        private void Spawn() { ActiveHole = NextIndex(9); }
        private int NextIndex(int count)
        {
            int index = _randomIndex(count);
            if (index < 0 || index >= count) throw new InvalidOperationException("Invalid random index.");
            return index;
        }
        private void EnsureActive()
        {
            if (!IsStarted || IsFinished) throw new InvalidOperationException("This attempt is not active.");
        }
    }
}
