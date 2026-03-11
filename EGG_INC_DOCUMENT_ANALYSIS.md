# Egg Inc. Document Analysis - Shawarma Dash Economic Design

**Source:** Shawarma X EggInc Doc.pdf  
**Analysis Date:** February 11, 2026

---

## 📋 Executive Summary

This document provides a comprehensive comparison between Egg Inc. and Shawarma Dash, along with detailed economic formulas, upgrade systems, and recommended features. The analysis reveals a well-structured incremental/idle game economy with clear progression paths.

---

## 🎯 Key Findings

### 1. Core Mechanics Comparison

| Mechanic | Egg Inc. | Shawarma Dash | Status |
|----------|----------|---------------|--------|
| **Core Loop** | Lay Eggs → Research → Upgrade → Prestige | Produce → Store → Deliver → Earn → Upgrade | ✅ Similar |
| **Production** | Manual tapping + internal hatchery (auto) | Manual tapping | ✅ Similar |
| **Storage** | Habitats with capacity limits | Warehouses with capacity limits | ✅ Similar |
| **Income Source** | Selling eggs to trucks | Delivery vans + catering vans | ✅ Similar |
| **Prestige System** | Soul Eggs + Prophecy Eggs | Chef Stars | ✅ Similar |
| **Research System** | Research trees (Common, Epic, etc.) | ❌ Missing | ⚠️ Recommended |
| **Challenges** | 3 challenges at a time | ❌ Missing | ⚠️ Recommended |
| **Boost System** | Golden eggs for boosts | Gold currency exists but no boost system | ⚠️ PARTIAL |
| **Statistics Screen** | Detailed stats & analytics | ❌ Missing | ⚠️ Recommended |
| **Offline Earnings** | Offline earnings with cap | ❌ Missing | ⚠️ Recommended |

---

## 💰 Economic Systems

### Delivery System Upgrade Table

Complete breakdown of delivery upgrades (Levels 0-20):

| Level | Upgrade Cost | Capacity | Deliveries/Min | Earnings/Delivery* | Earnings/Min* |
|-------|--------------|----------|----------------|-------------------|---------------|
| 0 | $1,875** | 2.0 | 1.00 | $70.00 | $70.00 |
| 1 | $1,875 | 2.8 | 1.05 | $98.00 | $102.90 |
| 5 | $10,688 | 6.0 | 1.25 | $210.00 | $262.50 |
| 10 | $20,991 | 10.0 | 1.50 | $350.00 | $525.00 |
| 20 | $28,575 | 18.0 | 2.00 | $630.00 | $1,260.00 |

\* Earnings calculated assuming base shawarma value of $50  
\** Level 0 represents initial purchase cost

### Kitchen/Warehouse Upgrade Table

| Level | Upgrade Cost | Capacity | Benefit |
|-------|--------------|----------|---------|
| 0 | - | 0 | Unpurchased |
| 1 | $3,750 | 250 | Base purchase |
| 2 | $4,875 | 500 | +250 capacity |
| 5 | $11,531 | 4,000 | +2,000 capacity |
| 10 | $34,050 | 128,000 | +64,000 capacity |

**Formula:** `250 × 2^(level-2)` for level > 1

---

## 🏢 Extra Buildings Income System

### Base Income (Per Hour)

| Building Type | Reward | Expense | Net Income/Hour | Status |
|---------------|--------|---------|-----------------|--------|
| Juice Point | $2 every 5s | $3 every 10s | +$720/hr | ✅ Profit |
| Dessert Point | $2 every 5s | $3 every 10s | +$720/hr | ✅ Profit |
| Merchandise | $3 every 5s | $5 every 10s | +$780/hr | ✅ Profit |
| Ingredients | $3 every 5s | $8 every 10s | -$360/hr | ❌ Loss |
| Shawarma Lounge | $5 every 5s | $10 every 10s | $0/hr | ⚖️ Break even |
| Park | $2 every 10s | $5 every 15s | -$600/hr | ❌ Loss |
| Gas Station | $5 every 5s | $12 every 10s | -$1,320/hr | ❌ Loss |
| Management | $10 every 5s | $15 every 10s | -$1,800/hr | ❌ Loss |

