using System;
using SecondSpring.Achievements;

int checks = 0;
void Check(bool value, string label) { checks++; if (!value) throw new Exception(label); }
void Throws<T>(Action action) where T : Exception { checks++; try { action(); } catch (T) { return; } throw new Exception("Expected " + typeof(T).Name); }
var game = new AchievementManager();
Check(game.States.Count == 30 && game.ClaimedCount == 0, "catalog and starting state");
Check(!game.Claim(1), "cannot claim locked achievement");
game.Record(AchievementMetric.SeedsSown);
Check(game.GetState(1).IsUnlocked && !game.GetState(1).IsClaimed, "first action unlocks without claiming");
Check(game.Claim(1) && !game.Claim(1) && game.ClaimedCount == 1, "claim exactly once");
game.Record(AchievementMetric.EggsCollected, 299);
Check(game.GetState(6).IsUnlocked && game.GetState(20).IsUnlocked && !game.GetState(28).IsUnlocked, "cumulative egg thresholds");
game.Record(AchievementMetric.MilkCollected, 150);
Check(!game.GetState(28).IsUnlocked, "composite requires both thresholds");
game.Record(AchievementMetric.EggsCollected);
Check(game.GetState(28).IsUnlocked, "composite completes");
game.SetAnimalCounts(3, 2, 0, 0);
Check(game.GetState(13).IsUnlocked && game.Stats.Get(AchievementMetric.PoultryCount) == 5, "combined ownership");
game.SetAnimalCounts(15, 15, 15, 14);
Check(!game.GetState(27).IsUnlocked, "ownership requires all four");
game.SetAnimalCounts(15, 15, 15, 15);
Check(game.GetState(27).IsUnlocked, "ownership all four");
game.SetAnimalCounts(0, 0, 0, 0);
Check(game.GetState(13).IsUnlocked && game.GetState(27).IsUnlocked, "unlock remains after counts decrease");
var days = new AchievementManager();
for (int day = 1; day <= 14; day++)
{
    foreach (FeedingPen pen in new[] { FeedingPen.Chicken, FeedingPen.Duck, FeedingPen.Pig, FeedingPen.Cow }) days.RecordFeeding(pen);
    days.EndDay(day);
}
Check(days.GetState(5).IsUnlocked && days.GetState(15).IsUnlocked && days.GetState(24).IsUnlocked && days.GetState(30).IsUnlocked, "feeding streak thresholds");
days.EndDay(15);
Check(days.Stats.Get(AchievementMetric.AllPensFeedingStreak) == 0 && days.GetState(30).IsUnlocked, "miss resets streak without relocking");
Throws<ArgumentOutOfRangeException>(() => days.EndDay(15));
Throws<ArgumentOutOfRangeException>(() => days.Record(AchievementMetric.Chickens));
Throws<ArgumentOutOfRangeException>(() => days.Record(AchievementMetric.FishCaught, 0));
Throws<ArgumentOutOfRangeException>(() => days.SetAnimalCounts(-1, 0, 0, 0));
Console.WriteLine($"Passed {checks} achievement checks.");
