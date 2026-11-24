using System;

namespace SecondSpring.Animals
{
    public sealed class FishCreator : AnimalCreator
    {
        private readonly FishType _fishType;

        public FishCreator(FishType fishType)
        {
            if (!Enum.IsDefined(typeof(FishType), fishType))
                throw new ArgumentOutOfRangeException(nameof(fishType));
            _fishType = fishType;
        }

        public override Animal CreateAnimal(int id) => new Fish(id, _fishType);
    }
}
