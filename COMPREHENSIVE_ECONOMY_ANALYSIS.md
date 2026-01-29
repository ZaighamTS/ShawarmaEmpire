# Comprehensive Economy & Progression Analysis - Shawarma Dash

## Executive Summary

This document provides a complete analysis of the Shawarma Dash economy system, including income sources, cost progressions, earning rates, and progression speed. The game is an idle/incremental business simulation where players produce shawarmas, store them in warehouses, and earn money through delivery and catering systems.

---

## 🎮 Game Overview

**Game Type**: Idle/Incremental Business Simulator  
**Core Loop**: Produce → Store → Deliver → Earn → Upgrade → Repeat  
**Primary Income**: Delivery vans (80% tax rate)  
**Secondary Income**: Catering vans (80% tax rate)  
**Production**: Manual tapping + auto-chef (no direct earnings)

---

## 💰 INCOME SYSTEM

### Income Sources

#### 1. **Delivery Van System** (Primary Income)
- **Location**: `DeliveryVan.cs:78-85`
- **Earning Formula**: 
  ```
  earnings = shawarmaValue × quantity × 0.80
  ```
- **Tax Rate**: 20% (0.80 multiplier)
- **Spawn Interval**: 
  ```
  interval = 30 / (1 + upgradeLevel × 0.08) seconds
  ```
- **Delivery Capacity**: 
  ```
  capacity = 3 × (1 + level × 0.5)
  ```
- **Base Capacity**: 3 shawarmas per delivery
- **Capacity Multiplier**: +0.5 per level (50% increase per level)

**Delivery Interval Progression:**
| Level | Interval (sec) | Deliveries/min | Notes |
|-------|----------------|----------------|-------|
| 0 | 30.0 | 2.0 | Base rate |
| 1 | 27.8 | 2.16 | First upgrade |
| 5 | 21.4 | 2.80 | Mid-game |
| 10 | 16.7 | 3.59 | Late-game |
| 20 | 12.5 | 4.80 | Max upgrade |

**Delivery Capacity Progression:**
| Level | Capacity | Shawarmas/Delivery |
|-------|----------|-------------------|
| 0 | 3 | 3 |
| 1 | 4.5 | 4-5 |
| 5 | 10.5 | 10-11 |
| 10 | 18 | 18 |
| 20 | 33 | 33 |

#### 2. **Catering Van System** (Secondary Income)
- **Location**: `CateringVan.cs:69-77`
- **Earning Formula**: 
  ```
  earnings = shawarmaValue × quantity × 0.80
  ```
- **Tax Rate**: 20% (0.80 multiplier) - matches delivery system
- **Spawn Interval**: 
  ```
  interval = 45 / (1 + upgradeLevel × 0.08) seconds
  ```
- **Delivery Capacity**: 
  ```
  capacity = 5 × (1 + level × 0.5)
  ```
- **Base Capacity**: 5 shawarmas per catering order
- **Capacity Multiplier**: +0.5 per level (50% increase per level)

**Catering Interval Progression:**
| Level | Interval (sec) | Orders/min | Notes |
|-------|----------------|------------|-------|
| 0 | 45.0 | 1.33 | Base rate |
| 1 | 41.7 | 1.44 | First upgrade |
| 5 | 32.1 | 1.87 | Mid-game |
| 10 | 25.0 | 2.40 | Late-game |
| 20 | 18.8 | 3.19 | Max upgrade |

**Catering Capacity Progression:**
| Level | Capacity | Shawarmas/Order |
|-------|----------|----------------|
| 0 | 5 | 5 |
| 1 | 7.5 | 7-8 |
| 5 | 17.5 | 17-18 |
| 10 | 30 | 30 |
| 20 | 55 | 55 |

#### 3. **Offline Earnings**
- **Location**: `GameManager.cs:61-169`
- **Calculation**: Based on estimated delivery rate
- **Time Cap**: 24 hours maximum
- **Earnings Cap**: 1 hour of active play
- **Absolute Max**: $10,000,000
- **Formula**:
  ```
  estimatedRate = (shawarmaValue × avgDeliverySize × deliveriesPerMin × 0.80) / 60
  offlineEarnings = min(estimatedRate × min(secondsOffline, 86400), estimatedRate × 3600)
  ```
- **Delivery Frequency Estimate**: 4 deliveries/minute
- **Average Delivery Size**: min(totalCapacity × 0.07, 30)

---

## 💵 SHAWARMA VALUE CALCULATION

### Base Value
- **Base Shawarma Value**: $100 (`UpgradeCosts.cs:84`)
- **Cook Rate Base Value**: 200 (`UpgradeCosts.cs:85`)

