using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Warehouse : MonoBehaviour
{
    public WarehouseData ScriptableWarehouseData;
    public Transform TargetPosition;
    public int id;
    public int Capacity;
    public int CurrentLoad;
    public string warehouseName;
    public int CurrentUpdate;
 
    private void Awake()
    {
        warehouseName = ScriptableWarehouseData.WarehouseName;
        CurrentUpdate = ScriptableWarehouseData.GetUpdateDetails(warehouseName);
       
        Capacity = ScriptableWarehouseData.updates[CurrentUpdate].Capacity;
    }
    
    public void UpdateWarehouse()
    {
        int CurrentUpdateId = ScriptableWarehouseData.GetUpdateDetails(warehouseName);
        if (CurrentUpdateId < ScriptableWarehouseData.updates.Count-1)
        {
            for (int i = 0; i < 5; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            ScriptableWarehouseData.SetUpdateDetails(warehouseName, CurrentUpdateId + 1);
            CurrentUpdate = ScriptableWarehouseData.GetUpdateDetails(warehouseName);

            transform.GetChild(CurrentUpdateId + 1).gameObject.SetActive(true);
           
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
}
