# Economy Formulas Documentation

## Overview
This document provides comprehensive documentation of all economy formulas used in Shawarma Inc. All formulas are centralized in `EconomyCalculator.cs` and `UpgradeCosts.cs`.

---

## 💰 INCOME FORMULAS

### Delivery Earnings
**Formula:**
```
earnings = shawarmaValue × quantity × taxRate
```

**Where:**
- `shawarmaValue`: Base value of shawarma (calculated separately)
- `quantity`: Number of shawarmas delivered
- `taxRate`: 0.95 (5% tax/deduction)

**Example:**
- 1 shawarma @ $200 value: 200 × 1 × 0.95 = $190
- 100 shawarmas @ $200 value: 200 × 100 × 0.95 = $19,000

**Location:** `EconomyCalculator.CalculateDeliveryEarnings()`

---

### Shawarma Value Calculation
**Formula:**
```
shawarmaValue = (baseValue + materialBonuses + prestigeBonus) × qualityBonus
```

**Breakdown:**
- `baseValue`: 200 (base shawarma value)
- `materialBonuses`: Bread + Chicken + Sauce upgrades
  - Bread: `level × 5` (2.5% of base per level)
  - Chicken: `level × 8` (4% of base per level)
  - Sauce: `level × 3` (1.5% of base per level)
- `prestigeBonus`: `prestigeStars × 0.1 × baseValue` (10% per star)
- `qualityBonus`: Multiplier (default 1.0)

**Example:**
- Base (no upgrades, 0 stars): (200 + 0 + 0) × 1 = $200
- With upgrades (Bread:5, Chicken:3, Sauce:2, 1 star): 
  - Material: 5×5 + 3×8 + 2×3 = 25 + 24 + 6 = 55
  - Prestige: 1 × 0.1 × 200 = 20
  - Total: (200 + 55 + 20) × 1 = $275

**Location:** `EconomyCalculator.CalculateShawarmaValue()`, `UpgradeCosts.GetShawarmaValue()`

---

### Income Per Minute
**Formula:**
```
incomePerMinute = shawarmaValue × deliveriesPerMinute × averageDeliverySize × taxRate
```

**Where:**
- `deliveriesPerMinute`: Number of deliveries per minute (typically 6)
- `averageDeliverySize`: Average shawarmas per delivery
- `taxRate`: 0.95

**Location:** `EconomyCalculator.CalculateIncomePerMinute()`

---

### Offline Earnings
**Formula:**
```
offlineEarnings = min(estimatedDeliveryRate × min(secondsOffline, maxOfflineSeconds), maxEarnings)
```

**Where:**
- `estimatedDeliveryRate`: Earnings per second (calculated from game state)
- `secondsOffline`: Time player was offline
- `maxOfflineSeconds`: 86,400 (24 hours)
- `maxEarnings`: `estimatedDeliveryRate × 3,600` (1 hour cap)

**Location:** `EconomyCalculator.CalculateOfflineEarnings()`, `GameManager.CheckOfflineEarning()`

---

## 💸 COST FORMULAS

### Upgrade Cost
**Formula:**
```
cost = (basePrice - prestigeReduction) × (level^multiplier) × (1 / (1 + level × 0.1))
```

**Breakdown:**
- `basePrice`: Base cost of upgrade type
- `prestigeReduction`: Cost reduction from prestige stars
- `level`: Current upgrade level
- `multiplier`: Exponential multiplier (varies by upgrade type)
- `(1 / (1 + level × 0.1))`: Diminishing returns factor

**Upgrade Configurations:**
| Upgrade Type | Base Price | Multiplier | Notes |
|--------------|------------|------------|-------|
| Storage | 1,000 | 1.4x | High value bottleneck |
| DeliveryVan | 500 | 1.35x | Critical bottleneck |
| Kitchen | 2,000 | 1.3x | Production boost |
| Catering | 1,500 | 1.25x | Bonus income |

**Example (Storage, Level 10, 0 stars):**
- Base: 1,000
- Exponential: 10^1.4 = 25.12
- Diminishing: 1 / (1 + 10 × 0.1) = 0.5
- Cost: 1,000 × 25.12 × 0.5 = $12,560

**Location:** `EconomyCalculator.CalculateUpgradeCost()`, `UpgradeCosts.GetUpgradeCost()`

---

### Prestige Cost Reduction
**Formula:**
```
costReduction = prestigeStars × 0.025 × baseValue
```

**Where:**
- `prestigeStars`: Number of chef stars
- `baseValue`: 200 (shawarma base value)
- Result: 2.5% of base per star

**Example:**
- 0 stars: 0 × 0.025 × 200 = $0
- 1 star: 1 × 0.025 × 200 = $5
- 5 stars: 5 × 0.025 × 200 = $25

**Location:** `EconomyCalculator.CalculatePrestigeCostReduction()`, `UpgradeCosts.GetPerstigeCostReduction()`

---

## 📊 CAPACITY FORMULAS

### Storage Capacity
**Formula:**
```
capacity = baseCapacity × (1 + level × capacityMultiplier)
```

**Where:**
- `baseCapacity`: 100 (base storage capacity)
- `capacityMultiplier`: 1.4 (40% increase per level)

