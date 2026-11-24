using System;

namespace SecondSpring.Animals
{
    public sealed class LivestockGrowthStrategy : IAnimalGrowthStrategy
    {
        public void AdvanceGrowth(Animal animal, bool penFed)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));
            GetFeedStartHour(animal.Type);
            animal.AdvanceOneDay(penFed);
        }

        public FeedType GetFeedType() => FeedType.Livestock;

        public int GetFeedStartHour(AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.Pig: return 11;
                case AnimalType.Cow: return 14;
                default: throw new ArgumentException("Expected pig or cow.", nameof(animalType));
            }
        }
    }
}
