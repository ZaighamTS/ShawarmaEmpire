using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
public enum UpgradeType
{
    Storage,
    DeliveryVan,
    Kitchen,
    Catering
}
public enum CapacityType
{
    Storage,
    Delivery,
    Catering
}
public enum ZoneType
{
    X,
    Y,
    Z
}
public enum ExtraBuildingType
{
    JuicePoint,        // Base: $1,500 → $7,500 (5x)
    DessertPoint,     // Base: $2,500 → $12,500 (5x)
    Merchandise,       // Base: $4,000 → $20,000 (5x)
    Ingredients,      // Base: $7,500 → $37,500 (5x)
    Park,             // Base: $12,000 → $60,000 (5x)
    ShawarmaLounge,   // Base: $20,000 → $100,000 (5x)
    GasStation,       // Base: $35,000 → $175,000 (5x)
    Management        // Base: $60,000 → $300,000 (5x)
}
public record UpgradeConfig(float basePrice, float purchaseMultiplier, float upgradeMultiplier);
public record CapacityConfig(int baseCapacity, float capacityMultiplier);
public record ZoneConfig(int baseValue, float demandMultiplier);
public record ExtraBuildingConfig(float basePrice, float purchaseMultiplier);

public record PrestigeConfig(float incomePercentageMultipler, float speedPercentageMultiplier, float upgradeCostPerstigeReduction, float goldenShawarmaSpawnRate);


public static class UpgradeCosts
{
    /// <summary>
    /// Price Map
    /// EXTENDED GAMEPLAY: Increased base costs by 5x to extend gameplay to 1+ week
    /// - Storage (1.4x): High value bottleneck, prevents production stops
    /// - DeliveryVan (1.35x): Critical bottleneck, primary income source
    /// - Kitchen (1.3x): Production boost, moderate value
    /// - Catering (1.25x): Bonus income, secondary value
    /// </summary>
    private static readonly Dictionary<UpgradeType, UpgradeConfig> priceMap = new()
    {
        {UpgradeType.Storage,new(3750,1.5f,1.4f) },        // Reduced by 25%: 5000 → 3750
        {UpgradeType.DeliveryVan, new(1875,1.5f,1.35f) },   // Reduced by 25%: 2500 → 1875
        {UpgradeType.Kitchen,new (7500,1.2f,1.3f) },       // Reduced by 25%: 10000 → 7500
        {UpgradeType.Catering,new (5625,1.2f,1.25f) }      // Reduced by 25%: 7500 → 5625
    };
    /// <summary>
    /// Capacity Map
    /// Storage capacity now doubles with each upgrade: 250 → 500 → 1000 → 2000, etc.
    /// Delivery and Catering capacities use additive scaling
    /// </summary>
    internal static readonly Dictionary<CapacityType, CapacityConfig> capacityMap = new()
    {
        {CapacityType.Storage,new(250,2.0f) }, // Base: 250, doubles each level (multiplier used for doubling formula)
        // Reduced delivery capacity: 2 base, 0.4 multiplier (was 3/0.5f)
        // Smaller deliveries = slower income = extended gameplay
        {CapacityType.Delivery,new(2,0.4f) },
        // Reduced catering capacity: 3 base, 0.4 multiplier (was 5/0.5f)
        // Smaller orders = slower income = extended gameplay
        {CapacityType.Catering,new(3,0.4f) }
    };
    /// <summary>
    /// Zone Multiplier Map
    /// </summary>
    private static readonly Dictionary<ZoneType, ZoneConfig> zoneMap = new()
    {
        {ZoneType.X,new(100,1.3f) },
        {ZoneType.Y,new(100,1.2f) }
    };
    /// <summary>
    /// Extra Building Price Map
    /// EXTENDED GAMEPLAY: Increased base costs by 5x to extend gameplay to 1+ week
    /// Base prices: Juice Point $1,500, Dessert Point $2,500, then $4K, $7.5K, $12K, $20K, $35K, $60K
    /// After 5x increase: $7.5K, $12.5K, $20K, $37.5K, $60K, $100K, $175K, $300K
    /// </summary>
    private static readonly Dictionary<ExtraBuildingType, ExtraBuildingConfig> extraBuildingPriceMap = new()
    {
        {ExtraBuildingType.JuicePoint, new(5625, 3.5f) },        // Reduced by 25%: 7500 → 5625
        {ExtraBuildingType.DessertPoint, new(9375, 3.5f) },      // Reduced by 25%: 12500 → 9375
        {ExtraBuildingType.Merchandise, new(15000, 3.5f) },       // Reduced by 25%: 20000 → 15000
        {ExtraBuildingType.Ingredients, new(28125, 3.5f) },      // Reduced by 25%: 37500 → 28125
        {ExtraBuildingType.Park, new(45000, 3.5f) },             // Reduced by 25%: 60000 → 45000
        {ExtraBuildingType.ShawarmaLounge, new(75000, 3.5f) },   // Reduced by 25%: 100000 → 75000
        {ExtraBuildingType.GasStation, new(131250, 3.5f) },       // Reduced by 25%: 175000 → 131250
        {ExtraBuildingType.Management, new(225000, 3.5f) }       // Reduced by 25%: 300000 → 225000
    };
    public static readonly PrestigeConfig prestigeConfig = new(5f, 2f, -1f, 1f);

