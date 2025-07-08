using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
public enum ItemType
{
    Storage,
    DeliveryVan
}
public record PriceConfig(int BasePrice, float purchaseMultiplier, float upgradeMultiplier);
public static class UpgradeCosts
{
    private static readonly Dictionary<ItemType, PriceConfig> priceMap = new()
    {
        {ItemType.Storage,new(100,1.5f,1.5f) }
    };
    public static float GetUpgradeCost(ItemType itemType, int level)
    {
        if (!priceMap.TryGetValue(itemType, out var config))
            throw new ArgumentException($"Unknown Item Type : {itemType}");
        return config.BasePrice * Mathf.Pow(level, config.upgradeMultiplier);
    }
}
