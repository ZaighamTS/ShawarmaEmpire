# Economy Analysis: Why Earnings Feel Too High

## Save file: `shawarma.json`

**Location:** `Application.persistentDataPath` / `shawarma.json`  
- **Windows:** `C:\Users\<You>\AppData\LocalLow\<CompanyName>\<ProductName>\shawarma.json`  
- **Android:** `/storage/emulated/0/Android/data/<package>/files/shawarma.json` (or similar)

**Format:** One JSON object. Keys = save system IDs; values = each system’s saved state (often nested objects).

| Key | What it saves |
|-----|----------------|
| `player_progress` | `playerCash`, `gold`, `chefStars`, `shwarmaCount`, `totalEarnings`, `totalDeliveriesCompleted`, `totalCateringOrdersCompleted`, `totalUpgradesPurchased`, `totalMoneySpentOnUpgrades`, `totalPlayTimeSeconds`, `lastLoginUtc`, `currentShawarmaTypeId` |
| `building` | `currentUpdate`, `extraBuildingLevels` (per–extra-building level 0–10) |
| `boosts` | `activeBoosts` (list of `{ boostId, endTimeUtc }`) |
| `warehouse0`, `warehouse1`, … | Per-warehouse: capacity, load, level, etc. |
| `daily_login` | `dayIndex`, `lastClaimDateUtc` (gift calendar) |
| Plus other ISaveable keys (e.g. delivery, kitchen, catering, challenges, achievements) | Their respective state |

**Important:** `shwarmaCount` in the JSON is **total shawarmas ever produced** (lifetime). It is used for automatic earning rate and stats. It is **not** “shawarmas currently in storage” (that comes from warehouse `currentLoad` / GetWholeLoad()).

---

## Where money comes from (income sources)

1. **Delivery van** – When a van delivers shawarmas to a customer:  
   `totalRewards = shawarmaValue * quantity * 0.70` (70% after “tax”), then `AddCash(totalRewards)`.

2. **Catering van** – Same idea: reward from order size and shawarma value, then `AddCash`.

3. **Automatic earning (idle)** – Every second, in `GameManager`:  
   - Base rate = **`ShwarmaCount * 0.01`** per second (0.01 per **lifetime produced** shawarma).  
   - Then multiplied by: prestige multiplier, upgrade multiplier, and production boost.  
   - So with **8 shawarmas** produced ever: 8 × 0.01 = **$0.08/sec** → ~\$288/hour from this alone. **This is small** and is not the cause of 200k.

4. **Extra buildings (Phase 8)** – Each purchased extra building earns **net \$/hr** by type and level (via `ExtraBuildingFunctionality`). Only runs **while the game is running** (Update loop). So when the app is closed, this does not add anything.

5. **Offline earnings** – When you **reopen the game**, `CheckOfflineEarning()` runs once. It:
   - Reads last exit time from **PlayerPrefs** (`CurrentDateTime`).
   - Computes time away (capped at 24 hours).
   - **Estimates** earnings per second from:
     - **Total warehouse capacity** (all warehouses’ capacity summed).
     - **Assumed** 2 deliveries per minute, **5% of total capacity** per delivery (capped at 20 shawarmas).
     - Current **shawarma value** (from upgrades + type).
   - Then: `amount = estimatedRate * min(secondsAway, 3600)` → **capped at 1 hour** of that estimated rate.
   - That amount is added in one go with `AddCash(amount)` and shown in a popup.

So after being away for a couple of hours, **all of the 200k+ can be a single offline-earnings grant**, not from the 8 shawarmas or from automatic earning during play.

---

## Why 200k+ from “only 8 shawarmas” is possible

- **Offline earnings do not use how many shawarmas you actually had.**  
  They use:
  - **Total capacity** (e.g. one warehouse with 500 → 500; two with 500 each → 1000).
  - **Current shawarma value** (e.g. \$50–\$150+ with type + upgrades).
  - Fixed assumption: 2 deliveries/min, 5% of capacity per delivery (max 20).

Example:

- Capacity 1000, shawarma value \$100:  
  Per delivery: 20 × \$100 = \$2000.  
  Per minute: 2 × \$2000 × 0.7 = \$2800.  
  Per second: ~\$46.67.  
  **1 hour cap → 46.67 × 3600 ≈ \$168,000.**

- Same capacity, value \$150 (e.g. Premium type + upgrades):  
  ~\$70/sec → **~\$252,000** in one offline grant.

So with moderate capacity and upgraded/value type, **200k+ in one pop when you reopen is expected** with the current formula, even if you only ever produced 8 shawarmas. The formula is a “theoretical” rate based on capacity and value, not on actual inventory or real deliveries.

---

## Summary table (quick reference)

| Source | When it runs | Uses | Your case (8 shawarmas) |
|--------|----------------|------|-------------------------|
| Automatic earning | Every second while playing | `ShwarmaCount` × 0.01 × multipliers | ~\$0.08/s → small |
| Delivery/Catering | When a van completes a run | Real deliveries, real value | Depends on play |
| Extra buildings | Every frame while playing | Level, type, net \$/hr | Only when app open |
| **Offline earnings** | **Once on load** | **Capacity + value, 1 hr cap** | **Can easily be 200k+** |

---

## Recommended changes (to make earnings feel right)

1. **Cap offline earnings by actual inventory**  
   - Use **min(capacity, GetWholeLoad())** or similar so we don’t assume deliveries of shawarmas you didn’t have.  
   - Or: assume only a small fraction of capacity is “filled” when away (e.g. 10–20% of capacity), then apply the same delivery assumption to that.

2. **Lower the “1 hour” cap**  
   - e.g. Cap at **15 or 30 minutes** of the estimated rate instead of 1 hour, so one return doesn’t grant as much.

3. **Tighten the rate assumption**  
   - e.g. Use **1 delivery per minute** instead of 2, or **2–3% of capacity** per delivery instead of 5%, so offline income is lower for the same capacity and value.

4. **Optional: scale by progress**  
   - Reduce offline rate when `totalEarnings` or `ShwarmaCount` is very low (e.g. first few thousand earned or first few dozen shawarmas), so early game doesn’t get a huge lump sum.

---

## Changes applied (in code)

1. **Offline earnings capped by actual stored shawarmas**  
   When warehouses exist, we use `GetWholeLoad()` (saved inventory). You cannot earn more than selling all stored shawarmas once (stored × value × 0.70).

2. **Offline “time” cap reduced from 1 hour to 30 minutes**  
   We use `min(secondsAway, 1800)` so at most 30 minutes of the estimated rate is applied per return. So even with high capacity, one login no longer grants 1 hour of theoretical earnings.

3. **Delivery size capped by inventory**  
   If `totalStored < averageDeliverySize`, we use `averageDeliverySize = totalStored` so the rate is based on what you actually had, not full capacity.

With **8 shawarmas** stored, offline earnings are now at most: **8 × shawarmaValue × 0.70** (e.g. ~\$280 at \$50 value, ~\$840 at \$150 value) per return, instead of 200k+.
