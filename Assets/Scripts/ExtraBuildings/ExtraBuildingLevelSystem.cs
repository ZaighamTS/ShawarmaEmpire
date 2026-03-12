using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Phase 8: Extra building upgrade levels (0..10).
/// We use doc anchor points (L0, L5, L10 net $/hr and total cost 1-10) and interpolate.
/// </summary>
public static class ExtraBuildingLevelSystem
{
    public record ExtraBuildingUpgradeProfile(float netPerHourL0, float netPerHourL5, float netPerHourL10, float totalCost1to10);

    private static readonly Dictionary<ExtraBuildingType, ExtraBuildingUpgradeProfile> profiles = new()
    {
        { ExtraBuildingType.JuicePoint,      new ExtraBuildingUpgradeProfile(  720f,  2174f,  5112f,   47020f) },
        { ExtraBuildingType.DessertPoint,    new ExtraBuildingUpgradeProfile(  720f,  2174f,  5112f,   78361f) },
        { ExtraBuildingType.Merchandise,     new ExtraBuildingUpgradeProfile(  780f,  2962f,  7368f,  125828f) },
        { ExtraBuildingType.Ingredients,     new ExtraBuildingUpgradeProfile( -360f,  1822f,  6228f,  235754f) },
        { ExtraBuildingType.ShawarmaLounge,  new ExtraBuildingUpgradeProfile(    0f,  3636f, 10980f,  628284f) },
        { ExtraBuildingType.Park,            new ExtraBuildingUpgradeProfile( -600f,   127f,  1596f,  303177f) },
        { ExtraBuildingType.GasStation,      new ExtraBuildingUpgradeProfile(-1320f,  2316f,  9660f, 1111769f) },
        { ExtraBuildingType.Management,      new ExtraBuildingUpgradeProfile(-1800f,  5472f, 20160f, 1917784f) },
    };

    /// <summary>
    /// Net income per hour at a given level (0..10). Uses a quadratic passing through L0, L5, L10.
    /// </summary>
    public static float GetNetIncomePerHour(ExtraBuildingType type, int level)
    {
        level = Mathf.Clamp(level, 0, 10);
        if (!profiles.TryGetValue(type, out var p)) return 0f;

        // Fit y = a x^2 + b x + c through (0,y0), (5,y5), (10,y10)
        float y0 = p.netPerHourL0;
        float y5 = p.netPerHourL5;
        float y10 = p.netPerHourL10;

        float c = y0;
        // From equations:
        // 25a + 5b + c = y5
        // 100a + 10b + c = y10
        float rhs1 = y5 - c;
        float rhs2 = y10 - c;
        // Solve:
        // 25a + 5b = rhs1
        // 100a + 10b = rhs2
        // Multiply first by 2: 50a + 10b = 2rhs1
        // Subtract: 50a = rhs2 - 2rhs1 => a = (rhs2 - 2rhs1)/50
        float a = (rhs2 - 2f * rhs1) / 50f;
        // b from 25a + 5b = rhs1 => b = (rhs1 - 25a)/5
        float b = (rhs1 - 25f * a) / 5f;

        float x = level;
        return a * x * x + b * x + c;
    }

    /// <summary>
    /// Upgrade cost to reach a given level (1..10). Costs across levels are distributed as a geometric series summing to totalCost1to10.
    /// </summary>
    public static float GetUpgradeCost(ExtraBuildingType type, int toLevel)
    {
        toLevel = Mathf.Clamp(toLevel, 1, 10);
        if (!profiles.TryGetValue(type, out var p)) return 0f;

        // Distribute total cost using weights r^(n-1)
        const float r = 1.28f; // slightly increasing costs toward later levels
        float sum = 0f;
        for (int n = 1; n <= 10; n++) sum += Mathf.Pow(r, n - 1);
        float baseCost = p.totalCost1to10 / sum;
        return Mathf.Round(baseCost * Mathf.Pow(r, toLevel - 1));
    }
}

