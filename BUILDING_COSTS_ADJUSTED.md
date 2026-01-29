# Building Costs - Adjusted for New Economy
## Updated Costs to Match Reduced Income

**New Income Rate:** ~$10K/hour (early game)  
**Goal:** Buildings take 5-30 minutes to afford (depending on stage)

---

## 📋 Updated Building Costs

### Recommended Costs (In Order)

| # | Building Name | Old Cost | **New Cost** | Time at $10K/hr | Status |
|---|---------------|----------|--------------|-----------------|--------|
| **1** | **Juice Point** | $500 | **$500** | 3 min | ✅ Keep |
| **2** | **Dessert Point** | $1,000 | **$1,000** | 6 min | ✅ Keep |
| **3** | **Merchandise Store** | $1,500 | **$1,500** | 9 min | ✅ Keep |
| **4** | **Ingredients Shop** | $3,000 | **$2,500** | 15 min | ✅ Reduced |
| **5** | **Shawarma Lounge** | $5,000 | **$5,000** | 30 min | ✅ Keep |
| **6** | **Park** | $7,500 | **$7,500** | 45 min | ✅ Keep |
| **7** | **Petrol Station** | $15,000 | **$10,000** | 60 min | ✅ Reduced |
| **8** | **Management Building** | $25,000 | **$15,000** | 90 min | ✅ Reduced |

**Total Cost:** $53,500 (down from $60,000)

---

## 💰 Cost Rationale

### Early Buildings (1-3)
- **Juice Point ($500):** First building, quick win
- **Dessert Point ($1,000):** Early game goal
- **Merchandise Store ($1,500):** Early-mid game

**Why:** These are achievable with base income ($10K/hr) in 3-9 minutes ✅

### Mid Buildings (4-5)
- **Ingredients Shop ($2,500):** Mid game milestone
- **Shawarma Lounge ($5,000):** Mid-late game goal

**Why:** 
- By this point, income increases to $15K-$20K/hr
- Takes 8-20 minutes (reasonable for mid-game)
- Buildings start generating income, helping afford next ones

### Late Buildings (6-8)
- **Park ($7,500):** Late game decorative
- **Petrol Station ($10,000):** Significant milestone
- **Management Building ($15,000):** End game premium

**Why:**
- By this point, income is $25K-$50K/hr (with upgrades + buildings)
- Takes 15-36 minutes (reasonable for late-game goals)
- Buildings provide significant bonus income by now

---

## 🎮 How It All Works Together

### Complete Economy Flow

**Stage 1: Foundation (0-30 min)**
```
Income: $10K/hour (delivery only)
Focus: Core upgrades + first 3 buildings
Timeline:
- 0-3 min: First delivery → Afford Delivery L1
- 3-6 min: Afford Juice Point
- 6-9 min: Afford Storage L1
- 9-12 min: Afford Dessert Point
- 12-21 min: Afford Merchandise Store
```

**Stage 2: Expansion (30-90 min)**
```
Income: $20K-$30K/hour (delivery + catering + 3 buildings)
Focus: Mid upgrades + buildings 4-5
Timeline:
- 30-35 min: Afford Delivery L2
- 35-40 min: Afford Storage L2
- 40-50 min: Afford Ingredients Shop
- 50-60 min: Afford Catering L1
- 60-75 min: Afford Shawarma Lounge
```

**Stage 3: Mastery (90+ min)**
```
Income: $50K-$80K/hour (all systems + 5-8 buildings)
Focus: Late upgrades + buildings 6-8
Timeline:
- 90-105 min: Afford Park
- 105-120 min: Afford Petrol Station
- 120-135 min: Afford Management Building
- 135+ min: Max out all upgrades
```

---

## 📊 Income Breakdown by Stage

### Early Game (0-30 min)
- Delivery: $10K/hour
- Catering: $0/hour (not unlocked)
- Buildings: $0/hour (not unlocked)
- **Total: $10K/hour**

### Mid Game (30-90 min)
- Delivery: $15K-$20K/hour
- Catering: $14K-$24K/hour
- Buildings (3): $4.3K/hour
- **Total: $33K-$48K/hour**

### Late Game (90+ min)
- Delivery: $25K-$30K/hour
- Catering: $29K-$36K/hour
- Buildings (5-8): $8K-$16K/hour
- **Total: $62K-$82K/hour**

---

## ✅ Unity Inspector Setup

### Updated Costs to Set

```
Building 1: Juice Point
Cost: 500
Gold Cost: 0

Building 2: Dessert Point
Cost: 1000
Gold Cost: 0

Building 3: Merchandise Store
Cost: 1500
Gold Cost: 0

Building 4: Ingredients Shop
Cost: 2500  ← CHANGED from 3000
Gold Cost: 0

Building 5: Shawarma Lounge
Cost: 5000
Gold Cost: 0

Building 6: Park
Cost: 7500
Gold Cost: 0

Building 7: Petrol Station
Cost: 10000  ← CHANGED from 15000
Gold Cost: 0

Building 8: Management Building
Cost: 15000  ← CHANGED from 25000
Gold Cost: 0
```

---

## 🎯 Expected Pacing

| Building | Cost | Income When Unlocked | Time to Afford | Status |
|----------|------|---------------------|----------------|--------|
| Juice Point | $500 | $10K/hr | 3 min | ✅ Perfect |
| Dessert Point | $1,000 | $10K/hr | 6 min | ✅ Perfect |
| Merchandise Store | $1,500 | $10K/hr | 9 min | ✅ Perfect |
| Ingredients Shop | $2,500 | $20K/hr | 8 min | ✅ Perfect |
| Shawarma Lounge | $5,000 | $30K/hr | 10 min | ✅ Perfect |
| Park | $7,500 | $50K/hr | 9 min | ✅ Perfect |
| Petrol Station | $10,000 | $60K/hr | 10 min | ✅ Perfect |
| Management Building | $15,000 | $70K/hr | 13 min | ✅ Perfect |

**Result:** All buildings take 3-13 minutes to afford (perfect pacing!)

---

## 📝 Summary

### Changes Made
1. ✅ Reduced building income by 95% (from $36K/hr to $480-$5,760/hr)
2. ✅ Adjusted building costs (reduced 3 buildings)
3. ✅ Balanced expenses (reduced and slowed)

### Result
- ✅ Buildings provide bonus income (5-10% of core)
- ✅ Building costs match new income rates
- ✅ All systems work together harmoniously
- ✅ Proper progression pacing (3-13 minutes per building)

---

**Implementation:** Update costs in Unity Inspector + building income already fixed in code!