### Value Formula
```
shawarmaValue = (baseValue + materialBonuses + prestigeBonus) × qualityBonus
```

### Material Upgrades
**Location**: `UpgradeCosts.cs:237-248`

| Upgrade Type | Formula | Value per Level | Max Level | Total Bonus (Max) |
|-------------|---------|----------------|-----------|------------------|
| Bread | `level × 5` | +$5 | 10 | +$50 |
| Chicken | `level × 8` | +$8 | 10 | +$80 |
| Sauce | `level × 3` | +$3 | 10 | +$30 |

**Total Material Bonus (All Max)**: +$160 per shawarma

### Prestige Income Bonus
**Location**: `UpgradeCosts.cs:219-222`
```
prestigeBonus = prestigeStars × 0.1 × baseValue
prestigeBonus = prestigeStars × 10
```

| Prestige Stars | Bonus per Shawarma | Total Value (Base) |
|---------------|-------------------|-------------------|
| 0 | +$0 | $100 |
| 1 | +$10 | $110 |
| 5 | +$50 | $150 |
| 10 | +$100 | $200 |

### Example Value Calculations

**Early Game (No Upgrades, 0 Stars)**:
- Base: $100
- Material: $0
- Prestige: $0
- **Total**: $100 per shawarma
- **After Tax (80%)**: $80 per shawarma

**Mid Game (Bread:5, Chicken:3, Sauce:2, 1 Star)**:
- Base: $100
- Material: (5×5) + (3×8) + (2×3) = $55
- Prestige: $10
- **Total**: $165 per shawarma
- **After Tax (80%)**: $132 per shawarma

**Late Game (All Max, 5 Stars)**:
- Base: $100
- Material: $160
- Prestige: $50
- **Total**: $310 per shawarma
- **After Tax (80%)**: $248 per shawarma

---

## 💸 COST SYSTEM

### Upgrade Cost Formula
**Location**: `UpgradeCosts.cs:121-142`
```
cost = (basePrice - prestigeReduction) × (level^multiplier) × (1 / (1 + level × 0.1))
```

**Components**:
- `basePrice`: Base cost (varies by type)
- `prestigeReduction`: `prestigeStars × 0.025 × 100` = `prestigeStars × 2.5`
- `level`: Current upgrade level (starts at 1)
- `multiplier`: Exponential multiplier (varies by type)
- `diminishingFactor`: `1 / (1 + level × 0.1)` - prevents astronomical costs

### Upgrade Type Configurations

#### 1. Storage (Warehouse)
- **Base Price**: $1,000
- **Purchase Multiplier**: 1.5x (for new buildings)
- **Upgrade Multiplier**: 1.4x
- **Role**: High value bottleneck, prevents production stops
- **Capacity Formula**: `100 × (1 + level × 1.4)`

**Cost Progression (0 prestige stars):**
| Level | Cost | Capacity | Cost per Capacity |
|-------|------|----------|-------------------|
| 1 | $1,000 | 240 | $4.17 |
| 2 | $2,400 | 380 | $6.32 |
| 3 | $4,200 | 520 | $8.08 |
| 5 | $9,000 | 800 | $11.25 |
| 10 | $25,120 | 1,500 | $16.75 |
| 15 | $44,800 | 2,200 | $20.36 |
| 20 | $67,200 | 2,900 | $23.17 |

#### 2. Delivery Van
- **Base Price**: $500
- **Purchase Multiplier**: 1.5x
- **Upgrade Multiplier**: 1.35x
- **Role**: Critical bottleneck, primary income source

**Cost Progression (0 prestige stars):**
| Level | Cost | Capacity | Interval (sec) |
|-------|------|----------|----------------|
| 1 | $500 | 4-5 | 27.8 |
| 2 | $1,200 | 6-7 | 25.9 |
| 3 | $2,000 | 7-8 | 24.2 |
| 5 | $3,900 | 10-11 | 21.4 |
| 10 | $10,600 | 18 | 16.7 |
| 15 | $18,900 | 25-26 | 13.6 |
| 20 | $28,400 | 33 | 12.5 |

#### 3. Kitchen
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
| 5 | $16,000 |
| 10 | $40,000 |
| 15 | $71,400 |
| 20 | $107,200 |

#### 4. Catering
- **Base Price**: $1,500
- **Purchase Multiplier**: 1.2x
- **Upgrade Multiplier**: 1.25x
- **Role**: Bonus income, secondary value

