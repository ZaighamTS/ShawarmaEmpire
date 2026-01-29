# How to Clear All Save Data

## 🔍 Problem

You cleared PlayerPrefs but some data is still being saved. This is because the game uses **TWO save systems**:

1. **PlayerPrefs** - Tutorial flags, one-time actions
2. **JSON File** - Main game data (cash, upgrades, buildings, etc.)

## ✅ Solution

### Method 1: Use the Reset Component (Recommended)

1. **In Unity Editor:**
   - Create an empty GameObject (or use existing)
   - Add Component → `Reset Game Data`
   - Right-click the component → **"Complete Reset (All Data)"**
   - This clears both PlayerPrefs AND the JSON file

2. **At Runtime:**
   - The component has context menu options
   - Or call `SaveLoadManager.saveLoadManagerInstance.CompleteReset()` from code

### Method 2: Manual File Deletion

**Windows:**
```
%USERPROFILE%\AppData\LocalLow\<CompanyName>\<ProductName>\shawarma.json
```

**Example:**
```
C:\Users\YourName\AppData\LocalLow\DefaultCompany\Sharwama_Dash\shawarma.json
```

**Steps:**
1. Close Unity/Game
2. Navigate to the path above
3. Delete `shawarma.json`
4. Also clear PlayerPrefs in Unity: `PlayerPrefs.DeleteAll()`

### Method 3: Code Method

**In Unity Console or Script:**
```csharp
// Complete reset (clears everything)
SaveLoadManager.saveLoadManagerInstance.CompleteReset();

// Or just delete the JSON file
SaveLoadManager.saveLoadManagerInstance.DeleteSaveFile();

// Or just clear PlayerPrefs
PlayerPrefs.DeleteAll();
PlayerPrefs.Save();
```

## 📁 Save File Locations

### JSON Save File
- **Path:** `Application.persistentDataPath + "/shawarma.json"`
- **Contains:** All game progress (cash, upgrades, buildings, etc.)
- **Format:** JSON

### PlayerPrefs
- **Location:** Windows Registry or Unity's PlayerPrefs storage
- **Contains:** Tutorial flags, one-time actions, reward counts
- **Keys:**
  - `CurrentDateTime` - Last play time
  - `Tutorial_*` - Tutorial completion flags
  - `*Purchased` - Building purchase flags
  - `RewardCount` - Ad reward count
  - `building_purchased_*` - Building unlock flags

## 🛠️ Available Methods

### SaveLoadManager Methods

**`CompleteReset()`**
- Deletes JSON save file
- Clears PlayerPrefs
- Resets all ISaveables to initial state
- Creates fresh save file

**`DeleteSaveFile()`**
- Only deletes the JSON save file
- Doesn't clear PlayerPrefs
- Doesn't reset ISaveables

**`GetSaveFilePath()`**
- Returns the full path to the save file
- Useful for debugging

### ResetGameData Component Methods

**Context Menu Options:**
- **Complete Reset (All Data)** - Clears everything
- **Delete Save File Only** - Only deletes JSON file
- **Clear PlayerPrefs Only** - Only clears PlayerPrefs
- **Show Save File Path** - Shows where the file is saved

## 🎮 In-Game Reset

The **Prestige Button** now uses `CompleteReset()` to clear all data when prestiging.

## 🔧 Testing

1. **Add ResetGameData component** to a GameObject
2. **Play the game** and make some progress
3. **Right-click component** → "Complete Reset (All Data)"
4. **Reload scene** - Should start fresh

## 📝 Notes

- **Save file is created automatically** when the game starts
- **Save file path is logged** in console: `"SavePath set to: ..."`
- **Both systems must be cleared** for complete reset
- **Prestige system** uses `CompleteReset()` automatically

## ⚠️ Warning

**Complete reset will:**
- Delete all progress
- Reset all upgrades
- Clear all buildings
- Reset cash and earnings
- Clear tutorial progress
- **Cannot be undone!**

Use with caution, especially in production builds.

---

**Quick Fix:** Add `ResetGameData` component to any GameObject and use the context menu!

