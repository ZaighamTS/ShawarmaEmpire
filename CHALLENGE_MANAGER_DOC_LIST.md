# Challenges from the Document → Challenge Manager (Phase 1)

The **Challenge Manager** only supports 4 goal types: **DeliverCount**, **EarnCash**, **ProduceCount**, **UpgradeCount**.  
Use this list to fill **Custom Definitions** in the Inspector. Set **Difficulty Tier** (0–4) so challenges scale with player progress; use **-1** for "any tier".

---

## ✅ ADD THESE TO CHALLENGE MANAGER (Custom Definitions)

Copy each row into a new list element in **Challenge Manager → Custom Definitions**.  
Suggested **Difficulty Tier**: 0 = starter, 1 = growing, 2 = mid, 3 = late, 4 = veteran.

| # | Title | Description | Goal Type | Target Value | Reward Type | Reward Value | Tier |
|---|--------|--------------|-----------|--------------|-------------|--------------|------|
| 1 | Two Hundred | Produce 200 shawarmas | ProduceCount | 200 | Cash | 1500 | 0 |
| 2 | Get Going | Earn $500 | EarnCash | 500 | Cash | 150000 | 0 |
| 3 | Bonus Practice | Complete 5 deliveries | DeliverCount | 5 | Gold | 12 | 0 |
| 4 | Production Grind | Produce 7,500 shawarmas | ProduceCount | 7500 | Gold | 24 | 0 |
| 5 | Tons of Production | Produce 10,000 shawarmas | ProduceCount | 10000 | Gold | 24 | 0 |
| 6 | More Production | Produce 50,000 shawarmas total | ProduceCount | 50000 | Cash | 1000000 | 1 |
| 7 | Production Everywhere | Produce 50,000 shawarmas total | ProduceCount | 50000 | ChefStar | 1 | 1 |
| 8 | Rack It In | Earn $1,000,000 | EarnCash | 1000000 | Gold | 48 | 2 |
| 9 | Get Rich Quick | Earn $5,000,000 | EarnCash | 5000000 | ChefStar | 1 | 2 |
| 10 | One Sec | Earn $10,000,000 | EarnCash | 10000000 | Gold | 48 | 3 |

**Notes:**
- **Goal Type** in Unity: use the enum: `DeliverCount`, `EarnCash`, `ProduceCount`, `UpgradeCount`.
- **Reward Type**: `Cash`, `Gold`, `ChefStar`.
- Doc “Earn $X in one second” is implemented as **EarnCash** with that target (lifetime earn during the challenge). For true “in one second” use the **Achievements** panel (Phase 2).

---

## ❌ FROM THE DOC BUT **NOT** FOR CHALLENGE MANAGER

These use goals the Challenge Manager doesn’t have (Storage, Delivery capacity, Research, Cash in bank, Chef Stars count, etc.). They belong in the **Achievements** panel (Phase 2) or future systems.

| Title | Reason |
|-------|--------|
| Early Tech | Research 30 things — no Research goal in challenges |
| Growing Family | Expand warehouses to 4,200 — no StorageCapacity goal |
| Shawarma Up | Start with upgraded shawarma type — special condition |
| Science! | Research 150 things — no Research goal |
| Big Storage | Warehouses hold 15,000 — no StorageCapacity goal |
| Supply Chain | Delivery capacity 250/min — no DeliveryCapacityPerMin goal |
| Research Champ | Complete Tier 5 research — no Research goal |
| Shawarma City | 1M shawarmas stored — no StoredShawarmas goal |
| YUUGE Storage | Warehouses hold 2M — no StorageCapacity goal |
| Cash Avalanche | Lifetime $500M — use Achievements (TotalEarnings) |
| Eleven Figure Shawarmas | Produce $10B worth — different metric |
| Money Vault | $1B in bank — use Achievements (PlayerCash) |
| Shawarma Metropolis | 5M stored — use Achievements (StoredShawarmas) |
| Epic Storage | Warehouses 50M — use Achievements (StorageCapacity) |
| Shawarma Country | 50M stored — use Achievements |
| Soul Search | 50,000 Chef Stars — use Achievements (ChefStars) |
| Shawarma Planet | 150M stored — use Achievements |
| Gourmet Grandmaster | 5T Gourmet — shawarma type / value metric |
| Mad Scientist | Research 1,100 — no Research goal |
| Shawarma Galaxy | 300M stored — use Achievements |
| Soul King | 250,000 Chef Stars — use Achievements |
| Signature Dealer | 10T Signature — shawarma type |
| Research Expert, Research Pro, High Tech, Science Overload, etc. | All Research-based — use Achievements when Research exists |
| Big Town, 75K Storage, Giant Storage, Big Warehouses, So Much Chaos, Bandwidth, Mega Fleet, etc. | Storage / capacity / special — use Achievements |

---

## Quick copy-paste for Unity (10 challenges)

Add **10** elements to **Custom Definitions** and set:

1. **Two Hundred** — ProduceCount, 200, Cash, 1500, Tier 0  
2. **Get Going** — EarnCash, 500, Cash, 150000, Tier 0  
3. **Bonus Practice** — DeliverCount, 5, Gold, 12, Tier 0  
4. **Production Grind** — ProduceCount, 7500, Gold, 24, Tier 0  
5. **Tons of Production** — ProduceCount, 10000, Gold, 24, Tier 0  
6. **More Production** — ProduceCount, 50000, Cash, 1000000, Tier 1  
7. **Production Everywhere** — ProduceCount, 50000, ChefStar, 1, Tier 1  
8. **Rack It In** — EarnCash, 1000000, Gold, 48, Tier 2  
9. **Get Rich Quick** — EarnCash, 5000000, ChefStar, 1, Tier 2  
10. **One Sec** — EarnCash, 10000000, Gold, 48, Tier 3  

You can add more **DeliverCount** / **EarnCash** / **ProduceCount** / **UpgradeCount** challenges from the doc using the same pattern (goal type, target, reward type, reward value, tier).
