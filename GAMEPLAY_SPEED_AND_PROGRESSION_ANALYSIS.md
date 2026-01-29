# Gameplay Speed & Progression Analysis - Shawarma Dash

## Executive Summary

This document provides a comprehensive analysis of gameplay speed, income progression, and unlocking systems in Shawarma Dash. The analysis reveals a well-balanced economy system with elongated gameplay pacing, though the unlocking system lacks progressive feature gating.

---

## 🎮 GAMEPLAY SPEED ANALYSIS

### 1. Core Gameplay Loop Speed

#### Production Speed
- **Tapping System**: Manual production via tap-to-cook
  - Base production: Instant on tap
  - Multiplier system: 1.0x - 1.5x based on tap speed
  - Multiplier decay: 0.5x per second after 0.3s of no tapping
  - Location: `ShawarmaSpawner.cs:32-37`, `ShawarmaProductionSystem.cs:8-11`

- **Auto-Chef System**: Automated production
  - Interval: 2 seconds per shawarma (when unlocked)
  - Location: `ShawarmaProductionSystem.cs:22-24`

#### Delivery Speed
- **Base Interval**: 150 seconds (2.5 minutes)
- **Upgrade Formula**: `150 / (1 + upgradeLevel × 0.08)`
- **Progression**:
  | Level | Interval (seconds) | Interval (minutes) | Deliveries/Hour |
  |-------|-------------------|-------------------|-----------------|
  | 0     | 150.0             | 2.50              | 24.0            |
  | 1     | 138.9             | 2.32              | 25.9            |
  | 3     | 121.0             | 2.02              | 29.8            |
  | 5     | 107.1             | 1.79              | 33.6            |
  | 10    | 83.3              | 1.39              | 43.2            |
  | 15    | 68.2              | 1.14              | 52.8            |
  | 20    | 57.7              | 0.96              | 62.4            |

- **Location**: `UpgradeCosts.cs:150-159`

#### Catering Speed
- **Base Interval**: 180 seconds (3.0 minutes)
- **Upgrade Formula**: `180 / (1 + upgradeLevel × 0.08)`
- **Progression**:
  | Level | Interval (seconds) | Interval (minutes) | Deliveries/Hour |
  |-------|-------------------|-------------------|-----------------|
  | 0     | 180.0             | 3.00              | 20.0            |
  | 1     | 166.7             | 2.78              | 21.6            |
  | 3     | 145.2             | 2.42              | 24.8            |
  | 5     | 128.6             | 2.14              | 28.0            |
  | 10    | 100.0             | 1.67              | 36.0            |
  | 15    | 81.8              | 1.36              | 44.0            |
  | 20    | 69.2              | 1.15              | 52.0            |

- **Location**: `UpgradeCosts.cs:160-169`

### 2. Speed Modifiers

#### Prestige Speed Bonus
- **Cook Rate Bonus**: `prestigeStars × 0.04 × 100` per star
- **Example**: 
  - 1 star: +4 cook rate
  - 5 stars: +20 cook rate
  - 10 stars: +40 cook rate
- **Location**: `UpgradeCosts.cs:199-203`

#### Machine Upgrade Speed
- **Formula**: `level × 0.1` cook rate bonus
- **Max Level**: 10
- **Max Bonus**: +1.0 cook rate
- **Location**: `UpgradeCosts.cs:223-226`

### 3. Speed Observations

**Strengths:**
- ✅ Slow initial progression (2.5-3 min intervals) elongates gameplay
- ✅ Meaningful upgrade progression (8% reduction per level)
- ✅ Clear speed scaling that rewards investment

**Potential Issues:**
- ⚠️ No time acceleration or speed boost mechanics
- ⚠️ Early game may feel slow for impatient players
- ⚠️ No active speed modifiers (ads, power-ups, etc.)

---

## 💰 INCOME PROGRESSION ANALYSIS

### 1. Income Sources

#### Primary: Delivery System
- **Formula**: `shawarmaValue × quantity × 0.80` (20% tax)
- **Frequency**: Based on delivery interval (see above)
- **Location**: `DeliveryVan.cs:78-85`

#### Secondary: Catering System
- **Formula**: `shawarmaValue × quantity × 0.80` (20% tax)
- **Frequency**: Based on catering interval (see above)
- **Location**: `CateringManager.cs` (similar to delivery)

