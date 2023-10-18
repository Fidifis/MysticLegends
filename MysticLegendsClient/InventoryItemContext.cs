namespace MysticLegendsClient
{
    public struct InventoryItemContext
    {
        public object Owner { get; set; }
        public int Id { get; set; }

        public InventoryItemContext(object owner, int id)
        {
            Owner = owner;
            Id = id;
        }
    }
}
