using System;
using System.Collections.Generic;

public sealed class ShopFacade
{
    private readonly IInventory _inventory;
    private readonly IWallet _wallet;
    private readonly ShopCatalog _catalog;
    public ShopFacade(IInventory inventory,IWallet wallet,ShopCatalog catalog)
    { _inventory=inventory??throw new ArgumentNullException(nameof(inventory)); _wallet=wallet??throw new ArgumentNullException(nameof(wallet));_catalog=catalog??throw new ArgumentNullException(nameof(catalog)); }
    public TradeResult Buy(string id,int quantity) => Trade(id,quantity,true);
    public TradeResult Sell(string id,int quantity) => Trade(id,quantity,false);
    private TradeResult Trade(string id,int quantity,bool buying)
    {
        if(quantity<=0) return TradeResult.InvalidQuantity;
        var offer=_catalog.FindOffer(id);
        if(offer==null) return TradeResult.ItemNotFound;
        if(buying && !offer.CanBuy) return TradeResult.NotBuyable;
        if(!buying && !offer.CanSell) return TradeResult.NotSellable;
        long total=(long)(buying ? offer.BuyPrice : offer.SellPrice)*quantity;
        if(total>int.MaxValue) return TradeResult.AmountOverflow;
        int amount=(int)total;
        if(buying)
        {
            if(!_inventory.CanAdd(offer.Item,quantity)) return TradeResult.InventoryFull;
            if(!_wallet.CanAfford(amount)) return TradeResult.NotEnoughMoney;
            if(!_wallet.TrySpend(amount)) return TradeResult.TransactionFailed;
            if(_inventory.TryAdd(offer.Item,quantity)) return TradeResult.Success;
            if(!_wallet.TryEarn(amount)) throw new InvalidOperationException("Buy rollback failed.");
        }
        else
        {
            if(_inventory.GetQuantity(id)<quantity) return TradeResult.NotEnoughItems;
            if((long)_wallet.Balance+amount>int.MaxValue) return TradeResult.AmountOverflow;
            if(!_inventory.TryRemove(id,quantity)) return TradeResult.TransactionFailed;
            if(_wallet.TryEarn(amount)) return TradeResult.Success;
            if(!_inventory.TryAdd(offer.Item,quantity)) throw new InvalidOperationException("Sell rollback failed.");
        }
        return TradeResult.TransactionFailed;
    }
}