**Cost Progression (0 prestige stars):**
| Level | Cost | Capacity | Interval (sec) |
|-------|------|----------|----------------|
| 1 | $1,500 | 7-8 | 41.7 |
| 2 | $3,400 | 10-11 | 38.5 |
| 3 | $5,600 | 12-13 | 35.7 |
| 5 | $10,600 | 17-18 | 32.1 |
| 10 | $26,800 | 30 | 25.0 |
| 15 | $47,800 | 42-43 | 20.7 |
| 20 | $71,700 | 55 | 18.8 |

### Building Purchase Costs
**Location**: `UpgradeCosts.cs:102-119`
```
purchaseCost = (basePrice - prestigeReduction) × (2.5^existingCount)
```

**Purchase Cost Progression:**
| Building # | Storage | Delivery Van | Kitchen | Catering |
|-----------|---------|--------------|---------|----------|
| 1st | $1,000 | $500 | $2,000 | $1,500 |
| 2nd | $2,500 | $1,250 | $5,000 | $3,750 |
| 3rd | $6,250 | $3,125 | $12,500 | $9,375 |
| 4th | $15,625 | $7,813 | $31,250 | $23,438 |
| 5th | $39,063 | $19,531 | $78,125 | $58,594 |

### Prestige Cost Reduction
**Location**: `UpgradeCosts.cs:223-226`
```
costReduction = prestigeStars × 0.025 × 100
costReduction = prestigeStars × 2.5
```

| Prestige Stars | Cost Reduction |
|---------------|----------------|
| 0 | $0 |
| 1 | $2.50 |
| 5 | $12.50 |
| 10 | $25.00 |

**Impact**: Minimal early game, becomes more significant at higher prestige levels.

### Raw Material Upgrade Costs
**Location**: `CommonAbilities.cs` (referenced in comments)
```
cost = (level + 1) × 100
```

**Cost Progression:**
| Level | Cost | Cumulative |
|-------|------|------------|
| 0→1 | $100 | $100 |
| 1→2 | $200 | $300 |
| 2→3 | $300 | $600 |
| 5→6 | $600 | $2,100 |
| 9→10 | $1,000 | $5,500 |

**Total Cost to Max One Material**: $5,500  
**Total Cost to Max All Materials**: $22,000 (Bread + Chicken + Sauce + Chef)

---

## 📊 EARNING RATE ANALYSIS

### Early Game Income (Level 0, Base Values)

**Delivery System:**
- Shawarma Value: $100
- Delivery Capacity: 3
- Delivery Interval: 30 seconds
- Deliveries per Minute: 2.0
- **Income per Delivery**: $100 × 3 × 0.80 = $240
- **Income per Minute**: $240 × 2.0 = **$480/min**
- **Income per Hour**: **$28,800/hour**

**Catering System:**
- Shawarma Value: $100
- Catering Capacity: 5
- Catering Interval: 45 seconds
- Orders per Minute: 1.33
- **Income per Order**: $100 × 5 × 0.80 = $400
- **Income per Minute**: $400 × 1.33 = **$533/min**
- **Income per Hour**: **$32,000/hour**

**Combined Early Game Income**: ~$60,800/hour

### Mid Game Income (Level 5, Moderate Upgrades)

**Assumptions:**
- Shawarma Value: $165 (Bread:5, Chicken:3, Sauce:2, 1 Star)
- Delivery Level: 5 (Capacity: 10-11, Interval: 21.4s)
- Catering Level: 5 (Capacity: 17-18, Interval: 32.1s)

**Delivery System:**
- Income per Delivery: $165 × 10.5 × 0.80 = $1,386
- Deliveries per Minute: 60 / 21.4 = 2.80
- **Income per Minute**: $1,386 × 2.80 = **$3,881/min**
- **Income per Hour**: **$232,860/hour**

**Catering System:**
- Income per Order: $165 × 17.5 × 0.80 = $2,310
- Orders per Minute: 60 / 32.1 = 1.87
- **Income per Minute**: $2,310 × 1.87 = **$4,320/min**
- **Income per Hour**: **$259,200/hour**

**Combined Mid Game Income**: ~$492,060/hour

### Late Game Income (Level 10, High Upgrades)

**Assumptions:**
- Shawarma Value: $310 (All materials max, 5 Stars)
- Delivery Level: 10 (Capacity: 18, Interval: 16.7s)
- Catering Level: 10 (Capacity: 30, Interval: 25.0s)

**Delivery System:**
- Income per Delivery: $310 × 18 × 0.80 = $4,464
- Deliveries per Minute: 60 / 16.7 = 3.59
- **Income per Minute**: $4,464 × 3.59 = **$16,026/min**
- **Income per Hour**: **$961,560/hour**

