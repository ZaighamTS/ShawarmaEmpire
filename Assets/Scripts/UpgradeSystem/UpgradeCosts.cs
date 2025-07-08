using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
public enum UpgradeType
{
    Storage,
    DeliveryVan,
    Kitchen
}
public enum CapacityType
{
    Storage,
    Delivery
}
public enum ZoneType
{
    X,
    Y,
    Z
}
public record UpgradeConfig(int basePrice, float purchaseMultiplier, float upgradeMultiplier);
public record CapacityConfig(int baseCapacity, float capacityMultiplier);
public record ZoneConfig(int baseValue, float demandMultiplier);
public record PerstigeConfig(float incomePercentageMultipler, float speedPercentageMultiplier, float upgradeCostPerstigeReduction, float goldenShawarmaSpawnRate);

public static class UpgradeCosts
{
    /// <summary>
    /// Price Map
    /// </summary>
    private static readonly Dictionary<UpgradeType, UpgradeConfig> priceMap = new()
    {
        {UpgradeType.Storage,new(100,1.5f,1.5f) },
        {UpgradeType.DeliveryVan, new(100,1.5f,1.5f) },
        {UpgradeType.Kitchen,new (200,1.2f,1.3f) }
    };
    /// <summary>
    /// Capacity MAp
    /// </summary>
    private static readonly Dictionary<CapacityType, CapacityConfig> capacityMap = new()
    {
        {CapacityType.Storage,new(100,1.4f) },
        {CapacityType.Delivery,new(100,1.3f) }
    };
    /// <summary>
    /// Zone Multiplier Map
    /// </summary>
    private static readonly Dictionary<ZoneType, ZoneConfig> zoneMap = new()
    {
        {ZoneType.X,new(100,1.3f) },
        {ZoneType.Y,new(100,1.2f) }
    };

    public static float GetUpgradeCost(UpgradeType itemType, int level)
    {
        if (!priceMap.TryGetValue(itemType, out var config))
            throw new ArgumentException($"Unknown Item Type : {itemType}");
        return config.basePrice * Mathf.Pow(level, config.upgradeMultiplier);
    }

    public static float GetDeliveryCapacity(CapacityType capacityType, int level)
    {
        if (!capacityMap.TryGetValue(capacityType, out var config))
            throw new ArgumentException($"Unknown Capacity Type : {capacityType}");
        return config.baseCapacity * (1 + level * config.capacityMultiplier);
    }
    public static float GetShawarmaDemand(ZoneType zoneType)
    {
        if (!zoneMap.TryGetValue(zoneType, out var config))
            throw new ArgumentException($"Unknown Zone Type : {zoneType}");
        return config.baseValue * (1 * config.demandMultiplier);
    }

    public static float GetShawarmaValue(int qualityBonus)
    {
        float baseValue = 200;
        return baseValue * (1 + qualityBonus);
    }
    public static float GetCookRate(float tapRate, float tapPower, float auoChefBonus)
    {
        float baseValue = 200;
        return baseValue + (tapPower * tapRate) + auoChefBonus;
    }
    public static float GetDeliveryInterval(int upgradeLevel)
    {
        float basePrice = 100f;
        float multiplier = .2f;
        return basePrice / (1 + upgradeLevel * multiplier);
    }
    public static int GetChefStars(float totalEarnings)
    {
        return Mathf.FloorToInt(Mathf.Log10(totalEarnings / 100000));
    }
}
