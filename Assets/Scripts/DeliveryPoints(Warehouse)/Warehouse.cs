using System.Collections.Generic;

using UnityEngine;


public class Warehouse : MonoBehaviour, ISaveable
{
   
    public Transform TargetPosition;
    public int id;    
    public int Capacity;
    public int CurrentLoad;
    public string warehouseName;
    public int CurrentUpdate;
    private int currentLevel;
    [Header("This Class References")]
    private bool isDirty = false;
    public string SaveKey => "warehouse" + id;
    public Transform DeliveryPosition;
    public bool HouseIsPurchased;
    [SerializeField] public List<UpdateDetails> updates = new List<UpdateDetails>();
    
    public int GetUpdateDetails(string HouseName)
    {
        return PlayerPrefs.GetInt(HouseName);
    }
    public void SetUpdateDetails(string HouseName, int value)
    {
        if (PlayerPrefs.GetInt(HouseName) >= updates.Count)
            return;
        PlayerPrefs.SetInt(HouseName, value);
    }

    public int GetCurrentCapacity(string HouseName)
    {
        return updates[GetUpdateDetails(HouseName)].Capacity;
    }
    public void SetHouseIsPurchased()
    {
        PlayerPrefs.SetInt(warehouseName+"Purchased",1);
        HouseIsPurchased = true;
    }


    private void Awake()
    {
        Debug.Log("check");
        if (PlayerPrefs.GetInt(warehouseName+ "Purchased") == 1)
        {
            HouseIsPurchased = true;
        }
        else
        {
            HouseIsPurchased = false;
        }
        CurrentUpdate = GetUpdateDetails(warehouseName);   
        Capacity = updates[CurrentUpdate].Capacity;
       
    }
    private void Start()
    {
      //  if (!HouseIsPurchased)
        
           
            
        
        SaveLoadManager.saveLoadManagerInstance.Register(this);
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
        int CurrentUpdateId = GetUpdateDetails(warehouseName);
        if (CurrentUpdateId < updates.Count-1)
        {
            for (int i = 0; i < 5; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            SetUpdateDetails(warehouseName, CurrentUpdateId + 1);
            CurrentUpdate = GetUpdateDetails(warehouseName);

            transform.GetChild(CurrentUpdateId + 1).gameObject.SetActive(true);
            WarehouseManager.Instance.UpdateWarehoueUI(id);
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
        CurrentLoad++;
    }


    #region Save/Load
    public bool IsDirty => isDirty;
    public object CaptureState()
    {
        return new WarehouseDataNew
        {
            id = id,
            capacity = Capacity,
            currentLoad = CurrentLoad,
            warehouseName = warehouseName,
            currentUpdate = CurrentUpdate,
        };
    }
    public void RestoreState(object state)
    {
        if (state is not WarehouseDataNew data)
            return;
        id = data.id;
        Capacity = data.capacity;
        CurrentLoad = data.currentLoad;
        warehouseName = data.warehouseName;
        CurrentUpdate = data.currentUpdate; 
        currentLevel = data.currentLevel;
        isDirty = false;
    }
    public void SetInitialData()
    {

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
    public string warehouseName;
    public int currentUpdate;
    public int currentLevel;

}
[System.Serializable]
public class UpdateDetails
{
    public int UpdateId;
    public int Capacity;
    public int Cost;

}