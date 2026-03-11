# Delivery System Upgrade Table

Complete breakdown of all delivery system upgrade levels with costs and earnings.

**Base Values:**
- Base Price: $1,875
- Base Capacity: 2 shawarmas per delivery
- Base Interval: 60 seconds between deliveries
- Base Shawarma Value: $50 (assumed for earnings calculations)
- Tax Rate: 0.70 (30% deduction)

**Formulas:**
- Upgrade Cost: `(basePrice - prestigeReduction) × (level^1.35) × (1/(1+level×0.1))` (Level 1 returns base price)
- Capacity: `2 × (1 + level × 0.4)`
- Interval: `60 / (1 + level × 0.05)` seconds
- Earnings per Delivery: `shawarmaValue × capacity × 0.70`
- Deliveries per Minute: `60 / interval`
- Earnings per Minute: `earningsPerDelivery × deliveriesPerMinute`
- Earnings per Hour: `earningsPerMinute × 60`

---

## Complete Upgrade Table (0 Prestige Stars)

| Level | Upgrade Cost | Capacity<br>(Shawarmas) | Interval<br>(seconds) | Deliveries<br>/Min | Earnings<br>/Delivery* | Earnings<br>/Min* | Earnings<br>/Hour* |
|-------|--------------|-------------------------|----------------------|-------------------|----------------------|------------------|-------------------|
| 0 | $1,875** | 2.0 | 60.0 | 1.00 | $70.00 | $70.00 | $4,200 |
| 1 | $1,875 | 2.8 | 57.1 | 1.05 | $98.00 | $102.90 | $6,174 |
| 2 | $3,984 | 3.6 | 54.5 | 1.10 | $126.00 | $138.60 | $8,316 |
| 3 | $6,274 | 4.4 | 52.2 | 1.15 | $154.00 | $177.10 | $10,626 |
| 4 | $9,000 | 5.2 | 50.0 | 1.20 | $182.00 | $218.40 | $13,104 |
| 5 | $10,688 | 6.0 | 48.0 | 1.25 | $210.00 | $262.50 | $15,750 |
| 6 | $13,500 | 6.8 | 46.2 | 1.30 | $238.00 | $309.40 | $18,564 |
| 7 | $16,875 | 7.6 | 44.4 | 1.35 | $266.00 | $359.10 | $21,546 |
| 8 | $20,813 | 8.4 | 42.9 | 1.40 | $294.00 | $411.60 | $24,696 |
| 9 | $25,313 | 9.2 | 41.4 | 1.45 | $322.00 | $466.90 | $28,014 |
| 10 | $20,991 | 10.0 | 40.0 | 1.50 | $350.00 | $525.00 | $31,500 |
| 11 | $24,188 | 10.8 | 38.7 | 1.55 | $378.00 | $585.90 | $35,154 |
| 12 | $27,844 | 11.6 | 37.5 | 1.60 | $406.00 | $649.60 | $38,976 |
| 13 | $31,950 | 12.4 | 36.4 | 1.65 | $434.00 | $716.10 | $42,966 |
| 14 | $36,506 | 13.2 | 35.3 | 1.70 | $462.00 | $785.40 | $47,124 |
| 15 | $25,380 | 14.0 | 34.3 | 1.75 | $490.00 | $857.50 | $51,450 |
| 16 | $46,406 | 14.8 | 33.3 | 1.80 | $518.00 | $932.40 | $55,944 |
| 17 | $52,031 | 15.6 | 32.4 | 1.85 | $546.00 | $1,010.10 | $60,606 |
| 18 | $58,156 | 16.4 | 31.6 | 1.90 | $574.00 | $1,090.60 | $65,436 |
| 19 | $64,781 | 17.2 | 30.8 | 1.95 | $602.00 | $1,173.90 | $70,434 |
| 20 | $28,575 | 18.0 | 30.0 | 2.00 | $630.00 | $1,260.00 | $75,600 |

\* Earnings calculated assuming base shawarma value of $50. Actual earnings will be higher with material upgrades (Bread, Chicken, Sauce) and prestige bonuses.

\** Level 0 represents the initial purchase cost. Level 1+ are upgrade costs.

---

## Key Metrics Summary

### Capacity Progression
- **Level 0**: 2 shawarmas per delivery
- **Level 5**: 6 shawarmas per delivery (3x increase)
- **Level 10**: 10 shawarmas per delivery (5x increase)
- **Level 20**: 18 shawarmas per delivery (9x increase)

### Speed Progression
- **Level 0**: 1 delivery per minute (60s interval)
- **Level 5**: 1.25 deliveries per minute (48s interval)
- **Level 10**: 1.5 deliveries per minute (40s interval)
- **Level 20**: 2 deliveries per minute (30s interval)

### Cost Efficiency (Base Shawarma Value $50)
- **Level 0**: $1,875 cost → $4,200/hour → **2.24 hours to break even**
- **Level 5**: $17,500 cost → $15,750/hour → **1.11 hours to break even**
- **Level 10**: $46,875 cost → $31,500/hour → **1.49 hours to break even**
- **Level 20**: $125,000 cost → $75,600/hour → **1.65 hours to break even**

---

## Earnings with Material Upgrades

The base shawarma value of $50 can be increased with material upgrades:
- **Bread**: +$5 per level (max +$50 at level 10)
- **Chicken**: +$8 per level (max +$80 at level 10)
- **Sauce**: +$3 per level (max +$30 at level 10)
- **Prestige Bonus**: +$5 per chef star

### Example Earnings with Upgrades

**Level 10 Delivery System with Max Materials (Bread:10, Chicken:10, Sauce:10, 0 stars):**
- Shawarma Value: $50 + $50 + $80 + $30 = $210
- Capacity: 10 shawarmas
- Earnings per Delivery: $210 × 10 × 0.70 = **$1,470**
- Deliveries per Minute: 1.5
- Earnings per Minute: $1,470 × 1.5 = **$2,205**
- Earnings per Hour: **$132,300**

**Level 20 Delivery System with Max Materials + 3 Prestige Stars:**
- Shawarma Value: $50 + $50 + $80 + $30 + $15 = $225
- Capacity: 18 shawarmas
- Earnings per Delivery: $225 × 18 × 0.70 = **$2,835**
- Deliveries per Minute: 2.0
- Earnings per Minute: $2,835 × 2.0 = **$5,670**
- Earnings per Hour: **$340,200**

---

## Notes

1. **Capacity values are rounded** for display. The game uses exact decimal values in calculations.

2. **Upgrade costs shown are for 0 prestige stars**. With prestige stars, costs are reduced by `chefStars × 1.25` per star.

3. **Earnings scale linearly** with shawarma value improvements from material upgrades and prestige bonuses.

4. **Delivery interval improvements** become less significant at higher levels due to the diminishing returns formula.

5. **Multiple delivery vans** can be purchased, with each additional van costing `(basePrice - prestigeReduction) × (3.5^existingCount)`.

---

**Last Updated:** Based on current codebase implementation  
**Source Files:** `UpgradeCosts.cs`, `DeliveryVan.cs`
