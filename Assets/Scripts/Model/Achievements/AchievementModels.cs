using System;
using System.Collections.Generic;

namespace SecondSpring.Achievements
{
    public enum AchievementMetric
    {
        SeedsSown, Waterings, HarvestActions, AnimalsBought, FeedActions,
        EggsCollected, MilkCollected, FishCaught, MoleWins, ShopPurchases, ItemsSold,
        CropsHarvested, Chickens, Ducks, Pigs, Cows,
        ChickenFeedingStreak, CowFeedingStreak, AllPensFeedingStreak, PoultryCount
    }

    public enum FeedingPen { Chicken, Duck, Pig, Cow, Fish }
    public enum AchievementDifficulty { Easy, Medium, Hard }

    public sealed class AchievementRequirement
    {
        public AchievementMetric Metric { get; }
        public long Target { get; }
        public AchievementRequirement(AchievementMetric metric, long target) { Metric = metric; Target = target; }
    }

    public sealed class AchievementDefinition
    {
        public int Id { get; }
        public string Title { get; }
        public string Description { get; }
        public AchievementDifficulty Difficulty { get; }
        public IReadOnlyList<AchievementRequirement> Requirements { get; }
        internal AchievementDefinition(int id, string title, string description, AchievementDifficulty difficulty, params AchievementRequirement[] requirements)
        {
            Id = id; Title = title; Description = description; Difficulty = difficulty;
            Requirements = Array.AsReadOnly((AchievementRequirement[])requirements.Clone());
        }
    }

    public sealed class AchievementState
    {
        public AchievementDefinition Definition { get; }
        public bool IsUnlocked { get; internal set; }
        public bool IsClaimed { get; internal set; }
        internal AchievementState(AchievementDefinition definition) { Definition = definition; }
    }
}
