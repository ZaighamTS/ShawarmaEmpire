# Complete Economy Formulas Table

**All formulas used in Sharwama Dash economy system**

---

## 💰 INCOME FORMULAS

| Formula Name | Formula | Variables | What It Changes | Base Values |
|-------------|---------|-----------|----------------|-------------|
| **Delivery Earnings** | `shawarmaValue × quantity × 0.70` | `shawarmaValue` = calculated value<br>`quantity` = shawarmas delivered<br>`0.70` = tax rate (30% deduction) | Player cash<br>Total earnings | - |
| **Catering Earnings** | `shawarmaValue × quantity × 0.70` | Same as delivery | Player cash<br>Total earnings | - |
| **Shawarma Value** | `(baseValue + materialBonuses + prestigeBonus) × qualityBonus` | `baseValue` = 50<br>`materialBonuses` = Bread + Chicken + Sauce<br>`prestigeBonus` = chefStars × 5<br>`qualityBonus` = multiplier (default 1.0) | Value per shawarma<br>Income per delivery | Base: $50 |
| **Cook Rate** | `(cookRateBaseValue + prestigeCookRateBonus + machineRate) + (tapPower × tapRate) + autoChefBonus` | `cookRateBaseValue` = 200<br>`prestigeCookRateBonus` = chefStars × 2<br>`machineRate` = machineLevel × 0.1<br>`tapPower` = 1.0-1.5x<br>`autoChefBonus` = auto production | Production speed<br>Shawarmas per second | Base: 200 units/sec |
| **Income Per Minute** | `shawarmaValue × deliveriesPerMinute × averageDeliverySize × 0.70` | `deliveriesPerMinute` = delivery frequency<br>`averageDeliverySize` = avg shawarmas per delivery | Estimated income rate | - |
| **Offline Earnings** | `min(estimatedDeliveryRate × min(secondsOffline, 86400), maxEarnings)` | `estimatedDeliveryRate` = earnings/sec<br>`secondsOffline` = time offline (max 24h)<br>`maxEarnings` = estimatedRate × 3600 | Player cash on return | Max: $10M |

---

## 📊 SHAWARMA VALUE COMPONENTS

| Component | Formula | Per Level | Max Contribution | What It Changes |
|-----------|---------|-----------|-------------------|------------------|
| **Base Value** | `50` | - | $50 | Starting shawarma value |
| **Bread Upgrade** | `breadLevel × 5` | +$5 | +$50 (level 10) | Added to shawarma value |
| **Chicken Upgrade** | `chickenLevel × 8` | +$8 | +$80 (level 10) | Added to shawarma value |
| **Sauce Upgrade** | `sauceLevel × 3` | +$3 | +$30 (level 10) | Added to shawarma value |
| **Prestige Bonus** | `chefStars × 0.1 × 50 = chefStars × 5` | +$5 per star | Unlimited | Added to shawarma value |
| **Quality Bonus** | `multiplier` | Variable | Variable | Multiplies total value |

---

## 💸 UPGRADE COST FORMULAS

| Cost Type | Formula | Variables | What It Changes |
|-----------|---------|-----------|------------------|
| **Upgrade Cost** | `(basePrice - prestigeReduction) × (level^multiplier) × (1/(1+level×0.1))` | `basePrice` = varies by type<br>`prestigeReduction` = chefStars × 1.25<br>`level` = upgrade level<br>`multiplier` = varies by type | Cost to upgrade existing building |
| **Purchase Cost** | `(basePrice - prestigeReduction) × (3.5^existingCount)` | `basePrice` = varies by type<br>`prestigeReduction` = chefStars × 1.25<br>`existingCount` = buildings already placed | Cost to buy new building |
| **Extra Building Cost** | `(basePrice - prestigeReduction) × (3.5^existingCount)` | Same as purchase cost | Cost to buy extra building |

---

## 🏗️ UPGRADE TYPE CONFIGURATIONS

| Upgrade Type | Base Price | Purchase Multiplier | Upgrade Multiplier | Role |
|-------------|------------|---------------------|-------------------|------|
| **Storage** | $3,750 | 1.5x | 1.4x | High value bottleneck |
| **Delivery Van** | $1,875 | 1.5x | 1.35x | Primary income source |
| **Kitchen** | $7,500 | 1.2x | 1.3x | Production boost |
| **Catering** | $5,625 | 1.2x | 1.25x | Secondary income |

