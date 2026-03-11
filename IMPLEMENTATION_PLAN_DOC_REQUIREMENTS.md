# Implementation Plan: Making the Game Match the Document

**Goal:** Implement all features from *Shawarma X EggInc Doc.pdf* in priority order.  
**Reference:** `DOC_VS_PROJECT_GAP_ANALYSIS.md` (gap analysis), `EGG_INC_DOCUMENT_ANALYSIS.md` (doc summary).

---

## What to Do First (Summary)

| Order | What to do first | Why | Est. effort |
|-------|-------------------|-----|-------------|
| **1** | **Progress tracking foundation** | Challenges, achievements, and statistics all need the same counters and events. Build once, use everywhere. | 2–3 days |
| **2** | **Challenges system** (3 active, daily/weekly/special/achievement) | Doc priority #1; drives engagement and goals. | 2–3 weeks |
| **3** | **Achievements** (doc table: rewards Cash/Gold/Chef Stars) | Uses same tracking as challenges; reward popups and persistence. | 1–2 weeks |
| **4** | **Statistics screen** | Doc priority #2; quick win; reuses progress data. | ~1 week |
| **5** | **Boost system** (shop + active boosts) | Doc priority #3; Gold already exists; monetization. | 1–2 weeks |
| **6** | **Gift calendar** (7-day, streak) | Retention; standalone. | ~2 weeks |
| **7** | **Multiple shawarma types** (Classic→Signature) | Doc #4; progression depth. | 2–3 weeks |
| **8** | **Research system** (13 Common + 22 Epic) | Large feature; do after engagement loop is solid. | 3–4 weeks |
| **9** | **Extra building upgrades** (levels 0–10 per building) | Doc says “implement later”; income scaling per building. | 2–3 weeks |

**First concrete step:** Implement the **progress tracking foundation**, then **Challenges + Achievements** together (they share data and UI patterns).

---

## Phase 0: Progress Tracking Foundation (Do This First)

**Purpose:** One place that tracks everything needed for challenges, achievements, and statistics. Add events so systems can react without coupling.

### 0.1 Add lifetime/run counters (in `PlayerProgress` or new `ProgressTracker`)

- [ ] **Total deliveries completed** (lifetime)
- [ ] **Total catering orders completed** (lifetime)
- [ ] **Total shawarmas produced** (already `ShwarmaCount` – confirm it’s lifetime)
- [ ] **Total cash earned** (already `TotalEarnings`)
- [ ] **Total upgrades purchased** (count of upgrade purchases)
- [ ] **Total money spent on upgrades** (optional, for stats)
- [ ] **Play time (seconds)** (track in GameManager, save with progress)
- [ ] **Last login UTC** (for gift calendar and “earn in one second” windows)

### 0.2 Add “this second” / “this session” tracking (for achievements like “Earn $500 in one second”)

- [ ] In `GameManager`: when `AddCash(value)` is called, add `value` to a “earnings this second” buffer.
- [ ] Each second (or fixed tick), check buffer; if ≥ $500 / $1M / $5M / $10M, mark achievement and clear buffer. Then reset buffer for next second.

### 0.3 Fire events when meaningful actions happen

So Challenge/Achievement/Stats can subscribe without touching every system:

- [ ] **Delivery completed:** In `DeliveryVan.cs` where you call `AddCash(totalRewards)`, add an event, e.g. `OnDeliveryCompleted?.Invoke(shawarmaCount, totalRewards)`.
- [ ] **Catering order completed:** In `CateringVan.cs` where you call `AddCash(totalRewards)`, add `OnCateringCompleted?.Invoke(quantity, totalRewards)`.
- [ ] **Cash earned:** In `GameManager.AddCash`, optionally fire `OnCashEarned?.Invoke(value)` (for “earn X in one second” and stats).
- [ ] **Upgrade purchased:** From existing `onDeliveryUpgraded`, `onWarehouseUpgraded`, etc., either unify into one `OnUpgradePurchased?(type, level)` or have a small helper that increments “total upgrades” and fires one event.
- [ ] **Shawarma produced:** Already have `onShawarmaCreated`; ensure one place subscribes and updates lifetime produced count if not already.

### 0.4 Persist new fields

- [ ] Add new counters to `PlayerProgress` (or `ProgressTracker`) and to save/load in `SaveLoadManager` / `ISaveable`.

**Exit criteria:** All data needed for “Deliver X”, “Earn $X”, “Produce X”, “Upgrade X”, and “Earn $X in one second” is tracked and persisted; events fire on delivery, catering, cash, upgrade.

---

## Phase 1: Challenges System (Doc Priority #1)

