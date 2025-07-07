using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewWarehouseData", menuName = "IdleGame/Warehouse Data")]
public class WarehouseData : ScriptableObject
{
    public string WarehouseName;
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

        HouseIsPurchased = true;
    }
   
   
}
[System.Serializable]
public class UpdateDetails
{
    public int UpdateId;
    public int Capacity;
    public int Cost;
    
}