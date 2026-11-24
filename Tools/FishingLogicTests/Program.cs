using System;
using SecondSpring.Minigames;

int checks = 0;
void Check(bool value, string label) { checks++; if (!value) throw new Exception(label); }
void Throws<T>(Action action) where T : Exception { checks++; try { action(); } catch (T) { return; } throw new Exception("Expected " + typeof(T).Name); }
FishingCatch[] all = { FishingCatch.Perch, FishingCatch.Carp, FishingCatch.Sturgeon };
FishingCatch Select(FishingCatch[] available, double roll) => new FishingMinigame(available, roll, .4, .6).SelectedCatch;
Check(Select(all, 0) == FishingCatch.Perch && Select(all, .349) == FishingCatch.Perch, "perch weight");
Check(Select(all, .35) == FishingCatch.Carp && Select(all, .599) == FishingCatch.Carp, "carp weight");
Check(Select(all, .60) == FishingCatch.Sturgeon && Select(all, .699) == FishingCatch.Sturgeon, "sturgeon weight");
Check(Select(all, .70) == FishingCatch.PlasticCan && Select(all, .80) == FishingCatch.OldShoe && Select(all, .90) == FishingCatch.TornClothes, "trash weights");
Check(Select(new[] { FishingCatch.Carp }, 24.0 / 55) == FishingCatch.Carp && Select(new[] { FishingCatch.Carp }, 25.0 / 55) == FishingCatch.PlasticCan, "redistribution preserves weights");
Check(Select(Array.Empty<FishingCatch>(), 0) == FishingCatch.PlasticCan && Select(Array.Empty<FishingCatch>(), .999) == FishingCatch.TornClothes, "empty pond yields trash");
Check(Select(new[] { FishingCatch.Carp, FishingCatch.Carp }, .45) == FishingCatch.Carp, "duplicates do not change weights");
var fish = new FishingMinigame(all, 0, .4, .6); var result = fish.Pull(.4);
Check(result.Outcome == FishingOutcome.Fish && result.Catch == FishingCatch.Perch && fish.IsFinished, "fish and inclusive boundary");
Throws<InvalidOperationException>(() => fish.Pull(.5));
var miss = new FishingMinigame(all, 0, .4, .6).Pull(.39);
Check(miss.Outcome == FishingOutcome.Miss && miss.Catch == null, "miss has no catch");
var trash = new FishingMinigame(all, .8, .4, .6).Pull(.6);
Check(trash.Outcome == FishingOutcome.Trash && trash.Catch == FishingCatch.OldShoe, "trash outcome");
Throws<ArgumentOutOfRangeException>(() => new FishingMinigame(all, 1, .4, .6));
Throws<ArgumentOutOfRangeException>(() => new FishingMinigame(all, 0, .6, .4));
Throws<ArgumentException>(() => new FishingMinigame(new[] { FishingCatch.OldShoe }, 0, .4, .6));
Throws<ArgumentOutOfRangeException>(() => new FishingMinigame(all, 0, .4, .6).Pull(double.NaN));
Console.WriteLine($"Passed {checks} fishing checks.");
