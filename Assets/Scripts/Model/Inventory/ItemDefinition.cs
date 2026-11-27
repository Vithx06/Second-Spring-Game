using System;

public sealed class ItemDefinition
{
    public ItemType Type { get; }
    public string Name { get; }
    public ItemCategory Category { get; }

    public ItemDefinition(ItemType type,string name,ItemCategory category)
    {
        if(!Enum.IsDefined(typeof(ItemType),type) || !Enum.IsDefined(typeof(ItemCategory),category) || string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Invalid item definition.");
        Type=type;Name=name;Category=category;
    }

    // Hai thành viên này dành cho prototype cũ. Kho và trồng trọt mới không dùng string ID.
    public string Id => ItemIdentifiers.GetId(Type);
    public ItemDefinition(string id,string name,int maxStackSize=50)
        :this(ItemIdentifiers.Parse(id),name,ItemCatalog.CategoryOf(ItemIdentifiers.Parse(id)))
    {
        if(maxStackSize<1)throw new ArgumentOutOfRangeException(nameof(maxStackSize));
    }
}
