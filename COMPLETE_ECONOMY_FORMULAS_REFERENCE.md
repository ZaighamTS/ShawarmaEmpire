# Complete Economy Formulas & Upgrade Costs Reference

**Last Updated:** Based on current codebase analysis  
**Unity Version:** 6000.2.6f2  
**File Locations:** `UpgradeCosts.cs`, `EconomyCalculator.cs`, `DeliveryVan.cs`, `CateringVan.cs`

---

## 📊 BASE VALUES

### Core Constants
- **Shawarma Base Value**: `$50` (reduced from $100 for extended gameplay)
- **Cook Rate Base Value**: `200` units/second
- **Tax Rate**: `30%` (0.70 multiplier - player receives 70% of shawarma value)

**Location:** `UpgradeCosts.cs:111-112`

---

## 💰 INCOME FORMULAS

### 1. Delivery Earnings (Primary Income)
**Formula:**
```
earnings = shawarmaValue × quantity × 0.70
```

**What it changes:**
- Player cash balance
- Total earnings (for prestige calculation)
- Income per delivery

**Components:**
- `shawarmaValue`: Calculated value per shawarma (see formula #2)
- `quantity`: Number of shawarmas delivered
- `0.70`: Tax rate (30% deduction, player gets 70%)

**Example:**
- 1 shawarma @ $50: `50 × 1 × 0.70 = $35`
- 5 shawarmas @ $50: `50 × 5 × 0.70 = $175`
- 10 shawarmas @ $82.50 (with upgrades): `82.50 × 10 × 0.70 = $577.50`

**Location:** `DeliveryVan.cs:82`

---

### 2. Catering Earnings (Secondary Income)
**Formula:**
```
earnings = shawarmaValue × quantity × 0.70
```

**What it changes:**
- Player cash balance
- Total earnings (for prestige calculation)
- Bonus income stream

**Components:**
- Same as delivery earnings
- `quantity`: Number of shawarmas in catering order

**Location:** `CateringVan.cs:73`

---

### 3. Shawarma Value Calculation
**Formula:**
```
shawarmaValue = (baseValue + materialBonuses + prestigeBonus) × qualityBonus
```

**What it changes:**
- Value of each shawarma sold
- Income per delivery
- Total earnings potential

**Breakdown:**
- **Base Value**: `$50`
- **Material Bonuses**:
  - Bread: `level × 5` (2.5% of base per level)
  - Chicken: `level × 8` (4% of base per level)
  - Sauce: `level × 3` (1.5% of base per level)
- **Prestige Bonus**: `prestigeStars × 0.1 × baseValue` (10% of base per star)
  - Formula: `chefStars × 0.1 × 50 = chefStars × 5`
- **Quality Bonus**: Multiplier (default 1.0, can be modified by upgrades)

**Full Formula Expanded:**
```
shawarmaValue = (50 + (breadLevel × 5) + (chickenLevel × 8) + (sauceLevel × 3) + (chefStars × 5)) × qualityBonus
```

**Examples:**
- **No upgrades, 0 stars**: `(50 + 0 + 0 + 0) × 1 = $50`
- **Bread:5, Chicken:3, Sauce:2, 1 star**: 
  - Material: `(5×5) + (3×8) + (2×3) = 25 + 24 + 6 = 55`
  - Prestige: `1 × 5 = 5`
  - Total: `(50 + 55 + 5) × 1 = $110`
- **All max (10/10/10), 5 stars**:
  - Material: `(10×5) + (10×8) + (10×3) = 50 + 80 + 30 = 160`
  - Prestige: `5 × 5 = 25`
  - Total: `(50 + 160 + 25) × 1 = $235`

**Location:** `UpgradeCosts.cs:231-246`

---

### 4. Cook Rate Calculation
**Formula:**
```
cookRate = (cookRateBaseValue + prestigeCookRateBonus + machineRate) + (tapPower × tapRate) + autoChefBonus
```

**What it changes:**
- Production speed (shawarmas per second)
- Time to fill storage
- Overall production efficiency

**Breakdown:**
- **Base Cook Rate**: `200` units/second
- **Prestige Cook Rate Bonus**: `chefStars × 0.04 × 50 = chefStars × 2` (4% of base per star)
- **Machine Rate**: `machineLevel × 0.1` (machine upgrade bonus)
- **Tap Power**: Manual tap multiplier (1.0x - 1.5x)
- **Tap Rate**: Manual tap frequency
- **Auto Chef Bonus**: Automatic production bonus when unlocked

**Full Formula Expanded:**
```
cookRate = (200 + (chefStars × 2) + (machineLevel × 0.1)) + (tapPower × tapRate) + autoChefBonus
```

**Location:** `UpgradeCosts.cs:247-252`

---

### 5. Offline Earnings
**Formula:**
```
offlineEarnings = min(estimatedDeliveryRate × min(secondsOffline, 86400), maxEarnings)
```

**What it changes:**
- Player cash when returning to game
- Rewards for idle gameplay

**Components:**
- `estimatedDeliveryRate`: Earnings per second
  - Formula: `(shawarmaValue × averageDeliverySize × deliveriesPerMinute × 0.70) / 60`
- `secondsOffline`: Time offline (capped at 24 hours = 86,400 seconds)
- `maxEarnings`: `estimatedDeliveryRate × 3600` (1 hour cap)
- **Absolute Maximum**: `$10,000,000`

**Location:** `GameManager.cs:72-169`

---

### 6. Income Per Minute (Estimated)
**Formula:**
```
incomePerMinute = shawarmaValue × deliveriesPerMinute × averageDeliverySize × 0.70
```

**What it changes:**
- Used for balance calculations
- Economy testing
- Progression estimates

**Location:** `EconomyCalculator.cs:36-39`

---

## 💸 UPGRADE COST FORMULAS

### 1. Upgrade Cost (Leveling Up Existing Buildings)
**Formula:**
```
cost = (basePrice - prestigeReduction) × (level^multiplier) × (1 / (1 + level × 0.1))
```

**What it changes:**
- Cost to upgrade existing buildings
- Progression speed
- Player spending decisions

**Components:**
- `basePrice`: Base cost of upgrade type (varies by type)
- `prestigeReduction`: Cost reduction from prestige stars
  - Formula: `chefStars × 0.025 × 50 = chefStars × 1.25`
- `level`: Current upgrade level (starts at 1)
- `multiplier`: Exponential multiplier (varies by upgrade type)
- `(1 / (1 + level × 0.1))`: Diminishing returns factor

**Special Case:**
- If `level <= 1`: Returns `basePrice - prestigeReduction` (exact base price)

**Location:** `UpgradeCosts.cs:171-192`

---

### 2. Purchase Cost (Buying New Buildings)
**Formula:**
```
cost = (basePrice - prestigeReduction) × (3.5^existingCount)
```

**What it changes:**
- Cost to purchase additional buildings of same type
- Building expansion decisions
- Late-game progression

**Components:**
- `basePrice`: Base cost of building type
- `prestigeReduction`: `chefStars × 1.25`
- `existingCount`: Number of buildings already placed
- `3.5`: Scaling factor (increased from 2.5x for extended gameplay)

**Special Case:**
- If `existingCount <= 0`: Returns `basePrice - prestigeReduction` (first building costs base price)

**Examples:**
- **First Warehouse** (0 existing): `$3,750 - (chefStars × 1.25)`
- **Second Warehouse** (1 existing): `($3,750 - reduction) × 3.5^1 = $13,125`
- **Third Warehouse** (2 existing): `($3,750 - reduction) × 3.5^2 = $45,937.50`
- **Fourth Warehouse** (3 existing): `($3,750 - reduction) × 3.5^3 = $160,781.25`

**Location:** `UpgradeCosts.cs:130-147`

---

### 3. Extra Building Purchase Cost
**Formula:**
```
cost = (basePrice - prestigeReduction) × (3.5^existingCount)
```

**What it changes:**
- Cost to purchase additional extra buildings of same type
- Passive income building expansion

**Components:**
- Same as purchase cost formula
- `basePrice`: Varies by building type (see Extra Buildings section)

**Location:** `UpgradeCosts.cs:154-169`

---

## 📈 CAPACITY & INTERVAL FORMULAS

### 1. Storage Capacity
**Formula:**
```
capacity = 250 × 2^(level - 2)  (for level > 1)
capacity = 0                     (for level <= 1)
```

**What it changes:**
- Maximum shawarmas that can be stored
- Production blocking threshold
- Delivery capacity limits

**Components:**
- `250`: Base capacity
- `level`: Warehouse upgrade level (currentUpdate)
- **Level Mapping:**
  - `currentUpdate = 1` → Level 0 (unpurchased) → 0 capacity
  - `currentUpdate = 2` → Level 1 (purchased) → 250 capacity
  - `currentUpdate = 3` → Level 2 (first upgrade) → 500 capacity
  - `currentUpdate = 4` → Level 3 (second upgrade) → 1,000 capacity

**Progression:**
- Level 1: 250 shawarmas
- Level 2: 500 shawarmas
- Level 3: 1,000 shawarmas
- Level 4: 2,000 shawarmas
- Level 5: 4,000 shawarmas
- Level 10: 64,000 shawarmas

**Location:** `UpgradeCosts.cs:194-223`

---

### 2. Delivery Capacity
**Formula:**
```
capacity = 2 × (1 + level × 0.4)
```

**What it changes:**
- Number of shawarmas per delivery
- Income per delivery
- Delivery efficiency

**Components:**
- `2`: Base capacity (reduced from 3 for extended gameplay)
- `level`: Delivery van upgrade level
- `0.4`: Capacity multiplier (reduced from 0.5 for extended gameplay)

**Progression:**
- Level 0: 2 shawarmas
- Level 1: 2.8 shawarmas (rounded to 3)
- Level 5: 6 shawarmas
- Level 10: 10 shawarmas
- Level 20: 18 shawarmas

**Location:** `UpgradeCosts.cs:194-223`

---

### 3. Catering Capacity
**Formula:**
```
capacity = 3 × (1 + level × 0.4)
```

**What it changes:**
- Number of shawarmas per catering order
- Catering income per order
- Bulk delivery efficiency

**Components:**
- `3`: Base capacity (reduced from 5 for extended gameplay)
- `level`: Catering upgrade level
- `0.4`: Capacity multiplier (reduced from 0.5 for extended gameplay)

**Progression:**
- Level 0: 3 shawarmas
- Level 1: 4.2 shawarmas (rounded to 4)
- Level 5: 9 shawarmas
- Level 10: 15 shawarmas
- Level 20: 27 shawarmas

**Location:** `UpgradeCosts.cs:194-223`

---

### 4. Delivery Interval (Time Between Deliveries)
**Formula:**
```
interval = 60 / (1 + upgradeLevel × 0.05)
```

**What it changes:**
- Time between delivery van spawns
- Delivery frequency
- Income rate

**Components:**
- `60`: Base interval in seconds (increased from 30 for extended gameplay)
- `upgradeLevel`: Delivery van upgrade level
- `0.05`: Reduction multiplier (reduced from 0.08 for extended gameplay)

**Progression:**
- Level 0: 60 seconds (1 delivery/minute)
- Level 1: 57.1 seconds (1.05 deliveries/minute)
- Level 5: 48 seconds (1.25 deliveries/minute)
- Level 10: 40 seconds (1.5 deliveries/minute)
- Level 20: 30 seconds (2 deliveries/minute)

**Location:** `UpgradeCosts.cs:253-262`

---

### 5. Catering Interval (Time Between Catering Orders)
**Formula:**
```
interval = 90 / (1 + upgradeLevel × 0.05)
```

**What it changes:**
- Time between catering van spawns
- Catering order frequency
- Secondary income rate

**Components:**
- `90`: Base interval in seconds (increased from 45 for extended gameplay)
- `upgradeLevel`: Catering upgrade level
- `0.05`: Reduction multiplier (reduced from 0.08 for extended gameplay)

**Progression:**
- Level 0: 90 seconds (0.67 orders/minute)
- Level 1: 85.7 seconds (0.70 orders/minute)
- Level 5: 72 seconds (0.83 orders/minute)
- Level 10: 60 seconds (1 order/minute)
- Level 20: 45 seconds (1.33 orders/minute)

**Location:** `UpgradeCosts.cs:263-272`

---

## ⭐ PRESTIGE SYSTEM FORMULAS

### 1. Chef Stars Calculation
**Formula:**
```
chefStars = floor(log10(totalEarnings / 100,000))
```

**What it changes:**
- Prestige level
- All prestige bonuses
- Cost reductions
- Income multipliers

**Components:**
- `totalEarnings`: Player's lifetime total earnings
- `100,000`: Base threshold (increased from 10,000 for extended gameplay)

**Prestige Thresholds:**
- 0 Stars: $0 - $999,999
- 1 Star: $1,000,000 - $9,999,999
- 2 Stars: $10,000,000 - $99,999,999
- 3 Stars: $100,000,000 - $999,999,999
- 4 Stars: $1,000,000,000+

**Location:** `UpgradeCosts.cs:273-280`

---

### 2. Next Prestige Value
**Formula:**
```
nextPrestigeValue = 10^chefStars × 1,000,000
```

**What it changes:**
- Shows player how much more they need to earn for next prestige
- Prestige progression goals

**Examples:**
- 0 Stars → Next: `10^0 × 1,000,000 = $1,000,000`
- 1 Star → Next: `10^1 × 1,000,000 = $10,000,000`
- 2 Stars → Next: `10^2 × 1,000,000 = $100,000,000`

**Location:** `UpgradeCosts.cs:281-288`

---

### 3. Prestige Income Bonus
**Formula:**
```
prestigeIncomeBonus = chefStars × 0.1 × baseValue = chefStars × 5
```

**What it changes:**
- Added to shawarma value
- Income per delivery
- Total earnings

**Per Star:**
- 1 Star: +$5 per shawarma
- 5 Stars: +$25 per shawarma
- 10 Stars: +$50 per shawarma

**Location:** `UpgradeCosts.cs:298-301`

---

### 4. Prestige Cost Reduction
**Formula:**
```
prestigeCostReduction = chefStars × 0.025 × baseValue = chefStars × 1.25
```

**What it changes:**
- Reduces all upgrade and purchase costs
- Makes progression faster after prestige
- Encourages prestige system usage

**Per Star:**
- 1 Star: -$1.25 from base prices
- 5 Stars: -$6.25 from base prices
- 10 Stars: -$12.50 from base prices

**Location:** `UpgradeCosts.cs:302-305`

---

### 5. Prestige Cook Rate Bonus
**Formula:**
```
prestigeCookRateBonus = chefStars × 0.04 × baseValue = chefStars × 2
```

**What it changes:**
- Production speed
- Time to fill storage
- Overall production efficiency

**Per Star:**
- 1 Star: +2 cook rate
- 5 Stars: +10 cook rate
- 10 Stars: +20 cook rate

**Location:** `UpgradeCosts.cs:306-310`

---

### 6. Automatic Earning Multiplier
**Formula:**
```
automaticEarningMultiplier = 1.0 + prestigeMultiplier + upgradeMultiplier
```

**What it changes:**
- Passive earning rate
- Offline earnings
- Automatic income generation

**Components:**
- **Prestige Multiplier**: `chefStars × 0.05` (5% per star)
- **Upgrade Multiplier**: `totalUpgradeLevels × 0.01` (1% per upgrade level)
  - Sums all upgrade levels across Storage, Delivery, Kitchen, and Catering

**Location:** `UpgradeCosts.cs:343-437`

---

## 🏗️ UPGRADE TYPE CONFIGURATIONS

### 1. Storage (Warehouse)
**Base Price:** `$3,750` (reduced from $5,000)  
**Purchase Multiplier:** `1.5x` (for buying new buildings)  
**Upgrade Multiplier:** `1.4x`  
**Role:** High value bottleneck, prevents production stops

**Upgrade Cost Examples (0 prestige stars):**
- Level 1: $3,750
- Level 2: $3,750 × (2^1.4) × (1/1.2) = $8,750
- Level 5: $3,750 × (5^1.4) × (1/1.5) = $21,000
- Level 10: $3,750 × (10^1.4) × (1/2.0) = $56,250

**Purchase Cost Examples (0 prestige stars):**
- First Warehouse: $3,750
- Second Warehouse: $3,750 × 3.5 = $13,125
- Third Warehouse: $3,750 × 3.5² = $45,937.50
- Fourth Warehouse: $3,750 × 3.5³ = $160,781.25

**Location:** `UpgradeCosts.cs:61`

---

### 2. Delivery Van
**Base Price:** `$1,875` (reduced from $2,500)  
**Purchase Multiplier:** `1.5x`  
**Upgrade Multiplier:** `1.35x`  
**Role:** Critical bottleneck, primary income source

**Upgrade Cost Examples (0 prestige stars):**
- Level 1: $1,875
- Level 2: $1,875 × (2^1.35) × (1/1.2) = $4,375
- Level 5: $1,875 × (5^1.35) × (1/1.5) = $10,500
- Level 10: $1,875 × (10^1.35) × (1/2.0) = $28,125

**Purchase Cost Examples (0 prestige stars):**
- First Van: $1,875
- Second Van: $1,875 × 3.5 = $6,562.50
- Third Van: $1,875 × 3.5² = $22,968.75
- Fourth Van: $1,875 × 3.5³ = $80,390.63

**Location:** `UpgradeCosts.cs:62`

---

### 3. Kitchen
**Base Price:** `$7,500` (reduced from $10,000)  
**Purchase Multiplier:** `1.2x`  
**Upgrade Multiplier:** `1.3x`  
**Role:** Production boost, moderate value

**Upgrade Cost Examples (0 prestige stars):**
- Level 1: $7,500
- Level 2: $7,500 × (2^1.3) × (1/1.2) = $17,500
- Level 5: $7,500 × (5^1.3) × (1/1.5) = $42,000
- Level 10: $7,500 × (10^1.3) × (1/2.0) = $112,500

**Purchase Cost Examples (0 prestige stars):**
- First Kitchen: $7,500
- Second Kitchen: $7,500 × 3.5 = $26,250
- Third Kitchen: $7,500 × 3.5² = $91,875
- Fourth Kitchen: $7,500 × 3.5³ = $321,562.50

**Location:** `UpgradeCosts.cs:63`

---

### 4. Catering
**Base Price:** `$5,625` (reduced from $7,500)  
**Purchase Multiplier:** `1.2x`  
**Upgrade Multiplier:** `1.25x`  
**Role:** Bonus income, secondary value

**Upgrade Cost Examples (0 prestige stars):**
- Level 1: $5,625
- Level 2: $5,625 × (2^1.25) × (1/1.2) = $13,125
- Level 5: $5,625 × (5^1.25) × (1/1.5) = $31,500
- Level 10: $5,625 × (10^1.25) × (1/2.0) = $84,375

**Purchase Cost Examples (0 prestige stars):**
- First Catering: $5,625
- Second Catering: $5,625 × 3.5 = $19,687.50
- Third Catering: $5,625 × 3.5² = $68,906.25
- Fourth Catering: $5,625 × 3.5³ = $241,171.88

**Location:** `UpgradeCosts.cs:64`

---

## 🏢 EXTRA BUILDINGS CONFIGURATIONS

All extra buildings use the same purchase cost formula:
```
cost = (basePrice - prestigeReduction) × (3.5^existingCount)
```

### Extra Building Types & Base Prices

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

**Location:** `UpgradeCosts.cs:95-105`

---

## 📊 MATERIAL UPGRADE FORMULAS

### 1. Bread Upgrade Value
**Formula:**
```
breadValue = breadLevel × 5
```

**What it changes:**
- Added to shawarma base value
- Income per shawarma
- Total earnings

**Per Level:** +$5 per shawarma (2.5% of base)

**Location:** `UpgradeCosts.cs:316-319`

---

### 2. Chicken Upgrade Value
**Formula:**
```
chickenValue = chickenLevel × 8
```

**What it changes:**
- Added to shawarma base value
- Income per shawarma
- Total earnings

**Per Level:** +$8 per shawarma (4% of base)

**Location:** `UpgradeCosts.cs:320-323`

---

### 3. Sauce Upgrade Value
**Formula:**
```
sauceValue = sauceLevel × 3
```

**What it changes:**
- Added to shawarma base value
- Income per shawarma
- Total earnings

**Per Level:** +$3 per shawarma (1.5% of base)

**Location:** `UpgradeCosts.cs:324-327`

---

## ⚙️ MACHINE UPGRADE FORMULAS

### Machine Cook Rate Bonus
**Formula:**
```
machineCookRate = machineLevel × 0.1
```

**What it changes:**
- Added to base cook rate
- Production speed
- Time to fill storage

**Per Level:** +0.1 cook rate

**Location:** `UpgradeCosts.cs:330-333`

---

## 📝 FORMULA SUMMARY TABLE

| Formula Type | Formula | What It Changes |
|-------------|---------|------------------|
| **Delivery Earnings** | `shawarmaValue × quantity × 0.70` | Player cash, total earnings |
| **Catering Earnings** | `shawarmaValue × quantity × 0.70` | Player cash, total earnings |
| **Shawarma Value** | `(50 + materialBonuses + prestigeBonus) × qualityBonus` | Value per shawarma, income |
| **Cook Rate** | `(200 + prestigeBonus + machineRate) + tapBonus + autoChef` | Production speed |
| **Upgrade Cost** | `(basePrice - prestigeReduction) × (level^multiplier) × (1/(1+level×0.1))` | Upgrade cost |
| **Purchase Cost** | `(basePrice - prestigeReduction) × (3.5^existingCount)` | New building cost |
| **Storage Capacity** | `250 × 2^(level-2)` | Max storage |
| **Delivery Capacity** | `2 × (1 + level × 0.4)` | Shawarmas per delivery |
| **Catering Capacity** | `3 × (1 + level × 0.4)` | Shawarmas per order |
| **Delivery Interval** | `60 / (1 + level × 0.05)` | Time between deliveries |
| **Catering Interval** | `90 / (1 + level × 0.05)` | Time between orders |
| **Chef Stars** | `floor(log10(totalEarnings / 100,000))` | Prestige level |
| **Prestige Income** | `chefStars × 5` | Added to shawarma value |
| **Prestige Cost Reduction** | `chefStars × 1.25` | Reduces all costs |
| **Prestige Cook Rate** | `chefStars × 2` | Added to cook rate |

---

## 🔍 IMPLEMENTATION NOTES

### Key Design Decisions
1. **Extended Gameplay**: All values reduced/increased to extend gameplay to 1+ week
   - Base shawarma value: $100 → $50 (50% reduction)
   - Tax rate: 20% → 30% (slower income)
   - Base intervals: 30s/45s → 60s/90s (slower delivery)
   - Purchase scaling: 2.5x → 3.5x (more expensive expansion)

2. **Diminishing Returns**: Upgrade costs use diminishing returns factor to prevent astronomical late-game costs

3. **Prestige System**: 10x threshold increase for extended gameplay (first prestige at $1M instead of $100K)

4. **Capacity Scaling**: Storage doubles each level, delivery/catering use additive scaling

---

## 📍 CODE LOCATIONS

- **Main Economy File**: `Assets/Scripts/UpgradeSystem/UpgradeCosts.cs`
- **Economy Calculator**: `Assets/Scripts/Economy/EconomyCalculator.cs`
- **Delivery Implementation**: `Assets/Scripts/DeliveryVan System/DeliveryVan.cs`
- **Catering Implementation**: `Assets/Scripts/Catering/CateringVan.cs`
- **Game Manager**: `Assets/Scripts/Managers/GameManager.cs`

---

**Document Generated:** Based on codebase analysis  
**All formulas verified against actual implementation**
