# Boost Shop Panel Setup (BoostShopUI) – Detailed Guide

This guide explains how to set up the Boost Shop panel in Unity **after** you have added a Boost Shop panel and attached the **BoostShopUI** component to it.

---

## 1. Prerequisites

- **BoostManager** must exist in the scene (e.g. on a persistent “Managers” GameObject). The Boost Shop UI reads boost definitions and active boosts from `BoostManager.Instance`. If BoostManager is missing, the panel will do nothing.
- **GameManager** and **SaveLoadManager** should be present (BoostManager registers with SaveLoadManager and uses GameManager for Gold).

---

## 2. Panel Hierarchy Overview

A typical structure:

```
BoostShopPanel                    ← GameObject with BoostShopUI
├── (optional) Background / frame
├── Title ("Boost Shop")
├── ActiveBoostsSection           ← Parent for "currently active" timers
│   └── ActiveBoostTemplate       ← One TMP_Text (template, can be disabled)
├── ScrollView
│   └── Viewport
│       └── Content               ← contentParent: rows go here
│           └── (rows created at runtime from rowPrefab)
└── CloseButton
```

- **BoostShopUI** is on the root of this panel (e.g. **BoostShopPanel**).
- The panel can start **disabled** (GameObject unchecked); you show it with `SetPanelVisible(true)` or `TogglePanel()`.

---

## 3. BoostShopUI Inspector Fields

On the **BoostShopUI** component you must assign:

### 3.1 Available boosts (scroll list)

| Field | Type | Purpose |
|-------|------|--------|
| **Content Parent** | `Transform` | The scroll **Content** (under Scroll View → Viewport). All boost rows are instantiated as children of this transform. Layout components (Vertical Layout Group, Content Size Fitter) are usually on this same object. |
| **Row Prefab** | `GameObject` | Prefab for **one** boost row. Must have (or receive) a **BoostRowUI** component; see section 4. |

- At runtime, **BoostShopUI** instantiates one instance of **Row Prefab** per boost in `BoostManager.Definitions` (10 by default) and parents them under **Content Parent**.
- If **Content Parent** or **Row Prefab** is missing, the “Available boosts” list will not be created (no rows).

### 3.2 Active boosts (timer list)

| Field | Type | Purpose |
|-------|------|--------|
| **Active Boosts Parent** | `Transform` | Parent for the “Active boosts” lines. Each line is one active boost and its remaining time (e.g. `Chef's Special Recipe: 5m 30s`). |
| **Active Boost Template** | `TMP_Text` | A **TextMeshPro - Text** component used as template. For each currently active boost, the script clones this GameObject, sets the text, and parents it under **Active Boosts Parent**. Can be disabled in the prefab/scene so it doesn’t show until used. |
| **Active Boosts Refresh Interval** | `float` | How often (in seconds) the remaining time is updated. Default `1` is fine. |

- If **Active Boosts Parent** or **Active Boost Template** is left empty, the panel still works; only the “active boosts” section will not show.
- **Active Boost Template** must be a **child** of **Active Boosts Parent** (so clones are siblings of the template). The script uses `activeBoostsParent.childCount` and `GetChild(index)`.

---

## 4. Building the Row Prefab (BoostRowUI)

The **Row Prefab** is one row in the scroll list: boost name, description, cost, and an Activate button.

### 4.1 Add BoostRowUI to the prefab

- Create a GameObject (e.g. under a temporary “Prefabs” folder or in the same scene).
- Add the **BoostRowUI** component to it (Scripts → Boosts → Boost Row UI).

### 4.2 Child UI elements

Add child UI elements and wire them in **BoostRowUI**:

| BoostRowUI field | Recommended setup |
|------------------|-------------------|
| **Title Text** | `TMP_Text` for the boost name (e.g. “Chef's Special Recipe”). |
| **Description Text** | `TMP_Text` for the short description (e.g. “3x earnings for 20min”). |
| **Cost Text** | `TMP_Text` for cost: either “X Gold” or “Watch Ad”. |
| **Activate Button** | `Button` that the player presses to buy / watch ad and activate the boost. |

- **BoostRowUI** does not create these; you add RectTransforms + TextMeshPro texts and a Button, then assign them in the Inspector.
- Layout: e.g. Horizontal Layout or a simple layout with title on top, description below, cost and button on the right.

### 4.3 Save as prefab

- Drag this GameObject into the Project window to create a **Prefab**.
- Assign this prefab to **BoostShopUI → Row Prefab**. You can remove the instance from the scene if the list is fully driven by the prefab.

---

## 5. Active Boosts Section in Detail

### 5.1 Purpose

- Shows each **currently active** boost and its **remaining time** (e.g. `Production Prism: 8m 45s`).
- Refreshed every **Active Boosts Refresh Interval** seconds.

### 5.2 Hierarchy

- **Active Boosts Parent**: empty GameObject (or with a layout group). All “active boost” lines are its children.
- **Active Boost Template**: one **TextMeshPro - Text (UI)** under that parent. This object is **cloned** for each active boost; the script never uses the template’s own text for display until it’s cloned. You can disable the template so the first visible line is a clone.

### 5.3 How cloning works

- When there are `N` active boosts (with non-expired time), the script ensures **Active Boosts Parent** has at least `N` children.
- If needed, it instantiates **Active Boost Template.gameObject** and parents under **Active Boosts Parent**.
- For each of the first `N` children it sets the **TMP_Text.text** to e.g. `"Chef's Special Recipe: 5m 30s"`.
- Any extra children (e.g. from a previous frame when there were more active boosts) are **disabled**, not destroyed.

So: **Active Boost Template** must be a **direct child** of **Active Boosts Parent** so that the first child can be the template and the rest clones (or you can make the template disabled and rely on clones only; the script still creates clones with `Instantiate(activeBoostTemplate.gameObject, activeBoostsParent)` so the template can be slot 0 and hidden).

**Recommended:** Make the template the **first child** of Active Boosts Parent and **disable** it. Then the script will create clones as siblings; the first time it needs one slot it will instantiate one and show it. So the “template” is only used as a source to clone; you don’t need to show the template itself.

---

## 6. Scroll View and Content

- Add a **Scroll View** (UI → Scroll View).
- Under **Viewport → Content**:
  - Add a **Vertical Layout Group** so rows stack.
  - Add **Content Size Fitter** (Vertical Fit: Preferred Size) so the scroll area grows with the number of rows.
- Assign **Content** to **BoostShopUI → Content Parent**.

---

## 7. Opening and Closing the Panel

- **BoostShopUI** controls visibility of the **GameObject** it’s on (the root of the Boost Shop panel).
- **SetPanelVisible(bool visible)**  
  - `true`: panel active and visible, and **Refresh()** is called so the list and active timers are up to date.  
  - `false`: panel hidden.
- **TogglePanel()**  
  - Toggles between shown and hidden (uses `gameObject.activeSelf`).

From your main UI (e.g. a “Boosts” or “Shop” button):

1. Get a reference to the **BoostShopUI** (e.g. on the Boost Shop panel).
2. In the button’s **On Click ()** list, add:
   - **BoostShopPanel** (or whatever holds BoostShopUI) → **BoostShopUI.TogglePanel()**  
   or  
   - **BoostShopUI.SetPanelVisible(true)** to open; use another button or overlay to call **SetPanelVisible(false)** to close.

---

## 8. Behaviour Summary After Setup

1. When the panel is **enabled** (or shown), it subscribes to **BoostManager.OnBoostsChanged** and calls **Refresh()**.
2. **Refresh()**:
   - Ensures there are as many row instances as there are boost definitions; creates/destroys rows from **Row Prefab** under **Content Parent**.
   - For each row, sets title, description, cost text, and button interactable (Gold: interactable only if player has enough Gold).
   - Updates the “Active boosts” list (remaining time per active boost).
3. **Activate** (row button):
   - **Gold cost**: calls **BoostManager.TryActivate(boostId)** (spends Gold and starts the boost); then **Refresh()**.
   - **Watch Ad**: calls **BoostManager.ActivateAfterAd(boostId)** (placeholder; replace with ad SDK + callback later); then **Refresh()**.
4. When the panel is **disabled**, it unsubscribes from **OnBoostsChanged**.

Once **Content Parent**, **Row Prefab**, and optionally **Active Boosts Parent** + **Active Boost Template** are assigned, the Boost Shop panel is fully wired and works as described above.
