using System;

namespace SecondSpring.Minigames
{
    public interface IMinigame
    {
        bool IsStarted { get; }
        bool IsFinished { get; }
        event Action Completed;
        void Start();
        void Advance(double realSeconds);
    }

    public abstract class Minigame<TResult> : IMinigame
    {
        public event Action Completed;
        public bool IsStarted { get; private set; }
        public bool IsFinished { get; private set; }
        public TResult Result { get; private set; }

        // Lượt chơi có thể chờ thao tác của người chơi giữa lúc bắt đầu và kết thúc.
        // Lớp cha giữ trình tự chung, còn lớp con cài đặt nội dung từng bước.
        public void Start()
        {
            if (IsStarted) throw new InvalidOperationException("This attempt has already started.");
            Setup();
            IsStarted = true;
            PlayCore();
        }

        protected TResult Complete()
        {
            if (!IsStarted || IsFinished) throw new InvalidOperationException("This attempt is not active.");
            EvaluateResult();
            Result = BuildReward();
            IsFinished = true;
            Completed?.Invoke();
            return Result;
        }

        public abstract void Advance(double realSeconds);
        protected abstract void Setup();
        protected abstract void PlayCore();
        protected abstract void EvaluateResult();
        protected abstract TResult BuildReward();
    }
}