---

## 📈 CAPACITY FORMULAS

| Capacity Type | Formula | Base | Multiplier | What It Changes |
|--------------|---------|------|------------|------------------|
| **Storage Capacity** | `250 × 2^(level-2)` (if level > 1)<br>`0` (if level ≤ 1) | 250 | 2.0x (doubles) | Max shawarmas stored |
| **Delivery Capacity** | `2 × (1 + level × 0.4)` | 2 | 0.4x | Shawarmas per delivery |
| **Catering Capacity** | `3 × (1 + level × 0.4)` | 3 | 0.4x | Shawarmas per order |

---

## ⏱️ INTERVAL FORMULAS

| Interval Type | Formula | Base (seconds) | Multiplier | What It Changes |
|--------------|---------|----------------|------------|------------------|
| **Delivery Interval** | `60 / (1 + upgradeLevel × 0.05)` | 60 | 0.05x | Time between deliveries |
| **Catering Interval** | `90 / (1 + upgradeLevel × 0.05)` | 90 | 0.05x | Time between orders |

---

## ⭐ PRESTIGE SYSTEM FORMULAS

| Prestige Formula | Formula | Variables | What It Changes |
|-----------------|---------|-----------|------------------|
| **Chef Stars** | `floor(log10(totalEarnings / 100,000))` | `totalEarnings` = lifetime earnings | Prestige level |
| **Next Prestige** | `10^chefStars × 1,000,000` | `chefStars` = current stars | Prestige threshold |
| **Prestige Income Bonus** | `chefStars × 0.1 × 50 = chefStars × 5` | `chefStars` = current stars | Added to shawarma value |
| **Prestige Cost Reduction** | `chefStars × 0.025 × 50 = chefStars × 1.25` | `chefStars` = current stars | Reduces all costs |
| **Prestige Cook Rate** | `chefStars × 0.04 × 50 = chefStars × 2` | `chefStars` = current stars | Added to cook rate |
| **Auto Earning Multiplier** | `1.0 + (chefStars × 0.05) + (totalUpgradeLevels × 0.01)` | `chefStars` = current stars<br>`totalUpgradeLevels` = sum of all upgrade levels | Passive earning rate |

---

## 🏢 EXTRA BUILDINGS BASE PRICES

| Building Type | Base Price | First Purchase | Second Purchase | Third Purchase |
|--------------|------------|----------------|-----------------|----------------|
| **Juice Point** | $5,625 | $5,625 | $19,687.50 | $68,906.25 |
| **Dessert Point** | $9,375 | $9,375 | $32,812.50 | $114,843.75 |
| **Merchandise** | $15,000 | $15,000 | $52,500 | $183,750 |
| **Ingredients** | $28,125 | $28,125 | $98,437.50 | $344,531.25 |
| **Park** | $45,000 | $45,000 | $157,500 | $551,250 |
| **Shawarma Lounge** | $75,000 | $75,000 | $262,500 | $918,750 |
| **Gas Station** | $131,250 | $131,250 | $459,375 | $1,607,812.50 |
| **Management** | $225,000 | $225,000 | $787,500 | $2,756,250 |

**Purchase Formula:** `(basePrice - prestigeReduction) × (3.5^existingCount)`

---

## 📊 UPGRADE COST PROGRESSION EXAMPLES

### Storage Upgrade Costs (0 prestige stars)

| Level | Formula Calculation | Cost |
|-------|---------------------|------|
| 1 | `3750 × (1^1.4) × (1/1.1)` | $3,409 |
| 2 | `3750 × (2^1.4) × (1/1.2)` | $8,750 |
| 3 | `3750 × (3^1.4) × (1/1.3)` | $15,000 |
| 5 | `3750 × (5^1.4) × (1/1.5)` | $35,000 |
| 10 | `3750 × (10^1.4) × (1/2.0)` | $93,750 |
| 15 | `3750 × (15^1.4) × (1/2.5)` | $168,750 |
| 20 | `3750 × (20^1.4) × (1/3.0)` | $250,000 |

### Delivery Van Upgrade Costs (0 prestige stars)

