using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SecondSpring.Animals
{
    public sealed class AnimalPen
    {
        private readonly List<Animal> _animals = new List<Animal>();
        private readonly ReadOnlyCollection<Animal> _readOnlyAnimals;
        private readonly IAnimalGrowthStrategy _strategy;
        private int _fedDay;

        public AnimalType AnimalType { get; }
        public int Capacity { get; }
        public IReadOnlyList<Animal> Animals => _readOnlyAnimals;
        public int FedDay => _fedDay;

        public AnimalPen(AnimalType animalType)
        {
            AnimalType = animalType;
            switch (animalType)
            {
                case AnimalType.Chicken:
                case AnimalType.Duck: _strategy = new PoultryGrowthStrategy(); break;
                case AnimalType.Pig:
                case AnimalType.Cow: _strategy = new LivestockGrowthStrategy(); break;
                case AnimalType.Fish: _strategy = new FishGrowthStrategy(); break;
                default: throw new ArgumentOutOfRangeException(nameof(animalType));
            }
            Capacity = animalType == AnimalType.Fish ? 30 : 20;
            _readOnlyAnimals = _animals.AsReadOnly();
        }

        public bool AddAnimal(Animal animal)
        {
            if (animal == null) throw new ArgumentNullException(nameof(animal));
            if (animal.Type != AnimalType || _animals.Count >= Capacity || _animals.Exists(a => a.Id == animal.Id)) return false;
            _animals.Add(animal);
            return true;
        }

        public int GetFeedRequired() => (_animals.Count + 4) / 5;

        public PenStatus GetStatus(int day, int hour)
        {
            if (day < 1) throw new ArgumentOutOfRangeException(nameof(day));
            if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException(nameof(hour));
            if (_animals.Count == 0) return PenStatus.Empty;
            if (day <= _fedDay) return PenStatus.Fed;
            return hour < _strategy.GetFeedStartHour(AnimalType) ? PenStatus.BeforeFeedTime : PenStatus.AwaitingFeed;
        }

        // The callback must atomically remove the requested feed and report success.
        public bool Feed(int day, int hour, Func<FeedType, int, bool> tryConsumeFeed)
        {
            if (tryConsumeFeed == null) throw new ArgumentNullException(nameof(tryConsumeFeed));
            if (GetStatus(day, hour) != PenStatus.AwaitingFeed) return false;
            if (!tryConsumeFeed(_strategy.GetFeedType(), GetFeedRequired())) return false;
            _fedDay = day;
            return true;
        }
    }
}
