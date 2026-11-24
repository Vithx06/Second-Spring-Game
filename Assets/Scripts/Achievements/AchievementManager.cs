using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SecondSpring.Achievements
{
    public sealed class AchievementManager
    {
        private readonly Dictionary<int, AchievementState> _byId = new Dictionary<int, AchievementState>();
        private readonly ReadOnlyCollection<AchievementState> _states;
        public GameStats Stats { get; } = new GameStats();
        public IReadOnlyList<AchievementState> States => _states;
        public int ClaimedCount { get; private set; }

        public AchievementManager()
        {
            var states = new List<AchievementState>();
            foreach (AchievementDefinition definition in AchievementCatalog.Create())
            {
                var state = new AchievementState(definition);
                _byId.Add(definition.Id, state); states.Add(state);
            }
            _states = states.AsReadOnly();
        }

        // Chỉ ghi nhận thao tác đã thành công; gọi một lần cho mỗi kết quả thực tế.
        public void Record(AchievementMetric metric, long amount = 1)
        {
            if (metric < AchievementMetric.SeedsSown || metric > AchievementMetric.CropsHarvested)
                throw new ArgumentOutOfRangeException(nameof(metric));
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            Stats.Add(metric, amount); Refresh();
        }

        public void SetAnimalCounts(int chickens, int ducks, int pigs, int cows)
        {
            if (chickens < 0 || ducks < 0 || pigs < 0 || cows < 0) throw new ArgumentOutOfRangeException(nameof(chickens));
            Stats.Set(AchievementMetric.Chickens, chickens); Stats.Set(AchievementMetric.Ducks, ducks);
            Stats.Set(AchievementMetric.Pigs, pigs); Stats.Set(AchievementMetric.Cows, cows);
            Refresh();
        }

        public void RecordFeeding(FeedingPen pen)
        {
            if (!Enum.IsDefined(typeof(FeedingPen), pen)) throw new ArgumentOutOfRangeException(nameof(pen));
            Stats.Add(AchievementMetric.FeedActions, 1); Stats.MarkFed(pen); Refresh();
        }

        public void EndDay(int day) { Stats.EndDay(day); Refresh(); }
        public AchievementState GetState(int id) => _byId.TryGetValue(id, out var state) ? state : throw new ArgumentOutOfRangeException(nameof(id));
        public bool Claim(int id)
        {
            AchievementState state = GetState(id);
            if (!state.IsUnlocked || state.IsClaimed) return false;
            state.IsClaimed = true; ClaimedCount++; return true;
        }

        private void Refresh()
        {
            foreach (AchievementState state in _states)
            {
                if (state.IsUnlocked) continue;
                bool met = true;
                foreach (AchievementRequirement requirement in state.Definition.Requirements)
                {
                    long value = Stats.Get(requirement.Metric);
                    if (value < requirement.Target) { met = false; break; }
                }
                if (met) state.IsUnlocked = true;
            }
        }
    }
}
