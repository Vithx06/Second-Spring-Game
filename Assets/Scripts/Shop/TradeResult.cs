using System;
using System.Collections.Generic;

public enum TradeResult { Success, InvalidQuantity, ItemNotFound, NotBuyable, NotSellable, NotEnoughMoney, NotEnoughItems, InventoryFull, AmountOverflow, TransactionFailed }