### Upgraded Income (Level 10)

| Building | Level 0 | Level 5 | Level 10 | Total Cost (1-10) |
|----------|---------|---------|----------|------------------|
| Juice Point | +$720/hr | +$2,174/hr | +$5,112/hr | $47,020 |
| Dessert Point | +$720/hr | +$2,174/hr | +$5,112/hr | $78,361 |
| Merchandise | +$780/hr | +$2,962/hr | +$7,368/hr | $125,828 |
| Ingredients | -$360/hr | +$1,822/hr | +$6,228/hr | $235,754 |
| Shawarma Lounge | $0/hr | +$3,636/hr | +$10,980/hr | $628,284 |
| Park | -$600/hr | +$127/hr | +$1,596/hr | $303,177 |
| Gas Station | -$1,320/hr | +$2,316/hr | +$9,660/hr | $1,111,769 |
| Management | -$1,800/hr | +$5,472/hr | +$20,160/hr | $1,917,784 |

---

## 🔬 Research System Design

### 13 Tiers of Common Research (4 per tier)

**Key Research Examples:**

| Tier | Research Name | Description | Max Bonus | Base Cost | Cost Formula | Max Level |
|------|---------------|-------------|-----------|-----------|--------------|-----------|
| 1 | Comfortable Kitchen | Increase production rate by 10% | +500% (x6) | $1,000 | Base × level^1.2 | 50 |
| 1 | Quality Ingredients | Increase shawarma value by 25% | +1,000% (x11) | $1,500 | Base × level^1.3 | 40 |
| 2 | Premium Shawarmas | DOUBLES shawarma value | +100% (x2) | $5,000 | Base × level^1.0 | 1 |
| 3 | Premium Certification | TRIPLES shawarma value | +200% (x3) | $15,000 | Base × level^1.0 | 1 |
| 7 | Gourmet Shawarmas | DOUBLES shawarma value | +3,100% (x32) | $75,000 | Base × level^1.4 | 5 |
| 11 | Multiversal Ingredients | 10× shawarma value | +99,900% (x1,000) | $2,000,000 | Base × level^1.4 | 3 |
| 13 | Timeline Splicing | 10× shawarma value | +900% (x10) | $15,000,000 | Base × level^1.0 | 1 |

### 22 Epic Research Upgrades (Permanent - Don't Reset on Prestige)

**Key Epic Research Examples:**

| Title | Description | Cumulative Effect | Base Cost [GOLD] | Cost Formula | Max Level |
|-------|-------------|-------------------|------------------|--------------|-----------|
| Quantum Storage Expansion | Increase capacity by 5% | +50% | 100 | Base × level^1.5 | 10 |
| Boost Duration Extension | Increase boost duration by 30 min | +360 min | 50 | Base × level^1.4 | 12 |
| Chef Star Power | Increase bonus per Chef Star by +1% | +140% | 200 | Base × level^1.4 | 140 |
| Prestige Multiplier | Increase bonus per prestige level by 1% (compounding) | +5% | 500 | Base × level^1.5 | 5 |
| Prestige Earnings Bonus | Earn +10% Chef Stars when you prestige | +200% | 150 | Base × level^1.4 | 20 |
| Epic Chef Multiplier | Increase max chef efficiency bonus by 2.0×! | +200× | 500 | Base × level^1.5 | 100 |

---

## 🎁 Boost System Design

### Boosts Shop

| Name | Description | Gold Cost |
|------|-------------|-----------|
| Quantum Kitchen Boost | Unlimited production for 10min | Watch Ad (0) |
| Chef's Special Recipe | 3x earnings for 20min | Watch Ad (0) |
| Chef's Premium Recipe | 10x earnings for 15min | Watch Ad (0) |
| Chef's Best Recipe | 50x earnings for 10min | 2,500 |
| Production Prism | 10x auto-production for 10min | Watch Ad (0) |
| Large Production Prism | 10x auto-production for 4hr | 500 |
| Boost Amplifier | 2x all active boosts for 30min | 1,000 |
| Epic Boost Amplifier | 10x all active boosts for 10min | 8,000 |
| Chef Star Beacon | 5x Chef Stars for 30min | 200 |
| Business Grant | +10% of Business Value | 200 |
| Chef Star 2x | Activates Chef Star 2x for 10min | 100 |

