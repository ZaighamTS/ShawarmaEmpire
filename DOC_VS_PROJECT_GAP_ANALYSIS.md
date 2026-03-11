# Shawarma X EggInc Doc vs Project – Gap Analysis

**Source document:** `Shawarma X EggInc Doc.pdf`  
**Project:** Sharwama Dash (Shawarma Inc)  
**Analysis date:** March 2026

This document compares the design spec (PDF) with the current Unity implementation and lists what is **implemented**, **partial**, or **missing**.

---

## 1. Side-by-side mechanic status (from doc + codebase check)

| Mechanic | Doc / Egg Inc | Project status |
|----------|----------------|----------------|
| **Core loop** | Produce → Store → Deliver → Earn → Upgrade | ✅ **Implemented** |
| **Production** | Manual tapping + auto | ✅ **Implemented** (two systems exist; consolidation recommended) |
| **Storage** | Warehouses with capacity | ✅ **Implemented** (250 base, doubling per level) |
| **Upgrades** | Warehouses + kitchen upgradable | ✅ **Implemented** (Storage, Delivery, Kitchen, Catering) |
| **Income source** | Delivery vans + catering vans | ✅ **Implemented** |
| **Prestige** | Chef Stars | ✅ **Implemented** (earnings-based, next at $1M, $10M, …) |
| **Research system** | 13 tiers Common + 22 Epic (Gold) | ❌ **Missing** (simulator only; not in game) |
| **Challenges** | 3 at a time, daily/weekly/special/achievement | ❌ **Missing** |
| **Multiple farm/types** | Multiple shawarma types (Classic→Signature) | ❌ **Missing** (single type + Golden visual) |
| **Gift calendar** | Daily calendar rewards, 7-day cycle | ❌ **Missing** |
| **Boost system** | Gold + “Watch Ad” boosts (11 items in doc) | ⚠️ **Partial** (Gold exists; no boost shop or active boosts) |
| **Statistics screen** | Detailed stats & analytics | ❌ **Missing** |
| **Offline earnings** | With cap | ✅ **Implemented** (24h cap, 1h max earnings, $10M cap) |
| **Space ships / artifacts** | Not recommended in doc | ✅ **Not implemented** (as intended) |

---

## 2. Implemented (aligned with doc)

### 2.1 Core economy

- **Delivery upgrades:** Base cost $1,875, capacity 2 + 0.4×level, interval 60/(1+level×0.05). Implemented in `UpgradeCosts` (with 25% reduction: 1875).
- **Kitchen/Warehouse:** Level 1 purchase $3,750, 250 capacity; then doubling (250×2^(level-2)). Matches doc intent (Level 1 = $3,750, 250; Level 2 = 500; etc.).
- **Extra buildings:** 8 types present (Juice Point → Management). Purchase costs use base × 3.5^existingCount; base prices are 25% reduced from doc’s 5× extended values (e.g. Juice 5625, Management 225000). **Building upgrade levels (0–10) that increase income are not implemented** (see Missing).

### 2.2 Prestige

- Chef Stars from total earnings; thresholds $1M, $10M, …; income/cost/cook bonuses. In `GameManager` / `UpgradeCosts`.

### 2.3 Offline earnings

- Implemented in `GameManager.CheckOfflineEarning()` and `EconomyCalculator.CalculateOfflineEarnings()`: time-based cap (e.g. 24h), max earnings cap (e.g. 1h equivalent), $10M cap. Matches “offline earnings with cap” from doc.

### 2.4 Gold currency

- Stored in `PlayerProgress.Gold`; add/spend in `GameManager`; UI in `UIManager`; used for building unlocks in `BuildingUnlockManager`. No boost shop or Epic Research in game yet.

### 2.5 Material-style upgrades

- Bread, Chicken, Sauce (value bonuses). Present in economy/upgrade logic and in `economic_simulator.html`.

---

## 3. Partial

### 3.1 Boost system

