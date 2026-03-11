# Milestone 1 Implementation Plan - Complete Timeline

**Project:** Sharwama Dash  
**Created:** December 2024  
**Goal:** Make the game playable, understandable, and defensible for Milestone 1  
**Timeline:** 4 weeks (74-102 hours) for Milestone 1

---

## 🎯 Milestone 1 Goal

Make the game playable, understandable, and defensible for Milestone 1 review.

**Status After 1 Month:**
- ✅ Core gameplay is functional
- ✅ Milestone 1 is defensible
- ⚠️ NOT complete - still needs 2-3 additional weeks for full completion

---

## 📅 4-WEEK TIMELINE (PRIORITY-BASED)

### **🔴 WEEK 1 — CORE LOOP MUST WORK (CRITICAL)**
**Goal:** Player understands why they tap and where money comes from  
**Time:** 20-28 hours

#### Tasks:

1. **Fix Core Loop Wiring** - 6-8h
   - Ensure Produce → Store → Deliver → Transport → Earn → Upgrade flow works
   - Storage caps stop production
   - Delivery drains storage
   - Vehicles convert delivery → money
   - Remove fake/decorative logic

2. **Fix Economy Consistency** - 4-6h
   - Single source of truth for: Production rate, Delivery capacity, Vehicle throughput, Earnings formula
   - Numbers must NEVER change "mysteriously"
   - Centralize all economy calculations

3. **Clickable Buildings (Info Panels)** - 3-4h
   - Tap building → shows: What it does, Current throughput, Next upgrade effect, Cost
   - Enhance existing `ShowWarehouseInfo()` method

4. **Remove/Disable Broken Systems** - 2-3h
   - Balloon mechanic (if no purpose yet)
   - Decorative cars not tied to transport
   - Remove "free update → paid update" confusion

5. **Economy Rebuild (Light)** - 5-7h
   - Implement base production values
   - Soft exponential upgrade costs
   - Clear per-upgrade benefits (+X/sec, +capacity)
   - No deep balancing yet—just sanity

**Output by end of Week 1:**
- ✅ Game loop works
- ✅ Player can earn money logically
- ✅ No economy-breaking confusion

---

### **🟠 WEEK 2 — ECONOMY & PROGRESSION STRUCTURE**
**Goal:** Game no longer feels "weird" or pointless  
**Time:** 18-24 hours

#### Tasks:

1. **Delivery Points as Bottlenecks** - 4-6h
   - Show: Capacity, Occupancy %, Throughput
   - Upgrades clearly increase throughput

2. **Vehicles Become Gameplay-Relevant** - 5-7h
   - Each vehicle has: Capacity per trip, Trips per minute
   - Earnings depend on vehicle throughput
   - Make vehicles matter in gameplay

3. **Premium Currency Separation** - 3-4h
   - Soft currency → progression
   - Premium currency → boosts, skips, convenience
   - Remove ability to buy premium using soft currency

4. **Store Build & Store Upgrade Popups** - 4-6h
   - Create Store Build popup UI with building list
   - Create Store Upgrade popup UI with upgrade options
   - Link store button to show appropriate popup

5. **Store Capacity Display Enhancement** - 1-2h
   - Show capacity as "current/max" format (e.g., "150/250")

6. **Warning Icon Click Explanation** - 2-3h
   - Add click handler to warning icon
   - Show detailed explanation of warning reason

**Output by end of Week 2:**
- ✅ Economy is understandable
- ✅ Bottlenecks are visible
- ✅ Vehicles finally matter
- ✅ Monetization logic is defensible

---

### **🟡 WEEK 3 — ONBOARDING, GOALS & FEEDBACK**
**Goal:** Player knows what to do next at all times  
**Time:** 20-28 hours

#### Tasks:

1. **Tutorial Implementation (Must-Have)** - 8-12h
   - Step-by-step: Produce, Fill storage, Activate delivery, Transport → earn, First upgrade, Unlock automation
   - Block UI where necessary

2. **Basic Missions System** - 4-6h
   - Tutorial missions
   - 3-5 simple progression goals: Produce X, Sell X, Upgrade Y

3. **Feedback & Celebration** - 4-6h
   - Visual feedback for: Earnings, Upgrades, New building unlocks
   - Simple animations (no polish obsession)

4. **Audio Settings Menu** - 2-3h
   - Music volume, SFX volume, Mute toggle

