using System;

namespace SecondSpring.Exceptions.Shop
{
    public sealed class TransactionFailedException : DomainException
    {
        public TransactionFailedException(string message) : base(message) { }
        public TransactionFailedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
