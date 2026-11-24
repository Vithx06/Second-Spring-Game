using System;
using System.Collections.Generic;
using SecondSpring.Minigames;

int checks = 0;
void Check(bool ok, string label) { checks++; if (!ok) throw new Exception(label); }
var values = new Queue<double>(new[] { .19, .49, .2, .0, .5, .99 });
IMinigame Create(MinigameType type) => type == MinigameType.Fishing
    ? new FishingMinigame(Array.Empty<FishingCatch>(), 0)
    : new WhackAMoleMinigame(_ => 0);
var manager = new MinigameManager(() => values.Dequeue(), Create);
manager.OnHourChanged(1, 23);
Check(manager.Invitation == MinigameType.Fishing, "invite fishing");
manager.OnHourChanged(2, 0);
Check(manager.Invitation == MinigameType.Fishing && values.Count == 4, "one hour remains across midnight");
manager.OnHourChanged(2, 1);
Check(manager.Invitation == null && values.Count == 3, "expires at two hours; threshold excludes .2");
manager.OnHourChanged(2, 2);
Check(manager.Invitation == MinigameType.WhackAMole, "equal selection boundary");
Check(manager.TryAcceptInvitation(out var chosen) && chosen == MinigameType.WhackAMole, "accept invitation");
Check(manager.CurrentGame is WhackAMoleMinigame && manager.CurrentGame.IsStarted, "create and start selected game");
manager.OnHourChanged(2, 3);
Check(manager.ActiveGame == chosen && manager.Invitation == null && values.Count == 1, "no invite while active");
Check(!manager.TryAcceptInvitation(out _), "cannot accept twice");
var running = manager.CurrentGame;
running.Advance(10);
Check(running.IsFinished && manager.CurrentGame == null && manager.ActiveGame == null, "finish clears active game");
manager.OnHourChanged(2, 4);
Check(manager.Invitation == null && values.Count == 0, "roll resumes after finish");
var dismiss = new MinigameManager(() => 0, Create);
dismiss.OnHourChanged(1, 8);
Check(dismiss.DismissInvitation() && !dismiss.DismissInvitation(), "dismiss once");
Console.WriteLine($"Passed {checks} invitation checks.");
