namespace SecondSpring.Exceptions.Animals
{
    public sealed class PenCapacityExceededException : DomainException
    {
        public int Capacity { get; }
        public int CurrentCount { get; }
        public int RequestedCount { get; }

        public PenCapacityExceededException(int capacity, int currentCount, int requestedCount)
            : base("The animal pen does not have enough capacity.")
        {
            Capacity = capacity;
            CurrentCount = currentCount;
            RequestedCount = requestedCount;
        }
    }
}