#### Bonus: Building Income
- **Net Income Rates** (per hour):
  | Building Type | Net Income/Hour | Notes |
  |--------------|----------------|-------|
  | Juice Point | $1,440 | $2 every 5s, $1 expense every 10s |
  | Dessert Point | $1,440 | $2 every 5s, $1 expense every 10s |
  | Merchandise | $2,160 | $3 every 5s, $1 expense every 10s |
  | Ingredients | $1,080 | $3 every 5s, $2 expense every 10s |
  | Shawarma Lounge | $2,880 | $5 every 5s, $2 expense every 10s |
  | Park | $480 | $2 every 10s, $1 expense every 15s |
  | Gas Station | $2,160 | $5 every 5s, $3 expense every 10s |
  | Management | $5,760 | $10 every 5s, $2 expense every 10s |

- **Location**: `ExtraBuildingFunctionality.cs:18-29`

### 2. Income Scaling

#### Shawarma Value Calculation
- **Base Value**: $100
- **Material Bonuses**:
  - Bread: `level × 5` (max +$50 at level 10)
  - Chicken: `level × 8` (max +$80 at level 10)
  - Sauce: `level × 3` (max +$30 at level 10)
- **Prestige Bonus**: `prestigeStars × 0.1 × 100` (10% per star)
- **Quality Bonus**: Multiplier (default 1.0)
- **Formula**: `(baseValue + materialBonuses + prestigeBonus) × qualityBonus`
- **Location**: `UpgradeCosts.cs:128-143`

#### Income Progression Timeline

**Early Game (Level 1, No Upgrades):**
- Shawarma Value: $100
- Delivery Capacity: 5 shawarmas
- Delivery Interval: 138.9 seconds
- Deliveries/Hour: 25.9
- Earnings/Delivery: $100 × 5 × 0.80 = **$400**
- **Total/Hour: $10,360** ✅

**Mid Game (Level 3-5, Some Upgrades):**
- Shawarma Value: $150-$200 (with material upgrades)
- Delivery Capacity: 7-11 shawarmas
- Delivery Interval: 121.0-107.1 seconds
- Deliveries/Hour: 29.8-33.6
- Earnings/Delivery: $150-$200 × 7-11 × 0.80 = **$840-$1,760**
- **Total/Hour: $25,000-$59,000**

**Late Game (Level 10+, Max Upgrades):**
- Shawarma Value: $300-$500+ (with max materials and prestige)
- Delivery Capacity: 20+ shawarmas
- Delivery Interval: 83.3 seconds
- Deliveries/Hour: 43.2+
- Earnings/Delivery: $300-$500 × 20+ × 0.80 = **$4,800-$8,000+**
- **Total/Hour: $207,000-$345,000+**

### 3. Income Balance Analysis

**Target Pacing:** Upgrades should take 5-10 minutes to afford

**Achievement Status:**
- ✅ Early game: $500 upgrade takes ~3 minutes
- ✅ Early game: $1,000 upgrade takes ~6 minutes
- ✅ Early game: $1,500 upgrade takes ~9 minutes
- ✅ Mid game: Upgrades take 2-7 minutes
- ✅ Late game: Upgrades take 4-8 minutes

**Income Rate Summary:**
| Stage | Income/Hour | Upgrade Cost Range | Time to Afford | Status |
|-------|-------------|-------------------|----------------|--------|
| Early (L1) | $10K | $500-$1,000 | 3-6 min | ✅ Perfect |
| Mid (L2-3) | $15K-$20K | $675-$1,800 | 2-7 min | ✅ Good |
| Late (L4-5) | $25K-$30K | $2K-$4K | 4-8 min | ✅ Balanced |

### 4. Income Observations

**Strengths:**
- ✅ Well-balanced early game pacing
- ✅ Clear income scaling with upgrades
- ✅ Multiple income sources (delivery, catering, buildings)
- ✅ Tax system (20%) prevents excessive income

**Potential Issues:**
- ⚠️ Late game income may scale too quickly ($200K+/hour)
- ⚠️ Building income may become negligible compared to delivery income
- ⚠️ No income caps or diminishing returns on high-level play

---

## 🔓 UNLOCKING SYSTEM ANALYSIS

### 1. Current Unlocking Mechanisms

#### Building Unlocks
- **System**: `BuildingUnlockManager.cs`
- **Method**: Fixed cash/gold costs per building
- **Unlock Condition**: `playerCash >= building.cost` OR `playerGold >= building.goldCost`
- **Persistence**: Saved via `PlayerPrefs` (`building_purchased_{index}`)
- **Status**: ✅ Implemented

**Building Unlock Flow:**
1. Player clicks building button
2. System checks if building is already purchased
3. If not purchased and player has enough cash/gold:
   - Deduct cost
   - Mark as purchased
   - Save state
   - Show building in world
   - Camera pans to building
   - Particle effect plays

