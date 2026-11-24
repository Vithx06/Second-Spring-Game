using System;
using System.Collections.Generic;

public sealed class Wallet : IWallet
{
    public int Balance { get; private set; }
    public Wallet(int initialBalance) { if(initialBalance<0) throw new ArgumentOutOfRangeException(nameof(initialBalance)); Balance=initialBalance; }
    public bool CanAfford(int amount) => amount>=0 && Balance>=amount;
    public bool TrySpend(int amount) { if(!CanAfford(amount)) return false; Balance-=amount;return true; }
    public bool TryEarn(int amount) { if(amount<0 || (long)Balance+amount>int.MaxValue) return false; Balance+=amount;return true; }
}
