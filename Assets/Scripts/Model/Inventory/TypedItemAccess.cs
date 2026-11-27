using System;

// Cầu nối cho prototype cũ. Bên trong Inventory/IInventory và FarmingService đều dùng ItemType.
public static class ItemIdentifiers
{
    public static string GetId(ItemType type)
    {
        switch(type)
        {
            case ItemType.RiceSeed: return "rice.seed";
            case ItemType.CornSeed: return "corn.seed";
            case ItemType.SugarcaneSeed: return "sugarcane.seed";
            case ItemType.RoseSeed: return "rose.seed";
            case ItemType.OrchidSeed: return "orchid.seed";
            case ItemType.StrawberrySeed: return "strawberry.seed";
            case ItemType.AppleSeed: return "apple.seed";
            case ItemType.OrangeSeed: return "orange.seed";
            case ItemType.Rice: return "rice.product";
            case ItemType.Corn: return "corn.product";
            case ItemType.Sugarcane: return "sugarcane.product";
            case ItemType.Rose: return "rose.product";
            case ItemType.Orchid: return "orchid.product";
            case ItemType.Strawberry: return "strawberry.product";
            case ItemType.Apple: return "apple.product";
            case ItemType.Orange: return "orange.product";
            case ItemType.ChickenEgg: return "animal.chicken-egg";
            case ItemType.DuckEgg: return "animal.duck-egg";
            case ItemType.Milk: return "animal.milk";
            case ItemType.ChickenMeat: return "animal.chicken-meat";
            case ItemType.DuckMeat: return "animal.duck-meat";
            case ItemType.CowMeat: return "animal.beef";
            case ItemType.PigMeat: return "animal.pork";
            case ItemType.Perch: return "fish.perch";
            case ItemType.Carp: return "fish.carp";
            case ItemType.Sturgeon: return "fish.sturgeon";
            case ItemType.PoultryFeed: return "feed.poultry";
            case ItemType.LivestockFeed: return "feed.livestock";
            case ItemType.FishFeed: return "feed.fish";
            case ItemType.YieldBoostFertilizer: return "fertilizer.yield";
            case ItemType.GrowthBoostFertilizer: return "fertilizer.growth";
            case ItemType.Manure: return "manure";
            case ItemType.Hoe: return "tool.hoe";
            case ItemType.WateringCan: return "tool.watering-can";
            default: throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
    public static bool TryParse(string id,out ItemType type)
    {
        foreach(ItemType candidate in Enum.GetValues(typeof(ItemType)))
            if(GetId(candidate)==id) { type=candidate;return true; }
        // Các bản thử trước đây đặt tên sản phẩm vật nuôi theo hai cách.
        switch(id)
        {
            case "animal.chickenegg":type=ItemType.ChickenEgg;return true;
            case "animal.duckegg":type=ItemType.DuckEgg;return true;
            case "animal.cowmilk":type=ItemType.Milk;return true;
            case "animal.chickenmeat":type=ItemType.ChickenMeat;return true;
            case "animal.duckmeat":type=ItemType.DuckMeat;return true;
            case "animal.pigmeat":type=ItemType.PigMeat;return true;
            case "animal.cowmeat":type=ItemType.CowMeat;return true;
            default:type=default;return false;
        }
    }
    public static ItemType Parse(string id) => TryParse(id,out var type)?type:throw new ArgumentException("Unknown legacy item ID: "+id);
}


// API cũ được chuyển đổi tại biên; không tạo thêm một kho thứ hai.
public static class LegacyItemAccess
{
    public static ItemDefinition FindItem(this IItemCatalog catalog,string id) => ItemIdentifiers.TryParse(id,out var type)?catalog.FindItem(type):null;
    public static int GetQuantity(this IInventory inventory,string id) => ItemIdentifiers.TryParse(id,out var type)?inventory.GetQuantity(type):0;
    public static bool TryRemove(this IInventory inventory,string id,int quantity) => ItemIdentifiers.TryParse(id,out var type) && inventory.TryRemove(type,quantity);
    public static bool CanAdd(this IInventory inventory,ItemDefinition item,int quantity) => item!=null && inventory.CanAdd(item.Type,quantity);
    public static bool TryAdd(this IInventory inventory,ItemDefinition item,int quantity) => item!=null && inventory.TryAdd(item.Type,quantity);
}
