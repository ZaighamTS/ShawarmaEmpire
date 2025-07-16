using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class KitchenManager : MonoBehaviour
{
    public static KitchenManager Instance;
    public GameObject[] Kitchens; // Assign Kitchena GameObjects (only 1 active at start)
    int currentKitchenCount;
    int currentSelectedObject;
    [SerializeField] int CurrentCash; // Temporary cash
    bool canUpgrade;
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
        ActionPerformedOneTime();
    }
    void Start()
    {  
        //Invoke("DelayOnStart",1);
        DelayOnStart().Forget();


    }
    public void ActionPerformedOneTime()
    {
        const string oneTimeKey = "OneTimeKitchen";

        if (PlayerPrefs.GetInt(oneTimeKey) != 1)
        {
            PlayerPrefs.SetInt(oneTimeKey, 1);

            string purchaseKey = Kitchens[0].GetComponent<Kitchen>().kitchenName + "Purchased";
            PlayerPrefs.SetInt(purchaseKey, 1);
        }

    }
    public async UniTask DelayOnStart()
    {
        await UniTask.NextFrame();
        for (int i = 0; i < Kitchens.Length; i++)
        {
            if (PlayerPrefs.GetInt(Kitchens[i].GetComponent<Kitchen>().kitchenName + "Purchased") == 1)
            {
                currentSelectedObject = i;
                PlaceNewKitchen();
                UpdateBuildNewKitchenUI();
            }
        }
    }
  
    public void UpdateBuildNewKitchenUI()
    {

        for (int i = 0; i < buidlNewPointParent.childCount; i++)
        {
           ;
            bool isPurchased = Kitchens[i].GetComponent<Kitchen>().KitchenIsPurchased /*&& CheckCanUpgrade(warehouses[i].GetComponent<Warehouse>().ScriptableWarehouseData.WarehouseName, i)*/;
            Transform point = buidlNewPointParent.GetChild(i);
            point.GetChild(0).gameObject.SetActive(!isPurchased);
            point.GetChild(1).gameObject.SetActive(isPurchased);

        }
    }
    public bool CheckCanUpgrade(string name, int i)
    {
        var kitchen = Kitchens[i].GetComponent<Kitchen>();
        int index = kitchen.GetUpdateDetails(name);
      //  Debug.Log("Index "+index);
        canUpgrade = kitchen.updates[index+1].Cost <= CurrentCash;
        return canUpgrade;
    }
    public void AddKitchenButtonClicked(int n)
    {
        currentSelectedObject = n;
        if (placedKitchens.Count < Kitchens.Length)
        {
            PlaceNewKitchen();
            UpdateBuildNewKitchenUI();
        }
    }
    public void UpdateKitchenUI(int n)
    {
        currentSelectedObject = n;
        for (int i = 0; i < buildDeliveryPointParent.childCount; i++)
        {
            buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(4).gameObject.SetActive(true);
        }
        Kitchen selectedKitchen = Kitchens[currentSelectedObject].GetComponent<Kitchen>();
        int KitchenCurrentupdate = selectedKitchen.GetUpdateDetails(Kitchens[currentSelectedObject].GetComponent<Kitchen>().kitchenName);

        if ((KitchenCurrentupdate + 1) < 3 && CheckCanUpgrade(selectedKitchen.kitchenName, currentSelectedObject))
        {
            //Debug.Log(selectedKitchen.kitchenName + " " + currentSelectedObject + " " + canUpgrade);
            buildDeliveryPointParent.transform.GetChild(selectedKitchen.GetUpdateDetails(selectedKitchen.kitchenName) + 1).GetChild(0).GetChild(4).gameObject.SetActive(false);
            buildDeliveryPointParent.transform.GetChild(selectedKitchen.GetUpdateDetails(selectedKitchen.
               kitchenName) + 1).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.RemoveAllListeners();
            buildDeliveryPointParent.transform.GetChild(selectedKitchen.GetUpdateDetails(selectedKitchen.
                kitchenName) + 1).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectedKitchen.UpdateKitchen();
                });
        }
    }
    public void PlaceNewKitchen()
    {
        GameObject kitchenObj = Kitchens[currentSelectedObject];
        kitchenObj.SetActive(true);
        kitchenObj.transform.GetChild(kitchenObj.GetComponent<Kitchen>().CurrentUpdate).gameObject.SetActive(true);
        //ShawarmaSpawner.Instance.AddNewTarget(kitchenObj.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().Capacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentSelectedObject]);
        kitchenObj.name = "kitchen" + (currentSelectedObject + 1);// For changing gameobject name to see in hierarchy (optional)
        kitchenObj.GetComponent<Kitchen>().SetKitchenIsPurchased();
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

