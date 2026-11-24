namespace SecondSpring.Animals
{
    public sealed class DuckCreator : AnimalCreator
    {
        public override Animal CreateAnimal(int id) => new Duck(id);
    }
}
