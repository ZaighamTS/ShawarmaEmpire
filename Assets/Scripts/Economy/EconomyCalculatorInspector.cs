using UnityEngine;

/// <summary>
/// Inspector-friendly wrapper for EconomyCalculator.
/// Add this component to any GameObject to use EconomyCalculator in the Unity Inspector.
/// Provides context menu options and serialized fields for easy testing.
/// </summary>
public class EconomyCalculatorInspector : MonoBehaviour
{
    [Header("Income Calculations")]
    [SerializeField] private float testShawarmaValue = 50f; // Updated to match new base value
    [SerializeField] private int testQuantity = 100;
    [SerializeField] private float testTaxRate = 0.70f; // Updated to match new 30% tax rate
    
    [Header("Cost Calculations")]
    [SerializeField] private float testBasePrice = 5000f; // Updated to match new Storage base price
    [SerializeField] private int testLevel = 10;
    [SerializeField] private float testMultiplier = 1.4f;
    [SerializeField] private float testPrestigeReduction = 0f;
    
    [Header("Shawarma Value Calculation")]
    [SerializeField] private int testBreadLevel = 5;
    [SerializeField] private int testChickenLevel = 3;
    [SerializeField] private int testSauceLevel = 2;
    [SerializeField] private int testPrestigeStars = 1;
    [SerializeField] private float testQualityBonus = 1f;
    
    [Header("Capacity Calculations")]
    [SerializeField] private int testBaseCapacity = 100;
    [SerializeField] private float testCapacityMultiplier = 1.4f;
    
    [Header("Results (Read-Only)")]
    [SerializeField, Tooltip("Calculated delivery earnings")] 
    private float deliveryEarningsResult;
    
    [SerializeField, Tooltip("Calculated upgrade cost")] 
    private float upgradeCostResult;
    
    [SerializeField, Tooltip("Calculated shawarma value")] 
    private float shawarmaValueResult;
    
    [SerializeField, Tooltip("Calculated capacity")] 
    private float capacityResult;
    
    [SerializeField, Tooltip("Deliveries needed to afford upgrade")] 
    private int deliveriesNeededResult;
    
    [SerializeField, Tooltip("Time in seconds to afford upgrade")] 
    private float timeToAffordResult;

    private void OnValidate()
    {
        // Auto-calculate when values change in Inspector
        CalculateAll();
    }

    /// <summary>
    /// Calculate all values based on current Inspector settings.
    /// </summary>
    [ContextMenu("Calculate All")]
    public void CalculateAll()
    {
        deliveryEarningsResult = EconomyCalculator.CalculateDeliveryEarnings(
            testShawarmaValue, testQuantity, testTaxRate);
        
        upgradeCostResult = EconomyCalculator.CalculateUpgradeCost(
            testBasePrice, testLevel, testMultiplier, testPrestigeReduction);
        
        shawarmaValueResult = EconomyCalculator.CalculateShawarmaValue(
            UpgradeCosts.shwarmaBaseValue, testBreadLevel, testChickenLevel, 
            testSauceLevel, testPrestigeStars, testQualityBonus);
        
        capacityResult = EconomyCalculator.CalculateCapacity(
            testBaseCapacity, testLevel, testCapacityMultiplier);
        
        deliveriesNeededResult = EconomyCalculator.CalculateDeliveriesNeeded(
            upgradeCostResult, deliveryEarningsResult / testQuantity);
        
        timeToAffordResult = EconomyCalculator.CalculateTimeToAfford(
            upgradeCostResult, (deliveryEarningsResult / testQuantity) / 60f);
    }

    [ContextMenu("Calculate Delivery Earnings")]
    public void CalculateDeliveryEarnings()
    {
        deliveryEarningsResult = EconomyCalculator.CalculateDeliveryEarnings(
            testShawarmaValue, testQuantity, testTaxRate);
        Debug.Log($"Delivery Earnings: ${deliveryEarningsResult:N2} " +
                  $"(Value: ${testShawarmaValue}, Quantity: {testQuantity}, Tax: {testTaxRate})");
    }

    [ContextMenu("Calculate Upgrade Cost")]
    public void CalculateUpgradeCost()
    {
        upgradeCostResult = EconomyCalculator.CalculateUpgradeCost(
            testBasePrice, testLevel, testMultiplier, testPrestigeReduction);
        Debug.Log($"Upgrade Cost: ${upgradeCostResult:N2} " +
                  $"(Base: ${testBasePrice}, Level: {testLevel}, Multiplier: {testMultiplier})");
    }

