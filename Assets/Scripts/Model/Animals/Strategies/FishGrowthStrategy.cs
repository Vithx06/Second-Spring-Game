using System;

namespace SecondSpring.Animals
{
    public sealed class FishGrowthStrategy : IAnimalGrowthStrategy
    {
        public void AdvanceGrowth(Animal animal, bool penFed)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));
            GetFeedStartHour(animal.Type);
            animal.AdvanceOneDay(penFed);
        }

        public FeedType GetFeedType() => FeedType.Fish;

        public int GetFeedStartHour(AnimalType animalType)
        {
            if (animalType != AnimalType.Fish)
                throw new ArgumentException("Expected fish.", nameof(animalType));
            return 17;
        }
    }
}
