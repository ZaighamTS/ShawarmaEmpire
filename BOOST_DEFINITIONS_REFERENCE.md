# Boost Definitions – What Each Boost Does

Aligned with **EGG_INC_DOCUMENT_ANALYSIS.md** (Boosts Shop table). Implemented in `BoostManager.BuildDefinitionsFromDoc()`.

---

## 1. What each boost does (in-game effect)

| Boost | Doc description | In-game effect | Duration | Cost |
|-------|-----------------|----------------|----------|------|
| **Quantum Kitchen Boost** | Unlimited production for 10min | **Production mult:** auto-earning rate (shawarmas × 0.01/s) is multiplied by **100×** for 10 min. ("Unlimited" implemented as 100×.) | 10 min | Watch Ad |
| **Chef's Special Recipe** | 3x earnings for 20min | **Earnings mult:** all cash earned (delivery, catering, auto-earning) is multiplied by **3×** for 20 min. | 20 min | Watch Ad |
| **Chef's Premium Recipe** | 10x earnings for 15min | **Earnings mult:** all cash earned is multiplied by **10×** for 15 min. | 15 min | Watch Ad |
| **Chef's Best Recipe** | 50x earnings for 10min | **Earnings mult:** all cash earned is multiplied by **50×** for 10 min. | 10 min | 2,500 Gold |
| **Production Prism** | 10x auto-production for 10min | **Production mult:** auto-earning rate is multiplied by **10×** for 10 min. | 10 min | Watch Ad |
| **Large Production Prism** | 10x auto-production for 4hr | **Production mult:** auto-earning rate is multiplied by **10×** for 4 hours. | 4 hr | 500 Gold |
| **Boost Amplifier** | 2x all active boosts for 30min | **All-boosts mult:** the combined effect of all other active boosts (earnings, production, Chef Stars) is multiplied by **2×** for 30 min. | 30 min | 1,000 Gold |
| **Epic Boost Amplifier** | 10x all active boosts for 10min | **All-boosts mult:** same as above but **10×** for 10 min. | 10 min | 8,000 Gold |
| **Chef Star Beacon** | 5x Chef Stars for 30min | **Chef Star mult:** when the player prestiges, Chef Stars gained are multiplied by **5×** (for 30 min). | 30 min | 200 Gold |
| **Business Grant** | +10% of Business Value | **One-time cash:** pay 200 Gold; instantly receive **10% of TotalEarnings** (lifetime) as cash. No timer; not added to “active boosts”. Business Value = `TotalEarnings`. | — | 200 Gold |
| **Chef Star 2x** | Activates Chef Star 2x for 10min | **Chef Star mult:** when the player prestiges, Chef Stars gained are multiplied by **2×** for 10 min. | 10 min | 100 Gold |

---

## 2. Where multipliers are applied (code)

- **Earnings mult:** `GameManager.AddCash()` multiplies the incoming value by `BoostManager.GetEarningsMultiplier()` (then adds to cash and TotalEarnings).
- **Production mult:** `GameManager.UpdateAutomaticEarningRate()` multiplies the automatic earning rate by `BoostManager.GetProductionMultiplier()`.
- **Chef Star mult:** `GameManager.ResetPlayerStats()` adds `max(1, round(1 × GetChefStarMultiplier()))` Chef Stars per prestige.
- **All-boosts mult:** Included in each of the three getters above (earnings, production, Chef Stars) so it multiplies the combined effect of other active boosts.
- **One-time cash (Business Grant):** When activated, `BoostManager.ApplyOneTimeCashGrant()` calls `GameManager.AddCash(TotalEarnings × 0.1f)` once; no active boost entry.

---

## 3. Watch Ad boosts – do they work after watching an ad?

**Current behaviour (no ad SDK):**  
Boosts that cost “Watch Ad” **do activate**, but **no ad is shown**. The UI calls `BoostManager.ActivateAfterAd(boostId)` as soon as the player taps Activate, so the boost starts immediately (placeholder behaviour for testing).

**To make them work only after the player watches an ad:**

1. Integrate your ad SDK (e.g. AdMob / Unity Ads) and show a rewarded video when the player taps Activate on a “Watch Ad” boost.
2. In the **ad-reward (on completed)** callback, call:
   ```csharp
   BoostManager.Instance.ActivateAfterAd(boostId);
   ```
   and refresh the Boost Shop UI (e.g. `Refresh()`).
3. Do **not** call `ActivateAfterAd` when the player taps the button; only call it after the ad has been watched and the reward callback fires. If the ad is skipped or fails, do not call `ActivateAfterAd`.

So: **yes**, the design supports “boost works after watching ads”; right now the game **simulates** that by activating immediately. Once you wire the ad reward callback to `ActivateAfterAd(boostId)`, Watch Ad boosts will work only after a real ad is watched.

---

## 4. Definition fixes made (vs earlier implementation)

- **Quantum Kitchen:** Doc says “Unlimited production”; implementation now uses **100×** production multiplier (was 10×) to better approximate “unlimited”.
- **Business Grant:** Was missing; added as **OneTimeCashGrant**: 200 Gold, instant cash = 10% of `TotalEarnings`, no duration.
- All other names, durations, costs, and multiplier types already matched the doc.
