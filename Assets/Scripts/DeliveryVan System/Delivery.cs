using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : MonoBehaviour, ISaveable
{
    private int id;
    public int cost;
    internal string DeliveryName;
    public GameObject[] DeliveryVanObjects; 
    public int currentUpdate;
    public bool DeliveryIsPurchased;
    private bool isDirty = false;
    public string SaveKey => "Delivery" + id;
    public static event Action<UIUpdateType, float> onDeliveryUpgraded;
    public List<DeliveryUpdateDetails> updates = new List<DeliveryUpdateDetails>();
   
    private void Start()
    {  
        SaveLoadManager.saveLoadManagerInstance.Register(this);
        GameManager.gameManagerInstance.RecordPersistentRegistrations().Forget();
        //Debug.Log("CP " + currentCapacity);
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
   
    public void SetDeliveryIsPurchased()
    {
        PlayerPrefs.SetInt(DeliveryName + "Purchased", 1);
        DeliveryIsPurchased = true;
    }

    public void UpdateDelivery()
    {    
        float cost = UpgradeCosts.GetUpgradeCost(UpgradeType.DeliveryVan, currentUpdate);
        GameManager.gameManagerInstance.SpendCash(cost);
        Debug.Log("cost " + cost);
        if (cost <= PlayerProgress.Instance.PlayerCash)
        {
            for (int i = 0; i < updates.Count; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            currentUpdate++;
            transform.GetChild(currentUpdate - 1).gameObject.SetActive(true);
           // for (int i = 0; i <= currentUpdate; i++)
            {
                DeliveryVanSpawner.Instance.vanPrefab.Add(DeliveryVanObjects[currentUpdate - 1]);
            }
            DeliveryManager.Instance.UpdateDeliveryUI(id);
            DeliveryManager.Instance.UpdateIcon(id);
            SoundManager.Instance.PlayButtonClick();
            onDeliveryUpgraded?.Invoke(UIUpdateType.Cash, PlayerProgress.Instance.PlayerCash);
            isDirty = true;
        }
        else
        {
            UIManager.Instance.lowCashPromt.SetActive(true);
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
        return new DeliveryData
        {
            id = id,
            currentUpdate = currentUpdate,
            cost= cost
        };
    }
    public void RestoreState(object state)
    {
        if (state is not DeliveryData data)
            return;
        id = data.id;
        currentUpdate = data.currentUpdate;
        cost = data.cost;
        isDirty = false;
    }
    public void SetInitialData()
    {
        currentUpdate = 1;  
        cost = (int)UpgradeCosts.GetUpgradeCost(UpgradeType.DeliveryVan, currentUpdate);
        isDirty = true;
       
    }
    public void ClearDirty()
    {
        isDirty = false;
    }
    #endregion
}
public class DeliveryData
{
    public int id;
    public int currentUpdate;
    public int cost;
}
[System.Serializable]
public class DeliveryUpdateDetails
{
    public int UpdateId;
    public Sprite Icon;

}