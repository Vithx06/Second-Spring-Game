namespace SecondSpring.Animals
{
    public interface IAnimalGrowthStrategy
    {
        void AdvanceGrowth(Animal animal, bool penFed);
        FeedType GetFeedType();
        int GetFeedStartHour(AnimalType animalType);
    }
}
