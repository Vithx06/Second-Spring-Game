using System;
using System.Collections.Generic;

public interface IWallet
{
    int Balance { get; }
    bool CanAfford(int amount);
    bool TrySpend(int amount);
    bool TryEarn(int amount);
}

