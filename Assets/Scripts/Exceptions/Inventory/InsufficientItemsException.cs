namespace SecondSpring.Exceptions.Inventory
{
    public sealed class InsufficientItemsException : DomainException
    {
        public string ItemId { get; }
        public int RequiredQuantity { get; }
        public int AvailableQuantity { get; }

        public InsufficientItemsException(string itemId, int requiredQuantity, int availableQuantity)
            : base("There are not enough items in the inventory.")
        {
            ItemId = itemId;
            RequiredQuantity = requiredQuantity;
            AvailableQuantity = availableQuantity;
        }
    }
}
