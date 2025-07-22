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
    public int cost;
    public Transform TargetPosition;
    public int id;
    public string warehouseName;
    // private int currentLevel;
    [Header("This Class References")]
    private bool isDirty = false;
    public Transform DeliveryPosition;
    public bool HouseIsPurchased;
    [SerializeField] public List<UpdateDetails> updates = new List<UpdateDetails>();

    //public int GetUpdateDetails(string HouseName)
    //{
    //    return PlayerPrefs.GetInt(HouseName);
    //}
    //public void SetUpdateDetails(string HouseName, int value)
    //{
    //    if (PlayerPrefs.GetInt(HouseName) >= updates.Count)
    //        return;
    //    PlayerPrefs.SetInt(HouseName, value);
    //}

    //public int GetCurrentCapacity(string HouseName)
    //{
    //    return updates[GetUpdateDetails(HouseName)].Capacity;
    //}
    public void SetHouseIsPurchased()
    {
        PlayerPrefs.SetInt(warehouseName + "Purchased", 1);
        HouseIsPurchased = true;
    }


    private void Awake()
    {
        //  Debug.Log("check");
        //if (PlayerPrefs.GetInt(warehouseName + "Purchased") == 1)
        //{
        //    HouseIsPurchased = true;
        //}
        //else
        //{
        //    HouseIsPurchased = false;
        //}
        //currentUpdate = GetUpdateDetails(warehouseName);
        //currentCapacity = updates[currentUpdate].Capacity;



    }
    private void Start()
    {
        SaveLoadManager.saveLoadManagerInstance.Register(this);
        GameManager.gameManagerInstance.RecordPersistentRegistrations().Forget();
        Debug.Log("CP " + currentCapacity);
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
        //int CurrentUpdateId = GetUpdateDetails(warehouseName);
        //if (CurrentUpdateId < updates.Count - 1)
        //{
        //    for (int i = 0; i < 5; i++)
        //    {
        //        transform.GetChild(i).gameObject.SetActive(false);
        //    }
        //    SetUpdateDetails(warehouseName, CurrentUpdateId + 1);
        //    currentUpdate = GetUpdateDetails(warehouseName);

        //    transform.GetChild(CurrentUpdateId + 1).gameObject.SetActive(true);
        //    WarehouseManager.Instance.UpdateWarehoueUI(id);
        //    SoundManager.Instance.PlayButtonClick();
        //}

        float cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Storage, currentUpdate);
        Debug.Log("cost "+ cost);
        if (cost <= PlayerProgress.Instance.PlayerCash)
        {
            for (int i = 0; i < 5; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            currentUpdate++;
            transform.GetChild(currentUpdate -1).gameObject.SetActive(true);
            WarehouseManager.Instance.UpdateWarehoueUI(id);
            WarehouseManager.Instance.UpdateIcon(id);
            SoundManager.Instance.PlayButtonClick();
            onWarehouseUpgraded?.Invoke(UIUpdateType.Cash, PlayerProgress.Instance.PlayerCash);
           
            isDirty = true;
        }
        else
        {
            ////Open SHop or so
            //Debug.Log("Cost "+cost);
            //Debug.Log("Low Cash");
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
        isDirty = true;
    }


    #region Save/Load
    public bool IsDirty => isDirty;
    public object CaptureState()
    {
        Debug.Log("CaptureState");
        return new WarehouseDataNew
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
        if (state is not WarehouseDataNew data)
            return;
        id = data.id;
        currentCapacity = data.capacity;
        currentLoad = data.currentLoad;
        currentUpdate = data.currentUpdate;
        cost = data.cost;
        //currentLevel = data.currentLevel;
        isDirty = false;
        Debug.Log("RestoreState");
    }
    public void SetInitialData()
    {
        currentUpdate = 1;
        currentCapacity = UpgradeCosts.capacityMap[CapacityType.Storage].baseCapacity;
        cost = (int)UpgradeCosts.GetUpgradeCost(UpgradeType.Storage, currentUpdate);
        currentLoad = 0;
        isDirty = true;
        Debug.Log("SetInitialData");
    }
    public void ClearDirty()
    {
        isDirty = false;
    }


    #endregion
}
public class WarehouseDataNew
{
    public int id;
    public int capacity;
    public int currentLoad;
    public int currentUpdate;
    public int cost;
   

}
[System.Serializable]
public class UpdateDetails
{
    public int UpdateId;
    public Sprite Icon;
}