- **Doc:** Boosts shop with 11 items (e.g. Quantum Kitchen Boost, Chef’s Special/Premium/Best Recipe, Production Prism, Large Production Prism, Boost Amplifier, Epic Boost Amplifier, Chef Star Beacon, Business Grant, Chef Star 2x); some “Watch Ad”, some Gold.
- **Project:** Gold exists; no boost shop, no active boost multipliers, no duration-based boosts. **Simulator only:** `economic_simulator.html` has Research and Boosts tabs and multiplier inputs for testing; these are not wired to the game.

### 3.2 IAP / monetization

- **Doc:** Gold packages (Profit Vault, Crate, Pallet, Truckload), subscriptions (Elite Chef Standard/Pro), Shawarma Empire Premium, “Watch Ads for Gold”.
- **Project:** `AdMobManager` present; `Unity_Ads` commented out. Gold is earnable/spendable in code but no in-game boost shop or doc-style IAP UI.

---

## 4. Missing (in game code)

### 4.1 Research system (high impact)

- **Doc:**  
  - **Common research:** 13 tiers, 4 per tier, RP requirements, cash cost, formulas like Base×level^1.2. Examples: Comfortable Kitchen, Quality Ingredients, Premium Shawarmas, Premium Certification, Gourmet Shawarmas, Multiversal Ingredients, Timeline Splicing.  
  - **Epic research:** 22 permanent upgrades, Gold cost, no reset on prestige (e.g. Quantum Storage Expansion, Boost Duration, Chef Star Power, Prestige Multiplier, Lab Upgrade, Offline Production Boost, etc.).
- **Project:** No research scripts, no research UI, no RP or tier unlock logic. Simulator has placeholder research formulas for balance only.

### 4.2 Challenges system (high priority in doc)

- **Doc:** 3 active challenges; types: Daily, Weekly, Special, Achievement. Goals: Deliver X, Earn $X, Produce X, Upgrade X. Rewards: Gold (5–200), Cash ($500–$50K), Prestige.
- **Project:** No challenge/objective system; no challenge UI or tracking.

### 4.3 Achievement challenges table (full list in doc)

- **Doc:** Large table of achievements with rewards (Cash, Gold, Chef Stars). Examples: Two Hundred ($1.5K), Early Tech ($30K), Get Going ($150K), Growing Family ($500K), Shawarma Up (96 Gold), More Production ($1M), Science! (24 Gold), Big Storage (24 Gold), Supply Chain (24 Gold), Rack It In (48 Gold), Get Rich Quick (1 Chef Star), Production Everywhere (1 Chef Star), Research Champ (1 Chef Star), Shawarma City (1 Chef Star), YUUGE Storage (500 Gold), Cash Avalanche (1 Chef Star), Money Vault (1 Chef Star), Soul Search (1.2K Gold), Soul King (3K Gold), etc.
- **Project:** No achievement definitions, no reward granting for achievements, no achievement UI.

### 4.4 Statistics screen

- **Doc:** Production (total produced, rate/hr), delivery (total deliveries, average size), earnings (total, per hour, best hour), upgrades (total upgrades, money spent), time (playtime, offline time).
- **Project:** No stats panel; data could be derived from `PlayerProgress` and delivery/catering managers but is not aggregated or displayed.

### 4.5 Gift calendar

- **Doc:** 7-day cycle, different rewards (Gold 5–50, Cash $1K–$10K, temporary boosts), streak bonus, calendar UI on login.
- **Project:** No login-date tracking, no streak, no calendar UI.

### 4.6 Multiple shawarma types

- **Doc:** Classic ($50) → Spicy ($60) → Premium ($75) → Gourmet ($100) → Signature ($150); unlock by milestones/Chef Stars; different values and optional bonuses.
- **Project:** Single value curve + “Golden” (visual/special); no type enum or unlock progression.

### 4.7 Extra building upgrades (per-building levels)

- **Doc:** Each extra building has **upgrade levels 0–10** with increasing income (e.g. Juice Point Level 0 +$720/hr → Level 5 +$2,174/hr → Level 10 +$5,112/hr; total cost 1–10 given). Some buildings start at loss/break-even and become profit at higher levels.
- **Project:** Buildings are purchased (and optionally placed); there is **no per-building level** or upgrade path that changes income. Income is effectively fixed per building type (or not yet tied to doc’s reward/expense table).