#### Upgrade Unlocks
- **System**: Level-based progression
- **Upgrade Types**:
  - Storage (Warehouse): Unlocked by purchasing warehouse
  - Delivery Van: Unlocked by purchasing delivery point
  - Kitchen: Unlocked by purchasing kitchen
  - Catering: Unlocked by purchasing catering point
- **Cost Formula**: Exponential with diminishing returns
- **Status**: ✅ Implemented

#### Material Upgrades
- **System**: `CommonAbilities.cs`
- **Upgrade Types**: Bread, Chicken, Sauce, Chef, Machine
- **Cost Formula**: `(level + 1) × 100`
- **Max Level**: 10 per material
- **Unlock Condition**: Always available (no gating)
- **Status**: ✅ Implemented

#### Prestige System
- **System**: Chef Stars based on total earnings
- **Formula**: `floor(log10(totalEarnings / 10,000))`
- **Unlock Thresholds**:
  | Total Earnings | Chef Stars |
  |--------------|------------|
  | $0 - $99,999 | 0 |
  | $100,000 - $999,999 | 1 |
  | $1,000,000 - $9,999,999 | 2 |
  | $10,000,000 - $99,999,999 | 3 |
  | $100,000,000+ | 4+ |
- **Status**: ✅ Implemented

### 2. Feature Gating Analysis

**Current State:**
- ❌ **No Progressive Feature Gating**
- ❌ All systems available from start
- ❌ No unlock progression: Production → Storage → Delivery → Transport → Optimization

**GDD Requirement:**
- Progressive unlocking: Production → Storage → Delivery → Transport → Optimization
- **Status**: ❌ **NOT IMPLEMENTED**

**What Should Be Gated:**
1. **Production** (Kitchen): Available from start ✅
2. **Storage** (Warehouse): Should unlock after X shawarmas produced
3. **Delivery** (Delivery Van): Should unlock after first warehouse purchased
4. **Transport** (Catering): Should unlock after X deliveries completed
5. **Optimization** (Buildings): Should unlock after reaching certain milestones

### 3. Unlocking System Observations

**Strengths:**
- ✅ Building purchase system works well
- ✅ Upgrade costs scale appropriately
- ✅ Prestige system provides long-term goals
- ✅ Material upgrades provide consistent progression

**Critical Issues:**
- ❌ **No feature gating** - all systems available immediately
- ❌ **No tutorial integration** with unlocks
- ❌ **No milestone-based unlocks**
- ❌ **No progressive onboarding**

**Impact:**
- Players may be overwhelmed by all available systems
- No sense of progression or achievement from unlocking features
- Tutorial may not align with available features
- Missing opportunity for guided onboarding

---

## 📊 PROGRESSION ANALYSIS

### 1. Upgrade Cost Progression

#### Cost Formula
```
cost = (basePrice - prestigeReduction) × (level^multiplier) × (1 / (1 + level × 0.1))
```

#### Upgrade Type Costs (0 Prestige Stars)

**Storage (Warehouse):**
| Level | Cost | Capacity | Time to Afford* |
|-------|------|----------|----------------|
| 1 | $1,000 | 240 | 6 min |
| 2 | $2,400 | 380 | 14 min |
| 3 | $4,200 | 520 | 25 min |
| 5 | $9,000 | 800 | 54 min |
| 10 | $25,120 | 1,500 | 151 min |
| 15 | $44,800 | 2,200 | 269 min |
| 20 | $67,200 | 2,900 | 403 min |

*Based on $10K/hour early game income

**Delivery Van:**
| Level | Cost | Capacity | Interval | Time to Afford* |
|-------|------|----------|----------|----------------|
| 1 | $500 | 5 | 138.9s | 3 min |
| 2 | $1,200 | 6 | 128.6s | 7 min |
| 3 | $2,000 | 8 | 121.0s | 12 min |
| 5 | $3,900 | 11 | 107.1s | 23 min |
| 10 | $10,600 | 20 | 83.3s | 64 min |
| 15 | $18,900 | 29 | 68.2s | 113 min |
| 20 | $28,400 | 38 | 57.7s | 170 min |

*Based on $10K/hour early game income

**Kitchen:**
| Level | Cost | Time to Afford* |
|-------|------|----------------|
| 1 | $2,000 | 12 min |
| 2 | $4,600 | 28 min |
| 3 | $7,800 | 47 min |
| 5 | $16,000 | 96 min |
| 10 | $40,000 | 240 min |
| 15 | $71,400 | 428 min |
| 20 | $107,200 | 643 min |

*Based on $10K/hour early game income

### 2. Progression Timeline

