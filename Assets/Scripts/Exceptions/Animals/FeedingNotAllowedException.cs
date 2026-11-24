namespace SecondSpring.Exceptions.Animals
{
    public enum FeedingBlockReason { EmptyPen, BeforeFeedingTime, AlreadyFed }

    public sealed class FeedingNotAllowedException : DomainException
    {
        public FeedingBlockReason Reason { get; }

        public FeedingNotAllowedException(FeedingBlockReason reason) : base("Feeding is not allowed for this pen.")
        {
            Reason = reason;
        }
    }
}
