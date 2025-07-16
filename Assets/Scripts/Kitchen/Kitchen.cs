using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kitchen : MonoBehaviour, ISaveable
{
  
    PlayerProgress playerProgress;
    private int id;
    private int capacity;
    private int kitchenLevel = 0; 
    internal string kitchenName;  
    public int CurrentUpdate;
    public bool KitchenIsPurchased;
    public Transform DeliveryPosition;
    [Header("This Class References")]
    private bool isDirty = false;
    public string SaveKey => "kitchen" + id;

     public List<KitchenUpdateDetails> updates = new List<KitchenUpdateDetails>();
    private void Awake()
    {
        if (PlayerPrefs.GetInt(kitchenName + "Purchased") == 1)
        {
            KitchenIsPurchased = true;
        }
        else
        {
            KitchenIsPurchased = false;
        }
        CurrentUpdate = GetUpdateDetails(kitchenName);
        capacity = updates[CurrentUpdate].Capacity;
    }
    private void Start()
    {
        playerProgress = PlayerProgress.Instance;
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
    public int GetUpdateDetails(string KitchenName)
    {
        return PlayerPrefs.GetInt(KitchenName);
    }
    public void SetUpdateDetails(string KitchenName, int value)
    {
        if (PlayerPrefs.GetInt(KitchenName) >= updates.Count)
            return;
        PlayerPrefs.SetInt(KitchenName, value);
    }

    public int GetCurrentCapacity(string KitchenName)
    {
        return updates[GetUpdateDetails(KitchenName)].Capacity;
    }
    public void SetKitchenIsPurchased()
    {
        PlayerPrefs.SetInt(kitchenName + "Purchased", 1);
        KitchenIsPurchased = true;
    }

    public void UpdateKitchen()
    {
        int CurrentUpdateId = GetUpdateDetails(kitchenName);
        if (CurrentUpdateId < updates.Count - 1)
        {
            for (int i = 0; i < 3; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            SetUpdateDetails(kitchenName, CurrentUpdateId + 1);
            CurrentUpdate = GetUpdateDetails(kitchenName);

            transform.GetChild(CurrentUpdateId + 1).gameObject.SetActive(true);
            KitchenManager.Instance.UpdateKitchenUI(id);
        }
    }

    public void OnShwarmaGen()
    {
        //currentLoad++;
    }

    #region Save/Load
    public bool IsDirty => isDirty;
    public object CaptureState()
    {
        return new KitchenData
        {
            id = id,
            capacity = capacity,
            kitchenLevel = kitchenLevel,  
            kitchenName = kitchenName,
            currentUpdate = CurrentUpdate
        };
    }
    public void RestoreState(object state)
    {
        if (state is not KitchenData data)
            return;
        id = data.id;
        capacity = data.capacity;
        kitchenLevel = data.kitchenLevel;
        kitchenName = data.kitchenName;
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
public class KitchenData
{
    public int id;
    public int capacity;
    public int kitchenLevel = 0;
    public string kitchenName;
    public int currentUpdate;
}
[System.Serializable]
public class KitchenUpdateDetails
{
    public int UpdateId;
    public int Capacity;
    public int Cost;

}