namespace SecondSpring.Animals
{
    public sealed class Pig : Animal
    {
        public Pig(int id) : base(id, new AnimalProfile(AnimalType.Pig, 5, 0, 0, 6)) { }
    }
}
