using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Centralized economy calculator for all game economy calculations.
/// This class provides a single source of truth for economy formulas,
/// making it easier to balance, test, and maintain the game economy.
/// </summary>
public static class EconomyCalculator
{
    #region Income Calculations
    
    /// <summary>
    /// Calculates earnings from delivering shawarmas.
    /// Formula: shawarmaValue * quantity * taxRate
    /// </summary>
    /// <param name="shawarmaValue">Base value of a single shawarma</param>
    /// <param name="quantity">Number of shawarmas delivered</param>
    /// <param name="taxRate">Tax/deduction rate (default 0.95 = 5% tax)</param>
    /// <returns>Total earnings from delivery</returns>
    public static float CalculateDeliveryEarnings(float shawarmaValue, int quantity, float taxRate = 0.95f)
    {
        if (quantity <= 0) return 0f;
        return shawarmaValue * quantity * taxRate;
    }
    
    /// <summary>
    /// Calculates estimated income per minute based on current game state.
    /// </summary>
    /// <param name="shawarmaValue">Base shawarma value</param>
    /// <param name="deliveriesPerMinute">Number of deliveries per minute</param>
    /// <param name="averageDeliverySize">Average shawarmas per delivery</param>
    /// <param name="taxRate">Tax rate (default 0.95)</param>
    /// <returns>Estimated income per minute</returns>
    public static float CalculateIncomePerMinute(float shawarmaValue, float deliveriesPerMinute, float averageDeliverySize, float taxRate = 0.95f)
    {
        return shawarmaValue * deliveriesPerMinute * averageDeliverySize * taxRate;
    }
    
    /// <summary>
    /// Calculates offline earnings based on game state and time elapsed.
    /// </summary>
    /// <param name="shawarmaValue">Base shawarma value</param>
    /// <param name="estimatedDeliveryRate">Estimated delivery rate (earnings per second)</param>
    /// <param name="secondsOffline">Seconds player was offline</param>
    /// <param name="maxOfflineHours">Maximum hours to calculate (default 24)</param>
    /// <param name="maxEarningsHours">Maximum earnings cap in hours (default 1)</param>
    /// <returns>Total offline earnings</returns>
    public static float CalculateOfflineEarnings(float shawarmaValue, float estimatedDeliveryRate, double secondsOffline, 
        float maxOfflineHours = 24f, float maxEarningsHours = 1f)
    {
        // Cap offline time
        double maxOfflineSeconds = maxOfflineHours * 3600;
        double cappedSeconds = Math.Min(secondsOffline, maxOfflineSeconds);
        
        // Calculate earnings
        double amount = estimatedDeliveryRate * cappedSeconds;
        
        // Cap maximum earnings
        double maxEarnings = estimatedDeliveryRate * (maxEarningsHours * 3600);
        amount = Math.Min(amount, maxEarnings);
        
        return (float)amount;
    }
    
    #endregion
    
    #region Cost Calculations
    
    /// <summary>
    /// Calculates upgrade cost with soft exponential scaling and diminishing returns.
    /// Formula: (basePrice - prestigeReduction) * (level^multiplier) * (1 / (1 + level * 0.1))
    /// </summary>
    /// <param name="basePrice">Base price of the upgrade</param>
    /// <param name="level">Current upgrade level</param>
    /// <param name="multiplier">Exponential multiplier</param>
    /// <param name="prestigeReduction">Cost reduction from prestige</param>
    /// <returns>Total upgrade cost</returns>
    public static float CalculateUpgradeCost(float basePrice, int level, float multiplier, float prestigeReduction = 0f)
    {
        float baseCost = basePrice - prestigeReduction;
        float exponentialPart = Mathf.Pow(level, multiplier);
        float diminishingFactor = 1f / (1f + level * 0.1f);  // Diminishing returns
        
        return baseCost * exponentialPart * diminishingFactor;
    }
    