**Example:**
- Level 1: 100 × (1 + 1 × 1.4) = 240
- Level 5: 100 × (1 + 5 × 1.4) = 800
- Level 10: 100 × (1 + 10 × 1.4) = 1,500

**Location:** `EconomyCalculator.CalculateCapacity()`, `UpgradeCosts.GetDeliveryCapacity()`

---

### Delivery Interval
**Formula:**
```
interval = baseInterval / (1 + upgradeLevel × multiplier)
```

**Where:**
- `baseInterval`: 30 seconds (base delivery interval)
- `multiplier`: 0.2 (20% reduction per level)

**Example:**
- Level 0: 30 / (1 + 0 × 0.2) = 30 seconds
- Level 5: 30 / (1 + 5 × 0.2) = 15 seconds
- Level 10: 30 / (1 + 10 × 0.2) = 10 seconds

**Location:** `EconomyCalculator.CalculateDeliveryInterval()`, `UpgradeCosts.GetDeliveryInterval()`

---

## ⭐ PRESTIGE FORMULAS

### Prestige Income Bonus
**Formula:**
```
incomeBonus = prestigeStars × 0.1 × baseValue
```

**Where:**
- `prestigeStars`: Number of chef stars
- `baseValue`: 200 (shawarma base value)
- Result: 10% of base per star

**Example:**
- 1 star: 1 × 0.1 × 200 = $20 per shawarma
- 5 stars: 5 × 0.1 × 200 = $100 per shawarma

**Location:** `EconomyCalculator.CalculatePrestigeIncomeBonus()`, `UpgradeCosts.GetPerstigeExtraIncome()`

---

### Prestige Cook Rate Bonus
**Formula:**
```
cookRateBonus = prestigeStars × 0.04 × baseValue
```

**Where:**
- `prestigeStars`: Number of chef stars
- `baseValue`: 200 (cook rate base value)
- Result: 4% of base per star

**Location:** `EconomyCalculator.CalculatePrestigeCookRateBonus()`, `UpgradeCosts.GetPrestigeExtraCookRate()`

---

### Chef Stars Calculation
**Formula:**
```
chefStars = floor(log10(totalEarnings / 10,000))
```

**Where:**
- `totalEarnings`: Player's total lifetime earnings
- Result: Logarithmic scaling (each star requires 10x more earnings)

**Example:**
- $10,000: floor(log10(10,000 / 10,000)) = 0 stars
- $100,000: floor(log10(100,000 / 10,000)) = 1 star
- $1,000,000: floor(log10(1,000,000 / 10,000)) = 2 stars

**Location:** `UpgradeCosts.GetChefStars()`

---

### Next Prestige Value
**Formula:**
```
nextPrestigeValue = floor(10^currentStars × 100,000)
```

**Where:**
- `currentStars`: Current number of chef stars
- Result: Earnings needed for next star

**Example:**
- 0 stars: floor(10^0 × 100,000) = $100,000
- 1 star: floor(10^1 × 100,000) = $1,000,000
- 2 stars: floor(10^2 × 100,000) = $10,000,000

**Location:** `UpgradeCosts.GetNextPrestigeValue()`

---

## 🔧 UTILITY FORMULAS

### Deliveries Needed to Afford Upgrade
**Formula:**
```
deliveriesNeeded = ceil(upgradeCost / incomePerDelivery)
```

**Location:** `EconomyCalculator.CalculateDeliveriesNeeded()`

---

### Time to Afford Upgrade
**Formula:**
```
timeSeconds = upgradeCost / incomePerSecond
```

**Location:** `EconomyCalculator.CalculateTimeToAfford()`

---

## 📈 BALANCE METRICS

### Early Game (Levels 1-5)
- **Target:** 5-15 deliveries per upgrade
- **Income:** ~$200 per delivery
- **Upgrade Costs:** $1,000 - $15,000

### Mid Game (Levels 5-10)
- **Target:** 20-50 deliveries per upgrade
- **Income:** ~$300-500 per delivery
- **Upgrade Costs:** $15,000 - $50,000

### Late Game (Levels 10+)
- **Target:** 50-150 deliveries per upgrade
- **Income:** ~$500-1,000 per delivery
- **Upgrade Costs:** $50,000 - $200,000

---

## 🧪 TESTING

Use `EconomyBalanceTester` component in Unity Editor to test balance:
1. Add component to any GameObject
2. Set test parameters
3. Right-click component → "Run All Balance Tests"
4. Check console for balance metrics

**Test Methods:**
- `RunAllBalanceTests()`: Tests all upgrade types
- `QuickTest()`: Tests levels 1-10
- `LateGameTest()`: Tests levels 20-30

---

## 📝 NOTES

### Soft Exponential Scaling
The diminishing returns factor `(1 / (1 + level × 0.1))` prevents late-game costs from becoming astronomical:
- Level 10: 50% of base exponential
- Level 20: 33% of base exponential
- Level 30: 25% of base exponential

### Material Upgrades
Material upgrades provide meaningful value:
- Bread: 2.5% of base per level
- Chicken: 4% of base per level
- Sauce: 1.5% of base per level

### Prestige System
Prestige provides significant bonuses:
- Income: +10% per star
- Cost Reduction: -2.5% per star
- Cook Rate: +4% per star

---

**Document Version:** 1.0  
**Last Updated:** After economy fixes implementation  
**Maintained By:** Economy System

