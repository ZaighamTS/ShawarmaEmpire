using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewKitchenData", menuName = "IdleGame/Kitchen Data")]
public class KitchenData : ScriptableObject
{
    public string KitchenName;
    public bool KitchenIsPurchased;
    [SerializeField] public List<UpdateDetails> updates = new List<UpdateDetails>();
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
    public void SetHouseIsPurchased()
    {

        KitchenIsPurchased = true;
    }


}
[System.Serializable]
public class UpdateDetailsKitchen
{
    public int UpdateId;
    public int Capacity;
    public int Cost;

}