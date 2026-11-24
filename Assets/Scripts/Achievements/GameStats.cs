using System;
using System.Collections.Generic;

namespace SecondSpring.Achievements
{
    public sealed class GameStats
    {
        private readonly Dictionary<AchievementMetric, long> _values = new Dictionary<AchievementMetric, long>();
        private readonly HashSet<FeedingPen> _fedToday = new HashSet<FeedingPen>();
        public int CurrentDay { get; private set; } = 1;
        public long Get(AchievementMetric metric) => metric == AchievementMetric.PoultryCount
            ? Get(AchievementMetric.Chickens) + Get(AchievementMetric.Ducks)
            : _values.TryGetValue(metric, out long value) ? value : 0;
        internal void Add(AchievementMetric metric, long amount) { _values[metric] = checked(Get(metric) + amount); }
        internal void Set(AchievementMetric metric, long value) { _values[metric] = value; }
        internal void MarkFed(FeedingPen pen) { _fedToday.Add(pen); }

        internal void EndDay(int day)
        {
            if (day != CurrentDay || day == int.MaxValue) throw new ArgumentOutOfRangeException(nameof(day));
            // Chỉ chốt chuỗi một lần khi kết thúc ngày; chuồng trống không được tính đã cho ăn.
            bool chicken = _fedToday.Contains(FeedingPen.Chicken);
            bool cow = _fedToday.Contains(FeedingPen.Cow);
            bool all = chicken && cow && _fedToday.Contains(FeedingPen.Duck) && _fedToday.Contains(FeedingPen.Pig);
            long chickenStreak = chicken ? checked(Get(AchievementMetric.ChickenFeedingStreak) + 1) : 0;
            long cowStreak = cow ? checked(Get(AchievementMetric.CowFeedingStreak) + 1) : 0;
            long allStreak = all ? checked(Get(AchievementMetric.AllPensFeedingStreak) + 1) : 0;
            Set(AchievementMetric.ChickenFeedingStreak, chickenStreak);
            Set(AchievementMetric.CowFeedingStreak, cowStreak);
            Set(AchievementMetric.AllPensFeedingStreak, allStreak);
            _fedToday.Clear();
            CurrentDay++;
        }
    }
}