**Doc:** 3 active challenges at a time; types: Daily, Weekly, Special, Achievement. Goals: Deliver X, Earn $X, Produce X, Upgrade X. Rewards: Gold (5–200), Cash ($500–$50K), Prestige bonuses.

### 1.1 Data model

- [ ] Define `ChallengeType` (Daily, Weekly, Special, Achievement).
- [ ] Define `ChallengeGoal` (DeliverCount, EarnCash, ProduceCount, UpgradeCount, etc.) and target value.
- [ ] Define `ChallengeReward` (Cash, Gold, or ChefStars).
- [ ] Challenge definition: id, type, goal, reward, expiry (for daily/weekly).
- [ ] Active challenge state: definitionId, currentProgress, completed, claimed.

### 1.2 Challenge definitions and generation

- [ ] Create a list or scriptable objects of challenge definitions (or generate daily/weekly from seeds).
- [ ] Logic to “roll” 3 challenges (e.g. 1 daily, 1 weekly, 1 special or achievement) and assign targets (Deliver 50, Earn $10,000, etc.) with rewards from doc ranges.

### 1.3 Progress and completion

- [ ] Subscribe to Phase 0 events: on delivery → update “Deliver X” challenges; on AddCash → update “Earn $X”; on shawarma produced → update “Produce X”; on upgrade → update “Upgrade X”.
- [ ] When progress ≥ target, mark challenge complete; allow “claim” reward (add Cash/Gold/ChefStars via GameManager).

### 1.4 UI

- [ ] Challenges panel: show 3 slots (icon, description, progress bar, “Claim” when done).
- [ ] Hook into existing panel flow (e.g. UIManager) and add button to open challenges.

### 1.5 Persistence and refresh

- [ ] Save active challenges and progress in `PlayerProgress` (or dedicated save).
- [ ] Daily/Weekly refresh: on login, check last refresh time; if new day/week, generate new daily/weekly challenges.

**Exit criteria:** Player can see 3 challenges, progress updates from gameplay, and claim Cash/Gold/ChefStar rewards.

---

## Phase 2: Achievements (Doc Table)

**Doc:** Full achievement list with rewards (Cash, Gold, Chef Stars). Examples: Two Hundred ($1.5K), Early Tech ($30K), Get Going ($150K), Growing Family ($500K), Shawarma Up (96 Gold), Rack It In (48 Gold), Get Rich Quick (1 Chef Star), etc.

### 2.1 Data model

- [ ] Achievement definition: id, title, description, condition type (e.g. ProduceCount ≥ 200, ResearchCount ≥ 30, EarnInOneSecond ≥ 500, StorageCapacity ≥ 4200, …), reward (Cash / Gold / ChefStars), reward value.
- [ ] Player state: list of achievement ids that are unlocked and whether reward was claimed.

### 2.2 Implement conditions (map doc table to code)

- [ ] Map each achievement to a condition: e.g. “Two Hundred” = total produced ≥ 200; “Get Going” = earn in one second ≥ $500; “Growing Family” = total storage capacity ≥ 4,200; “Supply Chain” = delivery capacity ≥ 250/min; “Research 30” / “Research 150” etc. (stub to false until Research exists).
- [ ] Use Phase 0 counters and “earn in one second” logic; add helpers where needed (e.g. “current total storage capacity”, “current delivery capacity per minute”).

### 2.3 Check and grant

- [ ] After each relevant event (or once per second for “earn in one second”), evaluate all unclaimed achievements; if condition met, mark unlocked and show popup; on “claim”, grant reward and mark claimed.
- [ ] Persist unlocked/claimed in save.

### 2.4 UI

- [ ] Achievements panel: list or grid of achievements (locked vs unlocked vs claimed), progress if applicable, reward.
- [ ] Optional: “New achievement” toast when unlocked.

**Exit criteria:** All doc achievements that don’t require Research are implementable; rewards grant correctly; list is persisted and visible in UI.

---

## Phase 3: Statistics Screen (Doc Priority #2)

**Doc:** Production (total produced, rate/hr), delivery (total deliveries, average size), earnings (total, per hour, best hour), upgrades (total upgrades, money spent), time (playtime, offline time).

### 3.1 Data to show

- [ ] **Production:** Total produced (ShwarmaCount or dedicated counter), current rate per hour (from production system).
- [ ] **Delivery:** Total deliveries (Phase 0), average delivery size (total delivered / total deliveries, or running average).
- [ ] **Earnings:** Total earned (TotalEarnings), earnings per hour (session or lifetime – define which), “best hour” (max earnings in any 1-hour window – optional, needs rolling window or session-only).
- [ ] **Upgrades:** Total upgrade count, total money spent on upgrades (if you added spending in Phase 0).
- [ ] **Time:** Total playtime, last offline time (for offline earnings display).

### 3.2 UI