| Level | Formula Calculation | Cost |
|-------|---------------------|------|
| 1 | `1875 × (1^1.35) × (1/1.1)` | $1,705 |
| 2 | `1875 × (2^1.35) × (1/1.2)` | $4,375 |
| 3 | `1875 × (3^1.35) × (1/1.3)` | $7,500 |
| 5 | `1875 × (5^1.35) × (1/1.5)` | $17,500 |
| 10 | `1875 × (10^1.35) × (1/2.0)` | $46,875 |
| 15 | `1875 × (15^1.35) × (1/2.5)` | $84,375 |
| 20 | `1875 × (20^1.35) × (1/3.0)` | $125,000 |

### Kitchen Upgrade Costs (0 prestige stars)

| Level | Formula Calculation | Cost |
|-------|---------------------|------|
| 1 | `7500 × (1^1.3) × (1/1.1)` | $6,818 |
| 2 | `7500 × (2^1.3) × (1/1.2)` | $17,500 |
| 3 | `7500 × (3^1.3) × (1/1.3)` | $30,000 |
| 5 | `7500 × (5^1.3) × (1/1.5)` | $70,000 |
| 10 | `7500 × (10^1.3) × (1/2.0)` | $187,500 |
| 15 | `7500 × (15^1.3) × (1/2.5)` | $337,500 |
| 20 | `7500 × (20^1.3) × (1/3.0)` | $500,000 |

### Catering Upgrade Costs (0 prestige stars)

| Level | Formula Calculation | Cost |
|-------|---------------------|------|
| 1 | `5625 × (1^1.25) × (1/1.1)` | $5,114 |
| 2 | `5625 × (2^1.25) × (1/1.2)` | $13,125 |
| 3 | `5625 × (3^1.25) × (1/1.3)` | $22,500 |
| 5 | `5625 × (5^1.25) × (1/1.5)` | $52,500 |
| 10 | `5625 × (10^1.25) × (1/2.0)` | $140,625 |
| 15 | `5625 × (15^1.25) × (1/2.5)` | $253,125 |
| 20 | `5625 × (20^1.25) × (1/3.0)` | $375,000 |

---

## 📈 CAPACITY PROGRESSION EXAMPLES

### Storage Capacity Progression

| Level | Formula | Capacity |
|-------|---------|----------|
| 1 (unpurchased) | `0` | 0 |
| 2 (purchased) | `250 × 2^0` | 250 |
| 3 (first upgrade) | `250 × 2^1` | 500 |
| 4 | `250 × 2^2` | 1,000 |
| 5 | `250 × 2^3` | 2,000 |
| 10 | `250 × 2^8` | 64,000 |
| 15 | `250 × 2^13` | 2,048,000 |
| 20 | `250 × 2^18` | 65,536,000 |

### Delivery Capacity Progression

| Level | Formula | Capacity |
|-------|---------|----------|
| 0 | `2 × (1 + 0 × 0.4)` | 2 |
| 1 | `2 × (1 + 1 × 0.4)` | 2.8 → 3 |
| 5 | `2 × (1 + 5 × 0.4)` | 6 |
| 10 | `2 × (1 + 10 × 0.4)` | 10 |
| 15 | `2 × (1 + 15 × 0.4)` | 14 |
| 20 | `2 × (1 + 20 × 0.4)` | 18 |

### Catering Capacity Progression

| Level | Formula | Capacity |
|-------|---------|----------|
| 0 | `3 × (1 + 0 × 0.4)` | 3 |
| 1 | `3 × (1 + 1 × 0.4)` | 4.2 → 4 |
| 5 | `3 × (1 + 5 × 0.4)` | 9 |
| 10 | `3 × (1 + 10 × 0.4)` | 15 |
| 15 | `3 × (1 + 15 × 0.4)` | 21 |
| 20 | `3 × (1 + 20 × 0.4)` | 27 |

---

## ⏱️ INTERVAL PROGRESSION EXAMPLES

### Delivery Interval Progression

| Level | Formula | Interval (sec) | Deliveries/Min |
|-------|---------|----------------|----------------|
| 0 | `60 / (1 + 0 × 0.05)` | 60.0 | 1.00 |
| 1 | `60 / (1 + 1 × 0.05)` | 57.1 | 1.05 |
| 5 | `60 / (1 + 5 × 0.05)` | 48.0 | 1.25 |
| 10 | `60 / (1 + 10 × 0.05)` | 40.0 | 1.50 |
| 15 | `60 / (1 + 15 × 0.05)` | 34.3 | 1.75 |
| 20 | `60 / (1 + 20 × 0.05)` | 30.0 | 2.00 |