    // EXTENDED GAMEPLAY: Reduced base value by 50% to extend gameplay to 1+ week
    // Before: $100 (upgrades unlocked too quickly for week-long gameplay)
    // After: $50 (slower income = upgrades take 10-20 minutes, extending to 1+ week)
    public static float shwarmaBaseValue = 50;
    public static float cookRateBaseValue = 200;

    /// <summary>
    /// Gets the upgrade configuration for a given upgrade type.
    /// Used for economy testing and balance analysis.
    /// </summary>
    public static UpgradeConfig GetUpgradeConfig(UpgradeType itemType)
    {
        if (!priceMap.TryGetValue(itemType, out var config))
            throw new ArgumentException($"Unknown Item Type : {itemType}");
        return config;
    }
    
    /// <summary>
    /// Gets the purchase cost for a new building based on how many are already placed
    /// EXTENDED GAMEPLAY: Increased scaling from 2.5x to 3.5x to extend gameplay
    /// Uses 3.5x scaling: Warehouse 1 = $5000, Warehouse 2 = $17,500, Warehouse 3 = $61,250, etc.
    /// </summary>
    public static float GetPurchaseCost(UpgradeType itemType, int existingCount)
    {
        if (!priceMap.TryGetValue(itemType, out var config))
            throw new ArgumentException($"Unknown Item Type : {itemType}");
        
        float costReduction = GetPerstigeCostReduction(PlayerProgress.Instance.ChefStars);
        float baseCost = config.basePrice - costReduction;
        
        // First purchase costs base price, then multiply by 3.5x for each additional purchase
        // Warehouse 1: $5000, Warehouse 2: $17,500, Warehouse 3: $61,250, Warehouse 4: $214,375, etc.
        if (existingCount <= 0)
        {
            return baseCost; // First warehouse costs base price
        }
        
        float scalingFactor = 3.5f; // Increased from 2.5x to 3.5x for extended gameplay
        return baseCost * Mathf.Pow(scalingFactor, existingCount);
    }
    
