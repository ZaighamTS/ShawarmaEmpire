# Implementation Status: What’s Done & What’s Next

**Reference:** `IMPLEMENTATION_PLAN_DOC_REQUIREMENTS.md`, `DOC_VS_PROJECT_GAP_ANALYSIS.md`, `EGG_INC_DOCUMENT_ANALYSIS.md`.

---

## All that has been done

### Phase 0 – Progress tracking
- **PlayerProgress** extended with: `TotalDeliveriesCompleted`, `TotalCateringOrdersCompleted`, `TotalUpgradesPurchased`, `TotalMoneySpentOnUpgrades`, `TotalPlayTimeSeconds`, `LastLoginUtc`; all persisted in save/load.
- **GameProgressEvents**: `OnDeliveryCompleted`, `OnCateringCompleted`, `OnCashEarned`, `OnUpgradePurchased`, `OnShawarmaProduced`, `OnEarningsThisSecondChecked`; helpers for recording and earnings-this-second buffer.
- **GameManager**: `AddCash` notifies cash earned; every second ticks earnings buffer and adds play time; SaveLoadManager sets `LastLoginUtc` after load.
- **DeliveryVan / CateringVan**: record delivery/catering and call `AddCash`.
- **Warehouse, Kitchen, Catering, Delivery**: record upgrade purchases via `GameProgressEvents.RecordUpgrade`.
- **ShawarmaSpawner**: records shawarma produced; `GameManager.OnShawarmaSpawned()` updates automatic earning rate.

### Phase 1 – Challenges
- **ChallengeManager**: 3 active challenges; types DeliverCount, EarnCash, ProduceCount, UpgradeCount; rewards Cash/Gold/ChefStar; daily/weekly refresh; custom definitions + tier scaling; save/load.
- **ChallengesPanelUI**: 3 slots (title, description, progress, claim); refresh and claim flow.

### Phase 2 – Achievements
- **AchievementManager**: condition types (TotalProduced, EarnInOneSecond, TotalEarnings, StorageCapacity, etc.); doc-style list; `maxEarningsInOneSecondEver`; IsUnlocked/IsClaimed; Claim grants reward; save/load.
- **AchievementRowUI** + **AchievementsPanelUI**: scroll list, progress, status colors, claim.

### Phase 3 – Statistics
- **StatisticsPanelUI**: reads PlayerProgress (production, delivery, earnings, upgrades, time); labels + values; FormatPlayTime / FormatLastLogin; show/hide panel.

### Phase 4 – Boost system
- **BoostManager**: definitions (Quantum Kitchen, Chef’s Special/Premium/Best, Production Prism, Large Prism, Boost/Epic Amplifier, Chef Star Beacon/2x, Business Grant); active boosts with UTC end time; GetEarningsMultiplier, GetProductionMultiplier, GetChefStarMultiplier; TryActivate (Gold), ActivateAfterAd / ActivateFree; save/load.
- **GameManager**: AddCash × earnings multiplier; UpdateAutomaticEarningRate × production multiplier; ResetPlayerStats × Chef Star multiplier.
- **BoostShopUI** + **BoostRowUI**: scroll list, cost (Gold / Watch Ad), activate; no re-activate while active; per-row timer; optional active-boosts list.
- **BOOST_DEFINITIONS_REFERENCE.md**, **BOOST_SHOP_PANEL_SETUP.md**.

### Phase 5 – Gift calendar
- **DailyLoginManager**: 7-day cycle; last claim date (UTC); rewards Cash/Gold/Boost; optional reset streak on missed day; save/load; auto-show panel when today not claimed.
- **GiftCalendarUI**: 7 slots (day, reward, today highlight, claimed overlay); claim button; correct today vs claimed logic after claim.
- Default rewards: Day 1–7 (1k coins, 1 gold, 5k coins, 5 gold, 10k coins, 5 gold, Chef’s Special).

### Phase 6 – Multiple shawarma types
- **ShawarmaTypes** (static): Classic $50, Spicy $60, Premium $75, Gourmet $100, Signature $150; unlock by TotalEarnings; GetBaseValue, IsUnlocked, GetUnlockDescription.
- **PlayerProgress**: `CurrentShawarmaTypeId`; save/load.
- **UpgradeCosts**: GetCurrentShawarmaBaseValue(); GetShawarmaValue / prestige helpers use current type.
- **ShawarmaTypesPanelUI** + **ShawarmaTypeRowUI**: list types, show **income per shawarma** (not base value), unlock text, Select.

### Phase 8 – Extra building upgrades (0–10)
- **ExtraBuildingLevelSystem**: net $/hr and upgrade cost per level (doc anchor points); GetNetIncomePerHour, GetUpgradeCost.
- **BuildingUnlockManager**: per-building `extraBuildingLevels[]`; GetExtraBuildingLevel, TryUpgradeExtraBuilding; save/load in BuildingsData.
- **ExtraBuildingFunctionality**: level-based net $/sec from type + level (uses `buildingIndex`); no reward/expense loops when using level system.
- **ExtraBuildingUpgradesPanelUI** + **ExtraBuildingUpgradeRowUI**: list purchased buildings; level, **$X/hr**, **$Y** or **MAX**; upgrade button; **Building.icon** + row **iconImage** for building image.

### Economy and save
- **Offline earnings rework**: cap by **stored shawarmas** (can’t earn more than selling all stored once); **30-minute** effective cap (was 1 hour); delivery size capped by inventory; **ECONOMY_ANALYSIS_AND_OFFLINE_FIX.md** (save path, JSON keys, income sources, why 200k happened, fixes).

---

## What is next

### Phase 7 – Research system (13 Common + 22 Epic)
- **Doc:** 13 tiers of Common Research (4 per tier, RP, cash cost, formulas); 22 Epic Research (Gold, permanent, no reset on prestige).
- **To do:** Data model (tiers, RP, definitions); player state (research levels); apply effects (value, capacity, delivery, boost duration, Chef Stars, etc.); Common UI (tree, buy with cash); Epic UI (panel, buy with Gold); persistence (Epic not reset on prestige).
- **Effort:** Large (3–4 weeks in plan).

### Other doc items (optional / later)
- **Calendar streak bonus** (e.g. 7-day streak reward) – Phase 5 is in; optional extra.
- **Ad integration** for “Watch Ad” boosts – currently placeholder (activate without ad).
- **Visual difference per shawarma type** – Phase 6 is in; optional art pass.
- Any **doc-specific research names and formulas** (Quality Ingredients, Premium Shawarmas, Epic list, etc.) when implementing Phase 7.

---

## Summary table

| Phase | Name                     | Status   |
|-------|--------------------------|----------|
| 0     | Progress tracking        | Done     |
| 1     | Challenges               | Done     |
| 2     | Achievements             | Done     |
| 3     | Statistics               | Done     |
| 4     | Boost system             | Done     |
| 5     | Gift calendar            | Done     |
| 6     | Multiple shawarma types  | Done     |
| 7     | Research (Common + Epic) | **Next** |
| 8     | Extra building levels    | Done     |
| —     | Offline earnings fix     | Done     |

**Next recommended:** Start **Phase 7 (Research system)** for the last major doc feature.
