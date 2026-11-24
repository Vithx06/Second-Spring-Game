namespace SecondSpring.Exceptions.Plants
{
    public sealed class InvalidPlotStateException : DomainException
    {
        public string Operation { get; }
        public string CurrentState { get; }

        public InvalidPlotStateException(string operation, string currentState)
            : base("The operation is not allowed in the current plot state.")
        {
            Operation = operation;
            CurrentState = currentState;
        }
    }
}