    /// <summary>
    /// Calculates how many deliveries are needed to afford an upgrade.
    /// </summary>
    /// <param name="upgradeCost">Cost of the upgrade</param>
    /// <param name="incomePerDelivery">Average income per delivery</param>
    /// <returns>Number of deliveries needed</returns>
    public static int CalculateDeliveriesNeeded(float upgradeCost, float incomePerDelivery)
    {
        if (incomePerDelivery <= 0) return int.MaxValue;
        return Mathf.CeilToInt(upgradeCost / incomePerDelivery);
    }
    
    /// <summary>
    /// Calculates time needed to afford upgrade based on income rate.
    /// </summary>
    /// <param name="upgradeCost">Cost of the upgrade</param>
    /// <param name="incomePerSecond">Income per second</param>
    /// <returns>Time in seconds needed</returns>
    public static float CalculateTimeToAfford(float upgradeCost, float incomePerSecond)
    {
        if (incomePerSecond <= 0) return float.MaxValue;
        return upgradeCost / incomePerSecond;
    }
    
    #endregion
    
    #region Value Calculations
    
    /// <summary>
    /// Calculates total shawarma value including all bonuses.
    /// </summary>
    /// <param name="baseValue">Base shawarma value</param>
    /// <param name="breadLevel">Bread upgrade level</param>
    /// <param name="chickenLevel">Chicken upgrade level</param>
    /// <param name="sauceLevel">Sauce upgrade level</param>
    /// <param name="prestigeStars">Number of prestige stars</param>
    /// <param name="qualityBonus">Quality bonus multiplier</param>
    /// <returns>Total shawarma value</returns>
    public static float CalculateShawarmaValue(float baseValue, int breadLevel, int chickenLevel, int sauceLevel, 
        int prestigeStars, float qualityBonus = 1f)
    {
        float breadValue = breadLevel * 5f;      // 2.5% of base per level
        float chickenValue = chickenLevel * 8f;  // 4% of base per level
        float sauceValue = sauceLevel * 3f;       // 1.5% of base per level
        float prestigeValue = prestigeStars * 0.1f * baseValue;  // 10% of base per star
        
        float sum = breadValue + chickenValue + sauceValue + prestigeValue;
        return (baseValue + sum) * qualityBonus;
    }
    
    #endregion
    
    #region Capacity Calculations
    
    /// <summary>
    /// Calculates storage capacity based on level.
    /// </summary>
    /// <param name="baseCapacity">Base capacity</param>
    /// <param name="level">Upgrade level</param>
    /// <param name="capacityMultiplier">Capacity multiplier per level</param>
    /// <returns>Total capacity</returns>
    public static float CalculateCapacity(int baseCapacity, int level, float capacityMultiplier)
    {
        return baseCapacity * (1 + level * capacityMultiplier);
    }
    
    /// <summary>
    /// Calculates delivery interval (time between deliveries).
    /// </summary>
    /// <param name="baseInterval">Base interval in seconds</param>
    /// <param name="upgradeLevel">Upgrade level</param>
    /// <param name="multiplier">Reduction multiplier per level</param>
    /// <returns>Delivery interval in seconds</returns>
    public static float CalculateDeliveryInterval(float baseInterval, int upgradeLevel, float multiplier = 0.2f)
    {
        return baseInterval / (1 + upgradeLevel * multiplier);
    }
    
    #endregion
    
    #region Prestige Calculations
    
    /// <summary>
    /// Calculates prestige income bonus.
    /// Formula: prestigeStars * 0.1 * baseValue (10% per star)
    /// </summary>
    /// <param name="prestigeStars">Number of prestige stars</param>
    /// <param name="baseValue">Base value to apply percentage to</param>
    /// <returns>Prestige income bonus</returns>
    public static float CalculatePrestigeIncomeBonus(int prestigeStars, float baseValue)
    {
        return prestigeStars * 0.1f * baseValue;  // 10% per star
    }
    
