using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Catering : MonoBehaviour, ISaveable
{
    private int id;
    public float cost;
    internal string CateringName;  
    public int currentUpdate;
    public bool CateringIsPurchased;
    private bool isDirty = false;
    public string SaveKey => "catering" + id;
    public static event Action<UIUpdateType, float> onCateringUpgraded;
    public List<CateringUpdateDetails> updates = new List<CateringUpdateDetails>();
   
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
   
    public void SetCateringIsPurchased()
    {
        PlayerPrefs.SetInt(CateringName + "Purchased", 1);
        CateringIsPurchased = true;
    }
    public void UpdateCatering()
    {    
       
        
        Debug.Log("cost " + cost);
        if (cost <= PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(cost);
            for (int i = 0; i < updates.Count; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            currentUpdate++;
            transform.GetChild(currentUpdate - 1).gameObject.SetActive(true);
            SoundManager.Instance.PlayButtonClick();
            onCateringUpgraded?.Invoke(UIUpdateType.Cash, PlayerProgress.Instance.PlayerCash);
            cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Storage, currentUpdate);
            CateringManager.Instance.UpdateCateringUI(id);
            CateringManager.Instance.UpdateIcon(id);
            CateringManager.Instance.UpdateCostText(id);

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
        return new CateringData
        {
            id = id,
            currentUpdate = currentUpdate,
            cost= cost
        };
    }
    public void RestoreState(object state)
    {
        if (state is not CateringData data)
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
public class CateringData
{
    public int id;
    public int currentUpdate;
    public float cost;
}
[System.Serializable]
public class CateringUpdateDetails
{
    public int UpdateId;
    public Sprite Icon;
    public string UpdateName;

}