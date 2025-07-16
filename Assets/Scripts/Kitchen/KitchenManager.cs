using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class KitchenManager : MonoBehaviour
{
    public static KitchenManager Instance;
    public GameObject[] Kitchens; // Assign Kitchena GameObjects (only 1 active at start)

    //WarehouseData CurrentWareHouseData;
    int currentKitchenCount;
    int currentSelectedObject;

    private List<GameObject> placedKitchens = new List<GameObject>();
    [Header("UI References")]
    public Transform buidlNewPointParent;
    public Transform buildDeliveryPointParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
       
        //Invoke("DelayOnStart",1);
        DelayOnStart().Forget();


    }
    public async UniTask DelayOnStart()
    {
        await UniTask.NextFrame();
        for (int i = 0; i < Kitchens.Length; i++)
        {

            //if (Kitchens[i].GetComponent<Warehouse>().ScriptableWarehouseData.HouseIsPurchased)
            //{
            //    currentSelectedObject = i;
            //  //  PlaceNewWarehouse();
            //    UpdateBuildNewKitchenUI();
            //}
        }
    }


    public void UpdateBuildNewKitchenUI()
    {

        for (int i = 0; i < buidlNewPointParent.childCount; i++)
        {
            //bool isPurchased = Kitchens[i].GetComponent<Warehouse>().ScriptableWarehouseData.HouseIsPurchased;
            //Transform point = buidlNewPointParent.GetChild(i);
            //point.GetChild(0).gameObject.SetActive(!isPurchased);
            //point.GetChild(1).gameObject.SetActive(isPurchased);

        }
    }
    public void UpdateKitchenUI(int n)
    {

        //currentSelectedObject = n;
        //for (int i = 0; i < buildDeliveryPointParent.childCount; i++)
        //{
        //    buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(4).gameObject.SetActive(true);
        //}
        //Kitchen selectedKitchen = Kitchens[currentSelectedObject].GetComponent<Kitchen>();
        //int KitchenCurrentupdate = selectedKitchen.ScriptableWarehouseData.GetUpdateDetails(Kitchens[currentSelectedObject].GetComponent<Warehouse>().warehouseName);

        //if ((WarehouseCurrentupdate + 1) < 5)
        //{
        //    buildDeliveryPointParent.transform.GetChild(selectedWarehouse.ScriptableWarehouseData.GetUpdateDetails(selectedWarehouse.warehouseName) + 1).GetChild(0).GetChild(4).gameObject.SetActive(false);
        //    buildDeliveryPointParent.transform.GetChild(selectedWarehouse.ScriptableWarehouseData.GetUpdateDetails(selectedWarehouse.
        //       warehouseName) + 1).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.RemoveAllListeners();
        //    buildDeliveryPointParent.transform.GetChild(selectedWarehouse.ScriptableWarehouseData.GetUpdateDetails(selectedWarehouse.
        //        warehouseName) + 1).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.AddListener(() =>
        //        {
        //            selectedWarehouse.UpdateWarehouse();
        //        });
        //}
    }


    public void PlaceNewKitchen()
    {

        GameObject kitchenObj = Kitchens[currentKitchenCount];
        kitchenObj.SetActive(true);
        // Kitchen.transform.GetChild(Kitchen.GetComponent<Kitchen>().CurrentUpdate).gameObject.SetActive(true);
        //  ShawarmaSpawner.Instance.AddNewTarget(WareHouse.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().Capacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentWarehouseCount]);

        kitchenObj.name = "Kitchen" + (currentKitchenCount + 1);// For changing gameobject name to see in hierarchy (optional)

        //Kitchen.GetComponent<Kitchen>().ScriptableKitchenData.SetUpdateDetails(Kitchen.name, 0);
        //Kitchen.GetComponent<Kitchen>().ScriptableKitchenData.SetHouseIsPurchased();
        placedKitchens.Add(kitchenObj);
        currentKitchenCount++;
        if (kitchenObj.TryGetComponent(out Kitchen kitchen))
        {
            kitchen.AssignId(currentKitchenCount + 1);
        }
    }

    public void BuyNewKitchen()
    {


    }

   
  


    

   

}