    /// <summary>
    /// Calculates prestige cost reduction.
    /// Formula: prestigeStars * 0.025 * baseValue (2.5% per star)
    /// </summary>
    /// <param name="prestigeStars">Number of prestige stars</param>
    /// <param name="baseValue">Base value to apply percentage to</param>
    /// <returns>Cost reduction amount</returns>
    public static float CalculatePrestigeCostReduction(int prestigeStars, float baseValue)
    {
        return prestigeStars * 0.025f * baseValue;  // 2.5% per star
    }
    
    /// <summary>
    /// Calculates prestige cook rate bonus.
    /// Formula: prestigeStars * 0.04 * baseValue (4% per star)
    /// </summary>
    /// <param name="prestigeStars">Number of prestige stars</param>
    /// <param name="baseValue">Base value to apply percentage to</param>
    /// <returns>Cook rate bonus</returns>
    public static float CalculatePrestigeCookRateBonus(int prestigeStars, float baseValue)
    {
        return prestigeStars * 0.04f * baseValue;  // 4% per star
    }
    
    #endregion
    
    #region Balance Testing
    
    /// <summary>
    /// Tests economy balance for a given level range.
    /// Returns balance metrics for analysis.
    /// </summary>
    /// <param name="startLevel">Starting level to test</param>
    /// <param name="endLevel">Ending level to test</param>
    /// <param name="upgradeType">Type of upgrade to test</param>
    /// <param name="averageIncomePerDelivery">Average income per delivery</param>
    /// <returns>Balance test results</returns>
    public static EconomyBalanceTestResult TestBalance(int startLevel, int endLevel, UpgradeType upgradeType, float averageIncomePerDelivery)
    {
        var result = new EconomyBalanceTestResult
        {
            UpgradeType = upgradeType,
            StartLevel = startLevel,
            EndLevel = endLevel
        };
        
        UpgradeConfig config;
        try
        {
            config = UpgradeCosts.GetUpgradeConfig(upgradeType);
        }
        catch (Exception e)
        {
            Debug.LogError($"No config found for upgrade type: {upgradeType} - {e.Message}");
            return result;
        }
        
        result.Costs = new List<float>();
        result.DeliveriesNeeded = new List<int>();
        result.TimeToAfford = new List<float>();
        
        for (int level = startLevel; level <= endLevel; level++)
        {
            float prestigeReduction = CalculatePrestigeCostReduction(0, UpgradeCosts.shwarmaBaseValue); // Test with 0 stars
            float cost = CalculateUpgradeCost(config.basePrice, level, config.upgradeMultiplier, prestigeReduction);
            int deliveries = CalculateDeliveriesNeeded(cost, averageIncomePerDelivery);
            float timeSeconds = CalculateTimeToAfford(cost, averageIncomePerDelivery / 60f); // Assuming 1 delivery per minute
            
            result.Costs.Add(cost);
            result.DeliveriesNeeded.Add(deliveries);
            result.TimeToAfford.Add(timeSeconds);
        }
        
        return result;
    }
    
    #endregion
}

/// <summary>
/// Result structure for economy balance testing.
/// </summary>
public class EconomyBalanceTestResult
{
    public UpgradeType UpgradeType;
    public int StartLevel;
    public int EndLevel;
    public List<float> Costs;
    public List<int> DeliveriesNeeded;
    public List<float> TimeToAfford;
    
    public void LogResults()
    {
        Debug.Log($"=== Economy Balance Test: {UpgradeType} ===");
        Debug.Log($"Level Range: {StartLevel} - {EndLevel}");
        
        for (int i = 0; i < Costs.Count; i++)
        {
            int level = StartLevel + i;
            Debug.Log($"Level {level}: Cost=${Costs[i]:N0}, Deliveries={DeliveriesNeeded[i]}, Time={TimeToAfford[i] / 60f:F1}min");
        }
    }
}


