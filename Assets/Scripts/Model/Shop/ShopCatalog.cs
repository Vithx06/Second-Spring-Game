using System;
using System.Collections.Generic;

public sealed class ShopCatalog
{
    public IReadOnlyList<ShopOffer> Offers { get; }
    public ShopCatalog(IEnumerable<ShopOffer> offers) { Offers=new List<ShopOffer>(offers).AsReadOnly(); }
    public ShopOffer FindOffer(string id) { foreach(var offer in Offers) if(ItemIdentifiers.TryParse(id,out var type) && offer.Item.Type==type) return offer;return null; }
}