---

## 🏆 Achievement Challenges System

### Sample Achievements

| Title | Description | Reward |
|-------|-------------|--------|
| Two Hundred | Produce 200 shawarmas | Cash: $1,500 |
| Early Tech | Research 30 things | Cash: $30,000 |
| Get Going | Earn $500 in one second | Cash: $150,000 |
| Growing Family | Expand warehouses to hold 4,200 shawarmas | Cash: $500,000 |
| Shawarma Up | Start a new game with upgraded shawarma type | Gold: 96 |
| More Production | Produce 50,000 shawarmas total | Cash: $1,000,000 |
| Science! | Research 150 things | Gold: 24 |
| Big Storage | Expand warehouses to hold 15,000 shawarmas | Gold: 24 |
| Supply Chain | Have delivery capacity of 250 shawarmas/min | Gold: 24 |
| Rack It In | Earn $1 Million in one second | Gold: 48 |
| Get Rich Quick | Earn $5 Million in one second | Chef Stars: 1 |
| Production Everywhere | Produce 50,000 shawarmas total | Chef Stars: 1 |
| Research Champ | Complete all Tier 5 common research | Chef Stars: 1 |
| Shawarma City | 1 Million shawarmas stored in warehouses | Chef Stars: 1 |
| YUUGE Storage | Expand warehouses to hold 2 Million shawarmas | Gold: 500 |
| Cash Avalanche | Lifetime earnings exceed $500 Million | Chef Stars: 1 |
| Eleven Figure Shawarmas | Produce shawarmas worth $10 Billion total | Gold: 250 |
| Money Vault | $1 Billion in the bank | Chef Stars: 1 |
| Shawarma Metropolis | 5 Million shawarmas stored in warehouses | Chef Stars: 1 |
| Epic Storage | Expand warehouses to hold 50 Million shawarmas | Chef Stars: 1 |
| Shawarma Country | 50 Million shawarmas stored in warehouses | Gold: 2,000 |
| Soul Search | Collect 50,000 Chef Stars | Gold: 1,200 |
| Shawarma Planet | 150 Million shawarmas stored in warehouses | Chef Stars: 1 |
| Gourmet Grandmaster | Produce 5 Trillion Gourmet Shawarmas | Chef Stars: 1 |
| Mad Scientist | Research 1,100 things on one game | Chef Stars: 1 |
| Shawarma Galaxy | 300 Million shawarmas stored in warehouses | Gold: 5,000 |
| Soul King | Collect 250,000 Chef Stars | Gold: 3,000 |
| Signature Dealer | Produce 10 Trillion Signature Shawarmas | Chef Stars: 1 |

---

## 💎 In-App Purchase (IAP) Shop

### Gold Packages

| Package | Cost ($USD) | Gold (Max) | Reward Formula |
|---------|-------------|------------|----------------|
| Profit Vault | $5.99 | Varies (~7,000 to big integer) | Depends (usually cheaper) |
| Gold Crate | $4.99 | 1,300+ 700n | 260+140n |
| Gold Pallet | $9.99 | 2,900+1,500n | 290+150n |
| Gold Truckload | $19.99 | 6,000+4,000n | 300+200n |
| Handfull Gold | Watch 1 Ad per day | 600+300n | N/A |

### Subscriptions

| Subscription | Cost ($USD) | Benefits |
|-------------|-------------|----------|
| Elite Chef (Standard) | $3.99/month | No Ads, 25% more gold |
| Elite Chef (Pro) | $7.99/month | No Ads, 40% more gold |

### Premium Package

| Package | Cost ($USD) | Benefits |
|---------|-------------|----------|
| Shawarma Empire Premium | One-time purchase | Various premium features |

---

## 🚀 Recommended Features (Priority Order)

### 🔴 HIGH PRIORITY

1. **Challenges System** ⭐⭐⭐⭐⭐
   - Priority: #1 - Implement first
   - 3 active challenges at a time
   - Daily/Weekly/Special/Achievement types
   - Goals: Deliver X, Earn $X, Produce X, Upgrade X
   - Rewards: Gold (5-200), Cash ($500-$50K), Prestige bonuses
   - Effort: 2-3 weeks

