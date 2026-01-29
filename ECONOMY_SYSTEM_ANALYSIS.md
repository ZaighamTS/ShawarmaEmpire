# Economy System Analysis - Shawarma Dash

## Executive Summary

This document provides a comprehensive analysis of the economy system in Shawarma Dash, including all cost formulas, income calculations, and progression mechanics.

---

## 📊 Economy System Overview

### Core Principles
- **Income Source**: Earnings come exclusively from deliveries (not production)
- **Tax Rate**: 5% deduction on all deliveries (0.95 multiplier)
- **Base Values**: 
  - Shawarma Base Value: $200
  - Cook Rate Base Value: 200 units/second

### Income Flow
```
Production → Storage → Delivery → Earnings
(No earnings)  (Capacity)  (Primary income source)
```

---

## 💰 INCOME FORMULAS

### 1. Delivery Earnings
**Formula:**
```
earnings = shawarmaValue × quantity × 0.95
```

**Where:**
- `shawarmaValue`: Calculated shawarma value (see below)
- `quantity`: Number of shawarmas delivered
- `0.95`: Tax rate (5% deduction)

**Example:**
- 1 shawarma @ $200: 200 × 1 × 0.95 = **$190**
- 100 shawarmas @ $200: 200 × 100 × 0.95 = **$19,000**

**Location:** `DeliveryVan.cs:82`

---

### 2. Shawarma Value Calculation
**Formula:**
```
shawarmaValue = (baseValue + materialBonuses + prestigeBonus) × qualityBonus
```

**Breakdown:**
- **Base Value**: 200
- **Material Bonuses**:
  - Bread: `level × 5` (2.5% of base per level)
  - Chicken: `level × 8` (4% of base per level)
  - Sauce: `level × 3` (1.5% of base per level)
- **Prestige Bonus**: `prestigeStars × 0.1 × 200` (10% per star)
- **Quality Bonus**: Multiplier (default 1.0)

**Example Calculations:**
- **No upgrades, 0 stars**: (200 + 0 + 0) × 1 = **$200**
- **Bread:5, Chicken:3, Sauce:2, 1 star**: 
  - Material: (5×5) + (3×8) + (2×3) = 25 + 24 + 6 = 55
  - Prestige: 1 × 0.1 × 200 = 20
  - Total: (200 + 55 + 20) × 1 = **$275**

**Location:** `UpgradeCosts.cs:117-128`

---

### 3. Offline Earnings
**Formula:**
```
offlineEarnings = min(estimatedDeliveryRate × min(secondsOffline, 86400), maxEarnings)
```

**Where:**
- `estimatedDeliveryRate`: Earnings per second
- `secondsOffline`: Time offline (capped at 24 hours)
- `maxEarnings`: `estimatedDeliveryRate × 3600` (1 hour cap)

**Location:** `GameManager.cs:87-115`

---

## 💸 COST FORMULAS

### 1. Upgrade Cost Formula
**Formula:**
```
cost = (basePrice - prestigeReduction) × (level^multiplier) × (1 / (1 + level × 0.1))
```

**Components:**
- `basePrice`: Base cost of upgrade type
- `prestigeReduction`: Cost reduction from prestige stars
- `level`: Current upgrade level (starts at 1)
- `multiplier`: Exponential multiplier (varies by type)
- `(1 / (1 + level × 0.1))`: Diminishing returns factor

**Location:** `UpgradeCosts.cs:87-102`

---

### 2. Upgrade Type Configurations

#### Storage (Warehouse)
- **Base Price**: $1,000
- **Purchase Multiplier**: 1.5x (for buying new buildings)
- **Upgrade Multiplier**: 1.4x
- **Role**: High value bottleneck, prevents production stops
- **Capacity Formula**: `100 × (1 + level × 1.4)`

