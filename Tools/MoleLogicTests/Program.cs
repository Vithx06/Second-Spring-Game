using System;
using SecondSpring.Minigames;

int checks = 0;
void Check(bool ok, string label) { checks++; if (!ok) throw new Exception(label); }
void Throws<T>(Action action) where T : Exception { checks++; try { action(); } catch (T) { return; } throw new Exception("Expected " + typeof(T).Name); }
var game = new WhackAMoleMinigame(count => count == 2 ? 1 : 4);
Throws<InvalidOperationException>(() => game.Hit(4));
game.Start();
Check(game.ActiveHole == 4 && game.Lives == 3, "initial state");
Check(!game.Hit(3) && game.Hit(4) && !game.Hit(4), "only active mole counts once");
for (int i = 1; i < 22; i++) { game.Advance(WhackAMoleMinigame.MoleIntervalSeconds); Check(game.ActiveHole == 4, "next mole"); game.Hit(4); }
game.Advance(WhackAMoleMinigame.MoleIntervalSeconds);
Check(game.IsFinished && game.Result.IsWon && game.Result.Hits == 22, "win at 30 seconds");
Check(game.Result.Fertilizer == MoleFertilizer.GrowthBoost && game.Result.Quantity == 3, "winning reward");
Throws<InvalidOperationException>(() => game.Advance(1));
var loss = new WhackAMoleMinigame(_ => 0); loss.Start(); loss.Advance(10);
Check(loss.IsFinished && loss.Lives == 0 && !loss.Result.IsWon && loss.Result.Quantity == 0, "misses lose lives in long update");
var low = new WhackAMoleMinigame(_ => 0); low.Start();
for (int i = 0; i < 22; i++) { if (i < 14 || i >= 16) low.Hit(0); low.Advance(WhackAMoleMinigame.MoleIntervalSeconds); }
Check(low.Result.IsWon && low.Result.Hits == 20 && low.Lives == 1, "win with exactly 20 hits and one life");
var invalid = new WhackAMoleMinigame(_ => 0); invalid.Start();
Throws<ArgumentOutOfRangeException>(() => invalid.Advance(double.NaN));
Throws<ArgumentOutOfRangeException>(() => invalid.Hit(9));
Console.WriteLine($"Passed {checks} mole checks.");