- [ ] New Statistics panel (scene + UIManager integration).
- [ ] Sections for each category; fill from PlayerProgress and Phase 0 tracker.

**Exit criteria:** One screen showing all above stats; no new core systems, just aggregation and UI.

---

## Phase 4: Boost System (Doc Priority #3)

**Doc:** 11 boosts (e.g. Quantum Kitchen, Chef’s Special/Premium/Best Recipe, Production Prism, Large Production Prism, Boost Amplifier, Epic Boost Amplifier, Chef Star Beacon, Business Grant, Chef Star 2x); some “Watch Ad”, some Gold cost.

### 4.1 Data model

- [ ] Boost definition: id, name, effect type (ProductionMult, EarningsMult, ChefStarMult, etc.), duration (seconds), multiplier or effect value, cost (Gold amount or “WatchAd”).
- [ ] Active boost state: boostId, endTime (UTC or Time.time + duration).

### 4.2 Apply multipliers in game

- [ ] **Earnings multiplier:** In `GameManager.AddCash` (and wherever delivery/catering add cash), multiply by product of all active “EarningsMult” boosts (e.g. 2×, 10×, 50×). Stacking: doc says “Boost Amplifier” 2× all boosts – define whether that doubles the effect or the duration.
- [ ] **Production multiplier:** In production (e.g. `ShawarmaProductionSystem` / `ShawarmaSpawner`), multiply rate by active production boosts (e.g. 10× for 10 min).
- [ ] **Chef Star multiplier:** When calculating “Chef Stars earned on prestige”, apply “Chef Star Beacon” / “Chef Star 2x” if active at prestige time (or store “next prestige uses 5× stars” if easier).

### 4.3 Boost shop UI

- [ ] Panel listing 11 boosts: name, effect, cost (Gold or “Watch Ad”), “Activate” button.
- [ ] If cost is Gold: check `PlayerProgress.Gold`, deduct, add to active boosts. If “Watch Ad”: trigger ad, on reward add to active boosts.
- [ ] Show active boosts with remaining time; when time expires, remove from active list.

### 4.4 Duration and stacking

- [ ] Use a list of (boostId, endTime); each frame or every second, remove expired. When applying earnings/production, iterate active boosts and combine multipliers (multiplicative stacking as per doc).

**Exit criteria:** Player can buy boosts with Gold or ad; active boosts multiply earnings and/or production for duration; UI shows shop and active timers.

---

## Phase 5: Gift Calendar (Doc Priority #5)

**Doc:** 7-day cycle, rewards (Gold 5–50, Cash $1K–$10K, temporary boosts), streak bonus, show on login.

### 5.1 Data model

- [ ] Last login date (date only); consecutive login days (streak).
- [ ] Day index in cycle (1–7); which day was last claimed.

### 5.2 On game start

- [ ] Load last login date. If it’s a new calendar day: if same as yesterday, increment streak; else reset streak to 1. If missed a day, doc doesn’t say – either reset streak or allow one miss (your design).
- [ ] If day 1–7 not yet claimed today, show calendar popup; on claim, grant reward (Gold/Cash/boost), advance day index, save.

### 5.3 UI

- [ ] Calendar panel: 7 days, current day highlighted, reward per day, “Claim” for today. Optional: streak counter and bonus for 7-day streak.

**Exit criteria:** Player gets one reward per day for 7 days; streak is tracked; popup on login when there’s a claim available.

---

## Phase 6: Multiple Shawarma Types (Doc Priority #4)

**Doc:** Classic ($50) → Spicy ($60) → Premium ($75) → Gourmet ($100) → Signature ($150); unlock by milestones or Chef Stars.

### 6.1 Data model

- [ ] Enum or id: Classic, Spicy, Premium, Gourmet, Signature.
- [ ] Base value per type (50, 60, 75, 100, 150).
- [ ] Unlock condition per type (e.g. Classic from start; Spicy at $10K total earned; Premium at 2 Chef Stars; …).
- [ ] Current type in `PlayerProgress`.

### 6.2 Economy integration

- [ ] In `UpgradeCosts.GetShawarmaValue` (or equivalent), use current type’s base value instead of single `shwarmaBaseValue`; keep material/prestige/quality on top.
- [ ] When unlocking a new type, allow selection (or auto-upgrade); persist selection.

### 6.3 UI

- [ ] Screen or panel to see types, unlock conditions, and “Select” for unlocked types.
- [ ] Optional: visual difference per type (model/color) as in doc.

**Exit criteria:** At least 5 types with correct base values and unlock rules; value in delivery/catering uses selected type.

---

## Phase 7: Research System (13 Common + 22 Epic)

