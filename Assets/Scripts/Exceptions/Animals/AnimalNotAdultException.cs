namespace SecondSpring.Exceptions.Animals
{
    public sealed class AnimalNotAdultException : DomainException
    {
        public int AnimalId { get; }

        public AnimalNotAdultException(int animalId) : base("The animal is not mature.")
        {
            AnimalId = animalId;
        }
    }
}
