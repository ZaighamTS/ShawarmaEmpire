# Economy Configuration Guide - What's Set vs What Needs Setting

## ✅ Already Set in Code (No Action Needed)

These values are **hardcoded in C# scripts** and work automatically:

### 1. Upgrade Cost Formulas
**Location:** `Assets/Scripts/UpgradeSystem/UpgradeCosts.cs` (lines 47-53)

```csharp
private static readonly Dictionary<UpgradeType, UpgradeConfig> priceMap = new()
{
    {UpgradeType.Storage,new(1000,1.5f,1.4f) },
    {UpgradeType.DeliveryVan, new(500,1.5f,1.35f) },
    {UpgradeType.Kitchen,new (2000,1.2f,1.3f) },
    {UpgradeType.Catering,new (1500,1.2f,1.25f) }
};
```

**Status:** ✅ **Already configured** - Costs are calculated automatically using the formula

### 2. Capacity Formulas
**Location:** `Assets/Scripts/UpgradeSystem/UpgradeCosts.cs` (lines 57-62)

```csharp
internal static readonly Dictionary<CapacityType, CapacityConfig> capacityMap = new()
{
    {CapacityType.Storage,new(100,1.4f) },
    {CapacityType.Delivery,new(100,1.3f) },
    {CapacityType.Catering,new(100,1.01f) }
};
```

**Status:** ✅ **Already configured** - Capacities are calculated automatically

### 3. Raw Material Upgrade Costs
**Location:** `Assets/Scripts/Common/CommonAbilities.cs` (line 37)

```csharp
Cost = (Level + 1) * 100;
```

**Status:** ✅ **Already configured** - Costs are calculated automatically

### 4. Base Values
**Location:** `Assets/Scripts/UpgradeSystem/UpgradeCosts.cs` (lines 73-74)

```csharp
public static float shwarmaBaseValue = 200;
public static float cookRateBaseValue = 200;
```

**Status:** ✅ **Already configured** - Base values are set

### 5. Prestige Formulas
**Location:** `Assets/Scripts/UpgradeSystem/UpgradeCosts.cs` (lines 168-180)

**Status:** ✅ **Already configured** - Prestige bonuses are calculated automatically

### 6. Delivery Interval Formulas
**Location:** `Assets/Scripts/UpgradeSystem/UpgradeCosts.cs` (lines 135-146)

**Status:** ✅ **Already configured** - Intervals are calculated automatically

---

## ⚠️ Needs to be Set in Unity Inspector

### Building Purchase Costs

**Location:** Unity Inspector → GameObject with `BuildingUnlockManager` component

**What to Set:**
1. Find the GameObject in your scene that has the `BuildingUnlockManager` component
2. In the Inspector, you'll see a `Buildings` list
3. For each building in the list, set:
   - **Name**: Building name
   - **Cost**: Cash cost to purchase (integer)
   - **Gold Cost**: Gold cost to purchase (integer)
   - **Building Object**: Reference to the building GameObject
   - **Particle**: Reference to particle effect GameObject

**Code Reference:** `Assets/Scripts/BuildingUnlockManager.cs` (lines 188-198)

```csharp
[System.Serializable]
public class Building
{
    public string name;
    public int cost;           // ← SET THIS in Inspector
    public bool isPurchased;
    public GameObject BuildingObject;
    public GameObject Particle;
    public int goldCost;       // ← SET THIS in Inspector
}
```

**How to Check:**
1. Open your main game scene
2. Find the GameObject with `BuildingUnlockManager` component
3. Check if the `Buildings` list is populated
4. Verify each building has `cost` and `goldCost` values set

**Note:** These are **NOT** calculated using the upgrade formula - they are fixed values you set manually.

---

## 🔍 How to Verify Configuration

### Step 1: Check Upgrade Costs (Already Set)
1. Open `Assets/Scripts/UpgradeSystem/UpgradeCosts.cs`
2. Verify the `priceMap` values match your desired economy balance
3. These are automatically used when upgrading buildings

### Step 2: Check Building Purchase Costs (May Need Setting)
1. Open Unity Editor
2. Find GameObject with `BuildingUnlockManager` component
3. Check the `Buildings` list in Inspector
4. Verify each building has:
   - ✅ `cost` value set (cash cost)
   - ✅ `goldCost` value set (gold cost)
   - ✅ `BuildingObject` reference assigned
   - ✅ `Particle` reference assigned (if applicable)

### Step 3: Test in Game
1. Run the game
2. Try to purchase a new building
3. Verify the cost matches what you set in Inspector
4. Try upgrading an existing building
5. Verify upgrade costs match the formula calculations

---

## 📝 Summary

| System | Status | Location |
|--------|--------|----------|
| Upgrade Costs | ✅ Set in Code | `UpgradeCosts.cs` |
| Capacity Formulas | ✅ Set in Code | `UpgradeCosts.cs` |
| Raw Material Costs | ✅ Set in Code | `CommonAbilities.cs` |
| Base Values | ✅ Set in Code | `UpgradeCosts.cs` |
| Prestige Formulas | ✅ Set in Code | `UpgradeCosts.cs` |
| **Building Purchase Costs** | ⚠️ **Set in Inspector** | Unity Inspector |

---

## 🛠️ If Building Costs Are Not Set

If you find that building purchase costs are not set in the Inspector:

1. **Open Unity Editor**
2. **Find the BuildingUnlockManager GameObject** in your scene
3. **Add buildings to the list** if empty:
   - Click the "+" button to add entries
   - Set `cost` (cash) and `goldCost` for each building
   - Assign `BuildingObject` and `Particle` references
4. **Save the scene**

**Example Values** (you can adjust these):
- First Kitchen: `cost = 5000`, `goldCost = 0`
- Second Kitchen: `cost = 10000`, `goldCost = 0`
- First Warehouse: `cost = 3000`, `goldCost = 0`
- etc.

---

## ⚡ Quick Action Items

1. ✅ **Verify** upgrade costs in `UpgradeCosts.cs` are correct
2. ⚠️ **Check** Unity Inspector for `BuildingUnlockManager` component
3. ⚠️ **Set** building purchase costs if not already set
4. ✅ **Test** in-game to verify all costs work correctly

---

**Last Updated:** Analysis Date  
**Document Version:** 1.0
