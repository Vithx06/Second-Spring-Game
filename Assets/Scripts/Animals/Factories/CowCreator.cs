namespace SecondSpring.Animals
{
    public sealed class CowCreator : AnimalCreator
    {
        public override Animal CreateAnimal(int id) => new Cow(id);
    }
}
