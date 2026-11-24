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
            if (day != _currentDay)
                throw new ArgumentOutOfRangeException(nameof(day));
            return _pens.GetPen(type).Feed(day, hour, tryConsumeFeed);
        }

        public void OnDayChanged(int day)
        {
            if (day != _currentDay + 1)
                throw new ArgumentOutOfRangeException(nameof(day), "Days must be processed in order.");
            foreach (AnimalType type in Enum.GetValues(typeof(AnimalType)))
            {
                AnimalPen pen = _pens.GetPen(type);
                IAnimalGrowthStrategy strategy = CreateStrategy(type);
                foreach (Animal animal in pen.Animals)
                {
                    bool fed = pen.WasFed(animal, _currentDay);
                    strategy.AdvanceGrowth(animal, fed);
                    AnimalProfile profile = animal.Profile;
                    // Chu kỳ tính từ ngày trưởng thành; ngày bỏ ăn không tạo sản phẩm.
                    if (fed && animal.IsAdult() && profile.ProductionCycleDays > 0 &&
                        (animal.Age - animal.MaturityAge) % profile.ProductionCycleDays == 0)
                        pen.StoreProducts(profile.ProductUnits);
                }
            }
            _currentDay = day;
        }

        // Kho trả về số lượng đã nhận; phần còn lại vẫn ở chuồng.
        public int CollectProducts(AnimalType type, Func<AnimalProductType, int, int> tryStore)
        {
            if (tryStore == null)
                throw new ArgumentNullException(nameof(tryStore));
            AnimalProductType product;
            switch (type)
            {
                case AnimalType.Chicken: product = AnimalProductType.ChickenEgg; break;
                case AnimalType.Duck: product = AnimalProductType.DuckEgg; break;
                case AnimalType.Cow: product = AnimalProductType.CowMilk; break;
                default: return 0;
            }
            AnimalPen pen = _pens.GetPen(type);
            int available = pen.StoredProducts;
            if (available == 0)
                return 0;
            int accepted = tryStore(product, available);
            if (accepted < 0 || accepted > available)
                throw new InvalidOperationException("Invalid accepted product quantity.");
            pen.RemoveStoredProducts(accepted);
            return accepted;
        }

        // Chỉ loại con vật khi kho đã nhận đủ thịt.
        public bool CollectMeat(AnimalType type, int animalId, Func<AnimalProductType, int, bool> tryStore)
        {
            if (tryStore == null)
                throw new ArgumentNullException(nameof(tryStore));
            AnimalProductType product;
            switch (type)
            {
                case AnimalType.Chicken: product = AnimalProductType.ChickenMeat; break;
                case AnimalType.Duck: product = AnimalProductType.DuckMeat; break;
                case AnimalType.Pig: product = AnimalProductType.PigMeat; break;
                case AnimalType.Cow: product = AnimalProductType.CowMeat; break;
                default: return false;
            }
            AnimalPen pen = _pens.GetPen(type);
            foreach (Animal animal in pen.Animals)
            {
                if (animal.Id != animalId || !animal.IsAdult())
                    continue;
                if (!tryStore(product, animal.Profile.MeatYield))
                    return false;
                return pen.RemoveAnimal(animal);
            }
            return false;
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
                if (animal is Fish fish && fish.IsAdult() && !types.Contains(fish.FishType))
                    types.Add(fish.FishType);
            }
            return types;
        }

        public bool ConsumeCaughtFish(FishType type)
        {
            if (!Enum.IsDefined(typeof(FishType), type))
                throw new ArgumentOutOfRangeException(nameof(type));
            AnimalPen pond = _pens.GetPen(AnimalType.Fish);
            foreach (Animal animal in pond.Animals)
            {
                if (animal is Fish fish && fish.FishType == type && fish.IsAdult())
                    return pond.RemoveAnimal(fish);
            }
            return false;
        }
    }
}
