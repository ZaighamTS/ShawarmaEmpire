# Issue to Code Mapping - Quick Reference

This document maps each issue from the gameplay analysis to specific code files and line numbers for quick fixes.

---

## 🚨 CRITICAL ISSUES

### Issue: "Economy is totally weird"
**Analysis Says:** "all income, spendings, prices etc. should be reworked from scratch"

**Code Locations:**
- `Assets/Scripts/Shawarma/ShawarmaSpawner.cs:85` - Earnings from production (SHOULD BE REMOVED)
- `Assets/Scripts/DeliveryVan System/DeliveryVan.cs:78-83` - Earnings from delivery (CORRECT)
- `Assets/Scripts/UpgradeSystem/UpgradeCosts.cs` - Cost formulas
- `Assets/Scripts/Managers/GameManager.cs:118-130` - Cash management

**Fix:** Remove production earnings, centralize economy formulas

---

### Issue: "Yellow marking - does not communicate what it's doing"
**Analysis Says:** Icon unclear, button too large

**Code Locations:**
- Check UI prefabs in `Assets/UI/`
- `Assets/Scripts/Managers/UIManager.cs` - UI element management
- Look for yellow/marking icons in scene files

**Fix:** Add tooltips, reduce size, add labels

---

### Issue: "Bronze icons - not communicating purpose"
**Analysis Says:** Icons overlap buildings, cryptic, some can be merged

**Code Locations:**
- `Assets/Scripts/Managers/UIManager.cs` - UI management
- Building manager scripts (KitchenManager, WarehouseManager, etc.)
- UI prefabs

**Fix:** Audit all bronze icons, merge redundant ones, add labels

---

### Issue: "Pink icons - value remains 0 or changes under unclear conditions"
**Analysis Says:** Unclear value changes

**Code Locations:**
- Check all UI scripts for pink icon references
- `Assets/Scripts/Managers/UIManager.cs`
- Building upgrade UIs

**Fix:** Add clear value display, show change conditions

---

### Issue: "Multiplier increases only from tapping"
**Analysis Says:** Should require ad watch or premium payment

**Code Locations:**
- `Assets/Scripts/Shawarma/ShawarmaSpawner.cs:107-121` - MultiplierFunctionality()
- `Assets/Scripts/Shawarma/ShawarmaProductionSystem.cs:64-80` - TapToCook()

**Fix:** Remove automatic multiplier from tapping, add ad/premium requirement

---

### Issue: "Red icon leads back to starting screen"
**Analysis Says:** Makes no sense

**Code Locations:**
- `Assets/Scripts/MenuHandler.cs`
- Main menu scene UI
- `Assets/Scripts/Managers/UIManager.cs` - Panel switching

**Fix:** Remove or change functionality

---

### Issue: "Green areas - small markings unclear"
**Analysis Says:** Player doesn't understand numbers

**Code Locations:**
- Building placement scripts
- `Assets/Scripts/Kitchen/KitchenManager.cs`
- `Assets/Scripts/DeliveryPoints(Warehouse)/WarehouseManager.cs`

**Fix:** Add tooltips, clear labels, tutorial explanation

---

### Issue: "Blue coins - left not clickable, purpose unclear"
**Analysis Says:** Two similar coin icons, one inactive

**Code Locations:**
- `Assets/Scripts/Managers/UIManager.cs:90-96` - Gold text
- UI prefabs for currency display
- Premium currency UI elements

**Fix:** Clarify purpose, make consistent, add tooltips

---

### Issue: "Buildings are empty (no seller inside)"
**Analysis Says:** Looks strange, no animations

**Code Locations:**
- Building prefabs in `Assets/Prefabs/`
- `Assets/Scripts/Kitchen/KitchenManager.cs:160-178` - PlaceNewKitchen()
- `Assets/Scripts/DeliveryPoints(Warehouse)/WarehouseManager.cs` - Building placement

**Fix:** Add NPC models to prefabs, add idle animations

---

### Issue: "Balloon purpose unclear"
**Analysis Says:** Can pop but don't know why

**Code Locations:**
- `Assets/Scripts/BaloonPop.cs` - Balloon pop logic
- Gives gold (should be premium-only or explained)

**Fix:** Add purpose explanation, or remove, or make premium-only

---

### Issue: "Cars/motorcycles not connected to game"
**Analysis Says:** Move independently, not connected to booths

**Code Locations:**
- Background vehicle scripts
- `Assets/Scripts/NPCMovement.cs` - May be related
- Vehicle spawners

**Fix:** Connect to delivery system or remove

---

### Issue: "New points built on edge, not celebrated"
**Analysis Says:** Easy to miss after building

**Code Locations:**
- `Assets/Scripts/Kitchen/KitchenManager.cs:160-178` - PlaceNewKitchen()
- `Assets/Scripts/DeliveryPoints(Warehouse)/WarehouseManager.cs` - Building placement
- `Assets/Scripts/DeliveryVan System/DeliveryManager.cs` - Delivery placement

