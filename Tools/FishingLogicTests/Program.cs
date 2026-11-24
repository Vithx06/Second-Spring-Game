using System;
using SecondSpring.Minigames;

int checks = 0;
void Check(bool value, string label) { checks++; if (!value) throw new Exception(label); }
void Throws<T>(Action action) where T : Exception { checks++; try { action(); } catch (T) { return; } throw new Exception("Expected " + typeof(T).Name); }
FishingCatch[] all = { FishingCatch.Perch, FishingCatch.Carp, FishingCatch.Sturgeon };
FishingMinigame Start(FishingCatch[] available, double roll) { var game = new FishingMinigame(available, roll); game.Start(); return game; }
FishingCatch Select(FishingCatch[] available, double roll) => Start(available, roll).SelectedCatch;
Check(Select(all, 0) == FishingCatch.Perch && Select(all, .349) == FishingCatch.Perch, "perch weight");
Check(Select(all, .35) == FishingCatch.Carp && Select(all, .599) == FishingCatch.Carp, "carp weight");
Check(Select(all, .60) == FishingCatch.Sturgeon && Select(all, .699) == FishingCatch.Sturgeon, "sturgeon weight");
Check(Select(all, .70) == FishingCatch.PlasticCan && Select(all, .80) == FishingCatch.OldShoe && Select(all, .90) == FishingCatch.TornClothes, "trash weights");
Check(Select(new[] { FishingCatch.Carp }, 24.0 / 55) == FishingCatch.Carp && Select(new[] { FishingCatch.Carp }, 25.0 / 55) == FishingCatch.PlasticCan, "redistribution");
Check(Select(Array.Empty<FishingCatch>(), 0) == FishingCatch.PlasticCan && Select(Array.Empty<FishingCatch>(), .999) == FishingCatch.TornClothes, "empty pond");
var waiting = new FishingMinigame(all, 0);
Throws<InvalidOperationException>(() => { var unused = waiting.SelectedCatch; });
Throws<InvalidOperationException>(() => waiting.Pull());
waiting.Start();
Check(waiting.ZoneWidth == .30 && waiting.SecondsPerDirection == 2, "perch difficulty");
waiting.Advance(1); Check(Math.Abs(waiting.MarkerPosition - .5) < 1e-9, "perch marker");
Check(waiting.Pull().Outcome == FishingOutcome.Fish && waiting.IsFinished, "successful catch");
Throws<InvalidOperationException>(() => waiting.Pull());
var carp = Start(all, .4); Check(carp.ZoneWidth == .22 && carp.SecondsPerDirection == 1.5, "carp difficulty");
var sturgeon = Start(all, .65); Check(sturgeon.ZoneWidth == .15 && sturgeon.SecondsPerDirection == 1, "sturgeon difficulty");
var trash = Start(all, .8); Check(trash.ZoneWidth == .3 && trash.SecondsPerDirection == 2, "trash difficulty"); trash.Advance(1); Check(trash.Pull().Outcome == FishingOutcome.Trash, "trash result");
var miss = Start(all, 0); Check(miss.Pull().Outcome == FishingOutcome.Miss, "miss");
var timeout = Start(all, 0); timeout.Advance(9.99); Check(!timeout.IsFinished, "before timeout"); timeout.Advance(.01);
Check(timeout.IsFinished && timeout.Result.Outcome == FishingOutcome.Miss && timeout.Result.Catch == null, "timeout");
Throws<ArgumentOutOfRangeException>(() => new FishingMinigame(all, 1));
Throws<ArgumentException>(() => new FishingMinigame(new[] { FishingCatch.OldShoe }, 0));
Throws<ArgumentOutOfRangeException>(() => Start(all, 0).Advance(double.NaN));
Console.WriteLine($"Passed {checks} fishing checks.");
