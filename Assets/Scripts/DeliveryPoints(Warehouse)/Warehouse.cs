using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;

using UnityEngine;


public class Warehouse : MonoBehaviour, ISaveable
{
    public static event Action<UIUpdateType, float> onWarehouseUpgraded;
    public string SaveKey => "warehouse" + id;
    public int currentCapacity;
    public int currentLoad;
    public int currentUpdate;
    public float cost;
    public Transform TargetPosition;
    public int id;
    public string warehouseName;
    // private int currentLevel;
    [Header("This Class References")]
    private bool isDirty = false;
    public Transform DeliveryPosition;
    public bool HouseIsPurchased;
    [SerializeField] public List<UpdateDetails> updates = new List<UpdateDetails>();
    public GameObject WarningImage;
    private bool wasFullLastCheck = false; // Track previous state to detect when storage becomes full
    
    public void SetHouseIsPurchased()
    {
        PlayerPrefs.SetInt(warehouseName + "Purchased", 1);
        HouseIsPurchased = true;
    }

    private void Start()
    {
        SaveLoadManager.saveLoadManagerInstance.Register(this);
        GameManager.gameManagerInstance.RecordPersistentRegistrations().Forget();
       // Debug.Log("wareHouse "+id);
    }
    private void OnDestroy()
    {
        SaveLoadManager.saveLoadManagerInstance.Unregister(this);
    }
    internal void MakePersistent(int newId)
    {
        id = newId; // Set the warehouse ID to match its position in placedWarehouses list
        isDirty = true;
    }
    public void UpdateWarehouse()
    {
        
        Debug.Log("cost "+ cost);
        if (cost <= PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(cost);
            GameProgressEvents.RecordUpgrade(UpgradeType.Storage, cost);
            for (int i = 0; i < updates.Count; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.GetChild(currentUpdate - 1).gameObject.SetActive(true);
            transform.GetChild(currentUpdate - 1).transform.GetComponent<DOTweenAnimation>().DOPlay();
            transform.GetChild(8).gameObject.SetActive(true);
            WarehouseManager.Instance.DisableEffect(gameObject).Forget();
            currentUpdate++;
          
           
           
            SoundManager.Instance.PlayButtonClick();
            onWarehouseUpgraded?.Invoke(UIUpdateType.Cash, PlayerProgress.Instance.PlayerCash);
            cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Storage, currentUpdate);

            // FIXED: Use proper capacity formula - capacity doubles with each upgrade
            // Level 1 (currentUpdate = 2): 250
            // Level 2 (currentUpdate = 3): 500
            // Level 3 (currentUpdate = 4): 1000
            // Level 4 (currentUpdate = 5): 2000, etc.
            int newCapacity = (int)UpgradeCosts.GetDeliveryCapacity(CapacityType.Storage, currentUpdate);
            int capacityIncrease = newCapacity - currentCapacity;
            currentCapacity = newCapacity;
            // Keep existing load, don't reset to 0
            // currentLoad remains the same (warehouse upgrade doesn't clear inventory)
            ShawarmaSpawner.Instance.UpdateCapacity(gameObject, currentCapacity);
            WarehouseManager.Instance.UpdateWarehoueUI(id);
            WarehouseManager.Instance.UpdateIcon(id);
            WarehouseManager.Instance.UpdateCostText(id);
            WarehouseManager.Instance.UpdateSlider(id, updates.Count, currentUpdate - 1);
            WarehouseManager.Instance.ShowAnimationEffect();
            CheckWaring();
            isDirty = true;
        }
        else
        {    
            UIManager.Instance.lowCashPromt.SetActive(true);
        }
    }

    /// <summary>
    /// Checks warehouse capacity and displays warnings when storage becomes full
    /// Shows automatic popup notification when storage transitions from not-full to full
    /// </summary>
    public void CheckWaring()
    {
        bool isNowFull = currentLoad >= currentCapacity;
        
        // Update warning image visibility
        if (WarningImage)
        {
            WarningImage.SetActive(isNowFull);
        }
        
        // Show popup notification when storage becomes full (not just when clicking)
        if (isNowFull && !wasFullLastCheck && UIManager.Instance != null)
        {
            // Get warehouse number for display
            int warehouseNumber = GetWarehouseNumber();
            string displayName = !string.IsNullOrEmpty(warehouseName) 
                ? warehouseName 
                : $"Warehouse #{warehouseNumber}";
            
            // Only show popup if this is a new full state (not already full)
            UIManager.Instance.ShowInfoPopup($"⚠️ {displayName} is FULL!\n\nCapacity: {currentLoad}/{currentCapacity}\n\nProduction will stop until you upgrade storage or deliver shawarmas.");
            
            // Also check if all warehouses are full
            if (WarehouseManager.Instance != null && WarehouseManager.Instance.AreAllWarehousesFull())
            {
                // Small delay to show individual warehouse warning first
                ShowAllWarehousesFullWarning().Forget();
            }
        }
        
        // Update state tracking
        wasFullLastCheck = isNowFull;
    }
    