2. **Statistics Screen** ⭐⭐⭐⭐
   - Priority: #2 - Quick win
   - Production stats: Total produced, rate per hour
   - Delivery stats: Total deliveries, average size
   - Earning stats: Total earned, per hour, best hour
   - Upgrade stats: Total upgrades, money spent
   - Time stats: Playtime, offline time
   - Effort: 1 week

### 🟡 MEDIUM PRIORITY

3. **Boost System** ⭐⭐⭐
   - Priority: #3 - Monetization opportunity
   - Production Boost: 2x production for 1 hour (10 Gold)
   - Income Boost: 2x earnings for 1 hour (15 Gold)
   - Speed Boost: 2x delivery speed for 30 min (5 Gold)
   - Capacity Boost: 2x storage for 2 hours (20 Gold)
   - Stacking: Multiple boosts active
   - Effort: 1-2 weeks

4. **Multiple Shawarma Types** ⭐⭐⭐
   - Priority: #4 - Adds variety
   - Types: Classic ($50) → Spicy ($60) → Premium ($75) → Gourmet ($100) → Signature ($150)
   - Unlock: Reach earnings milestones or Chef Stars
   - Unique bonuses per type
   - Effort: 2-3 weeks

5. **Gift Calendar** ⭐⭐⭐
   - Priority: #5 - Retention mechanic
   - 7-day cycle with different rewards
   - Rewards: Gold (5-50), Cash ($1K-$10K), temporary boosts
   - Streak bonus for consecutive days
   - Effort: 2 weeks

---

## 📊 Complete Economy Formulas

### Income Formulas

- **Delivery Earnings:** `shawarmaValue × quantity × 0.70`
- **Catering Earnings:** `shawarmaValue × quantity × 0.70`
- **Shawarma Value:** `(baseValue + materialBonuses + prestigeBonus) × qualityBonus`
  - Base Value: $50
  - Material Bonuses: Bread (+$5/level), Chicken (+$8/level), Sauce (+$3/level)
  - Prestige Bonus: `chefStars × 5`
- **Cook Rate:** `(cookRateBaseValue + prestigeCookRateBonus + machineRate) + (tapPower × tapRate) + autoChefBonus`
  - Base: 200 units/sec
- **Offline Earnings:** `min(estimatedDeliveryRate × min(secondsOffline, 86400), maxEarnings)`
  - Max: $10M

### Cost Formulas

- **Upgrade Cost:** `(basePrice - prestigeReduction) × (level^multiplier) × (1/(1+level×0.1))`
- **Purchase Cost:** `(basePrice - prestigeReduction) × (3.5^existingCount)`

### Capacity Formulas

- **Storage Capacity:** `250 × 2^(level-2)` (if level > 1)
- **Delivery Capacity:** `2 × (1 + level × 0.4)`
- **Catering Capacity:** `3 × (1 + level × 0.4)`

### Interval Formulas

- **Delivery Interval:** `60 / (1 + upgradeLevel × 0.05)` seconds
- **Catering Interval:** `90 / (1 + upgradeLevel × 0.05)` seconds

### Prestige Formulas

- **Chef Stars:** `floor(log10(totalEarnings / 100,000))`
- **Next Prestige:** `10^chefStars × 1,000,000`
- **Prestige Income Bonus:** `chefStars × 5`
- **Prestige Cost Reduction:** `chefStars × 1.25`
- **Prestige Cook Rate:** `chefStars × 2`

---

## 💡 Implementation Recommendations

### Immediate Actions
1. Implement Challenges System (highest priority)
2. Add Statistics Screen (quick win)
3. Implement Boost System (monetization)

### Short-term Goals
4. Add Multiple Shawarma Types
5. Implement Gift Calendar
6. Add Research System (13 tiers common + 22 epic)

### Long-term Goals
7. Add Offline Earnings
8. Expand Achievement System
9. Implement Advanced Analytics

---

**Document Status:** Complete Analysis  
**Next Steps:** Create economic simulator based on these formulas
