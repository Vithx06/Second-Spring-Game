using System;

namespace SecondSpring.Minigames
{
    public enum MinigameType { Fishing, WhackAMole }

    public sealed class MinigameManager : IGameTimeListener
    {
        private readonly Func<double> _random;
        private readonly Func<MinigameType, IMinigame> _createGame;
        private long _expiresAtHour;
        private long _lastHour = -1;
        public MinigameType? Invitation { get; private set; }
        public MinigameType? ActiveGame { get; private set; }
        public IMinigame CurrentGame { get; private set; }

        // Mỗi lần gọi trả về số ngẫu nhiên phân bố đều trong khoảng [0, 1).
        public MinigameManager(Func<double> random, Func<MinigameType, IMinigame> createGame)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _createGame = createGame ?? throw new ArgumentNullException(nameof(createGame));
        }

        public void OnHourChanged(int day, int hour)
        {
            if (day < 1) throw new ArgumentOutOfRangeException(nameof(day));
            if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException(nameof(hour));
            long currentHour = ((long)day - 1) * 24 + hour;
            if (currentHour <= _lastHour) throw new ArgumentOutOfRangeException(nameof(hour), "Hours must advance in order.");
            _lastHour = currentHour;

            // Dùng tổng số giờ để lời mời hết hạn đúng cả khi qua nửa đêm.
            if (Invitation.HasValue && currentHour >= _expiresAtHour) Invitation = null;
            if (Invitation.HasValue || ActiveGame.HasValue) return;
            if (NextRandom() >= 0.2) return;
            Invitation = NextRandom() < 0.5 ? MinigameType.Fishing : MinigameType.WhackAMole;
            _expiresAtHour = currentHour + 2;
        }

        public bool TryAcceptInvitation(out MinigameType game)
        {
            if (!Invitation.HasValue || ActiveGame.HasValue) { game = default; return false; }
            game = Invitation.Value;
            IMinigame instance = _createGame(game) ?? throw new InvalidOperationException("The minigame factory returned null.");
            if ((game == MinigameType.Fishing && !(instance is FishingMinigame)) ||
                (game == MinigameType.WhackAMole && !(instance is WhackAMoleMinigame)))
                throw new InvalidOperationException("The minigame factory returned the wrong game.");
            if (instance.IsStarted || instance.IsFinished) throw new InvalidOperationException("The minigame must be new.");
            instance.Start();
            if (instance.IsFinished) throw new InvalidOperationException("The minigame ended during startup.");
            ActiveGame = game;
            CurrentGame = instance;
            instance.Completed += OnGameCompleted;
            Invitation = null;
            return true;
        }

        public bool DismissInvitation()
        {
            if (!Invitation.HasValue) return false;
            Invitation = null;
            return true;
        }

        private void OnGameCompleted()
        {
            CurrentGame.Completed -= OnGameCompleted;
            CurrentGame = null;
            ActiveGame = null;
        }

        public void OnTimeElapsed(float deltaTime) { }
        public void OnDayChanged(int day) { }

        private double NextRandom()
        {
            double value = _random();
            if (double.IsNaN(value) || value < 0 || value >= 1) throw new InvalidOperationException("Invalid random value.");
            return value;
        }
    }
}