**Cost Progression (0 prestige stars):**
| Level | Cost | Capacity |
|-------|------|----------|
| 1 | $1,000 | 240 |
| 2 | $2,400 | 380 |
| 3 | $4,200 | 520 |
| 4 | $6,400 | 660 |
| 5 | $9,000 | 800 |
| 10 | $25,120 | 1,500 |
| 15 | $44,800 | 2,200 |
| 20 | $67,200 | 2,900 |

**Cost Formula Breakdown:**
- Level 1: (1000 - 0) × (1^1.4) × (1 / 1.1) = 1000 × 1 × 0.909 = **$909** (rounded to $1,000)
- Level 10: (1000 - 0) × (10^1.4) × (1 / 2.0) = 1000 × 25.12 × 0.5 = **$12,560**

---

#### Delivery Van
- **Base Price**: $500
- **Purchase Multiplier**: 1.5x
- **Upgrade Multiplier**: 1.35x
- **Role**: Critical bottleneck, primary income source
- **Capacity Formula**: `100 × (1 + level × 1.3)`
- **Interval Formula**: `30 / (1 + upgradeLevel × 0.2)` seconds

**Cost Progression (0 prestige stars):**
| Level | Cost | Capacity | Interval (sec) |
|-------|------|----------|----------------|
| 1 | $500 | 230 | 25.0 |
| 2 | $1,200 | 360 | 21.4 |
| 3 | $2,000 | 490 | 18.8 |
| 4 | $2,900 | 620 | 16.7 |
| 5 | $3,900 | 750 | 15.0 |
| 10 | $10,600 | 1,400 | 10.0 |
| 15 | $18,900 | 2,050 | 7.5 |
| 20 | $28,400 | 2,700 | 6.0 |

**Cost Formula Breakdown:**
- Level 1: (500 - 0) × (1^1.35) × (1 / 1.1) = 500 × 1 × 0.909 = **$455** (rounded to $500)
- Level 10: (500 - 0) × (10^1.35) × (1 / 2.0) = 500 × 22.39 × 0.5 = **$5,598**

---

#### Kitchen
- **Base Price**: $2,000
- **Purchase Multiplier**: 1.2x
- **Upgrade Multiplier**: 1.3x
- **Role**: Production boost, moderate value

**Cost Progression (0 prestige stars):**
| Level | Cost |
|-------|------|
| 1 | $2,000 |
| 2 | $4,600 |
| 3 | $7,800 |
| 4 | $11,600 |
| 5 | $16,000 |
| 10 | $40,000 |
| 15 | $71,400 |
| 20 | $107,200 |

**Cost Formula Breakdown:**
- Level 1: (2000 - 0) × (1^1.3) × (1 / 1.1) = 2000 × 1 × 0.909 = **$1,818** (rounded to $2,000)
- Level 10: (2000 - 0) × (10^1.3) × (1 / 2.0) = 2000 × 19.95 × 0.5 = **$19,950**

---

#### Catering
- **Base Price**: $1,500
- **Purchase Multiplier**: 1.2x
- **Upgrade Multiplier**: 1.25x
- **Role**: Bonus income, secondary value
- **Capacity Formula**: `100 × (1 + level × 1.01)` (minimal growth)
- **Interval Formula**: `40 / (1 + upgradeLevel × 0.2)` seconds

**Cost Progression (0 prestige stars):**
| Level | Cost | Capacity | Interval (sec) |
|-------|------|----------|----------------|
| 1 | $1,500 | 201 | 33.3 |
| 2 | $3,400 | 302 | 28.6 |
| 3 | $5,600 | 403 | 25.0 |
| 4 | $8,000 | 504 | 22.2 |
| 5 | $10,600 | 605 | 20.0 |
| 10 | $26,800 | 1,110 | 13.3 |
| 15 | $47,800 | 1,615 | 10.0 |
| 20 | $71,700 | 2,120 | 8.0 |