**Catering System:**
- Income per Order: $310 × 30 × 0.80 = $7,440
- Orders per Minute: 60 / 25.0 = 2.40
- **Income per Minute**: $7,440 × 2.40 = **$17,856/min**
- **Income per Hour**: **$1,071,360/hour**

**Combined Late Game Income**: ~$2,032,920/hour (~$2M/hour)

---

## ⏱️ PROGRESSION SPEED ANALYSIS

### Time to Afford Upgrades

#### Early Game (Level 0 → Level 1)

**Delivery Van Upgrade (Level 1: $500)**
- Current Income: $480/min
- Time Required: $500 / $480 = **1.04 minutes**

**Storage Upgrade (Level 1: $1,000)**
- Current Income: $480/min
- Time Required: $1,000 / $480 = **2.08 minutes**

**Catering Upgrade (Level 1: $1,500)**
- Current Income: $480/min
- Time Required: $1,500 / $480 = **3.13 minutes**

#### Mid Game (Level 5 → Level 6)

**Delivery Van Upgrade (Level 6: ~$5,200)**
- Current Income: $3,881/min
- Time Required: $5,200 / $3,881 = **1.34 minutes**

**Storage Upgrade (Level 6: ~$11,500)**
- Current Income: $3,881/min
- Time Required: $11,500 / $3,881 = **2.96 minutes**

**Catering Upgrade (Level 6: ~$13,200)**
- Current Income: $4,320/min
- Time Required: $13,200 / $4,320 = **3.06 minutes**

#### Late Game (Level 10 → Level 11)

**Delivery Van Upgrade (Level 11: ~$12,500)**
- Current Income: $16,026/min
- Time Required: $12,500 / $16,026 = **0.78 minutes**

**Storage Upgrade (Level 11: ~$28,000)**
- Current Income: $16,026/min
- Time Required: $28,000 / $16,026 = **1.75 minutes**

**Catering Upgrade (Level 11: ~$30,000)**
- Current Income: $17,856/min
- Time Required: $30,000 / $17,856 = **1.68 minutes**

### Upgrade Frequency Analysis

**Early Game (Levels 1-5)**:
- Average upgrade cost: $2,000-$5,000
- Average income: $480-$3,881/min
- **Average time per upgrade**: 2-5 minutes
- **Upgrades per hour**: ~12-30 upgrades

**Mid Game (Levels 5-10)**:
- Average upgrade cost: $5,000-$15,000
- Average income: $3,881-$16,026/min
- **Average time per upgrade**: 1-4 minutes
- **Upgrades per hour**: ~15-60 upgrades

**Late Game (Levels 10-20)**:
- Average upgrade cost: $15,000-$50,000
- Average income: $16,026-$50,000+/min
- **Average time per upgrade**: 0.5-3 minutes
- **Upgrades per hour**: ~20-120 upgrades

---

## 🎯 PROGRESSION BOTTLENECKS

### 1. **Storage Capacity** (Critical Bottleneck)
- **Impact**: When storage is full, production stops completely
- **Solution**: Upgrade warehouses regularly
- **Cost**: $1,000-$67,200 (Levels 1-20)
- **Capacity Growth**: 240 → 2,900 shawarmas

### 2. **Delivery Rate** (Primary Income Bottleneck)
- **Impact**: Limits how fast you can convert shawarmas to cash
- **Solution**: Upgrade delivery van capacity and speed
- **Cost**: $500-$28,400 (Levels 1-20)
- **Capacity Growth**: 3 → 33 shawarmas per delivery
- **Speed Growth**: 30s → 12.5s intervals

### 3. **Production Rate** (Secondary Bottleneck)
- **Impact**: Limits how fast you can fill storage
- **Solution**: Upgrade kitchen, tap faster, unlock auto-chef
- **Cost**: $2,000-$107,200 (Levels 1-20)
- **Note**: Production doesn't directly earn money, only enables deliveries

### 4. **Shawarma Value** (Long-term Bottleneck)
- **Impact**: Determines income per delivery
- **Solution**: Upgrade materials (Bread, Chicken, Sauce) and prestige
- **Cost**: $100-$1,000 per material level
- **Total Investment**: $22,000 to max all materials

---

## ⭐ PRESTIGE SYSTEM

### Chef Stars Calculation
**Location**: `UpgradeCosts.cs:198-203`
```
chefStars = floor(log10(totalEarnings / 10,000))
```

