using System;

namespace SecondSpring.MainHouse
{
    public enum GameOutcome { InProgress, Won, Lost }

    public sealed class WinConditionManager
    {
        public const int MoneyGoal = 100000;
        public const int RequiredAchievements = 25;
        public const int TotalAchievements = 30;
        public const int LastDay = 365;
        public GameOutcome Outcome { get; private set; } = GameOutcome.InProgress;
        public int CurrentDay { get; private set; } = 1;
        public int Balance { get; private set; }
        public int ClaimedAchievements { get; private set; }
        public double MoneyProgress => Math.Min(1d, (double)Balance / MoneyGoal);
        public double AchievementProgress => Math.Min(1d, (double)ClaimedAchievements / RequiredAchievements);

        // Bên điều phối gọi sau khi ngày, số dư hoặc số thành tựu đã nhận thay đổi.
        // Giá trị đầu vào là trạng thái hiện tại, không phải tổng tiền đã kiếm.
        public GameOutcome UpdateProgress(int day, int balance, int claimedAchievements)
        {
            if (day < CurrentDay) throw new ArgumentOutOfRangeException(nameof(day));
            if (balance < 0) throw new ArgumentOutOfRangeException(nameof(balance));
            if (claimedAchievements < 0 || claimedAchievements > TotalAchievements)
                throw new ArgumentOutOfRangeException(nameof(claimedAchievements));
            if (Outcome != GameOutcome.InProgress) return Outcome;

            CurrentDay = day;
            Balance = balance;
            ClaimedAchievements = claimedAchievements;
            // Hết hạn được xét trước: đạt mục tiêu từ ngày 366 không thể thắng.
            if (day > LastDay) Outcome = GameOutcome.Lost;
            else if (balance >= MoneyGoal && claimedAchievements >= RequiredAchievements) Outcome = GameOutcome.Won;
            return Outcome;
        }
    }
}
