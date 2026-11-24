namespace SecondSpring.Exceptions.Economy
{
    public sealed class InsufficientFundsException : DomainException
    {
        public int RequiredAmount { get; }
        public int Balance { get; }

        public InsufficientFundsException(int requiredAmount, int balance) : base("The wallet has insufficient funds.")
        {
            RequiredAmount = requiredAmount;
            Balance = balance;
        }
    }
}
