using System;
using System.Collections.Generic;

namespace SecondSpring.Animals
{
    public sealed class AnimalService
    {
        private readonly AnimalPenRegistry _pens;
        private int _currentDay = 1;

        public AnimalService(AnimalPenRegistry pens)
        {
            _pens = pens ?? throw new ArgumentNullException(nameof(pens));
        }

        public bool FeedPen(AnimalType type, int day, int hour, Func<FeedType, int, bool> tryConsumeFeed)
        {
            if (day != _currentDay) throw new ArgumentOutOfRangeException(nameof(day));
            return _pens.GetPen(type).Feed(day, hour, tryConsumeFeed);
        }

        public void OnDayChanged(int day)
        {
            if (day != _currentDay + 1) throw new ArgumentOutOfRangeException(nameof(day), "Days must be processed in order.");
            foreach (AnimalType type in Enum.GetValues(typeof(AnimalType)))
            {
                AnimalPen pen = _pens.GetPen(type);
                IAnimalGrowthStrategy strategy = CreateStrategy(type);
                foreach (Animal animal in pen.Animals) strategy.AdvanceGrowth(animal, pen.WasFed(animal, _currentDay));
            }
            _currentDay = day;
        }

        private static IAnimalGrowthStrategy CreateStrategy(AnimalType type)
        {
            switch (type)
            {
                case AnimalType.Chicken:
                case AnimalType.Duck: return new PoultryGrowthStrategy();
                case AnimalType.Pig:
                case AnimalType.Cow: return new LivestockGrowthStrategy();
                case AnimalType.Fish: return new FishGrowthStrategy();
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        public List<FishType> GetAdultFishTypes()
        {
            var types = new List<FishType>();
            foreach (Animal animal in _pens.GetPen(AnimalType.Fish).Animals)
            {
                if (animal is Fish fish && fish.IsAdult() && !types.Contains(fish.FishType)) types.Add(fish.FishType);
            }
            return types;
        }

        public bool ConsumeCaughtFish(FishType type)
        {
            if (!Enum.IsDefined(typeof(FishType), type)) throw new ArgumentOutOfRangeException(nameof(type));
            AnimalPen pond = _pens.GetPen(AnimalType.Fish);
            foreach (Animal animal in pond.Animals)
            {
                if (animal is Fish fish && fish.FishType == type && fish.IsAdult()) return pond.RemoveAnimal(fish);
            }
            return false;
        }
    }
}
