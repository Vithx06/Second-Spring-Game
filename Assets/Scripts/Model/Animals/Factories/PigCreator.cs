namespace SecondSpring.Animals
{
    public sealed class PigCreator : AnimalCreator
    {
        public override Animal CreateAnimal(int id) => new Pig(id);
    }
}
