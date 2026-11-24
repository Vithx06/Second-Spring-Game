using System;
using SecondSpring.MainHouse;

int checks = 0;
void Check(bool ok, string label) { checks++; if (!ok) throw new Exception(label); }
void Throws<T>(Action action) where T : Exception { checks++; try { action(); } catch (T) { return; } throw new Exception("Expected " + typeof(T).Name); }
var game = new WinConditionManager();
Check(game.Outcome == GameOutcome.InProgress && game.CurrentDay == 1, "initial state");
Check(game.UpdateProgress(1, 50000, 20) == GameOutcome.InProgress && game.MoneyProgress == .5 && game.AchievementProgress == .8, "separate progress");
Check(game.UpdateProgress(1, 100000, 24) == GameOutcome.InProgress, "insufficient achievements");
Check(game.UpdateProgress(1, 99999, 25) == GameOutcome.InProgress, "insufficient balance");
Check(game.UpdateProgress(365, 100000, 25) == GameOutcome.Won, "wins on day 365");
Check(game.UpdateProgress(366, 0, 0) == GameOutcome.Won, "win stays final");
var loss = new WinConditionManager();
Check(loss.UpdateProgress(366, 100000, 25) == GameOutcome.Lost, "deadline precedes victory");
Check(loss.UpdateProgress(367, 200000, 30) == GameOutcome.Lost, "loss stays final");
var progress = new WinConditionManager(); progress.UpdateProgress(2, 120000, 24);
Check(progress.MoneyProgress == 1 && progress.AchievementProgress < 1, "progress capped");
Throws<ArgumentOutOfRangeException>(() => progress.UpdateProgress(1, 0, 0));
Throws<ArgumentOutOfRangeException>(() => progress.UpdateProgress(2, -1, 0));
Throws<ArgumentOutOfRangeException>(() => progress.UpdateProgress(2, 0, 31));
Console.WriteLine($"Passed {checks} win condition checks.");
