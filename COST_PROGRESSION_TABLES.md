# Cost Progression Tables - Shawarma Dash

## Quick Reference: Upgrade Costs by Level

### Storage (Warehouse) Costs
| Level | Base Cost (0 stars) | Cost (5 stars) | Cost (10 stars) | Capacity |
|-------|---------------------|----------------|-----------------|----------|
| 1 | $1,000 | $995 | $990 | 240 |
| 2 | $2,400 | $2,390 | $2,380 | 380 |
| 3 | $4,200 | $4,190 | $4,180 | 520 |
| 4 | $6,400 | $6,390 | $6,380 | 660 |
| 5 | $9,000 | $8,990 | $8,980 | 800 |
| 6 | $12,000 | $11,990 | $11,980 | 940 |
| 7 | $15,400 | $15,390 | $15,380 | 1,080 |
| 8 | $19,200 | $19,190 | $19,180 | 1,220 |
| 9 | $23,400 | $23,390 | $23,380 | 1,360 |
| 10 | $28,000 | $27,990 | $27,980 | 1,500 |
| 15 | $44,800 | $44,790 | $44,780 | 2,200 |
| 20 | $67,200 | $67,190 | $67,180 | 2,900 |
| 25 | $95,200 | $95,190 | $95,180 | 3,600 |

**Formula:** `(1000 - prestigeReduction) × (level^1.4) × (1 / (1 + level × 0.1))`

---

### Delivery Van Costs
| Level | Base Cost (0 stars) | Cost (5 stars) | Cost (10 stars) | Capacity | Interval (sec) |
|-------|---------------------|----------------|-----------------|----------|----------------|
| 1 | $500 | $498 | $495 | 230 | 25.0 |
| 2 | $1,200 | $1,198 | $1,195 | 360 | 21.4 |
| 3 | $2,000 | $1,998 | $1,995 | 490 | 18.8 |
| 4 | $2,900 | $2,898 | $2,895 | 620 | 16.7 |
| 5 | $3,900 | $3,898 | $3,895 | 750 | 15.0 |
| 6 | $5,000 | $4,998 | $4,995 | 880 | 13.6 |
| 7 | $6,200 | $6,198 | $6,195 | 1,010 | 12.5 |
| 8 | $7,500 | $7,498 | $7,495 | 1,140 | 11.5 |
| 9 | $8,900 | $8,898 | $8,895 | 1,270 | 10.7 |
| 10 | $10,400 | $10,398 | $10,395 | 1,400 | 10.0 |
| 15 | $18,900 | $18,898 | $18,895 | 2,050 | 7.5 |
| 20 | $28,400 | $28,398 | $28,395 | 2,700 | 6.0 |
| 25 | $39,000 | $38,998 | $38,995 | 3,350 | 5.0 |

**Formula:** `(500 - prestigeReduction) × (level^1.35) × (1 / (1 + level × 0.1))`

---

### Kitchen Costs
| Level | Base Cost (0 stars) | Cost (5 stars) | Cost (10 stars) |
|-------|---------------------|----------------|-----------------|
| 1 | $2,000 | $1,995 | $1,990 |
| 2 | $4,600 | $4,595 | $4,590 |
| 3 | $7,800 | $7,795 | $7,790 |
| 4 | $11,600 | $11,595 | $11,590 |
| 5 | $16,000 | $15,995 | $15,990 |
| 6 | $21,000 | $20,995 | $20,990 |
| 7 | $26,600 | $26,595 | $26,590 |
| 8 | $32,800 | $32,795 | $32,790 |
| 9 | $39,600 | $39,595 | $39,590 |
| 10 | $47,000 | $46,995 | $46,990 |
| 15 | $71,400 | $71,395 | $71,390 |
| 20 | $107,200 | $107,195 | $107,190 |
| 25 | $155,000 | $154,995 | $154,990 |

**Formula:** `(2000 - prestigeReduction) × (level^1.3) × (1 / (1 + level × 0.1))`

---

