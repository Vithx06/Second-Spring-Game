namespace SecondSpring.Animals
{
    public sealed class ChickenCreator : AnimalCreator
    {
        public override Animal CreateAnimal(int id) => new Chicken(id);
    }
}