### 4.8 Delivery upgrade table (exact doc numbers)

- **Doc:** Per-level table (Level 0–20): upgrade cost, capacity, deliveries/min, earnings/delivery, earnings/min. Example: Level 0 $1,875, 2.0 cap, 1.00 del/min, $70/del.
- **Project:** Same base cost (1875) and similar formulas; exact table values may differ slightly due to formula tuning (e.g. capacity 2+0.4×level, interval formula). Not a full “missing feature” but worth verifying level-by-level if you want pixel-perfect doc match.

---

## 5. Summary tables

### Implemented vs doc

| Feature | Status |
|---------|--------|
| Core loop (produce → store → deliver → earn → upgrade) | ✅ |
| Storage/warehouse capacity upgrades | ✅ |
| Delivery van upgrades (cost, capacity, interval) | ✅ |
| Catering system | ✅ |
| Prestige (Chef Stars) | ✅ |
| Offline earnings (with caps) | ✅ |
| Gold currency (store, add, spend) | ✅ |
| Extra buildings (8 types, purchase cost scaling) | ✅ |
| Material-style upgrades (Bread, Chicken, Sauce) | ✅ |
| Save/load (progress, gold, etc.) | ✅ |

### Missing vs doc

| Feature | Doc priority / note |
|---------|----------------------|
| Research system (13 Common + 22 Epic) | Recommended; high depth |
| Challenges (3 active, daily/weekly/special/achievement) | #1 recommended |
| Achievement list (rewards: Cash, Gold, Chef Stars) | Part of challenges |
| Statistics screen | #2 recommended, quick win |
| Boost system (shop + active boosts) | #3, monetization |
| Multiple shawarma types | #4, variety |
| Gift calendar | #5, retention |
| Extra building upgrade levels (0–10 per building) | Doc says “implement later” |

### Partial vs doc

| Feature | What exists | What’s missing |
|---------|-------------|-----------------|
| Boost system | Gold currency | Boost shop UI, active boost multipliers, durations, “Watch Ad” for boosts |
| IAP / Shop | AdMob; Gold in code | Doc-style Gold packages, subscriptions, premium; “Watch Ads for Gold” flow |

---

## 6. Recommended implementation order (from doc + gap analysis)

1. **Challenges + achievements** – 3 active challenges, achievement table with rewards (Cash/Gold/Chef Stars). Integrate with DeliveryManager, CateringManager, GameManager, PlayerProgress, UIManager.
2. **Statistics screen** – Aggregate from PlayerProgress and managers; single panel (production, delivery, earnings, upgrades, time).
3. **Boost system** – Boost shop (doc’s 11 boosts), Gold/Ad costs, active multipliers and durations in GameManager, apply to production/earnings/delivery as per doc.
4. **Research system** – Common (13 tiers, 4 per tier, RP, cash) and Epic (22, Gold, permanent); new scripts and UI; hook into value/capacity/prestige formulas.
5. **Multiple shawarma types** – Type enum, unlock by milestones/Chef Stars, value curve per type.
6. **Gift calendar** – Login dates and streak in PlayerProgress, 7-day rewards, calendar popup on login.
7. **Extra building upgrades** – Per-building level 0–10, income and total cost per doc table; UI for upgrading each building.

---

## 7. Simulator vs game

- **economic_simulator.html** includes Research and Boosts tabs and formulas that mirror the doc (common research quality multiplier, epic research capacity/Chef Star, boosts list). These are **for design/balance only**; the Unity game does not yet have Research or Boost systems. Bringing the game in line with the doc would require implementing those systems in C# and wiring them to the same (or reconciled) formulas.

---

**Document status:** Gap analysis complete.  
**Next steps:** Prioritize from §6; implement Challenges and Statistics first for quick wins, then Boosts and Research for depth and monetization.
