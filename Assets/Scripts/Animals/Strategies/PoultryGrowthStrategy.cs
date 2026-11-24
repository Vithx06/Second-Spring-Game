using System;

namespace SecondSpring.Animals
{
    public sealed class PoultryGrowthStrategy : IAnimalGrowthStrategy
    {
        public void AdvanceGrowth(Animal animal, bool penFed)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));
            GetFeedStartHour(animal.Type);
            animal.AdvanceOneDay(penFed);
        }

        public FeedType GetFeedType() => FeedType.Poultry;

        public int GetFeedStartHour(AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.Chicken: return 7;
                case AnimalType.Duck: return 9;
                default: throw new ArgumentException("Expected chicken or duck.", nameof(animalType));
            }
        }
    }
}