### Catering Costs
| Level | Base Cost (0 stars) | Cost (5 stars) | Cost (10 stars) | Capacity | Interval (sec) |
|-------|---------------------|----------------|-----------------|----------|----------------|
| 1 | $1,500 | $1,498 | $1,495 | 201 | 33.3 |
| 2 | $3,400 | $3,398 | $3,395 | 302 | 28.6 |
| 3 | $5,600 | $5,598 | $5,595 | 403 | 25.0 |
| 4 | $8,000 | $7,998 | $7,995 | 504 | 22.2 |
| 5 | $10,600 | $10,598 | $10,595 | 605 | 20.0 |
| 6 | $13,400 | $13,398 | $13,395 | 706 | 18.2 |
| 7 | $16,400 | $16,398 | $16,395 | 807 | 16.7 |
| 8 | $19,600 | $19,598 | $19,595 | 908 | 15.4 |
| 9 | $23,000 | $22,998 | $22,995 | 1,009 | 14.3 |
| 10 | $26,600 | $26,598 | $26,595 | 1,110 | 13.3 |
| 15 | $47,800 | $47,798 | $47,795 | 1,615 | 10.0 |
| 20 | $71,700 | $71,698 | $71,695 | 2,120 | 8.0 |
| 25 | $98,100 | $98,098 | $98,095 | 2,625 | 6.7 |

**Formula:** `(1500 - prestigeReduction) × (level^1.25) × (1 / (1 + level × 0.1))`

---

## Raw Material Upgrade Costs

### Linear Cost Progression
| Current Level | Next Level | Cost | Cumulative Cost |
|---------------|------------|------|-----------------|
| 0 | 1 | $100 | $100 |
| 1 | 2 | $200 | $300 |
| 2 | 3 | $300 | $600 |
| 3 | 4 | $400 | $1,000 |
| 4 | 5 | $500 | $1,500 |
| 5 | 6 | $600 | $2,100 |
| 6 | 7 | $700 | $2,800 |
| 7 | 8 | $800 | $3,600 |
| 8 | 9 | $900 | $4,500 |
| 9 | 10 | $1,000 | $5,500 |

**Total Cost to Max (0-10):** $5,500 per material type

**Materials:**
- Bread (max level 10)
- Chicken (max level 10)
- Sauce (max level 10)
- Chef (max level 10)
- Machine (max level 10)

**Total Cost to Max All Materials:** $27,500

---

## Raw Material Value Bonuses

### Bread Upgrade Value
| Level | Value Bonus | Cumulative Bonus |
|-------|-------------|-------------------|
| 1 | +$5 | +$5 |
| 2 | +$10 | +$15 |
| 3 | +$15 | +$30 |
| 4 | +$20 | +$50 |
| 5 | +$25 | +$75 |
| 10 | +$50 | +$275 |

**Formula:** `level × 5`

---

### Chicken Upgrade Value
| Level | Value Bonus | Cumulative Bonus |
|-------|-------------|-------------------|
| 1 | +$8 | +$8 |
| 2 | +$16 | +$24 |
| 3 | +$24 | +$48 |
| 4 | +$32 | +$80 |
| 5 | +$40 | +$120 |
| 10 | +$80 | +$440 |

**Formula:** `level × 8`

---

### Sauce Upgrade Value
| Level | Value Bonus | Cumulative Bonus |
|-------|-------------|-------------------|
| 1 | +$3 | +$3 |
| 2 | +$6 | +$9 |
| 3 | +$9 | +$18 |
| 4 | +$12 | +$30 |
| 5 | +$15 | +$45 |
| 10 | +$30 | +$165 |

**Formula:** `level × 3`

---

### Combined Material Value Impact

**Base Shawarma Value:** $200

**With Max Materials (Bread:10, Chicken:10, Sauce:10):**
- Material Bonuses: $275 + $440 + $165 = **+$880**
- Total Value: $200 + $880 = **$1,080**

**With Mid-Level Materials (Bread:5, Chicken:5, Sauce:5):**
- Material Bonuses: $75 + $120 + $45 = **+$240**
- Total Value: $200 + $240 = **$440**

---

## Prestige Cost Reduction Impact