    /// <summary>
    /// Gets the purchase cost for an extra building based on how many are already purchased
    /// EXTENDED GAMEPLAY: Uses 3.5x scaling to extend gameplay
    /// First purchase costs base price, then multiply by 3.5x for each additional purchase of same type
    /// </summary>
    public static float GetExtraBuildingCost(ExtraBuildingType buildingType, int existingCount)
    {
        if (!extraBuildingPriceMap.TryGetValue(buildingType, out var config))
            throw new ArgumentException($"Unknown Extra Building Type: {buildingType}");
        
        float costReduction = GetPerstigeCostReduction(PlayerProgress.Instance.ChefStars);
        float baseCost = config.basePrice - costReduction;
        
        // First purchase costs base price, then multiply by 3.5x for each additional purchase
        if (existingCount <= 0)
        {
            return baseCost;
        }
        
        return baseCost * Mathf.Pow(config.purchaseMultiplier, existingCount);
    }
    
    public static float GetUpgradeCost(UpgradeType itemType, int level)
    {
        if (!priceMap.TryGetValue(itemType, out var config))
            throw new ArgumentException($"Unknown Item Type : {itemType}");
        float costReduction = GetPerstigeCostReduction(PlayerProgress.Instance.ChefStars);
        float baseCost = config.basePrice - costReduction;
        
        // FIXED: Level 1 returns exact base price, diminishing returns only apply from level 2+
        if (level <= 1)
        {
            return baseCost;
        }
        
        // FIXED: Implement soft exponential scaling with diminishing returns
        // This prevents late-game costs from becoming astronomical
        // Formula: baseCost * (level^multiplier) * (1 / (1 + level * 0.1))
        // The diminishing factor reduces cost growth as level increases
        float exponentialPart = Mathf.Pow(level, config.upgradeMultiplier);
        float diminishingFactor = 1f / (1f + level * 0.1f);  // Diminishing returns
        
        return baseCost * exponentialPart * diminishingFactor;
    }

