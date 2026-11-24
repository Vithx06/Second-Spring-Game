using System;

namespace SecondSpring.Animals
{
    public sealed class Fish : Animal
    {
        public FishType FishType { get; }

        public Fish(int id, FishType fishType) : base(id, CreateProfile(fishType))
        {
            FishType = fishType;
        }

        private static AnimalProfile CreateProfile(FishType fishType)
        {
            switch (fishType)
            {
                case FishType.Perch: return new AnimalProfile(AnimalType.Fish, 2, 0, 0, 0);
                case FishType.Carp: return new AnimalProfile(AnimalType.Fish, 4, 0, 0, 0);
                case FishType.Sturgeon: return new AnimalProfile(AnimalType.Fish, 7, 0, 0, 0);
                default: throw new ArgumentOutOfRangeException(nameof(fishType));
            }
        }
    }
}