**Prestige Thresholds:**
| Total Earnings | Chef Stars | Next Star Requires |
|---------------|------------|-------------------|
| $0 - $99,999 | 0 | $100,000 |
| $100,000 - $999,999 | 1 | $1,000,000 |
| $1,000,000 - $9,999,999 | 2 | $10,000,000 |
| $10,000,000 - $99,999,999 | 3 | $100,000,000 |
| $100,000,000+ | 4+ | $1,000,000,000+ |

### Prestige Bonuses

#### Income Bonus
```
bonus = prestigeStars × 10 per shawarma
```
- 1 Star: +$10 per shawarma (+10%)
- 5 Stars: +$50 per shawarma (+50%)
- 10 Stars: +$100 per shawarma (+100%)

#### Cost Reduction
```
reduction = prestigeStars × 2.5
```
- 1 Star: -$2.50 from base prices
- 5 Stars: -$12.50 from base prices
- 10 Stars: -$25.00 from base prices

#### Cook Rate Bonus
**Location**: `UpgradeCosts.cs:227-231`
```
bonus = prestigeStars × 0.04 × 100 = prestigeStars × 4
```
- 1 Star: +4 cook rate
- 5 Stars: +20 cook rate
- 10 Stars: +40 cook rate

### Prestige Reset
- **Location**: `GameManager.cs:256-261`
- **What Resets**: Cash, Shawarma count (keeps Chef Stars and Total Earnings)
- **What's Kept**: Chef Stars, Total Earnings, Prestige bonuses

---

## 📈 ECONOMY BALANCE ANALYSIS

### Strengths

1. **Clear Income Sources**: Delivery and catering systems provide predictable income
2. **Balanced Tax Rate**: 20% tax prevents excessive early game income
3. **Soft Exponential Costs**: Diminishing returns prevent astronomical late-game costs
4. **Multiple Upgrade Paths**: Storage, delivery, catering, materials, and prestige provide varied progression
5. **Offline Earnings**: Rewards players for returning, capped to prevent abuse

### Potential Issues

1. **Early Game Speed**: Very fast progression (upgrades every 1-3 minutes) may feel rushed
2. **Mid Game Plateau**: Income scales faster than costs, making mid-game too easy
3. **Late Game Costs**: While soft exponential helps, costs still grow significantly
4. **Storage Bottleneck**: Full storage completely stops production - may frustrate players
5. **Material Upgrade Value**: $22,000 total investment for +$160 value may not feel rewarding enough

### Balance Recommendations

1. **Early Game**: Consider slightly increasing base costs or reducing early income
2. **Mid Game**: Add more meaningful upgrades or increase costs slightly
3. **Late Game**: Ensure prestige bonuses scale appropriately with costs
4. **Storage**: Consider partial production when storage is full (reduced rate)
5. **Materials**: Increase material upgrade values or reduce costs

---

## 🔢 KEY METRICS SUMMARY

### Income Rates
- **Early Game**: ~$60,800/hour
- **Mid Game**: ~$492,060/hour
- **Late Game**: ~$2,032,920/hour

### Upgrade Costs
- **Early Game**: $500-$2,000 (1-3 minutes to afford)
- **Mid Game**: $5,000-$15,000 (1-4 minutes to afford)
- **Late Game**: $15,000-$50,000+ (0.5-3 minutes to afford)

### Progression Speed
- **Early Game**: 12-30 upgrades/hour
- **Mid Game**: 15-60 upgrades/hour
- **Late Game**: 20-120 upgrades/hour

### Capacity Growth
- **Storage**: 240 → 2,900 shawarmas (12x growth)
- **Delivery**: 3 → 33 shawarmas (11x growth)
- **Catering**: 5 → 55 shawarmas (11x growth)

### Value Growth
- **Base Value**: $100
- **Max Material Bonus**: +$160 (+160%)
- **Max Prestige Bonus**: +$100+ (+100%+)
- **Total Max Value**: $360+ per shawarma (+260%+)

---

## 📝 CONCLUSION

The Shawarma Dash economy is well-balanced for an idle/incremental game, with clear progression paths and multiple upgrade systems. The soft exponential cost scaling prevents late-game stagnation while the multiple income sources (delivery + catering) provide varied gameplay. The prestige system offers long-term progression goals, though material upgrades may need value adjustments.

**Overall Assessment**: The economy supports engaging progression with reasonable pacing throughout all game stages. Minor adjustments to early game speed and material upgrade values could further improve the experience.

---

**Document Version**: 2.0  
**Last Updated**: Current Analysis  
**Based On**: Code analysis of `UpgradeCosts.cs`, `DeliveryVan.cs`, `CateringVan.cs`, `GameManager.cs`, and related systems