### Catering Interval Progression

| Level | Formula | Interval (sec) | Orders/Min |
|-------|---------|----------------|------------|
| 0 | `90 / (1 + 0 × 0.05)` | 90.0 | 0.67 |
| 1 | `90 / (1 + 1 × 0.05)` | 85.7 | 0.70 |
| 5 | `90 / (1 + 5 × 0.05)` | 72.0 | 0.83 |
| 10 | `90 / (1 + 10 × 0.05)` | 60.0 | 1.00 |
| 15 | `90 / (1 + 15 × 0.05)` | 51.4 | 1.17 |
| 20 | `90 / (1 + 20 × 0.05)` | 45.0 | 1.33 |

---

## ⭐ PRESTIGE THRESHOLDS

| Total Earnings | Chef Stars | Next Star Requires | Prestige Income Bonus | Cost Reduction | Cook Rate Bonus |
|---------------|------------|-------------------|----------------------|----------------|----------------|
| $0 - $999,999 | 0 | $1,000,000 | +$0 | -$0 | +0 |
| $1,000,000 - $9,999,999 | 1 | $10,000,000 | +$5 | -$1.25 | +2 |
| $10,000,000 - $99,999,999 | 2 | $100,000,000 | +$10 | -$2.50 | +4 |
| $100,000,000 - $999,999,999 | 3 | $1,000,000,000 | +$15 | -$3.75 | +6 |
| $1,000,000,000+ | 4+ | $10,000,000,000+ | +$20+ | -$5.00+ | +8+ |

---

## 🔧 MATERIAL UPGRADE VALUES

| Material Type | Formula | Value Per Level | Max Level | Max Total Bonus |
|--------------|---------|-----------------|-----------|-----------------|
| **Bread** | `breadLevel × 5` | +$5 | 10 | +$50 |
| **Chicken** | `chickenLevel × 8` | +$8 | 10 | +$80 |
| **Sauce** | `sauceLevel × 3` | +$3 | 10 | +$30 |

---

## ⚙️ MACHINE UPGRADE

| Upgrade Type | Formula | Value Per Level | What It Changes |
|-------------|---------|-----------------|------------------|
| **Machine Cook Rate** | `machineLevel × 0.1` | +0.1 cook rate | Production speed |

---

## 📋 COMPLETE FORMULA REFERENCE

| Category | Formula | Location |
|----------|---------|----------|
| **Delivery Earnings** | `shawarmaValue × quantity × 0.70` | `DeliveryVan.cs:82` |
| **Catering Earnings** | `shawarmaValue × quantity × 0.70` | `CateringVan.cs:73` |
| **Shawarma Value** | `(50 + materialBonuses + prestigeBonus) × qualityBonus` | `UpgradeCosts.cs:231-246` |
| **Cook Rate** | `(200 + prestigeBonus + machineRate) + tapBonus + autoChef` | `UpgradeCosts.cs:247-252` |
| **Upgrade Cost** | `(basePrice - prestigeReduction) × (level^multiplier) × (1/(1+level×0.1))` | `UpgradeCosts.cs:171-192` |
| **Purchase Cost** | `(basePrice - prestigeReduction) × (3.5^existingCount)` | `UpgradeCosts.cs:130-147` |
| **Storage Capacity** | `250 × 2^(level-2)` | `UpgradeCosts.cs:194-223` |
| **Delivery Capacity** | `2 × (1 + level × 0.4)` | `UpgradeCosts.cs:194-223` |
| **Catering Capacity** | `3 × (1 + level × 0.4)` | `UpgradeCosts.cs:194-223` |
| **Delivery Interval** | `60 / (1 + level × 0.05)` | `UpgradeCosts.cs:253-262` |
| **Catering Interval** | `90 / (1 + level × 0.05)` | `UpgradeCosts.cs:263-272` |
| **Chef Stars** | `floor(log10(totalEarnings / 100,000))` | `UpgradeCosts.cs:273-280` |
| **Prestige Income** | `chefStars × 5` | `UpgradeCosts.cs:298-301` |
| **Prestige Cost Reduction** | `chefStars × 1.25` | `UpgradeCosts.cs:302-305` |
| **Prestige Cook Rate** | `chefStars × 2` | `UpgradeCosts.cs:306-310` |

---

**All formulas verified against codebase implementation**  
**Last Updated:** Based on current codebase analysis