**Doc:** 13 tiers of Common Research (4 per tier, RP requirements, cash cost, formulas); 22 Epic Research (Gold, permanent, no reset on prestige).

### 7.1 Common Research

- [ ] Define all researches: tier, RP required to unlock tier, name, effect (e.g. +10% production, +25% value), base cost, cost formula (Base×level^exp), max level.
- [ ] Player state: researchId → current level. “Research points” (RP) = function of something (e.g. total spent on research, or tier completion) to unlock next tier.
- [ ] Apply effects: production rate, shawarma value, capacity, delivery speed, etc. in `UpgradeCosts` and production/delivery code.
- [ ] UI: research tree (tiers 1–13), buy with cash, show RP and tier locks.

### 7.2 Epic Research

- [ ] Define 22 Epic researches: name, effect (e.g. +5% capacity, +30 min boost duration), Gold cost formula, max level. Persist in separate structure (not reset on prestige).
- [ ] Player state: epicResearchId → level (saved in permanent progress).
- [ ] Apply effects in same places as Common (capacity, boost duration, Chef Star bonus, offline production, etc.).
- [ ] UI: Epic Research panel, buy with Gold.

### 7.3 Integration

- [ ] Wire value formula to Quality Ingredients, Premium Shawarmas, etc.; capacity to Quantum Storage Expansion; boost duration to Boost Duration Extension; Chef Stars to Chef Star Power; etc. Align with doc formulas and simulator where possible.

**Exit criteria:** All 13 Common and 22 Epic in data; effects applied; purchasable with Cash/Gold; Epic persists across prestige.

---

## Phase 8: Extra Building Upgrades (Levels 0–10)

**Doc:** Each extra building has levels 0–10; income and total cost per level from doc table (e.g. Juice Point L0 +$720/hr → L10 +$5,112/hr, total cost $47,020).

### 8.1 Data model

- [ ] Per-building instance: type + **level** (0–10). Level 0 = base income/expense from doc; each level has upgrade cost and new $/hr.
- [ ] Store per-placed building: type, level (or one level per building type if you don’t support multiple of same type).

### 8.2 Economy

- [ ] Income per building = f(type, level) using doc table (reward/expense per building, then net $/hr at level).
- [ ] Upgrade cost for building type from level L to L+1 from doc (or formula). Pay with cash; increment level.

### 8.3 UI

- [ ] In building UI: show level and “Upgrade” with cost; on purchase, deduct cash and level up.

**Exit criteria:** Each extra building has level 0–10; upgrading increases net $/hr per doc; costs match doc table.

---

## Dependency Overview

```
Phase 0 (Tracking) ──┬── Phase 1 (Challenges)
                     ├── Phase 2 (Achievements)
                     └── Phase 3 (Statistics)

Phase 4 (Boosts) ──────── Uses Gold (existing)

Phase 5 (Calendar) ───── Standalone; needs login date

Phase 6 (Shawarma types) ─ Standalone; touches value formula

Phase 7 (Research) ────── Touches value, capacity, boosts; do after 4

Phase 8 (Building levels) ─ Touches extra building income only
```

---

## Suggested Sprint Order

1. **Sprint 1:** Phase 0 (tracking + events) + start Phase 1 (challenge data model + progress hooks).
2. **Sprint 2:** Finish Phase 1 (challenge UI, claim, daily/weekly refresh) + Phase 2 (achievements: conditions + rewards + UI).
3. **Sprint 3:** Phase 3 (Statistics screen).
4. **Sprint 4:** Phase 4 (Boost system: data, multipliers, shop, ads).
5. **Sprint 5:** Phase 5 (Gift calendar) and Phase 6 (Multiple shawarma types) in parallel if resources allow.
6. **Sprint 6–7:** Phase 7 (Research: Common then Epic).
7. **Sprint 8:** Phase 8 (Extra building levels).

---

## What We Need to Do First (Concrete)

1. **Implement Phase 0** in the Unity project:
   - Add a small **ProgressTracker** (or extend **PlayerProgress**) with: total deliveries, total catering orders, upgrade count, play time, last login.
   - Add “earnings this second” buffer and check each second for “Earn $X in one second” achievements.
   - In **DeliveryVan.cs** and **CateringVan.cs**, fire `OnDeliveryCompleted` / `OnCateringCompleted` with count and cash.
   - In **GameManager.AddCash**, update TotalEarnings (already) and “earnings this second”; fire `OnCashEarned` if you use it.
   - Persist all new fields in save/load.

2. **Then implement Phases 1 and 2** (Challenges + Achievements) using that data and those events, so the game has clear goals and rewards as in the doc.

After that, Statistics (Phase 3) and Boosts (Phase 4) give quick visible wins and monetization; then Calendar, Shawarma types, Research, and Building upgrades complete the doc alignment.
