namespace SecondSpring.Exceptions.Inventory
{
    public sealed class InventoryCapacityExceededException : DomainException
    {
        public string ItemId { get; }
        public int RequestedQuantity { get; }

        public InventoryCapacityExceededException(string itemId, int requestedQuantity)
            : base("The inventory cannot hold the requested quantity.")
        {
            ItemId = itemId;
            RequestedQuantity = requestedQuantity;
        }
    }
}
