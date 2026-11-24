using System;
using System.Collections.Generic;

namespace SecondSpring.Animals
{
    public sealed class AnimalPurchaseService
    {
        private readonly AnimalPenRegistry _pens;
        private readonly Func<AnimalType, FishType?, int?> _findPrice;
        private readonly Func<int> _nextId;
        private readonly Func<int, bool> _trySpend;
        private readonly Action<int> _refund;

        // The wallet callbacks must debit atomically and reliably restore a successful debit.
        public AnimalPurchaseService(AnimalPenRegistry pens, Func<AnimalType, FishType?, int?> findPrice,
            Func<int> nextId, Func<int, bool> trySpend, Action<int> refund)
        {
            _pens = pens ?? throw new ArgumentNullException(nameof(pens));
            _findPrice = findPrice ?? throw new ArgumentNullException(nameof(findPrice));
            _nextId = nextId ?? throw new ArgumentNullException(nameof(nextId));
            _trySpend = trySpend ?? throw new ArgumentNullException(nameof(trySpend));
            _refund = refund ?? throw new ArgumentNullException(nameof(refund));
        }

        public bool BuyAnimal(AnimalType type, int quantity, FishType? fishType = null)
        {
            if (quantity <= 0 || !Enum.IsDefined(typeof(AnimalType), type)) return false;
            if (type == AnimalType.Fish)
            {
                if (!fishType.HasValue || !Enum.IsDefined(typeof(FishType), fishType.Value)) return false;
            }
            else if (fishType.HasValue) return false;

            AnimalPen pen = _pens.GetPen(type);
            if (quantity > pen.Capacity - pen.Animals.Count) return false;
            int? unitPrice = _findPrice(type, fishType);
            if (!unitPrice.HasValue || unitPrice.Value < 0 || (long)unitPrice.Value * quantity > int.MaxValue) return false;
            int total = unitPrice.Value * quantity;
            AnimalCreator creator = ResolveCreator(type, fishType);
            var reservedIds = new HashSet<int>();
            foreach (AnimalType penType in Enum.GetValues(typeof(AnimalType)))
                foreach (Animal existing in _pens.GetPen(penType).Animals) reservedIds.Add(existing.Id);
            var animals = new List<Animal>();
            for (int i = 0; i < quantity; i++)
            {
                int id = _nextId();
                if (id <= 0 || !reservedIds.Add(id)) return false;
                animals.Add(creator.CreateAnimal(id));
            }
            if (!_trySpend(total)) return false;
            var added = new List<Animal>();
            bool completed = false;
            try
            {
                foreach (Animal animal in animals)
                {
                    if (!pen.AddAnimal(animal)) return false;
                    added.Add(animal);
                }
                completed = true;
                return true;
            }
            finally
            {
                if (!completed)
                {
                    foreach (Animal previous in added) pen.RemoveAnimal(previous);
                    _refund(total);
                }
            }
        }

        private static AnimalCreator ResolveCreator(AnimalType type, FishType? fishType)
        {
            switch (type)
            {
                case AnimalType.Chicken: return new ChickenCreator();
                case AnimalType.Duck: return new DuckCreator();
                case AnimalType.Pig: return new PigCreator();
                case AnimalType.Cow: return new CowCreator();
                case AnimalType.Fish: return new FishCreator(fishType.Value);
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