    public static float GetDeliveryCapacity(CapacityType capacityType, int level)
    {
        if (!capacityMap.TryGetValue(capacityType, out var config))
            throw new ArgumentException($"Unknown Capacity Type : {capacityType}");
        
        // Storage capacity doubles with each upgrade: 250 → 500 → 1000 → 2000, etc.
        // Warehouse level mapping:
        // currentUpdate = 1 → Level 0 (unpurchased) → 0 capacity
        // currentUpdate = 2 → Level 1 (purchased) → 250 capacity
        // currentUpdate = 3 → Level 2 (first upgrade) → 500 capacity
        // currentUpdate = 4 → Level 3 (second upgrade) → 1000 capacity
        if (capacityType == CapacityType.Storage)
        {
            // Handle unpurchased warehouse (currentUpdate = 1, Level 0)
            if (level <= 1)
            {
                return 0f; // Unpurchased warehouses have no capacity
            }
            
            // Formula: baseCapacity * 2^(currentUpdate - 2)
            // Level 1 (currentUpdate = 2): 250 * 2^0 = 250
            // Level 2 (currentUpdate = 3): 250 * 2^1 = 500
            // Level 3 (currentUpdate = 4): 250 * 2^2 = 1000
            // Level 4 (currentUpdate = 5): 250 * 2^3 = 2000
            return config.baseCapacity * Mathf.Pow(2, level - 2);
        }
        
        // Delivery and Catering use additive scaling
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
        float extraValue = GetPerstigeExtraIncome(PlayerProgress.Instance.ChefStars);

        // FIXED: Read actual upgrade levels from PlayerPrefs instead of hardcoded level 1
        int breadLevel = PlayerPrefs.GetInt("Bread", 0);
        int chickenLevel = PlayerPrefs.GetInt("Chicken", 0);
        int sauceLevel = PlayerPrefs.GetInt("Sause", 0); // Note: typo in PlayerPrefs key

        float breadValue = GetBreadUpgradeValue(breadLevel);
        float chickenValue = GetChickenUpgradeValue(chickenLevel);
        float sauceValue = GetSauceUpgradeValue(sauceLevel);

        float sum = breadValue + chickenValue + sauceValue + extraValue;
        return (shwarmaBaseValue + sum) * (1 + qualityBonus);
    }
    public static float GetCookRate(float tapRate, float tapPower, float auoChefBonus)
    {
        float extraValue = GetPrestigeExtraCookRate(PlayerProgress.Instance.ChefStars);
        float machineRate = GetMachineUpgradeCookRate(1);
        return (cookRateBaseValue + extraValue+machineRate) + (tapPower * tapRate) + auoChefBonus;
    }
    public static float GetDeliveryInterval(int upgradeLevel)
    {
        // EXTENDED GAMEPLAY: Slowed down delivery intervals to extend gameplay to 1+ week
        // Before: 30s base (too fast for week-long gameplay)
        // After: 60s base, 0.05 multiplier (2x slower, slower improvement)
        // Level 0: 60s, Level 1: 57.1s, Level 5: 48s, Level 10: 40s
        float baseInterval = 60f;  // Increased from 30f (2x slower)
        float multiplier = 0.05f;   // Reduced from 0.08f (slower improvement per level)
        return baseInterval / (1 + upgradeLevel * multiplier);
    }
    public static float GetCateringInterval(int upgradeLevel)
    {
        // EXTENDED GAMEPLAY: Slowed down catering intervals to extend gameplay to 1+ week
        // Before: 45s base (too fast for week-long gameplay)
        // After: 90s base, 0.05 multiplier (2x slower, slower improvement)
        // Level 0: 90s, Level 1: 85.7s, Level 5: 72s, Level 10: 60s
        float baseInterval = 90f;  // Increased from 45f (2x slower)
        float multiplier = 0.05f;   // Reduced from 0.08f (slower improvement per level)
        return baseInterval / (1 + upgradeLevel * multiplier);
    }
    public static int GetChefStars(float totalEarnings)
    {
        // EXTENDED GAMEPLAY: Increased prestige thresholds by 10x to extend gameplay
        // Before: $10,000 base (first prestige at $100K)
        // After: $100,000 base (first prestige at $1M)
        // This extends first prestige from ~10-20 hours to ~50-100 hours
        return Mathf.FloorToInt(Mathf.Log10(totalEarnings / (100000)));
    }
    public static int GetNextPrestigeValue()
    {
        // EXTENDED GAMEPLAY: Increased prestige thresholds by 10x
        // Before: 100,000 multiplier (prestige at $100K, $1M, $10M)
        // After: 1,000,000 multiplier (prestige at $1M, $10M, $100M)
        int nextStar = PlayerProgress.Instance.ChefStars;
        return Mathf.FloorToInt(Mathf.Pow(10, nextStar) * 1000000f);
    }
    public static float GetTotalSalaries(int officeLevel)
    {
        return 1;
    }


    // FIXED: Prestige bonuses now provide significant value
    // Before: 5% income, 0.1% cost reduction per star
    // After: 10% income, 2.5% cost reduction per star
    static float GetPerstigeExtraIncome(int level)
    {
        return level * 0.1f * shwarmaBaseValue;  // 10% of base per star (was 5%)
    }
    static float GetPerstigeCostReduction(int level)
    {
        return level * 0.025f * shwarmaBaseValue;  // 2.5% of base per star (was 0.1%)
    }
    static float GetPrestigeExtraCookRate(int level)
    {
        return level * 0.04f * shwarmaBaseValue;  // 4% of base per star (was 2%)

    }

    #region Raw Material Upgrades
    // FIXED: Material upgrades now provide meaningful value (2.5-4% of base per level)
    // Before: 0.03-0.04 per level (0.015-0.02% of base 200)
    // After: 5-8 per level (2.5-4% of base 200)
    public static float GetBreadUpgradeValue(int upgradeLevel)
    {
        return upgradeLevel * 5f;  // 2.5% of base per level
    }
    public static float GetChickenUpgradeValue(int upgradeLevel)
    {
        return upgradeLevel * 8f;  // 4% of base per level
    }
    public static float GetSauceUpgradeValue(int upgradeLevel)
    {
        return upgradeLevel * 3f;  // 1.5% of base per level
    }
    #endregion
    #region Cooking Machines Upgrade
    public static float GetMachineUpgradeCookRate(int level)
    {
        return level * 0.1f;
    }
    #endregion
    