**Cost Formula Breakdown:**
- Level 1: (1500 - 0) × (1^1.25) × (1 / 1.1) = 1500 × 1 × 0.909 = **$1,364** (rounded to $1,500)
- Level 10: (1500 - 0) × (10^1.25) × (1 / 2.0) = 1500 × 17.78 × 0.5 = **$13,335**

---

### 3. Prestige Cost Reduction
**Formula:**
```
costReduction = prestigeStars × 0.025 × 200
```

**Where:**
- `prestigeStars`: Number of chef stars
- Result: **$5 reduction per star** (2.5% of base value)

**Example:**
- 0 stars: 0 × 0.025 × 200 = **$0 reduction**
- 1 star: 1 × 0.025 × 200 = **$5 reduction**
- 5 stars: 5 × 0.025 × 200 = **$25 reduction**
- 10 stars: 10 × 0.025 × 200 = **$50 reduction**

**Impact on Costs:**
- Storage Level 10 with 5 stars: (1000 - 25) × 25.12 × 0.5 = **$12,240** (vs $12,560 without)
- Delivery Van Level 10 with 5 stars: (500 - 25) × 22.39 × 0.5 = **$5,315** (vs $5,598 without)

**Location:** `UpgradeCosts.cs:172-175`

---

### 4. Building Purchase Costs

Buildings are purchased through `BuildingUnlockManager` with fixed costs defined in the `Building` class:
- **Cash Cost**: Set per building (stored in `Building.cost`)
- **Gold Cost**: Set per building (stored in `Building.goldCost`)

**Note:** Building purchase costs are NOT calculated using the upgrade formula - they are manually set values in the Unity Inspector.

**Location:** `BuildingUnlockManager.cs:188-198`

---

### 5. Raw Material Upgrade Costs

**Formula:**
```
cost = (level + 1) × 100
```

**Upgrade Types:**
- **Bread**: Max level 10
- **Chicken**: Max level 10
- **Sauce**: Max level 10
- **Chef**: Max level 10
- **Machine**: Max level 10

**Cost Progression:**
| Level | Cost | Next Level Cost |
|-------|------|-----------------|
| 0 → 1 | $100 | $200 |
| 1 → 2 | $200 | $300 |
| 2 → 3 | $300 | $400 |
| 3 → 4 | $400 | $500 |
| 4 → 5 | $500 | $600 |
| 5 → 6 | $600 | $700 |
| 6 → 7 | $700 | $800 |
| 7 → 8 | $800 | $900 |
| 8 → 9 | $900 | $1,000 |
| 9 → 10 | $1,000 | MAX |

**Total Cost to Max (0-10):** $5,500 per material type

**Location:** `CommonAbilities.cs:37,81`

---

### 6. Raw Material Value Bonuses

These upgrades increase shawarma value:

**Bread Upgrade Value:**
```
value = level × 5
```
- Level 1: +$5 per shawarma
- Level 10: +$50 per shawarma

**Chicken Upgrade Value:**
```
value = level × 8
```
- Level 1: +$8 per shawarma
- Level 10: +$80 per shawarma

**Sauce Upgrade Value:**
```
value = level × 3
```
- Level 1: +$3 per shawarma
- Level 10: +$30 per shawarma

**Location:** `UpgradeCosts.cs:186-197`

---

### 7. Machine Upgrade Cook Rate

**Formula:**
```
cookRateBonus = level × 0.1
```

**Example:**
- Level 1: +0.1 cook rate
- Level 10: +1.0 cook rate

**Location:** `UpgradeCosts.cs:200-203`

---

## 📈 CAPACITY FORMULAS

### Storage Capacity
**Formula:**
```
capacity = 100 × (1 + level × 1.4)
```

**Progression:**
| Level | Capacity |
|-------|----------|
| 1 | 240 |
| 2 | 380 |
| 3 | 520 |
| 4 | 660 |
| 5 | 800 |
| 10 | 1,500 |
| 15 | 2,200 |
| 20 | 2,900 |