5. **Menu Options Popups** - 2-3h
   - Wire menu buttons to open respective popups (Settings, Info, Help, etc.)

**Output by end of Week 3:**
- ✅ New player is no longer lost
- ✅ Clear short-term goals exist
- ✅ Game feels alive, not static

---

### **🟢 WEEK 4 — STABILIZATION & MILESTONE-READY**
**Goal:** Make it presentable, stable, and review-safe  
**Time:** 16-22 hours

#### Tasks:

1. **Bug Fixing & Cleanup** - 5-7h
   - Economy edge cases
   - Incorrect UI values
   - Broken upgrade states

2. **Text & Copy Fixes** - 3-4h
   - All popups rewritten clearly
   - Ads show exact rewards
   - Remove poor grammar everywhere

3. **Light Balancing Pass** - 4-6h
   - Early game pacing
   - Ensure player can't soft-lock easily
   - Prevent infinite money exploits

4. **Milestone 1 Validation** - 4-5h
   - Does the game: Have a clear loop? Teach itself? Show progression? Make sense economically?

**Output by end of Month:**
- ✅ Milestone 1 is defensible
- ✅ Core gameplay is functional
- ✅ Review feedback addressed at a high level

---

## ⏱️ WHAT WILL STILL BE MISSING AFTER 1 MONTH

After 1 month, Milestone 1 is acceptable but NOT complete.

**Still incomplete:**
- Deep economy balancing (numbers tuning)
- Offline earnings system (proper caps + boosts)
- Full manager system depth
- Polished animations & world liveliness
- Telemetry / analytics
- Long-term meta (prestige, regions, events)

---

## ⏳ EXTRA TIME REQUIRED AFTER 1 MONTH

To fully complete Milestone 1 to a strong, professional level:

**➕ 2-3 additional weeks**

**Breakdown:**
- Economy fine-tuning: 5-7 days
- Offline earnings + ads clarity: 3-5 days
- Manager depth & automation polish: 3-4 days
- World liveliness & visual feedback polish: 3-5 days
- Final QA + iteration: 4-6 days

---

## 📊 Complete Task List with Time Estimates

### **WEEK 1 - Core Loop Must Work (20-28 hours)**

| # | Task | Time | Priority |
|---|------|------|----------|
| 1 | Fix Core Loop Wiring | 6-8h | 🔴 Critical |
| 2 | Fix Economy Consistency | 4-6h | 🔴 Critical |
| 3 | Clickable Buildings (Info Panels) | 3-4h | 🔴 Critical |
| 4 | Remove/Disable Broken Systems | 2-3h | 🔴 Critical |
| 5 | Economy Rebuild (Light) | 5-7h | 🔴 Critical |

### **WEEK 2 - Economy & Progression (18-24 hours)**

| # | Task | Time | Priority |
|---|------|------|----------|
| 6 | Delivery Points as Bottlenecks | 4-6h | 🟠 High |
| 7 | Vehicles Become Gameplay-Relevant | 5-7h | 🟠 High |
| 8 | Premium Currency Separation | 3-4h | 🟠 High |
| 9 | Store Build & Store Upgrade Popups | 4-6h | 🟠 High |
| 10 | Store Capacity Display Enhancement | 1-2h | 🟡 Medium |
| 11 | Warning Icon Click Explanation | 2-3h | 🟡 Medium |

### **WEEK 3 - Onboarding & Feedback (20-28 hours)**

| # | Task | Time | Priority |
|---|------|------|----------|
| 12 | Tutorial Implementation (Must-Have) | 8-12h | 🟠 High |
| 13 | Basic Missions System | 4-6h | 🟠 High |
| 14 | Feedback & Celebration | 4-6h | 🟡 Medium |
| 15 | Audio Settings Menu | 2-3h | 🟡 Medium |
| 16 | Menu Options Popups | 2-3h | 🟡 Medium |

### **WEEK 4 - Stabilization (16-22 hours)**

| # | Task | Time | Priority |
|---|------|------|----------|
| 17 | Bug Fixing & Cleanup | 5-7h | 🟠 High |
| 18 | Text & Copy Fixes | 3-4h | 🟡 Medium |
| 19 | Light Balancing Pass | 4-6h | 🟠 High |
| 20 | Milestone 1 Validation | 4-5h | 🟠 High |

---

## 📋 Additional Features (Post-Milestone 1)

