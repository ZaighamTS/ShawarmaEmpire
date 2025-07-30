using Cysharp.Threading.Tasks;
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

    public void SetHouseIsPurchased()
    {
        PlayerPrefs.SetInt(warehouseName + "Purchased", 1);
        HouseIsPurchased = true;
    }

    private void Start()
    {
        SaveLoadManager.saveLoadManagerInstance.Register(this);
        GameManager.gameManagerInstance.RecordPersistentRegistrations().Forget();
    }
    private void OnDestroy()
    {
        SaveLoadManager.saveLoadManagerInstance.Unregister(this);
    }
    internal void AssignId(int newId)
    {
        if (id == -1)
        {
            id = newId;
        }
    }
    public void UpdateWarehouse()
    {
        
        Debug.Log("cost "+ cost);
        if (cost <= PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(cost);
            for (int i = 0; i < updates.Count; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            currentUpdate++;
            transform.GetChild(currentUpdate -1).gameObject.SetActive(true);
           
           
            SoundManager.Instance.PlayButtonClick();
            onWarehouseUpgraded?.Invoke(UIUpdateType.Cash, PlayerProgress.Instance.PlayerCash);
            cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Storage, currentUpdate);
           
            WarehouseManager.Instance.UpdateWarehoueUI(id);
            WarehouseManager.Instance.UpdateIcon(id);
            WarehouseManager.Instance.UpdateCostText(id);
            isDirty = true;
        }
        else
        {    
            UIManager.Instance.lowCashPromt.SetActive(true);
        }
    }
    private void OnMouseDown()
    {
        // Block interaction if a UI element is under the pointer
        //if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        //    return;

        //if (gameObject.activeInHierarchy)
        //{
        //    FindObjectOfType<WarehouseManager>().ShowUpgradePanel(this);
        //}
    }
    public void OnShwarmaGen()
    {
        currentLoad++;
        currentCapacity--;
        isDirty = true;
    }

    #region Save/Load
    public bool IsDirty => isDirty;
    public object CaptureState()
    {
        Debug.Log("CaptureState");
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
        currentLoad = data.currentLoad;
        currentUpdate = data.currentUpdate;
        cost = data.cost;
        isDirty = false;
    }
    public void SetInitialData()
    {
        currentUpdate = 1;
        currentCapacity = UpgradeCosts.capacityMap[CapacityType.Storage].baseCapacity;
        cost = (int)UpgradeCosts.GetUpgradeCost(UpgradeType.Storage, currentUpdate);
        currentLoad = 0;
        isDirty = true;
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
}