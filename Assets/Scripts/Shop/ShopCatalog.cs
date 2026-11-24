using System;
using System.Collections.Generic;

public sealed class ShopCatalog
{
    public IReadOnlyList<ShopOffer> Offers { get; }
    public ShopCatalog(IEnumerable<ShopOffer> offers) { Offers=new List<ShopOffer>(offers).AsReadOnly(); }
    public ShopOffer FindOffer(string id) { foreach(var offer in Offers) if(offer.Item.Id==id) return offer;return null; }
}
