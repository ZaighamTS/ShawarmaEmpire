using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Kitchen : MonoBehaviour, ISaveable
{
    public int id;
    public float cost;
    public string kitchenName;
    public int currentUpdate;
    public bool KitchenIsPurchased;
    private bool isDirty = false;
    public string SaveKey => "kitchen" + id;
    public static event Action<UIUpdateType, float> onKitchenUpgraded;
    public List<KitchenUpdateDetails> updates = new List<KitchenUpdateDetails>();

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

    public void SetKitchenIsPurchased()
    {
        PlayerPrefs.SetInt(kitchenName + "Purchased", 1);
        KitchenIsPurchased = true;
    }

    public void UpdateKitchen()
    {
        if (cost <= PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(cost);
            for (int i = 0; i < updates.Count; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.GetChild(currentUpdate - 1).gameObject.SetActive(true);
            currentUpdate++;

            SoundManager.Instance.PlayButtonClick();
            onKitchenUpgraded?.Invoke(UIUpdateType.Cash, PlayerProgress.Instance.PlayerCash);
            cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Kitchen, currentUpdate);
            KitchenManager.Instance.UpdateKitchenUI(id);
            KitchenManager.Instance.UpdateIcon(id);
            KitchenManager.Instance.UpdateSlider(id, updates.Count, currentUpdate - 1);
            KitchenManager.Instance.UpdateCostText(id);
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
        return new KitchenData
        {
            id = id,
            currentUpdate = currentUpdate,
            cost = cost
        };
    }
    public void RestoreState(object state)
    {
        if (state is not KitchenData data)
            return;
        id = data.id;
        currentUpdate = data.currentUpdate;
        cost = data.cost;
        isDirty = false;
    }
    public void SetInitialData()
    {
        currentUpdate = 1;
        cost = (int)UpgradeCosts.GetUpgradeCost(UpgradeType.Kitchen, currentUpdate);
        isDirty = true;
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
    public int currentUpdate;
    public float cost;
}
[System.Serializable]
public class KitchenUpdateDetails
{
    public int UpdateId;
    public Sprite Icon;
    public string UpdateName;

}