**Fix:** Add celebration effects, camera focus, particle effects

---

### Issue: "Buildings not clickable, no info"
**Analysis Says:** Can't see upgrades, purpose, etc.

**Code Locations:**
- `Assets/Scripts/DeliveryPoints(Warehouse)/Warehouse.cs:102-112` - OnMouseDown() is commented out!
- `Assets/Scripts/Kitchen/Kitchen.cs` - Check for click handlers
- All building scripts

**Fix:** Enable click handlers, add info panels, show upgrade options

---

### Issue: "Only one car and motorcycle, same slow speed"
**Analysis Says:** Game area empty and boring

**Code Locations:**
- `Assets/Scripts/DeliveryVan System/DeliveryVanSpawner.cs:55-74` - SpawnVan()
- `Assets/Scripts/DeliveryVan System/DeliveryVan.cs:8` - speed variable
- Vehicle prefabs

**Fix:** Add vehicle variety, speed upgrades, more activity

---

### Issue: "No missions/tasks/goals"
**Analysis Says:** No clear game loop, no purpose

**Code Locations:**
- **MISSING:** No mission system exists
- Need to create mission system

**Fix:** Create mission system (high priority)

---

### Issue: "Extra buildings say 'increases auto earnings' but no details"
**Analysis Says:** No info on how much, how it works, which building

**Code Locations:**
- `Assets/Scripts/Kitchen/Kitchen.cs`
- `Assets/Scripts/DeliveryPoints(Warehouse)/Warehouse.cs`
- Building upgrade UIs

**Fix:** Add detailed tooltips, show exact values, explain mechanics

---

### Issue: "No reason to use premium currency"
**Analysis Says:** Can buy with common currency, no exclusivity

**Code Locations:**
- `Assets/Scripts/BaloonPop.cs:33` - Gives gold for free!
- `Assets/Scripts/Data/PlayerProgress.cs:58-66` - Gold property
- Premium purchase scripts

**Fix:** Remove free gold sources, create premium-only features

---

### Issue: "Why 'Common' upgrades?"
**Analysis Says:** Ten levels, can't buy with premium currency

**Code Locations:**
- `Assets/Scripts/Common/CommonAbilities.cs` - "Common" upgrades
- Upgrade UI scripts

**Fix:** Rename or clarify, add premium upgrade options

---

### Issue: "Update button confusing"
**Analysis Says:** Free update turns to cost update, unclear purpose

**Code Locations:**
- `Assets/Scripts/DeliveryPoints(Warehouse)/Warehouse.cs:47-85` - UpdateWarehouse()
- `Assets/Scripts/DeliveryPoints(Warehouse)/Updradable.cs` - Base upgrade class
- Upgrade UI panels

**Fix:** Clarify upgrade system, show clear progression

---

### Issue: "Update buildings raised in different areas"
**Analysis Says:** Look like visual updates only, no purpose

**Code Locations:**
- Building placement scripts
- `Assets/Scripts/ExtraBuildingsPlacement.cs`
- Building manager scripts

**Fix:** Connect to gameplay, show purpose, add functionality

---

### Issue: "No restaurant manager for auto-earning"
**Analysis Says:** Missing manager system

**Code Locations:**
- **MISSING:** Manager system not implemented
- GDD requires manager system for automation

**Fix:** Implement manager system (high priority)

---

### Issue: "No clear info on ad reward"
**Analysis Says:** Don't know what bonus is granted

**Code Locations:**
- `Assets/AdMobManager.cs:101-106` - GiveReward()
- `Assets/Scripts/Managers/UIManager.cs` - OnRewardedAdSuccess()
- Ad UI panels

**Fix:** Show clear reward preview before ad, display reward after

---

### Issue: "Gift box info weird grammar"
**Analysis Says:** Poorly written message

**Code Locations:**
- Gift/reward popup scripts
- `Assets/Scripts/Managers/UIManager.cs:115-120` - ShowInfoPopup()
- Reward UI

**Fix:** Fix grammar, clarify message

---

## 🎯 QUICK FIX PRIORITY

### Immediate (Can fix in 1-2 hours):
1. Remove earnings from production (`ShawarmaSpawner.cs:85`)
2. Fix grammar in gift box (`UIManager.cs:115-120`)
3. Enable building click handlers (`Warehouse.cs:102-112`)
4. Remove free gold from balloons (`BaloonPop.cs:33`)

### Short-term (1-2 days):
1. Add upgrade previews (all upgrade scripts)
2. Add capacity display to delivery points
3. Fix UI overlaps (main menu, icons)
4. Add audio settings menu

### Medium-term (1 week):
1. Implement tutorial system
2. Add mission system
3. Fix economy formulas
4. Add building celebrations

### Long-term (2+ weeks):
1. Implement manager system
2. Add NPC animations
3. Connect vehicles to gameplay
4. Full economy rebuild

---

**Quick Reference:** Use this document to quickly locate code for each reported issue.

