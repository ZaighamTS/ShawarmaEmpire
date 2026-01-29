# Building Purchase Costs - Final Configuration

## Building Order & Recommended Costs

Based on your building order, here are the recommended costs:

---

## 📋 Building Costs (In Order)

| # | Building Name | Cash Cost | Gold Cost | Notes |
|---|---------------|-----------|-----------|-------|
| **1** | **Juice Point** | $500 | 0 | First building - very affordable |
| **2** | **Dessert Point** | $1,000 | 0 | Early game goal |
| **3** | **Merchandise Store** | $1,500 | 0 | Early-mid game |
| **4** | **Ingredients Shop** | $3,000 | 0 | Mid game |
| **5** | **Shawarma Lounge** | $5,000 | 0 | Mid-late game |
| **6** | **Park** | $7,500 | 0 | Late game |
| **7** | **Petrol Station** | $15,000 | 0 | Late game milestone |
| **8** | **Management Building** | $25,000 | 0 | End game premium |

---

## 💰 Cost Progression Rationale

### Early Buildings (1-3)
- **Juice Point ($500)**: First building, achievable within 2-3 minutes
- **Dessert Point ($1,000)**: Second building, achievable within 5-7 minutes
- **Merchandise Store ($1,500)**: Third building, achievable within 8-10 minutes

**Why this works:**
- Base shawarma value: $200
- Early deliveries: ~$190 per shawarma
- First building: ~3 deliveries
- Second building: ~5 deliveries
- Third building: ~8 deliveries

### Mid Game Buildings (4-5)
- **Ingredients Shop ($3,000)**: Requires some progression (15-20 minutes)
- **Shawarma Lounge ($5,000)**: Mid-game milestone (25-30 minutes)

**Why this works:**
- Mid-game shawarma value: $300-400 (with upgrades)
- Mid-game deliveries: ~$285-380 per shawarma
- Ingredients Shop: ~8-11 deliveries
- Shawarma Lounge: ~13-18 deliveries

### Late Game Buildings (6-8)
- **Park ($7,500)**: Late game decorative (35-45 minutes)
- **Petrol Station ($15,000)**: Significant milestone (60-90 minutes)
- **Management Building ($25,000)**: End game premium (90-120 minutes)

**Why this works:**
- Late-game shawarma value: $500-800+ (with max materials)
- Late-game deliveries: ~$475-760 per shawarma
- Park: ~10-16 deliveries
- Petrol Station: ~20-32 deliveries
- Management Building: ~33-53 deliveries

---

## 🎯 Unity Inspector Setup

### Step-by-Step Instructions

1. **Open Unity Editor**
2. **Find GameObject with `BuildingUnlockManager` component**
3. **In Inspector, find the `Buildings` list**
4. **Set up each building in order:**

#### Building 1: Juice Point
```
Name: "Juice Point"
Cost: 500
Gold Cost: 0
Building Object: [Assign your Juice Point GameObject]
Particle: [Assign particle effect if applicable]
```

#### Building 2: Dessert Point
```
Name: "Dessert Point"
Cost: 1000
Gold Cost: 0
Building Object: [Assign your Dessert Point GameObject]
Particle: [Assign particle effect if applicable]
```

#### Building 3: Merchandise Store
```
Name: "Merchandise Store"
Cost: 1500
Gold Cost: 0
Building Object: [Assign your Merchandise Store GameObject]
Particle: [Assign particle effect if applicable]
```

#### Building 4: Ingredients Shop
```
Name: "Ingredients Shop"
Cost: 3000
Gold Cost: 0
Building Object: [Assign your Ingredients Shop GameObject]
Particle: [Assign particle effect if applicable]
```

#### Building 5: Shawarma Lounge
```
Name: "Shawarma Lounge"
Cost: 5000
Gold Cost: 0
Building Object: [Assign your Shawarma Lounge GameObject]
Particle: [Assign particle effect if applicable]
```

