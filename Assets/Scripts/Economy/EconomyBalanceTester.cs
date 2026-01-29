using UnityEngine;

/// <summary>
/// Economy balance testing utility.
/// Use this in the Unity Editor to test economy balance at different levels.
/// </summary>
public class EconomyBalanceTester : MonoBehaviour
{
    [Header("Balance Test Settings")]
    [SerializeField] private bool runTestsOnStart = false;
    [SerializeField] private int testStartLevel = 1;
    [SerializeField] private int testEndLevel = 20;
    [SerializeField] private float averageIncomePerDelivery = 200f;
    
    private void Start()
    {
        if (runTestsOnStart)
        {
            RunAllBalanceTests();
        }
    }
    
    /// <summary>
    /// Runs balance tests for all upgrade types.
    /// </summary>
    [ContextMenu("Run All Balance Tests")]
    public void RunAllBalanceTests()
    {
        Debug.Log("=== ECONOMY BALANCE TEST SUITE ===");
        Debug.Log($"Testing levels {testStartLevel} to {testEndLevel}");
        Debug.Log($"Average income per delivery: ${averageIncomePerDelivery:N0}");
        Debug.Log("");
        
        TestUpgradeType(UpgradeType.Storage, "Storage");
        TestUpgradeType(UpgradeType.DeliveryVan, "Delivery Van");
        TestUpgradeType(UpgradeType.Kitchen, "Kitchen");
        TestUpgradeType(UpgradeType.Catering, "Catering");
        
        Debug.Log("=== TEST SUITE COMPLETE ===");
    }
    
    private void TestUpgradeType(UpgradeType type, string name)
    {
        var result = EconomyCalculator.TestBalance(testStartLevel, testEndLevel, type, averageIncomePerDelivery);
        
        Debug.Log($"\n--- {name} Balance Test ---");
        Debug.Log($"Level Range: {result.StartLevel} - {result.EndLevel}");
        Debug.Log($"Level | Cost | Deliveries Needed | Time to Afford");
        Debug.Log($"------|------|-------------------|----------------");
        
        for (int i = 0; i < result.Costs.Count; i++)
        {
            int level = result.StartLevel + i;
            float cost = result.Costs[i];
            int deliveries = result.DeliveriesNeeded[i];
            float timeMinutes = result.TimeToAfford[i] / 60f;
            
            Debug.Log($"  {level,2}  | ${cost,8:N0} | {deliveries,15} | {timeMinutes,6:F1} min");
        }
        
        // Calculate averages
        float avgCost = CalculateAverage(result.Costs);
        float avgDeliveries = CalculateAverage(result.DeliveriesNeeded);
        float avgTime = CalculateAverage(result.TimeToAfford) / 60f;
        
        Debug.Log($"Average: ${avgCost:N0} | {avgDeliveries:F1} deliveries | {avgTime:F1} min");
    }
    
    private float CalculateAverage(System.Collections.Generic.List<float> values)
    {
        if (values.Count == 0) return 0f;
        float sum = 0f;
        foreach (var value in values) sum += value;
        return sum / values.Count;
    }
    
    private float CalculateAverage(System.Collections.Generic.List<int> values)
    {
        if (values.Count == 0) return 0f;
        int sum = 0;
        foreach (var value in values) sum += value;
        return (float)sum / values.Count;
    }
    
    /// <summary>
    /// Quick test for a specific level range.
    /// </summary>
    [ContextMenu("Quick Test (Levels 1-10)")]
    public void QuickTest()
    {
        testStartLevel = 1;
        testEndLevel = 10;
        RunAllBalanceTests();
    }
    
    /// <summary>
    /// Test late-game balance.
    /// </summary>
    [ContextMenu("Late Game Test (Levels 20-30)")]
    public void LateGameTest()
    {
        testStartLevel = 20;
        testEndLevel = 30;
        RunAllBalanceTests();
    }
}

