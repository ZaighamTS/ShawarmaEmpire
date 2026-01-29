# Income Balancing Calculations
## Target Pacing Analysis

### Upgrade Costs (Early Game)
- Delivery Van Level 1: $500
- Storage Level 1: $1,000
- Catering Level 1: $1,500
- Kitchen Level 1: $2,000

### Target Pacing Goals
**Ideal time to afford upgrade:** 5-10 minutes of active play

**Calculations:**
- $500 / 5 min = $100/min = **$6,000/hour**
- $500 / 10 min = $50/min = **$3,000/hour**
- $1,000 / 5 min = $200/min = **$12,000/hour**
- $1,000 / 10 min = $100/min = **$6,000/hour**

**Target Income Range:** $3,000 - $12,000/hour (early game)

### Current Income (After Previous Fixes)
- Early Game: ~$50,000/hour
- **Still 4-16x too high!**

### Required Reduction
- Need: $5,000/hour (target)
- Current: $50,000/hour
- **Reduction needed: 90%**

---

## Proposed Changes

### 1. Reduce Shawarma Base Value
- Current: $200
- Proposed: $100
- **Reduction: 50%**

### 2. Further Reduce Van Capacity
- Current: 50 base, 1.0x multiplier
- Proposed: 25 base, 0.8x multiplier
- **Reduction: ~60%**

### 3. Further Increase Spawn Intervals
- Current: 45s base (delivery), 60s base (catering)
- Proposed: 90s base (delivery), 120s base (catering)
- **Reduction: 50% (2x slower)**

### 4. Increase Tax Rate
- Current: 15% (0.85)
- Proposed: 25% (0.75)
- **Reduction: 12%**

### 5. Reduce Average Delivery Size Estimate
- Current: 10% of capacity (capped at 50)
- Proposed: 5% of capacity (capped at 25)
- **Reduction: 50%**

### Combined Effect
- Value: 50% reduction
- Capacity: 60% reduction
- Intervals: 50% slower (2x time)
- Tax: 12% more reduction
- Delivery Size: 50% reduction

**Total Reduction:** ~0.5 * 0.4 * 0.5 * 0.88 * 0.5 = **0.044 = 95.6% reduction**

**Expected Income:** $50,000 * 0.044 = **$2,200/hour**

This is slightly below target, so we can adjust slightly upward.

---

## Final Recommendations

1. **Shawarma Value:** $200 → $100 (-50%)
2. **Delivery Capacity:** 50/1.0x → 30/0.8x (-40%)
3. **Spawn Intervals:** 45s/60s → 75s/100s (-40%)
4. **Tax Rate:** 15% → 20% (-6%)
5. **Delivery Size:** 10% → 7% (-30%)

**Expected Result:** ~$5,000-$8,000/hour (perfect for pacing!)