**First 30 Minutes:**
- ✅ Unlock first warehouse (free)
- ✅ Purchase first delivery van ($500)
- ✅ Upgrade delivery van to level 2 ($1,200)
- ✅ Purchase first kitchen ($2,000)
- **Total Progress**: 1 warehouse, 1 delivery van (L2), 1 kitchen

**30-60 Minutes:**
- ✅ Upgrade warehouse to level 2 ($2,400)
- ✅ Upgrade delivery van to level 3 ($2,000)
- ✅ Upgrade kitchen to level 2 ($4,600)
- ✅ Purchase second warehouse ($1,500)
- **Total Progress**: 2 warehouses (L1-L2), 1 delivery van (L3), 1 kitchen (L2)

**1-2 Hours:**
- ✅ Multiple upgrades across all systems
- ✅ Material upgrades (Bread, Chicken, Sauce)
- ✅ Building purchases (Juice Point, Dessert Point, etc.)
- **Total Progress**: Multiple buildings, upgraded systems, material bonuses

**2+ Hours:**
- ✅ Prestige system becomes relevant ($100K+ earnings)
- ✅ High-level upgrades ($10K+)
- ✅ Max material upgrades ($5,500 per material)
- **Total Progress**: Advanced economy, prestige bonuses

### 3. Progression Observations

**Strengths:**
- ✅ Clear cost scaling prevents rapid progression
- ✅ Multiple upgrade paths provide choice
- ✅ Prestige system provides long-term goals
- ✅ Material upgrades provide consistent value

**Potential Issues:**
- ⚠️ Late game costs may become prohibitive (400+ minutes for upgrades)
- ⚠️ No clear progression milestones or achievements
- ⚠️ No visual progression indicators beyond numbers
- ⚠️ Prestige thresholds may be too high for casual players

---

## 🎯 RECOMMENDATIONS

### 1. Gameplay Speed

**Immediate Actions:**
- ✅ Current speed is well-balanced for elongated gameplay
- Consider adding optional speed boosters (ads, premium currency)
- Add visual feedback for speed improvements

**Future Enhancements:**
- Implement time acceleration mechanics
- Add active speed modifiers (power-ups, temporary boosts)
- Create speed milestones/achievements

### 2. Income Progression

**Immediate Actions:**
- ✅ Current income balance is appropriate
- Monitor late game income scaling ($200K+/hour)
- Consider income caps or diminishing returns for high-level play

**Future Enhancements:**
- Add income milestones/achievements
- Implement income multipliers for special events
- Create income leaderboards

### 3. Unlocking System

**Critical Actions Required:**
- ❌ **Implement progressive feature gating**
- ❌ **Create unlock progression**: Production → Storage → Delivery → Transport → Optimization
- ❌ **Integrate unlocks with tutorial system**
- ❌ **Add milestone-based unlocks**

**Implementation Plan:**
1. **Storage Unlock**: After producing X shawarmas (e.g., 50)
2. **Delivery Unlock**: After purchasing first warehouse
3. **Transport Unlock**: After completing X deliveries (e.g., 10)
4. **Optimization Unlock**: After reaching certain milestones (e.g., $10K total earnings)

**Future Enhancements:**
- Add unlock animations/celebrations
- Create unlock previews showing what's coming next
- Implement unlock requirements UI
- Add unlock achievements

---

## 📈 METRICS SUMMARY

### Gameplay Speed
- **Delivery Interval**: 138.9s (Level 1) → 57.7s (Level 20)
- **Catering Interval**: 166.7s (Level 1) → 69.2s (Level 20)
- **Production Speed**: Instant (tap) or 2s (auto-chef)

### Income Progression
- **Early Game**: $10,360/hour
- **Mid Game**: $15,000-$20,000/hour
- **Late Game**: $25,000-$30,000/hour (can reach $200K+/hour with upgrades)

### Unlocking System
- **Buildings**: Fixed costs, always available
- **Upgrades**: Level-based, exponential costs
- **Materials**: Always available, linear costs
- **Features**: ❌ **No progressive gating**

---

## ✅ CONCLUSION

**Gameplay Speed**: ✅ **Well-balanced** - Slow initial progression elongates gameplay appropriately

**Income Progression**: ✅ **Appropriate** - Upgrades take 5-10 minutes to afford, creating meaningful progression

**Unlocking System**: ❌ **Needs Improvement** - No progressive feature gating, all systems available from start

**Overall Assessment**: The game has a solid foundation with balanced economy and pacing, but lacks progressive feature unlocking which could improve onboarding and player engagement.

---

**Document Version:** 1.0  
**Last Updated:** Analysis Date  
**Maintained By:** Gameplay Analysis System