These features are NOT required for Milestone 1 but should be implemented afterward:

| # | Task | Time | Priority |
|---|------|------|----------|
| 21 | Legal Rewards System | 4-6h | 🟡 Medium |
| 22 | Rebirth Option Modification | 3-4h | 🟡 Medium |
| 23 | Epic Upgrades - Hold to Generate | 6-8h | 🟡 Medium |
| 24 | Upgrade Required Warning (200-300 Shawarmas) | 2-3h | 🟢 Low |
| 25 | Challenges System | 8-12h | 🟡 Medium |
| 26 | Challenge Rewards Integration | 2-3h | 🟡 Medium |
| 27 | Tap Button Resize Option | 2-3h | 🟢 Low |
| 28 | Tap Button Works with Popups | 1-2h | 🟡 Medium |
| 29 | Remove IAP Shop | 1h | 🟢 Low |

**Total Additional:** 29-45 hours

---

## 📈 Timeline Summary

| Week | Phase | Tasks | Time Range | Status |
|------|-------|-------|------------|--------|
| **Week 1** | Core Loop & Economy | 5 tasks | 20-28 hours | 🔴 Critical |
| **Week 2** | Economy & Progression | 6 tasks | 18-24 hours | 🟠 High |
| **Week 3** | Onboarding & Feedback | 5 tasks | 20-28 hours | 🟠 High |
| **Week 4** | Stabilization | 4 tasks | 16-22 hours | 🟠 High |
| **Additional** | Extra Features | 9 tasks | 29-45 hours | 🟡 Post-M1 |
| **TOTAL** | **All Phases** | **29 tasks** | **103-147 hours** | |

---

## 🎯 Milestone 1 Focus (Weeks 1-4)

- **Total tasks:** 20
- **Total time:** 74-102 hours
- **Timeline:** 4 weeks (full-time) or 6-8 weeks (part-time)

---

## ✅ Completion Checklist

### Week 1 - Core Loop
- [ ] Fix Core Loop Wiring
- [ ] Fix Economy Consistency
- [ ] Clickable Buildings (Info Panels)
- [ ] Remove/Disable Broken Systems
- [ ] Economy Rebuild (Light)

### Week 2 - Economy & Progression
- [ ] Delivery Points as Bottlenecks
- [ ] Vehicles Become Gameplay-Relevant
- [ ] Premium Currency Separation
- [ ] Store Build & Store Upgrade Popups
- [ ] Store Capacity Display Enhancement
- [ ] Warning Icon Click Explanation

### Week 3 - Onboarding & Feedback
- [ ] Tutorial Implementation (Must-Have)
- [ ] Basic Missions System
- [ ] Feedback & Celebration
- [ ] Audio Settings Menu
- [ ] Menu Options Popups

### Week 4 - Stabilization
- [ ] Bug Fixing & Cleanup
- [ ] Text & Copy Fixes
- [ ] Light Balancing Pass
- [ ] Milestone 1 Validation

---

## 📝 Key Files Reference

### Core Systems
- `Assets/Scripts/Managers/GameManager.cs` - Main game manager
- `Assets/Scripts/Managers/UIManager.cs` - UI management
- `Assets/Scripts/Shawarma/ShawarmaSpawner.cs` - Production system
- `Assets/Scripts/DeliveryPoints(Warehouse)/Warehouse.cs` - Storage system
- `Assets/Scripts/DeliveryVan System/DeliveryVan.cs` - Delivery system
- `Assets/Scripts/UpgradeSystem/UpgradeCosts.cs` - Economy calculations

### Data Systems
- `Assets/Scripts/Data/PlayerProgress.cs` - Player data
- `Assets/Scripts/Data/SaveLoadManager.cs` - Save/load system

---

## 🔍 Implementation Notes

### Dependencies
- Week 2 tasks depend on Week 1 core loop fixes
- Tutorial (Week 3) depends on core loop working
- Missions system depends on tutorial

### Testing Requirements
- Test core loop flow end-to-end
- Verify economy calculations are consistent
- Test all popups and UI interactions
- Verify tutorial progression
- Test edge cases (full storage, no money, etc.)

### Risk Factors
- Economy consistency may require significant refactoring
- Tutorial implementation complexity
- Vehicle system integration with economy

---

**Last Updated:** December 2024  
**Next Review:** After Week 1 completion  
**Status:** Planning Phase
