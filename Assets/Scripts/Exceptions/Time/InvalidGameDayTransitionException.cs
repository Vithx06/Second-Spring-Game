namespace SecondSpring.Exceptions.Time
{
    public sealed class InvalidGameDayTransitionException : DomainException
    {
        public int CurrentDay { get; }
        public int RequestedDay { get; }

        public InvalidGameDayTransitionException(int currentDay, int requestedDay)
            : base("Game days must be processed in order.")
        {
            CurrentDay = currentDay;
            RequestedDay = requestedDay;
        }
    }
}