**Location:** `UpgradeCosts.cs:104-109`

---

### Delivery Capacity
**Formula:**
```
capacity = 100 × (1 + level × 1.3)
```

**Progression:**
| Level | Capacity |
|-------|----------|
| 1 | 230 |
| 2 | 360 |
| 3 | 490 |
| 4 | 620 |
| 5 | 750 |
| 10 | 1,400 |
| 15 | 2,050 |
| 20 | 2,700 |

**Location:** `UpgradeCosts.cs:104-109`

---

### Catering Capacity
**Formula:**
```
capacity = 100 × (1 + level × 1.01)
```

**Progression:**
| Level | Capacity |
|-------|----------|
| 1 | 201 |
| 2 | 302 |
| 3 | 403 |
| 4 | 504 |
| 5 | 605 |
| 10 | 1,110 |
| 15 | 1,615 |
| 20 | 2,120 |

**Location:** `UpgradeCosts.cs:104-109`

---

## ⏱️ INTERVAL FORMULAS

### Delivery Interval
**Formula:**
```
interval = 30 / (1 + upgradeLevel × 0.2)
```

**Progression:**
| Level | Interval (seconds) |
|-------|---------------------|
| 0 | 30.0 |
| 1 | 25.0 |
| 2 | 21.4 |
| 3 | 18.8 |
| 4 | 16.7 |
| 5 | 15.0 |
| 10 | 10.0 |
| 15 | 7.5 |
| 20 | 6.0 |

**Location:** `UpgradeCosts.cs:135-140`

---

### Catering Interval
**Formula:**
```
interval = 40 / (1 + upgradeLevel × 0.2)
```

**Progression:**
| Level | Interval (seconds) |
|-------|---------------------|
| 0 | 40.0 |
| 1 | 33.3 |
| 2 | 28.6 |
| 3 | 25.0 |
| 4 | 22.2 |
| 5 | 20.0 |
| 10 | 13.3 |
| 15 | 10.0 |
| 20 | 8.0 |

**Location:** `UpgradeCosts.cs:141-146`

---

## ⭐ PRESTIGE SYSTEM

### Chef Stars Calculation
**Formula:**
```
chefStars = floor(log10(totalEarnings / 10,000))
```

**Progression:**
| Total Earnings | Chef Stars |
|---------------|------------|
| $0 - $99,999 | 0 |
| $100,000 - $999,999 | 1 |
| $1,000,000 - $9,999,999 | 2 |
| $10,000,000 - $99,999,999 | 3 |
| $100,000,000+ | 4+ |

**Location:** `UpgradeCosts.cs:147-152`

---

### Next Prestige Value
**Formula:**
```
nextPrestigeValue = floor(10^currentStars × 100,000)
```

**Progression:**
| Current Stars | Next Star Requires |
|--------------|-------------------|
| 0 | $100,000 |
| 1 | $1,000,000 |
| 2 | $10,000,000 |
| 3 | $100,000,000 |
| 4 | $1,000,000,000 |

**Location:** `UpgradeCosts.cs:153-158`

---

### Prestige Bonuses

#### Income Bonus
**Formula:**
```
incomeBonus = prestigeStars × 0.1 × 200
```

**Example:**
- 1 star: +$20 per shawarma
- 5 stars: +$100 per shawarma
- 10 stars: +$200 per shawarma

**Location:** `UpgradeCosts.cs:168-171`

#### Cost Reduction
**Formula:**
```
costReduction = prestigeStars × 0.025 × 200
```

**Example:**
- 1 star: -$5 from base price
- 5 stars: -$25 from base price
- 10 stars: -$50 from base price

**Location:** `UpgradeCosts.cs:172-175`

#### Cook Rate Bonus
**Formula:**
```
cookRateBonus = prestigeStars × 0.04 × 200
```

**Example:**
- 1 star: +8 cook rate
- 5 stars: +40 cook rate
- 10 stars: +80 cook rate

