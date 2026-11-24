using System;

namespace SecondSpring.Animals
{
    public sealed class AnimalProfile
    {
        public AnimalType AnimalType { get; }
        public int MaturityDays { get; }
        public int ProductionCycleDays { get; }
        public int ProductUnits { get; }
        public int MeatYield { get; }

        public AnimalProfile(AnimalType animalType, int maturityDays, int productionCycleDays, int productUnits, int meatYield)
        {
            if (!Enum.IsDefined(typeof(AnimalType), animalType)) throw new ArgumentOutOfRangeException(nameof(animalType));
            if (maturityDays <= 0) throw new ArgumentOutOfRangeException(nameof(maturityDays));
            if (productionCycleDays < 0) throw new ArgumentOutOfRangeException(nameof(productionCycleDays));
            if (productUnits < 0) throw new ArgumentOutOfRangeException(nameof(productUnits));
            if (meatYield < 0) throw new ArgumentOutOfRangeException(nameof(meatYield));
            if ((productionCycleDays == 0) != (productUnits == 0)) throw new ArgumentException("Production cycle and units must both be zero or positive.");
            AnimalType = animalType;
            MaturityDays = maturityDays;
            ProductionCycleDays = productionCycleDays;
            ProductUnits = productUnits;
            MeatYield = meatYield;
        }
    }
}
