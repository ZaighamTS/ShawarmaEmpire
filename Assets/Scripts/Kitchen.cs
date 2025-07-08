using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kitchen : MonoBehaviour, ISaveable
{
    // public KitchenData ScriptableKitchenData;
    PlayerProgress playerProgress;
    private int id;
    private int capacity;
    private int kitchenLevel = 0;
    private int currentLoad;
    internal string kitchenName;
    private int currentUpdate;

    [Header("This Class References")]
    private bool isDirty = false;
    public string SaveKey => "kitchen" + id;


    private void Awake()
    {
        //KitchenName = ScriptableKitchenData.KitchenName;
        //CurrentUpdate = ScriptableKitchenData.GetUpdateDetails(KitchenName);

        //Capacity = ScriptableKitchenData.updates[CurrentUpdate].Capacity;
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

    public void UpdateKitchen()
    {
        float updateCost = UpgradeCosts.GetUpgradeCost(UpgradeType.Kitchen, kitchenLevel);
        if (updateCost <= playerProgress.PlayerCash)
        {
            playerProgress.PlayerCash -= updateCost;
            kitchenLevel++;
            isDirty = true;
        }
        else
        {
            //Open Store
        }
        //int CurrentUpdateId = ScriptableKitchenData.GetUpdateDetails(KitchenName);
        //if (CurrentUpdateId < ScriptableKitchenData.updates.Count - 1)
        //{
        //    for (int i = 0; i < 5; i++)
        //    {
        //        transform.GetChild(i).gameObject.SetActive(false);
        //    }
        //    ScriptableKitchenData.SetUpdateDetails(KitchenName, CurrentUpdateId + 1);
        //    CurrentUpdate = ScriptableKitchenData.GetUpdateDetails(KitchenName);

        //    transform.GetChild(CurrentUpdateId + 1).gameObject.SetActive(true);

        //}

    }

    public void OnShwarmaGen()
    {
        currentLoad++;
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
            currentLoad = currentLoad,
            kitchenName = kitchenName,
            currentUpdate = currentUpdate
        };
    }
    public void RestoreState(object state)
    {
        if (state is not KitchenData data)
            return;
        id = data.id;
        capacity = data.capacity;
        kitchenLevel = data.kitchenLevel;
        currentLoad = data.currentLoad;
        kitchenName = data.kitchenName;
        currentUpdate = data.currentUpdate;

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
    public int currentLoad;
    public string kitchenName;
    public int currentUpdate;
}