    [ContextMenu("Calculate Shawarma Value")]
    public void CalculateShawarmaValue()
    {
        shawarmaValueResult = EconomyCalculator.CalculateShawarmaValue(
            UpgradeCosts.shwarmaBaseValue, testBreadLevel, testChickenLevel, 
            testSauceLevel, testPrestigeStars, testQualityBonus);
        Debug.Log($"Shawarma Value: ${shawarmaValueResult:N2} " +
                  $"(Bread: {testBreadLevel}, Chicken: {testChickenLevel}, " +
                  $"Sauce: {testSauceLevel}, Stars: {testPrestigeStars})");
    }

    [ContextMenu("Calculate Capacity")]
    public void CalculateCapacity()
    {
        capacityResult = EconomyCalculator.CalculateCapacity(
            testBaseCapacity, testLevel, testCapacityMultiplier);
        Debug.Log($"Capacity: {capacityResult:N0} " +
                  $"(Base: {testBaseCapacity}, Level: {testLevel}, Multiplier: {testCapacityMultiplier})");
    }

    [ContextMenu("Calculate Deliveries Needed")]
    public void CalculateDeliveriesNeeded()
    {
        float incomePerDelivery = deliveryEarningsResult / testQuantity;
        deliveriesNeededResult = EconomyCalculator.CalculateDeliveriesNeeded(
            upgradeCostResult, incomePerDelivery);
        Debug.Log($"Deliveries Needed: {deliveriesNeededResult} " +
                  $"(Cost: ${upgradeCostResult:N0}, Income/Delivery: ${incomePerDelivery:N2})");
    }

    [ContextMenu("Calculate Time to Afford")]
    public void CalculateTimeToAfford()
    {
        float incomePerSecond = (deliveryEarningsResult / testQuantity) / 60f;
        timeToAffordResult = EconomyCalculator.CalculateTimeToAfford(
            upgradeCostResult, incomePerSecond);
        Debug.Log($"Time to Afford: {timeToAffordResult / 60f:F1} minutes " +
                  $"(Cost: ${upgradeCostResult:N0}, Income/Second: ${incomePerSecond:N2})");
    }

    [ContextMenu("Test Prestige Bonuses")]
    public void TestPrestigeBonuses()
    {
        float incomeBonus = EconomyCalculator.CalculatePrestigeIncomeBonus(
            testPrestigeStars, UpgradeCosts.shwarmaBaseValue);
        float costReduction = EconomyCalculator.CalculatePrestigeCostReduction(
            testPrestigeStars, UpgradeCosts.shwarmaBaseValue);
        float cookRateBonus = EconomyCalculator.CalculatePrestigeCookRateBonus(
            testPrestigeStars, UpgradeCosts.cookRateBaseValue);
        
        Debug.Log($"=== Prestige Bonuses ({testPrestigeStars} stars) ===");
        Debug.Log($"Income Bonus: +${incomeBonus:N2} per shawarma");
        Debug.Log($"Cost Reduction: -${costReduction:N2} per upgrade");
        Debug.Log($"Cook Rate Bonus: +{cookRateBonus:N2}");
    }

    [ContextMenu("Quick Test: Level 1-10 Costs")]
    public void QuickTestCosts()
    {
        Debug.Log("=== Quick Cost Test ===");
        for (int level = 1; level <= 10; level++)
        {
            float cost = EconomyCalculator.CalculateUpgradeCost(
                testBasePrice, level, testMultiplier, testPrestigeReduction);
            Debug.Log($"Level {level}: ${cost:N0}");
        }
    }

    [ContextMenu("Compare: Before vs After Fixes")]
    public void CompareBeforeAfter()
    {
        Debug.Log("=== Before vs After Economy Fixes ===");
        
        // Before: Old income formula (value + quantity)
        float oldIncome = (testShawarmaValue + testQuantity) * testTaxRate;
        // After: New income formula (value * quantity)
        float newIncome = EconomyCalculator.CalculateDeliveryEarnings(
            testShawarmaValue, testQuantity, testTaxRate);
        
        Debug.Log($"Old Formula (value + qty): ${oldIncome:N2}");
        Debug.Log($"New Formula (value * qty): ${newIncome:N2}");
        Debug.Log($"Improvement: {newIncome / oldIncome:F1}x more income");
        
        // Before: Old material upgrades (0.03-0.04)
        float oldMaterialValue = testBreadLevel * 0.03f + testChickenLevel * 0.04f + testSauceLevel * 0.02f;
        // After: New material upgrades (5-8)
        float newMaterialValue = testBreadLevel * 5f + testChickenLevel * 8f + testSauceLevel * 3f;
        
        Debug.Log($"Old Material Value: +${oldMaterialValue:N2}");
        Debug.Log($"New Material Value: +${newMaterialValue:N2}");
        Debug.Log($"Improvement: {newMaterialValue / oldMaterialValue:F0}x more value");
    }
}

