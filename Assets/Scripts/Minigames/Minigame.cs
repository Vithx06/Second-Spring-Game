using System;

namespace SecondSpring.Minigames
{
    public abstract class Minigame<TResult>
    {
        public bool IsStarted { get; private set; }
        public bool IsFinished { get; private set; }

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
            TResult result = BuildReward();
            IsFinished = true;
            return result;
        }

        protected abstract void Setup();
        protected abstract void PlayCore();
        protected abstract void EvaluateResult();
        protected abstract TResult BuildReward();
    }
}