#### Building 6: Park
```
Name: "Park"
Cost: 7500
Gold Cost: 0
Building Object: [Assign your Park GameObject]
Particle: [Assign particle effect if applicable]
```

#### Building 7: Petrol Station
```
Name: "Petrol Station"
Cost: 15000
Gold Cost: 0
Building Object: [Assign your Petrol Station GameObject]
Particle: [Assign particle effect if applicable]
```

#### Building 8: Management Building
```
Name: "Management Building"
Cost: 25000
Gold Cost: 0
Building Object: [Assign your Management Building GameObject]
Particle: [Assign particle effect if applicable]
```

---

## 📊 Cost Summary Table

| Building # | Name | Cash Cost | Gold Cost | Est. Time to Afford* |
|------------|------|-----------|-----------|---------------------|
| 1 | Juice Point | $500 | 0 | 2-3 min |
| 2 | Dessert Point | $1,000 | 0 | 5-7 min |
| 3 | Merchandise Store | $1,500 | 0 | 8-10 min |
| 4 | Ingredients Shop | $3,000 | 0 | 15-20 min |
| 5 | Shawarma Lounge | $5,000 | 0 | 25-30 min |
| 6 | Park | $7,500 | 0 | 35-45 min |
| 7 | Petrol Station | $15,000 | 0 | 60-90 min |
| 8 | Management Building | $25,000 | 0 | 90-120 min |

*Time estimates assume active play with basic upgrades

---

## 🔄 Alternative Cost Scaling (If Needed)

If you find these costs too high or too low, here's an alternative scaling:

### Option A: More Affordable (Easier Progression)
| Building # | Name | Cash Cost |
|------------|------|-----------|
| 1 | Juice Point | $300 |
| 2 | Dessert Point | $600 |
| 3 | Merchandise Store | $1,000 |
| 4 | Ingredients Shop | $2,000 |
| 5 | Shawarma Lounge | $3,500 |
| 6 | Park | $5,000 |
| 7 | Petrol Station | $10,000 |
| 8 | Management Building | $20,000 |

### Option B: More Expensive (Longer Progression)
| Building # | Name | Cash Cost |
|------------|------|-----------|
| 1 | Juice Point | $750 |
| 2 | Dessert Point | $1,500 |
| 3 | Merchandise Store | $2,500 |
| 4 | Ingredients Shop | $5,000 |
| 5 | Shawarma Lounge | $7,500 |
| 6 | Park | $12,000 |
| 7 | Petrol Station | $25,000 |
| 8 | Management Building | $50,000 |

---

## ✅ Quick Copy-Paste Values

For easy reference when setting up in Unity:

```
Building 1: Juice Point
Cost: 500
Gold Cost: 0

Building 2: Dessert Point
Cost: 1000
Gold Cost: 0

Building 3: Merchandise Store
Cost: 1500
Gold Cost: 0

Building 4: Ingredients Shop
Cost: 3000
Gold Cost: 0

Building 5: Shawarma Lounge
Cost: 5000
Gold Cost: 0

Building 6: Park
Cost: 7500
Gold Cost: 0

Building 7: Petrol Station
Cost: 15000
Gold Cost: 0

Building 8: Management Building
Cost: 25000
Gold Cost: 0
```

---

## 🧪 Testing Checklist

After setting these costs, test:

- [ ] Can player afford Juice Point within 3 minutes?
- [ ] Is Dessert Point achievable within 10 minutes?
- [ ] Do mid-game buildings (4-5) feel appropriately priced?
- [ ] Are late-game buildings (6-8) aspirational but achievable?
- [ ] Does the progression feel rewarding?

---

## 📝 Notes

- **All gold costs set to 0** - Adjust if you have a premium currency system
- **Costs scale smoothly** - Each building is roughly 1.5-2x the previous
- **Balanced for progression** - Early buildings quick wins, late buildings goals
- **Adjust as needed** - Use playtesting data to fine-tune

---

**Last Updated:** Based on your building order  
**Document Version:** 1.0
