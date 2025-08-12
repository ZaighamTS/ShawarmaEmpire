using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : MonoBehaviour, ISaveable
{
    internal int id;
    public float cost;
    public float spawnInterval;
    public float deliverCapacity;
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
    internal void MakePersistent(int newId)
    {
        isDirty = true;
    }
    public void SetDeliveryIsPurchased()
    {
        PlayerPrefs.SetInt(DeliveryName + "Purchased", 1);
        DeliveryIsPurchased = true;
    }
   

    public void UpdateDelivery()
    {    
       
      
        Debug.Log("cost " + cost);
        if (cost <= PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(cost);
            for (int i = 0; i < updates.Count; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.GetChild(currentUpdate - 1).gameObject.SetActive(true);
            DeliveryVanObjects[currentUpdate - 1].transform.GetComponent<DeliveryVan>().deliveryCapacity = (int)deliverCapacity;

            DeliveryVanSpawner.Instance.vanPrefab.Add(DeliveryVanObjects[currentUpdate - 1]);
            DeliveryVanSpawner.Instance.spawnInterval = UpgradeCosts.GetDeliveryInterval(currentUpdate - 1);
            currentUpdate++;
            SoundManager.Instance.PlayButtonClick();
            onDeliveryUpgraded?.Invoke(UIUpdateType.Cash, PlayerProgress.Instance.PlayerCash);
            cost = UpgradeCosts.GetUpgradeCost(UpgradeType.DeliveryVan, currentUpdate);
            DeliveryManager.Instance.UpdateDeliveryUI(id);
            DeliveryManager.Instance.UpdateSlider(id, updates.Count, currentUpdate - 1);
            DeliveryManager.Instance.UpdateIcon(id);
            DeliveryManager.Instance.UpdateCostText(id);
          

          
            

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
            cost= cost,
            deliverCapacity = deliverCapacity,
            spawnInterval = spawnInterval

        };
    }
    public void RestoreState(object state)
    {
        if (state is not DeliveryData data)
            return;
        id = data.id;
        currentUpdate = data.currentUpdate;
        cost = data.cost;
        spawnInterval = data.spawnInterval;
        deliverCapacity= data.deliverCapacity;
        isDirty = false;
    }
    public void SetInitialData()
    {
        currentUpdate = 1;  
        cost = UpgradeCosts.GetUpgradeCost(UpgradeType.DeliveryVan, currentUpdate);
        spawnInterval = UpgradeCosts.GetDeliveryInterval(currentUpdate);
        deliverCapacity = UpgradeCosts.GetDeliveryCapacity(CapacityType.Delivery, currentUpdate);
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
    public float cost;
    public float spawnInterval;
    public float deliverCapacity;
}
[System.Serializable]
public class DeliveryUpdateDetails
{
    public int UpdateId;
    public Sprite Icon;
    public string UpdateName;

}