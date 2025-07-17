//using Cysharp.Threading.Tasks;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class DeliveryVanManager : MonoBehaviour
//{

//    public static DeliveryVanManager Instance;
//    public GameObject[] Vehicles;
//    int currentVehicleCount;
//    int currentSelectedObject;
//    [SerializeField] int CurrentCash; // Temporary cash
//    bool canUpgrade;
//    private List<GameObject> placedVehicles = new List<GameObject>();
//    [Header("UI References")]
//    public Transform buidlNewPointParent;
//    public Transform buildDeliveryPointParent;


//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        ActionPerformedOneTime();
//    }
//    void Start()
//    {
//        //Invoke("DelayOnStart",1);
//        DelayOnStart().Forget();


//    }
//    public void ActionPerformedOneTime()
//    {
//        const string oneTimeKey = "OneTimeVehicle";

//        if (PlayerPrefs.GetInt(oneTimeKey) != 1)
//        {
//            PlayerPrefs.SetInt(oneTimeKey, 1);

//            string purchaseKey = Vehicles[0].GetComponent<Vehicle>().VehicleName + "Purchased";
//            PlayerPrefs.SetInt(purchaseKey, 1);
//        }

//    }
//    public async UniTask DelayOnStart()
//    {
//        await UniTask.NextFrame();
//        for (int i = 0; i < Vehicles.Length; i++)
//        {
//            if (PlayerPrefs.GetInt(Vehicles[i].GetComponent<Vehicle>().VehicleName + "Purchased") == 1)
//            {
//                currentSelectedObject = i;
//                PlaceNewKitchen();
//                UpdateBuildNewKitchenUI();
//            }
//        }
//    }

//    public void UpdateBuildNewKitchenUI()
//    {

//        for (int i = 0; i < buidlNewPointParent.childCount; i++)
//        {
//            ;
//            bool isPurchased = Vehicles[i].GetComponent<Vehicle>().VehicleIsPurchased /*&& CheckCanUpgrade(warehouses[i].GetComponent<Warehouse>().ScriptableWarehouseData.WarehouseName, i)*/;
//            Transform point = buidlNewPointParent.GetChild(i);
//            point.GetChild(0).gameObject.SetActive(!isPurchased);
//            point.GetChild(1).gameObject.SetActive(isPurchased);

//        }
//    }
//    public bool CheckCanUpgrade(string name, int i)
//    {
//        var vehicle = Vehicles[i].GetComponent<Vehicle>();
//        int index = vehicle.GetUpdateDetails(name);
//        //  Debug.Log("Index "+index);
//        canUpgrade = vehicle.updates[index + 1].Cost <= CurrentCash;
//        return canUpgrade;
//    }
//    public void AddKitchenButtonClicked(int n)
//    {
//        currentSelectedObject = n;
//        if (placedVehicles.Count < Vehicles.Length)
//        {
//            PlaceNewKitchen();
//            UpdateBuildNewKitchenUI();
//        }
//    }
//    public void UpdateKitchenUI(int n)
//    {
//        currentSelectedObject = n;
//        for (int i = 0; i < buildDeliveryPointParent.childCount; i++)
//        {
//            buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(4).gameObject.SetActive(true);
//        }
//        Vehicle selectedKitchen = Vehicles[currentSelectedObject].GetComponent<Vehicle>();
//        int KitchenCurrentupdate = selectedKitchen.GetUpdateDetails(Kitchens[currentSelectedObject].GetComponent<Kitchen>().kitchenName);

//        if ((KitchenCurrentupdate + 1) < 3 && CheckCanUpgrade(selectedKitchen.kitchenName, currentSelectedObject))
//        {
//            //Debug.Log(selectedKitchen.kitchenName + " " + currentSelectedObject + " " + canUpgrade);
//            buildDeliveryPointParent.transform.GetChild(selectedKitchen.GetUpdateDetails(selectedKitchen.kitchenName) + 1).GetChild(0).GetChild(4).gameObject.SetActive(false);
//            buildDeliveryPointParent.transform.GetChild(selectedKitchen.GetUpdateDetails(selectedKitchen.
//               kitchenName) + 1).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.RemoveAllListeners();
//            buildDeliveryPointParent.transform.GetChild(selectedKitchen.GetUpdateDetails(selectedKitchen.
//                kitchenName) + 1).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.AddListener(() =>
//                {
//                    selectedKitchen.UpdateKitchen();
//                });
//        }
//    }
//    public void PlaceNewKitchen()
//    {
//        GameObject vehicleObject = Vehicles[currentSelectedObject];
//        //kitchenObj.SetActive(true); add vehicle in van production
//        vehicleObject.transform.GetChild(vehicleObject.GetComponent<Kitchen>().CurrentUpdate).gameObject.SetActive(true);
//        //ShawarmaSpawner.Instance.AddNewTarget(kitchenObj.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().Capacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentSelectedObject]);
//        vehicleObject.name = "kitchen" + (currentSelectedObject + 1);// For changing gameobject name to see in hierarchy (optional)
//        vehicleObject.GetComponent<Kitchen>().SetKitchenIsPurchased();
//        placedVehicles.Add(vehicleObject);
//        currentVehicleCount++;

//        if (vehicleObject.TryGetComponent(out Vehicle vehicle))
//        {
//            vehicle.AssignId(currentVehicleCount + 1);
//        }
//    }
//}