    #region Automatic Earning Multiplier
    /// <summary>
    /// Calculates the total multiplier for automatic earning based on prestige and upgrades
    /// Formula: 1.0 + (prestigeMultiplier) + (upgradeMultiplier)
    /// - Prestige: +5% per Chef Star (0.05 per star)
    /// - Upgrades: +1% per upgrade level across all upgrade types (0.01 per level)
    /// </summary>
    public static float GetAutomaticEarningMultiplier()
    {
        float prestigeMultiplier = GetPrestigeAutomaticEarningMultiplier();
        float upgradeMultiplier = GetUpgradeAutomaticEarningMultiplier();
        
        // Base multiplier is 1.0, then add bonuses
        return 1.0f + prestigeMultiplier + upgradeMultiplier;
    }
    
    /// <summary>
    /// Gets prestige multiplier for automatic earning
    /// +5% per Chef Star
    /// </summary>
    private static float GetPrestigeAutomaticEarningMultiplier()
    {
        int chefStars = PlayerProgress.Instance.ChefStars;
        return chefStars * 0.05f; // 5% per star
    }
    
    /// <summary>
    /// Gets upgrade multiplier for automatic earning
    /// +1% per upgrade level across all upgrade types
    /// </summary>
    private static float GetUpgradeAutomaticEarningMultiplier()
    {
        float totalUpgradeLevels = 0f;
        
        // Sum all Storage upgrade levels
        if (WarehouseManager.Instance != null && WarehouseManager.Instance.placedWarehouses != null)
        {
            foreach (var warehouse in WarehouseManager.Instance.placedWarehouses)
            {
                if (warehouse != null)
                {
                    var warehouseComponent = warehouse.GetComponent<Warehouse>();
                    if (warehouseComponent != null)
                    {
                        // currentUpdate - 1 because level 1 is base (no upgrade)
                        totalUpgradeLevels += Mathf.Max(0, warehouseComponent.currentUpdate - 1);
                    }
                }
            }
        }
        
        // Sum all Delivery upgrade levels
        if (DeliveryManager.Instance != null && DeliveryManager.Instance.Deliverys != null)
        {
            foreach (var delivery in DeliveryManager.Instance.Deliverys)
            {
                if (delivery != null)
                {
                    var deliveryComponent = delivery.GetComponent<Delivery>();
                    if (deliveryComponent != null && deliveryComponent.currentUpdate > 1)
                    {
                        totalUpgradeLevels += Mathf.Max(0, deliveryComponent.currentUpdate - 1);
                    }
                }
            }
        }
        
        // Sum all Kitchen upgrade levels
        if (KitchenManager.Instance != null && KitchenManager.Instance.Kitchens != null)
        {
            foreach (var kitchen in KitchenManager.Instance.Kitchens)
            {
                if (kitchen != null)
                {
                    var kitchenComponent = kitchen.GetComponent<Kitchen>();
                    if (kitchenComponent != null && kitchenComponent.currentUpdate > 1)
                    {
                        totalUpgradeLevels += Mathf.Max(0, kitchenComponent.currentUpdate - 1);
                    }
                }
            }
        }
        
        // Sum all Catering upgrade levels
        if (CateringManager.Instance != null && CateringManager.Instance.Caterings != null)
        {
            foreach (var catering in CateringManager.Instance.Caterings)
            {
                if (catering != null)
                {
                    var cateringComponent = catering.GetComponent<Catering>();
                    if (cateringComponent != null && cateringComponent.currentUpdate > 1)
                    {
                        totalUpgradeLevels += Mathf.Max(0, cateringComponent.currentUpdate - 1);
                    }
                }
            }
        }
        
        // 1% per upgrade level
        return totalUpgradeLevels * 0.01f;
    }
    #endregion
}
