namespace SecondSpring.Animals
{
    public sealed class Cow : Animal
    {
        public Cow(int id) : base(id, new AnimalProfile(AnimalType.Cow, 7, 2, 2, 8)) { }
    }
}
