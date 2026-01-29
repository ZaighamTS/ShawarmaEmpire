# Building Purchase Costs - Recommended Values

## Overview

Based on the economy analysis, here are recommended `cost` and `goldCost` values for each building in the `BuildingUnlockManager`.

**Note:** These are for buildings purchased through `BuildingUnlockManager` (likely decorative/special buildings), NOT the core gameplay buildings (Kitchen, Warehouse, Delivery Van, Catering) which use calculated upgrade costs.

---

## 💰 Recommended Cost Structure

### Cost Progression Philosophy
- **Early buildings**: Affordable, achievable within first few minutes
- **Mid buildings**: Require some progression (5-15 minutes)
- **Late buildings**: Require significant progression (30+ minutes)
- **Gold costs**: Optional premium currency alternative (typically 0 unless premium feature)

---

## 📋 Recommended Building Costs

### Early Game Buildings (First 5-10 minutes)

These should be affordable early to give players quick wins:

| Building Name | Cash Cost | Gold Cost | Notes |
|--------------|-----------|-----------|-------|
| **Ingredients Shop** | $500 | 0 | First building, very affordable |
| **Merchandise Stand** | $1,000 | 0 | Early game goal |
| **Small Park** | $1,500 | 0 | Decorative, moderate cost |

**Rationale:**
- Base shawarma value: $200
- Early deliveries: ~$190 per shawarma
- First building: ~3 deliveries
- Second building: ~5 deliveries
- Third building: ~8 deliveries

---

### Mid Game Buildings (10-30 minutes)

These require some progression and upgrades:

| Building Name | Cash Cost | Gold Cost | Notes |
|--------------|-----------|-----------|-------|
| **Gas Station** | $3,000 | 0 | Useful mid-game |
| **Juice Point** | $5,000 | 0 | Moderate progression |
| **Dessert Point** | $7,500 | 0 | Higher cost |

**Rationale:**
- Mid-game shawarma value: $300-400 (with upgrades)
- Mid-game deliveries: ~$285-380 per shawarma
- Gas Station: ~8-10 deliveries
- Juice Point: ~13-18 deliveries
- Dessert Point: ~20-26 deliveries

---

### Late Game Buildings (30+ minutes)

These require significant progression:

| Building Name | Cash Cost | Gold Cost | Notes |
|--------------|-----------|-----------|-------|
| **Management Office** | $15,000 | 0 | Significant milestone |
| **Shawarma Lounge** | $25,000 | 0 | Premium building |
| **Large Park** | $50,000 | 0 | End-game decorative |

**Rationale:**
- Late-game shawarma value: $500-800+ (with max materials)
- Late-game deliveries: ~$475-760 per shawarma
- Management Office: ~20-32 deliveries
- Shawarma Lounge: ~33-53 deliveries
- Large Park: ~66-105 deliveries

---

## 🎯 Alternative: Progressive Scaling

If you want buildings to scale with player progress, use this formula:

```
Cost = BaseCost × (1.5 ^ BuildingIndex)
```

**Example Progression:**
| Building Index | Base Cost | Calculated Cost | Rounded |
|----------------|-----------|-----------------|---------|
| 0 | $500 | $500 | $500 |
| 1 | $500 | $750 | $750 |
| 2 | $500 | $1,125 | $1,100 |
| 3 | $500 | $1,688 | $1,700 |
| 4 | $500 | $2,531 | $2,500 |
| 5 | $500 | $3,797 | $3,800 |
| 6 | $500 | $5,695 | $5,700 |
| 7 | $500 | $8,543 | $8,500 |
| 8 | $500 | $12,814 | $12,800 |

---

## 💎 Gold Cost Recommendations

**General Rule:** Use gold costs sparingly, only for premium/premium features.

### Option 1: No Gold Costs (Recommended)
Set all `goldCost` to **0** - keep buildings cash-only for better economy balance.

