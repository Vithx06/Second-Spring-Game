using System;

namespace SecondSpring.Animals
{
    public abstract class Animal
    {
        public int Id { get; }
        public AnimalType Type { get; }
        public int Age { get; private set; }
        public int GrowthProgress { get; private set; }
        public GrowthStage GrowthStage { get; private set; }
        public AnimalProfile Profile { get; }
        internal int MaturityAge { get; private set; }

        protected Animal(int id, AnimalProfile profile)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            Id = id;
            Type = profile.AnimalType;
            GrowthStage = GrowthStage.Young;
        }

        public bool IsAdult() => GrowthStage == GrowthStage.Adult;

        internal void AdvanceOneDay(bool penFed)
        {
            Age++;
            if (!penFed || IsAdult()) return;
            GrowthProgress++;
            if (GrowthProgress >= Profile.MaturityDays)
            {
                GrowthStage = GrowthStage.Adult;
                MaturityAge = Age;
            }
        }
    }
}