    /// <summary>
    /// Shows warning when all warehouses are full
    /// </summary>
    private async UniTask ShowAllWarehousesFullWarning()
    {
        await UniTask.Delay(1500); // Wait 1.5 seconds after individual warning
        
        if (UIManager.Instance != null && WarehouseManager.Instance != null && WarehouseManager.Instance.AreAllWarehousesFull())
        {
            UIManager.Instance.ShowInfoPopup("🚨 ALL WAREHOUSES FULL!\n\n⚠️ Production has STOPPED!\n\nUpgrade storage or wait for deliveries to free up space.");
        }
    }
    private void OnMouseDown()
    {
        // FIXED: Enabled building click handler - buildings are now clickable and show info
        // Block interaction if a UI element is under the pointer
        if (UnityEngine.EventSystems.EventSystem.current != null && 
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (gameObject.activeInHierarchy)
        {
            ShowWarehouseInfo();
        }
    }

    private void ShowWarehouseInfo()
    {
        // Get the actual warehouse number from placedWarehouses list (1-based)
        int warehouseNumber = GetWarehouseNumber();
        
        // Use warehouseName if set, otherwise use "Warehouse #X"
        string displayName = !string.IsNullOrEmpty(warehouseName) 
            ? warehouseName 
            : $"Warehouse #{warehouseNumber}";
        
        // Ensure capacity is up-to-date (recalculate if needed)
        int actualCapacity = (int)UpgradeCosts.GetDeliveryCapacity(CapacityType.Storage, currentUpdate);
        if (currentCapacity != actualCapacity)
        {
            currentCapacity = actualCapacity;
        }
        
        // Show warehouse information using existing info popup system
        string infoText = $"{displayName}\n\n" +
                         $"Capacity: {currentLoad}/{currentCapacity}\n" +
                         $"Level: {currentUpdate - 1}\n" +
                         $"Upgrade Cost: ${cost:N0}\n\n" +
                         $"Storage: {((float)currentLoad / currentCapacity * 100):F0}% full";
        
        if (currentLoad >= currentCapacity)
        {
            infoText += "\n\n⚠️ STORAGE FULL - Production will stop!";
        }
        
        UIManager.Instance.ShowInfoPopup(infoText);
        
        // Also update the warehouse UI panel if available
        WarehouseManager.Instance?.UpdateWarehoueUI(id);
    }
    
    /// <summary>
    /// Gets the warehouse number based on its position in the placedWarehouses list
    /// Returns 1-based index (first warehouse = 1, second = 2, etc.)
    /// </summary>
    private int GetWarehouseNumber()
    {
        if (WarehouseManager.Instance == null || WarehouseManager.Instance.placedWarehouses == null)
        {
            return id > 0 ? id : 1; // Fallback to id if available, otherwise 1
        }
        
        // Find this warehouse's index in the placedWarehouses list
        for (int i = 0; i < WarehouseManager.Instance.placedWarehouses.Count; i++)
        {
            if (WarehouseManager.Instance.placedWarehouses[i] == gameObject)
            {
                return i + 1; // Return 1-based index
            }
        }
        
        // If not found in list, use id or fallback
        return id > 0 ? id : 1;
    }
    public void OnShwarmaGen()
    {
        currentLoad++;
      
        isDirty = true;
    }


    #region Save/Load
    public bool IsDirty => isDirty;
    public object CaptureState()
    {
      //  Debug.Log("CaptureState");
        return new WarehouseData
        {
            id = id,
            capacity = currentCapacity,
            currentLoad = currentLoad,
            cost = cost,
            currentUpdate = currentUpdate,
        };
      
    }
    public void RestoreState(object state)
    {
        if (state is not WarehouseData data)
            return;
        id = data.id;
        currentCapacity = data.capacity;
       // Debug.Log("currentCapacity restore " + currentCapacity);
        currentLoad = data.currentLoad;
        currentUpdate = data.currentUpdate;
        cost = data.cost;
        isDirty = false;
        
        // Check warning state after loading game data
        // Set wasFullLastCheck to current state to prevent popup on load
        wasFullLastCheck = currentLoad >= currentCapacity;
        CheckWaring();
    }
    public void SetInitialData()
    {
        currentUpdate = 1; // Level 0 (unpurchased) - capacity will be 0
        // Use GetDeliveryCapacity for consistency
        // Level 0 (currentUpdate = 1): 0 capacity (unpurchased)
        // Level 1 (currentUpdate = 2): 250 capacity (purchased)
        currentCapacity = (int)UpgradeCosts.GetDeliveryCapacity(CapacityType.Storage, currentUpdate);
      //  Debug.Log("currentCapacity initial "+ currentCapacity);
        cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Storage, currentUpdate);
        currentLoad = 0;
        wasFullLastCheck = false; // Initialize tracking
        isDirty = true;
        
        // Check warning state on initialization
        CheckWaring();
    }
    public void ClearDirty()
    {
        isDirty = false;
    }
    #endregion
}
public class WarehouseData
{
    public int id;
    public int capacity;
    public int currentLoad;
    public int currentUpdate;
    public float cost;
}
[System.Serializable]
public class UpdateDetails
{
    public int UpdateId;
    public Sprite Icon;
    public string UpdateName;
}