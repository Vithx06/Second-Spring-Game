namespace SecondSpring.Animals
{
    public sealed class Duck : Animal
    {
        public Duck(int id) : base(id, new AnimalProfile(AnimalType.Duck, 4, 2, 1, 3)) { }
    }
}