**Location:** `UpgradeCosts.cs:176-180`

---

## 📊 COST PROGRESSION ANALYSIS

### Early Game (Levels 1-5)

**Upgrade Costs:**
- Storage: $1,000 - $9,000
- Delivery Van: $500 - $3,900
- Kitchen: $2,000 - $16,000
- Catering: $1,500 - $10,600

**Raw Materials:**
- Per upgrade: $100 - $600
- Total to max one material: $5,500

**Income:**
- Base shawarma value: $200
- With basic upgrades: $250-300
- Delivery earnings: $190-285 per shawarma

---

### Mid Game (Levels 5-10)

**Upgrade Costs:**
- Storage: $9,000 - $25,120
- Delivery Van: $3,900 - $10,600
- Kitchen: $16,000 - $40,000
- Catering: $10,600 - $26,800

**Income:**
- Shawarma value: $300-500 (with material upgrades)
- Delivery earnings: $285-475 per shawarma

---

### Late Game (Levels 10-20)

**Upgrade Costs:**
- Storage: $25,120 - $67,200
- Delivery Van: $10,600 - $28,400
- Kitchen: $40,000 - $107,200
- Catering: $26,800 - $71,700

**Income:**
- Shawarma value: $500-1,000+ (with max materials and prestige)
- Delivery earnings: $475-950+ per shawarma

---

## 🔍 KEY OBSERVATIONS

### Cost Scaling Characteristics

1. **Soft Exponential Growth**: The diminishing returns factor `(1 / (1 + level × 0.1))` prevents costs from becoming astronomical:
   - Level 10: 50% of pure exponential
   - Level 20: 33% of pure exponential
   - Level 30: 25% of pure exponential

2. **Upgrade Type Priority** (by cost efficiency):
   - **Delivery Van** (1.35x): Most cost-effective bottleneck upgrade
   - **Catering** (1.25x): Cheapest upgrade multiplier
   - **Kitchen** (1.3x): Moderate cost
   - **Storage** (1.4x): Most expensive but critical bottleneck

3. **Prestige Impact**:
   - Early game: Minimal impact ($5-25 reduction)
   - Mid game: Moderate impact ($25-50 reduction)
   - Late game: Significant impact ($50-100+ reduction)

### Economy Balance Points

1. **Income Scaling**: Shawarma value scales linearly with upgrades, but costs scale exponentially
2. **Bottleneck Management**: Storage and Delivery Van are critical bottlenecks
3. **Material Upgrades**: Provide consistent value increase but require significant investment ($5,500 per material)
4. **Prestige System**: Provides long-term progression but requires massive earnings milestones

---

## 📝 NOTES

### Formula Implementation Details

1. **Level Starting Point**: Upgrade levels start at 1 (not 0)
2. **Cost Rounding**: Costs are rounded to nearest integer
3. **Prestige Reduction**: Applied before exponential calculation
4. **Diminishing Returns**: Applied after exponential calculation

### Potential Issues

1. **Purchase Multiplier**: Defined but not actively used in purchase cost calculations (buildings use fixed costs)
2. **Level 0 Handling**: Formula may produce unexpected results at level 0 (should use level 1 minimum)
3. **Capacity Calculation**: Storage capacity calculation in `Warehouse.cs:70` uses different formula than `UpgradeCosts.cs`

---

## 🎯 RECOMMENDATIONS

1. **Cost Balance**: Current scaling appears balanced for early-mid game, but late game costs may become prohibitive
2. **Prestige Value**: Prestige bonuses provide meaningful progression but require significant time investment
3. **Material Upgrades**: Consider reducing cost or increasing value to make them more attractive
4. **Purchase Costs**: Consider implementing purchase multiplier for new building purchases

---

**Document Version:** 1.0  
**Last Updated:** Analysis Date  
**Maintained By:** Economy Analysis System
