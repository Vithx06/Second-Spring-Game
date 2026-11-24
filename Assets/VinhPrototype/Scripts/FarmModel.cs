using System;
using System.Collections.Generic;

namespace SecondSpring.Prototype
{
    // Plain C# demo model; replace its inventory and persistence with the team's shared services.
    public enum Species { Chicken, Duck, Pig, Cow, Perch, Carp, Sturgeon }
    public sealed class Animal { public Species Species; public int Age, Growth; public bool Adult { get { return Growth >= Farm.RequiredDays(Species); } } }
    public sealed class Pen
    {
        public readonly List<Animal> Animals = new List<Animal>();
        public int FedDay = -1, Eggs, Milk;
        public bool IsFish; public int Capacity { get { return IsFish ? 30 : 20; } }
        public int FeedRequired { get { return (Animals.Count + 4) / 5; } }
    }
    public sealed class Farm : IGameTimeListener
    {
        public readonly Pen[] Pens = { new Pen(), new Pen(), new Pen(), new Pen(), new Pen { IsFish = true } };
        public readonly int[] Feed = { 20, 20, 20 }; // poultry, livestock, fish; temporary inventory
        public GameClock Clock { get; }
        public int Day => Clock.Day;
        public double Minute => Clock.MinuteOfDay;
        public Farm() : this(new GameClock()) { }
        public Farm(GameClock clock) { Clock=clock ?? throw new ArgumentNullException(nameof(clock));Clock.Subscribe(this); }
        public void OnTimeElapsed(float deltaTime) { }
        public void OnHourChanged(int day,int hour) { }
        public void OnDayChanged(int day) { EndDay(day-1); }
         public int Score, Lives, Caught; public int[] Fertilizer = new int[2];
        public const double MinutesPerRealSecond = 4.8;
        public static int RequiredDays(Species s) { switch (s) { case Species.Chicken: return 3; case Species.Duck: case Species.Carp: return 4; case Species.Pig: return 5; case Species.Cow: case Species.Sturgeon: return 7; default: return 2; } }
        public static int StartHour(int pen) { return new[] {7,9,11,14,17}[pen]; }
        public int FeedType(int p) { return p < 2 ? 0 : p < 4 ? 1 : 2; }
        public bool Add(int p, Species s) { if (Pens[p].Animals.Count >= Pens[p].Capacity) return false; Pens[p].Animals.Add(new Animal { Species=s }); return true; }
        public bool FeedPen(int p) { Pen pen=Pens[p]; int t=FeedType(p); if(pen.Animals.Count == 0 || pen.FedDay == Day || Minute < StartHour(p)*60 || Feed[t] < pen.FeedRequired) return false; Feed[t]-=pen.FeedRequired; pen.FedDay=Day; return true; }
        public void Advance(double seconds) { Clock.Advance(seconds); }
        void EndDay(int completedDay) { foreach (Pen p in Pens) foreach (Animal a in p.Animals) { a.Age++; if (p.FedDay != completedDay) continue; a.Growth++; if (!a.Adult) continue; if (a.Species == Species.Chicken) p.Eggs++; if (a.Species == Species.Duck && a.Growth % 2 == 0) p.Eggs++; if (a.Species == Species.Cow && a.Growth % 2 == 0) p.Milk += 2; } }
        public int AdultCount(int p) { return Pens[p].Animals.FindAll(a => a.Adult).Count; }
        public bool Catch(Species s) { var a=Pens[4].Animals.Find(x=>x.Species==s && x.Adult); if(a==null) return false; Pens[4].Animals.Remove(a); Caught++; return true; }
    }
}