### Prestige Reduction Values
| Chef Stars | Cost Reduction | Example Impact (Storage L10) |
|------------|----------------|-------------------------------|
| 0 | $0 | $28,000 |
| 1 | $5 | $27,990 |
| 2 | $10 | $27,980 |
| 3 | $15 | $27,970 |
| 4 | $20 | $27,960 |
| 5 | $25 | $27,950 |
| 10 | $50 | $27,900 |
| 20 | $100 | $27,800 |

**Formula:** `prestigeStars × 0.025 × 200`

**Note:** Impact is minimal at low levels but becomes more significant at higher upgrade levels and prestige stars.

---

## Cost Comparison: Upgrade Types

### Level 10 Comparison (0 prestige stars)
| Upgrade Type | Cost | Multiplier | Cost Efficiency |
|-------------|------|------------|-----------------|
| Delivery Van | $10,400 | 1.35x | Best |
| Catering | $26,600 | 1.25x | Good |
| Storage | $28,000 | 1.4x | Moderate |
| Kitchen | $47,000 | 1.3x | Expensive |

### Level 20 Comparison (0 prestige stars)
| Upgrade Type | Cost | Multiplier | Cost Efficiency |
|-------------|------|------------|-----------------|
| Delivery Van | $28,400 | 1.35x | Best |
| Catering | $71,700 | 1.25x | Good |
| Storage | $67,200 | 1.4x | Moderate |
| Kitchen | $107,200 | 1.3x | Expensive |

---

## Income vs Cost Analysis

### Early Game (Level 1-5)
**Typical Income:**
- Base shawarma: $200
- With materials (level 3 each): $200 + $45 = $245
- Delivery earnings: $245 × 0.95 = **$232.75 per shawarma**

**Typical Costs:**
- Storage upgrade: $1,000 - $9,000
- Delivery Van upgrade: $500 - $3,900
- Raw material upgrade: $100 - $500

**Deliveries Needed for Upgrade:**
- Storage L1→L2: ~4,300 shawarmas
- Delivery Van L1→L2: ~5,200 shawarmas
- Raw Material L3→L4: ~430 shawarmas

---

### Mid Game (Level 5-10)
**Typical Income:**
- Base shawarma: $200
- With materials (level 7 each): $200 + $140 = $340
- With 1 prestige star: +$20 = $360
- Delivery earnings: $360 × 0.95 = **$342 per shawarma**

**Typical Costs:**
- Storage upgrade: $9,000 - $28,000
- Delivery Van upgrade: $3,900 - $10,400
- Raw material upgrade: $700 - $1,000

**Deliveries Needed for Upgrade:**
- Storage L5→L6: ~35,000 shawarmas
- Delivery Van L5→L6: ~11,400 shawarmas
- Raw Material L7→L8: ~2,000 shawarmas

---

### Late Game (Level 10-20)
**Typical Income:**
- Base shawarma: $200
- With max materials: $200 + $880 = $1,080
- With 5 prestige stars: +$100 = $1,180
- Delivery earnings: $1,180 × 0.95 = **$1,121 per shawarma**

**Typical Costs:**
- Storage upgrade: $28,000 - $67,200
- Delivery Van upgrade: $10,400 - $28,400
- Raw material upgrade: $1,000 (max level)

**Deliveries Needed for Upgrade:**
- Storage L10→L11: ~60,000 shawarmas
- Delivery Van L10→L11: ~18,500 shawarmas

---

## Cost Efficiency Rankings

### By Upgrade Multiplier (Lower = More Efficient)
1. **Catering** (1.25x) - Cheapest scaling
2. **Kitchen** (1.3x) - Moderate scaling
3. **Delivery Van** (1.35x) - Good scaling
4. **Storage** (1.4x) - Highest scaling

### By Base Price (Lower = More Affordable Early)
1. **Delivery Van** ($500) - Most affordable
2. **Storage** ($1,000) - Affordable
3. **Catering** ($1,500) - Moderate
4. **Kitchen** ($2,000) - Most expensive

### By Value (Critical Bottlenecks)
1. **Delivery Van** - Primary income source
2. **Storage** - Prevents production stops
3. **Kitchen** - Production boost
4. **Catering** - Bonus income

---

**Document Version:** 1.0  
**Last Updated:** Analysis Date
