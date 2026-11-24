using System;
using System.Collections.Generic;

public sealed class ShopOffer
{
    public ItemDefinition Item { get; }
    public int BuyPrice { get; }
    public int SellPrice { get; }
    public bool CanBuy => BuyPrice>0;
    public bool CanSell => SellPrice>0;
    public ShopOffer(ItemDefinition item,int buyPrice,int sellPrice)
    {
        if(item==null || buyPrice<0 || sellPrice<0) throw new ArgumentException("Invalid shop offer.");
        Item=item;BuyPrice=buyPrice;SellPrice=sellPrice;
    }
}
