using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class WarehouseManager : MonoBehaviour
{
    public GameObject[] warehouses; 
    public GameObject[] Tracks;
    public GameObject[] Positions;
    int currentWarehouseCount;
    public static WarehouseManager Instance;
    public List<GameObject> placedWarehouses = new List<GameObject>();
   // [SerializeField] int CurrentCash; // Temporary cash
    int currentSelectedObject;
    int requiredCostToUpgrade;
    bool canUpgrade;
    [Header("Belt Settings")]
    public Material BeltMat;
    public Vector2 scrollSpeed = new Vector2(0, 1f);
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
    public void ActionPerformedOneTime()
    {
        const string oneTimeKey = "OneTime";

        if (PlayerPrefs.GetInt(oneTimeKey) != 1)
        {
            PlayerPrefs.SetInt(oneTimeKey, 1);
          //  warehouses[0].GetComponent<Warehouse>().currentUpdate = 2;
            string purchaseKey = warehouses[0].GetComponent<Warehouse>().warehouseName + "Purchased";
            PlayerPrefs.SetInt(purchaseKey, 1);
        }

    }
    void Start()
    {
         Invoke("DelayOnStart", 1.1f);
    }

    public void DelayOnStart()
    {

        for (int i = 0; i < warehouses.Length; i++)
        {
            if (PlayerPrefs.GetInt(warehouses[i].GetComponent<Warehouse>().warehouseName + "Purchased") == 1)
            {
                currentSelectedObject = i;
                PlaceNewWarehouse();
                UpdateBuildNewWarehouseUI();
            }
        }
    }
    public void PlaceNewWarehouse()
    {
        GameObject WareHouse = warehouses[currentSelectedObject];
        WareHouse.SetActive(true);
        WareHouse.transform.position = Positions[currentWarehouseCount].transform.position;
        WareHouse.transform.GetChild(WareHouse.GetComponent<Warehouse>().currentUpdate-1).gameObject.SetActive(true);
        ShawarmaSpawner.Instance.AddNewTarget(WareHouse.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().currentCapacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentSelectedObject]);
        Tracks[currentWarehouseCount].SetActive(true);
        WareHouse.name = "warehouse" + (currentSelectedObject + 1);// For changing gameobject name to see in hierarchy (optional)
        WareHouse.GetComponent<Warehouse>().SetHouseIsPurchased();
        placedWarehouses.Add(WareHouse);
        currentWarehouseCount++;
      //  WareHouse.GetComponent<Warehouse>().AssignId(currentWarehouseCount);
    }
    public void AddWarehouseButtonClicked(int n)
    {
        SoundManager.Instance.PlayButtonClick();
        currentSelectedObject = n;
        if (placedWarehouses.Count < warehouses.Length && warehouses[currentSelectedObject].GetComponent<Warehouse>().cost < PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(warehouses[currentSelectedObject].GetComponent<Warehouse>().cost);
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);
            PlaceNewWarehouse();
            UpdateBuildNewWarehouseUI();
        }
        else
        {
            UIManager.Instance.lowCashPromt.SetActive(true);
            Debug.Log("Low CAsh");
        }
    }
 
    public void UpdateBuildNewWarehouseUI()
    {
        for (int i = 0; i < buidlNewPointParent.childCount; i++)
        {
            bool isPurchased = warehouses[i].GetComponent<Warehouse>().HouseIsPurchased /*&& CheckCanUpgrade(warehouses[i].GetComponent<Warehouse>().ScriptableWarehouseData.WarehouseName, i)*/;
            Transform point = buidlNewPointParent.GetChild(i);
            point.GetChild(0).gameObject.SetActive(!isPurchased);
            point.GetChild(1).gameObject.SetActive(isPurchased);
            point.GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = warehouses[i].GetComponent<Warehouse>().updates[warehouses[i].GetComponent<Warehouse>().currentUpdate-1].Icon;
        }
    }
    public void UpdateIcon(int warehosueNumber)
    {
        buidlNewPointParent.GetChild(warehosueNumber).GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = warehouses[warehosueNumber].GetComponent<Warehouse>().updates[warehouses[warehosueNumber].GetComponent<Warehouse>().currentUpdate - 1].Icon;
    }
    public void UpdateWarehoueUI(int n)
    {

        currentSelectedObject = n;
        Debug.Log("currentSelectedObject "+ currentSelectedObject);
        for (int i = 0; i < buildDeliveryPointParent.childCount; i++)
        {
            buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(4).gameObject.SetActive(true);
        }
        Warehouse selectedWarehouse = warehouses[currentSelectedObject].GetComponent<Warehouse>();
        int WarehouseCurrentupdate = selectedWarehouse.currentUpdate;

        if ((WarehouseCurrentupdate) < selectedWarehouse.updates.Count)
        {
            // Debug.Log(selectedWarehouse.warehouseName + " "+ currentSelectedObject+" "+ canUpgrade);
            buildDeliveryPointParent.transform.GetChild(selectedWarehouse.currentUpdate ).GetChild(0).GetChild(4).gameObject.SetActive(false);
            buildDeliveryPointParent.transform.GetChild(selectedWarehouse.currentUpdate ).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.RemoveAllListeners();
            buildDeliveryPointParent.transform.GetChild(selectedWarehouse.currentUpdate ).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectedWarehouse.UpdateWarehouse();
                });
        }
        SoundManager.Instance.PlayButtonClick();
    }
    private void Update()
    {
        Vector2 offset = BeltMat.mainTextureOffset;
        offset += scrollSpeed * Time.deltaTime;
        BeltMat.mainTextureOffset = offset;
    }
}
