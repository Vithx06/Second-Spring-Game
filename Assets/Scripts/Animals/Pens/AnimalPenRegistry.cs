using System;
using System.Collections.Generic;

namespace SecondSpring.Animals
{
    public sealed class AnimalPenRegistry
    {
        private readonly Dictionary<AnimalType, AnimalPen> _pens = new Dictionary<AnimalType, AnimalPen>();

        public AnimalPenRegistry()
        {
            foreach (AnimalType type in Enum.GetValues(typeof(AnimalType))) _pens.Add(type, new AnimalPen(type));
        }

        public AnimalPen GetPen(AnimalType type)
        {
            if (!_pens.TryGetValue(type, out var pen)) throw new ArgumentOutOfRangeException(nameof(type));
            return pen;
        }
    }
}
