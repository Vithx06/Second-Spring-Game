namespace SecondSpring.Animals
{
    public sealed class Chicken : Animal
    {
        public Chicken(int id) : base(id, new AnimalProfile(AnimalType.Chicken, 3, 1, 1, 2)) { }
    }
}
