using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.GraphView.Port;

public class Warehouse : MonoBehaviour, ISaveable
{
   
    public Transform TargetPosition;
    public int id;    
    public int Capacity;
    public int CurrentLoad;
    public string warehouseName;
    public int CurrentUpdate;
   
    [Header("This Class References")]
    private bool isDirty = false;
    public string SaveKey => "warehouse" + id;

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
        PlayerPrefs.SetInt(warehouseName,1);
        HouseIsPurchased = true;
    }


    private void Awake()
    {
        
        CurrentUpdate = GetUpdateDetails(warehouseName);   
        Capacity = updates[CurrentUpdate].Capacity;
        if (PlayerPrefs.GetInt(warehouseName) == 1)
        {
            HouseIsPurchased = true;
        }
        else
        {
            HouseIsPurchased = false;
        }
    }
    private void Start()
    {
      //  if (!HouseIsPurchased)
        {
           
            
        }
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
  
}
[System.Serializable]
public class UpdateDetails
{
    public int UpdateId;
    public int Capacity;
    public int Cost;

}