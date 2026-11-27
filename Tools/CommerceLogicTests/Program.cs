using System;
using System.Collections.Generic;
int checks=0;
void Check(bool condition,string label) { checks++;if(!condition)throw new Exception(label); }
var seed=new ItemDefinition("rice.seed","Giống lúa");
var rice=new ItemDefinition("rice.product","Lúa");
var inventory=new Inventory();inventory.Register(seed);inventory.Register(rice);
var wallet=new Wallet(100);
var catalog=new ShopCatalog(new[]{new ShopOffer(seed,10,0),new ShopOffer(rice,0,8)});
var shop=new ShopFacade(inventory,wallet,catalog);
Check(shop.Buy(seed.Id,3)==TradeResult.Success,"buy");
Check(wallet.Balance==70 && inventory.GetQuantity(seed.Id)==3,"buy updates shared data");
Check(shop.Buy(seed.Id,8)==TradeResult.NotEnoughMoney,"insufficient coins");
Check(wallet.Balance==70 && inventory.GetQuantity(seed.Id)==3,"failed purchase unchanged");
Check(shop.Sell(seed.Id,1)==TradeResult.NotSellable,"seed not sellable");
Check(shop.Buy(rice.Id,1)==TradeResult.NotBuyable,"product not buyable");
inventory.TryAdd(rice,5);
Check(shop.Sell(rice.Id,2)==TradeResult.Success,"sell");
Check(wallet.Balance==86 && inventory.GetQuantity(rice.Id)==3,"sell updates shared data");
Check(shop.Sell(rice.Id,4)==TradeResult.NotEnoughItems,"oversell rejected");
Check(wallet.Balance==86 && inventory.GetQuantity(rice.Id)==3,"oversell unchanged");
Check(shop.Buy(seed.Id,0)==TradeResult.InvalidQuantity,"zero rejected");
Check(shop.Buy(seed.Id,-2)==TradeResult.InvalidQuantity,"negative rejected");
Check(shop.Buy("unknown",1)==TradeResult.ItemNotFound,"unknown rejected");
Check(shop.Buy(seed.Id,int.MaxValue)==TradeResult.AmountOverflow,"buy overflow");
var full=new Inventory(new ItemCatalog(new[]{seed,rice}),0);var fullShop=new ShopFacade(full,wallet,catalog);
Check(fullShop.Buy(seed.Id,1)==TradeResult.InventoryFull,"full warehouse rejected");
Check(wallet.Balance==86 && inventory.GetQuantity(seed.Id)==3,"full warehouse unchanged");

var rich=new Wallet(int.MaxValue);
Check(new ShopFacade(inventory,rich,catalog).Sell(rice.Id,1)==TradeResult.AmountOverflow,"wallet overflow");
Check(inventory.GetQuantity(rice.Id)==3,"overflow preserves goods");
var rejecting=new RejectAddInventory(inventory);
Check(new ShopFacade(rejecting,wallet,catalog).Buy(seed.Id,1)==TradeResult.TransactionFailed,"commit failure");
Check(wallet.Balance==86 && inventory.GetQuantity(seed.Id)==3,"buy refund");
var refusingWallet=new RefuseEarnWallet();
Check(new ShopFacade(inventory,refusingWallet,catalog).Sell(rice.Id,1)==TradeResult.TransactionFailed,"sell commit failure");
Check(inventory.GetQuantity(rice.Id)==3,"sell returns items");
Console.WriteLine($"Passed {checks} commerce checks.");
sealed class RejectAddInventory : IInventory
{
    private readonly IInventory _inner;
    public RejectAddInventory(IInventory inner) {_inner=inner;}
    public int GetQuantity(ItemType id)=>_inner.GetQuantity(id);
    public IReadOnlyList<ItemStack> GetItems()=>_inner.GetItems();
    public bool CanAdd(ItemType item,int quantity)=>true;
    public bool TryAdd(ItemType item,int quantity)=>false;
    public bool TryRemove(ItemType id,int quantity)=>_inner.TryRemove(id,quantity);
}
sealed class RefuseEarnWallet : IWallet
{
    public int Balance=>0;
    public bool CanAfford(int amount)=>false;
    public bool TrySpend(int amount)=>false;
    public bool TryEarn(int amount)=>false;
}
