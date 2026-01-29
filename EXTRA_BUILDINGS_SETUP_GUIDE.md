# Extra Buildings Dynamic Cost Setup Guide

## ✅ Code Changes Completed

All code changes have been implemented successfully. Extra buildings now use dynamic cost calculation with 5x base price increase and 3.5x scaling.

---

## 🎮 Unity Inspector Setup Required

### Step 1: Select BuildingUnlockManager GameObject

1. Open your scene
2. Find the GameObject with `BuildingUnlockManager` component
3. Select it

### Step 2: Configure Building Types Array

In the Inspector, you'll see a new field:
- **Building Types** (array)

**Configuration Steps:**
1. Set the array **Size** to match your `Buildings` list size (should be 8)
2. Assign each building type in order:

| Index | Building Type | Old Cost | New Base Cost |
|-------|--------------|----------|---------------|
| 0 | **JuicePoint** | $1,500 | $7,500 |
| 1 | **DessertPoint** | $2,500 | $12,500 |
| 2 | **Merchandise** | $4,000 | $20,000 |
| 3 | **Ingredients** | $7,500 | $37,500 |
| 4 | **Park** | $12,000 | $60,000 |
| 5 | **ShawarmaLounge** | $20,000 | $100,000 |
| 6 | **GasStation** | $35,000 | $175,000 |
| 7 | **Management** | $60,000 | $300,000 |

### Step 3: Verify Building List Order

Make sure your `Buildings` list in the Inspector matches the order above:
- Index 0 = Juice Point
- Index 1 = Dessert Point
- Index 2 = Merchandise
- Index 3 = Ingredients
- Index 4 = Park
- Index 5 = Shawarma Lounge
- Index 6 = Gas Station
- Index 7 = Management

---

## 📊 How It Works

### Dynamic Cost Calculation

**First Purchase:**
- Uses base price from `extraBuildingPriceMap`
- Example: First Juice Point = $7,500

**Additional Purchases:**
- Cost scales by 3.5x per additional purchase of the same type
- Example: 
  - 1st Juice Point: $7,500
  - 2nd Juice Point: $26,250 (7,500 × 3.5)
  - 3rd Juice Point: $91,875 (7,500 × 3.5²)

**Prestige Reduction:**
- Prestige stars reduce base cost by $2.50 per star
- Example: With 5 prestige stars, base costs are reduced by $12.50

### Cost Updates Automatically

- Costs are recalculated when:
  - Building is purchased
  - Prestige level changes
  - Game starts/loads
- UI displays current calculated cost
- No manual updates needed

---

## ⚠️ Important Notes

1. **Building Order Matters**: The `buildingTypes` array must match the order of your `buildings` list
2. **Fallback Behavior**: If a building doesn't have a type assigned, it will use the inspector cost value
3. **Existing Saves**: Players with existing saves will see new costs immediately
4. **Gold Costs**: Gold costs remain unchanged (still use inspector values)

---

## 🧪 Testing Checklist

After setup, verify:
- [ ] Building costs display correctly in UI
- [ ] First purchase costs match base prices ($7,500, $12,500, etc.)
- [ ] Second purchase of same type costs 3.5x more
- [ ] Costs update after purchasing a building
- [ ] Prestige cost reduction applies correctly

---

## 📝 Code Files Modified

1. **UpgradeCosts.cs**
   - Added `ExtraBuildingType` enum
   - Added `ExtraBuildingConfig` record
   - Added `extraBuildingPriceMap` dictionary
   - Added `GetExtraBuildingCost()` method

2. **BuildingUnlockManager.cs**
   - Added `buildingTypes` array field
   - Modified `GenerateBuildingUI()` to calculate dynamic costs
   - Added `GetPurchasedCountOfType()` helper method
   - Modified `TryUnlockBuilding()` to recalculate costs

---

**Status**: ✅ Code Complete - Unity Inspector Setup Required  
**Next Step**: Configure Building Types array in Unity Inspector
