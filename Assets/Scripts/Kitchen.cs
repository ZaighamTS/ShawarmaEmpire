using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kitchen : MonoBehaviour
{
    public KitchenData ScriptableKitchenData;
    public Transform TargetPosition;
    public int id;
    public int Capacity;
    public int CurrentLoad;
    public string KitchenName;
    public int CurrentUpdate;

    private void Awake()
    {
        KitchenName = ScriptableKitchenData.KitchenName;
        CurrentUpdate = ScriptableKitchenData.GetUpdateDetails(KitchenName);

        Capacity = ScriptableKitchenData.updates[CurrentUpdate].Capacity;
    }

    public void UpdateKitchen()
    {
        int CurrentUpdateId = ScriptableKitchenData.GetUpdateDetails(KitchenName);
        if (CurrentUpdateId < ScriptableKitchenData.updates.Count - 1)
        {
            for (int i = 0; i < 5; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            ScriptableKitchenData.SetUpdateDetails(KitchenName, CurrentUpdateId + 1);
            CurrentUpdate = ScriptableKitchenData.GetUpdateDetails(KitchenName);

            transform.GetChild(CurrentUpdateId + 1).gameObject.SetActive(true);

        }

    }
   
    public void OnShwarmaGen()
    {
        CurrentLoad++;
    }
}