### Option 2: Premium Buildings Only
| Building Name | Cash Cost | Gold Cost | Notes |
|--------------|-----------|-----------|-------|
| **Shawarma Lounge** | $25,000 | 50 | Premium option |
| **Large Park** | $50,000 | 100 | Premium option |

**Gold Cost Formula:** `GoldCost = CashCost / 500` (roughly)

---

## 📊 Complete Recommended List

### Full Building List with Costs

```csharp
// Building 0: Ingredients Shop
cost = 500
goldCost = 0

// Building 1: Merchandise Stand
cost = 1000
goldCost = 0

// Building 2: Small Park
cost = 1500
goldCost = 0

// Building 3: Gas Station
cost = 3000
goldCost = 0

// Building 4: Juice Point
cost = 5000
goldCost = 0

// Building 5: Dessert Point
cost = 7500
goldCost = 0

// Building 6: Management Office
cost = 15000
goldCost = 0

// Building 7: Shawarma Lounge
cost = 25000
goldCost = 0  // or 50 if using gold option

// Building 8: Large Park
cost = 50000
goldCost = 0  // or 100 if using gold option
```

---

## 🔍 How to Set in Unity

1. **Open Unity Editor**
2. **Find GameObject with `BuildingUnlockManager` component**
3. **In Inspector, find the `Buildings` list**
4. **For each building entry:**
   - Set `Name` (e.g., "Ingredients Shop")
   - Set `Cost` (use values from table above)
   - Set `Gold Cost` (typically 0)
   - Assign `Building Object` (drag from scene/hierarchy)
   - Assign `Particle` (if applicable)

---

## ⚖️ Balance Considerations

### Early Game Balance
- **First building** should be achievable within **2-5 minutes** of starting
- **Second building** should be achievable within **5-10 minutes**
- Players should feel progress without grinding

### Mid Game Balance
- Buildings should require **meaningful progression**
- Not too cheap (no challenge) but not too expensive (frustrating)
- Should feel rewarding when achieved

### Late Game Balance
- Premium buildings should be **aspirational goals**
- Require significant investment
- Provide sense of achievement

---

## 🧪 Testing Recommendations

After setting costs, test:

1. **Early Game:**
   - Can player afford first building within 5 minutes?
   - Is it satisfying to purchase?

2. **Mid Game:**
   - Are mid-game buildings achievable with reasonable playtime?
   - Do they feel appropriately priced?

3. **Late Game:**
   - Are late-game buildings aspirational but achievable?
   - Do they provide meaningful goals?

---

## 📝 Quick Reference Table

| Building Type | Recommended Cash Cost | Gold Cost | Time to Afford* |
|---------------|----------------------|-----------|-----------------|
| Ingredients Shop | $500 | 0 | 2-3 min |
| Merchandise Stand | $1,000 | 0 | 5-7 min |
| Small Park | $1,500 | 0 | 8-10 min |
| Gas Station | $3,000 | 0 | 15-20 min |
| Juice Point | $5,000 | 0 | 25-30 min |
| Dessert Point | $7,500 | 0 | 35-45 min |
| Management Office | $15,000 | 0 | 60-90 min |
| Shawarma Lounge | $25,000 | 0 | 90-120 min |
| Large Park | $50,000 | 0 | 3+ hours |

*Time estimates assume active play with basic upgrades

---

## 🎯 Final Recommendations

**Recommended Starting Values:**

1. **Start conservative** - Use the lower end of suggested costs
2. **Test in-game** - Play through early game and adjust
3. **Monitor player feedback** - Are buildings too cheap/expensive?
4. **Iterate** - Adjust based on actual gameplay data

**Quick Setup:**
- First 3 buildings: $500, $1,000, $1,500
- Next 3 buildings: $3,000, $5,000, $7,500
- Last 3 buildings: $15,000, $25,000, $50,000
- All gold costs: **0** (unless you have a premium currency system)

---

**Last Updated:** Analysis Date  
**Document Version:** 1.0
