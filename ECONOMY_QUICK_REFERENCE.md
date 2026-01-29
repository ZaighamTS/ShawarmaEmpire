# Economy Quick Reference - Shawarma Dash

## 💰 Income Sources

### Delivery Van (Primary)
- **Formula**: `shawarmaValue × quantity × 0.80`
- **Base Rate**: 2 deliveries/min, 3 shawarmas each
- **Early Game**: ~$480/min (~$28,800/hour)
- **Late Game**: ~$16,026/min (~$961,560/hour)

### Catering Van (Secondary)
- **Formula**: `shawarmaValue × quantity × 0.80`
- **Base Rate**: 1.33 orders/min, 5 shawarmas each
- **Early Game**: ~$533/min (~$32,000/hour)
- **Late Game**: ~$17,856/min (~$1,071,360/hour)

---

## 💵 Shawarma Value

### Base Value: $100

### Bonuses:
- **Bread**: +$5 per level (max +$50)
- **Chicken**: +$8 per level (max +$80)
- **Sauce**: +$3 per level (max +$30)
- **Prestige**: +$10 per star

### Example Values:
- **No Upgrades**: $100
- **Mid Game**: $165 (Bread:5, Chicken:3, Sauce:2, 1 Star)
- **Max Upgrades**: $360+ (All materials max, 10+ Stars)

---

## 💸 Upgrade Costs

### Cost Formula:
```
cost = (basePrice - prestigeReduction) × (level^multiplier) × (1 / (1 + level × 0.1))
```

### Base Prices & Multipliers:

| Type | Base Price | Multiplier | Role |
|------|------------|------------|------|
| Storage | $1,000 | 1.4x | Critical bottleneck |
| Delivery Van | $500 | 1.35x | Primary income |
| Kitchen | $2,000 | 1.3x | Production boost |
| Catering | $1,500 | 1.25x | Secondary income |

### Cost Examples (Level 1-10):

| Level | Storage | Delivery | Kitchen | Catering |
|-------|---------|----------|---------|----------|
| 1 | $1,000 | $500 | $2,000 | $1,500 |
| 5 | $9,000 | $3,900 | $16,000 | $10,600 |
| 10 | $25,120 | $10,600 | $40,000 | $26,800 |
| 20 | $67,200 | $28,400 | $107,200 | $71,700 |

---

## 📊 Capacity & Speed

### Storage Capacity:
```
capacity = 100 × (1 + level × 1.4)
```
- Level 1: 240 shawarmas
- Level 10: 1,500 shawarmas
- Level 20: 2,900 shawarmas

### Delivery Capacity:
```
capacity = 3 × (1 + level × 0.5)
```
- Level 0: 3 shawarmas
- Level 10: 18 shawarmas
- Level 20: 33 shawarmas

### Delivery Interval:
```
interval = 30 / (1 + level × 0.08) seconds
```
- Level 0: 30.0s (2.0/min)
- Level 10: 16.7s (3.59/min)
- Level 20: 12.5s (4.80/min)

### Catering Capacity:
```
capacity = 5 × (1 + level × 0.5)
```
- Level 0: 5 shawarmas
- Level 10: 30 shawarmas
- Level 20: 55 shawarmas

### Catering Interval:
```
interval = 45 / (1 + level × 0.08) seconds
```
- Level 0: 45.0s (1.33/min)
- Level 10: 25.0s (2.40/min)
- Level 20: 18.8s (3.19/min)

---

## ⏱️ Progression Speed

### Time to Afford Upgrades:

**Early Game (Level 0-5)**:
- Delivery Van: 1-2 minutes
- Storage: 2-3 minutes
- Catering: 3-4 minutes

**Mid Game (Level 5-10)**:
- Delivery Van: 1-2 minutes
- Storage: 2-3 minutes
- Catering: 3-4 minutes

**Late Game (Level 10-20)**:
- Delivery Van: 0.5-1 minute
- Storage: 1-2 minutes
- Catering: 1-2 minutes

### Upgrades per Hour:
- **Early Game**: 12-30 upgrades/hour
- **Mid Game**: 15-60 upgrades/hour
- **Late Game**: 20-120 upgrades/hour

---

## ⭐ Prestige System

### Chef Stars Thresholds:
- **0 Stars**: $0 - $99,999
- **1 Star**: $100,000 - $999,999
- **2 Stars**: $1,000,000 - $9,999,999
- **3 Stars**: $10,000,000 - $99,999,999
- **4+ Stars**: $100,000,000+

### Prestige Bonuses:
- **Income**: +$10 per shawarma per star
- **Cost Reduction**: -$2.50 per star
- **Cook Rate**: +4 per star

---

## 🎯 Key Bottlenecks

1. **Storage** - Full storage stops production
2. **Delivery Rate** - Limits income conversion
3. **Production Rate** - Limits storage filling
4. **Shawarma Value** - Long-term income scaling

---

## 📈 Income Scaling

| Stage | Income/Hour | Upgrades/Hour |
|-------|-------------|---------------|
| Early | ~$60,800 | 12-30 |
| Mid | ~$492,060 | 15-60 |
| Late | ~$2,032,920 | 20-120 |

---

## 🔑 Key Formulas

### Income:
```
deliveryEarnings = shawarmaValue × quantity × 0.80
cateringEarnings = shawarmaValue × quantity × 0.80
```

### Value:
```
shawarmaValue = (100 + materialBonuses + prestigeBonus) × qualityBonus
```

### Cost:
```
cost = (basePrice - prestigeReduction) × (level^multiplier) × (1 / (1 + level × 0.1))
```

### Capacity:
```
storageCapacity = 100 × (1 + level × 1.4)
deliveryCapacity = 3 × (1 + level × 0.5)
cateringCapacity = 5 × (1 + level × 0.5)
```

### Intervals:
```
deliveryInterval = 30 / (1 + level × 0.08)
cateringInterval = 45 / (1 + level × 0.08)
```

---

**Quick Reference Version**: 1.0  
**See**: `COMPREHENSIVE_ECONOMY_ANALYSIS.md` for detailed analysis
