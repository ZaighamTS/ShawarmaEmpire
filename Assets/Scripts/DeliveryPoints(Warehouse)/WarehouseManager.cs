using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class WarehouseManager : Upgdradable
{
   // public GameObject[] warehouses; 
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
   // public Transform buidlNewPointParent;
    public Transform buildDeliveryPointParent;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
       
    }
    public void ActionPerformedOneTime()
    {
        const string oneTimeKey = "OneTime";

        if (PlayerPrefs.GetInt(oneTimeKey) != 1)
        {
            PlayerPrefs.SetInt(oneTimeKey, 1);
            warehouses[0].GetComponent<Warehouse>().currentUpdate = 2;
            string purchaseKey = warehouses[0].GetComponent<Warehouse>().warehouseName + "Purchased";
            PlayerPrefs.SetInt(purchaseKey, 1);
        }

    }
    void Start()
    {
         Invoke("DelayOnStart", 1.1f);
    }
    protected override void OnUpgradeItem()
    {

        base.OnUpgradeItem();
    }
    public int GetWholeCapacity()
    { 
        return placedWarehouses.Sum(w => w.GetComponent<Warehouse>().currentCapacity);
    }
    public int GetWholeLoad()
    {
        return placedWarehouses.Sum(w => w.GetComponent<Warehouse>().currentLoad);
    }
    internal void DeliverShawarma(int value, int current)
    {
        if (placedWarehouses[current].transform.GetComponent<Warehouse>().currentLoad > value)
        {
            placedWarehouses[current].transform.GetComponent<Warehouse>().currentLoad -= value;

        }
        else
        {
            placedWarehouses[current].transform.GetComponent<Warehouse>().currentLoad = 0;
        }
        ShawarmaSpawner.Instance.targets[current].CurrentLoad = placedWarehouses[current].transform.GetComponent<Warehouse>().currentLoad;
        ShawarmaSpawner.Instance.UpdateRecord(current);
        placedWarehouses[current].transform.GetComponent<Warehouse>().CheckWaring();
        ShawarmaSpawner.Instance.targets[current].CurrentLoad = placedWarehouses[current].transform.GetComponent<Warehouse>().currentLoad;
    }
    public void DelayOnStart()
    {
        
        ActionPerformedOneTime();

        for (int i = 0; i < warehouses.Length; i++)
        {
            
            if (warehouses[i].GetComponent<Warehouse>().currentUpdate >1)
            {
                Debug.Log("cHECK");
                currentSelectedObject = i;
                PlaceNewWarehouse();
                OnUpgradeItem();

            }
        }
    }
    public void PlaceNewWarehouse()
    {
        GameObject WareHouse = warehouses[currentSelectedObject];
        WareHouse.SetActive(true);
        WareHouse.transform.position = Positions[currentWarehouseCount].transform.position;
        WareHouse.transform.GetChild(WareHouse.GetComponent<Warehouse>().currentUpdate-2).gameObject.SetActive(true);
        
        ShawarmaSpawner.Instance.AddNewTarget(WareHouse.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().currentCapacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentSelectedObject], WareHouse.GetComponent<Warehouse>().currentLoad);
        Tracks[currentWarehouseCount].SetActive(true);
        WareHouse.name = "warehouse" + (currentSelectedObject + 1);// For changing gameobject name to see in hierarchy (optional)
       // WareHouse.GetComponent<Warehouse>().SetHouseIsPurchased();
        WareHouse.GetComponent<Warehouse>().cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Storage, WareHouse.GetComponent<Warehouse>().currentUpdate);
        placedWarehouses.Add(WareHouse);
        currentWarehouseCount++;
        WareHouse.GetComponent<Warehouse>().MakePersistent(currentWarehouseCount);
    }
    public void AddWarehouseButtonClicked(int n)
    {
        SoundManager.Instance.PlayButtonClick();
        currentSelectedObject = n;
        if (placedWarehouses.Count < warehouses.Length && warehouses[currentSelectedObject].GetComponent<Warehouse>().cost < PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(warehouses[currentSelectedObject].GetComponent<Warehouse>().cost);
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);
            warehouses[currentSelectedObject].GetComponent<Warehouse>().currentUpdate++;
            //warehouses[currentSelectedObject].GetComponent<Warehouse>().IsDirty = true;
            warehouses[currentSelectedObject].GetComponent<Warehouse>().cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Storage, warehouses[currentSelectedObject].GetComponent<Warehouse>().currentUpdate);

            PlaceNewWarehouse();
           
            OnUpgradeItem();

        }
        else
        {
            UIManager.Instance.lowCashPromt.SetActive(true);
            Debug.Log("Low CAsh");
        }
    }
 
    public void UpdateBuildNewWarehouseUI()
    {
        //for (int i = 0; i < buidlNewPointParent.childCount; i++)
        //{
        //    bool isPurchased = warehouses[i].GetComponent<Warehouse>().HouseIsPurchased /*&& CheckCanUpgrade(warehouses[i].GetComponent<Warehouse>().ScriptableWarehouseData.WarehouseName, i)*/;
        //    Transform point = buidlNewPointParent.GetChild(i);
        //    point.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text= warehouses[i].GetComponent<Warehouse>().cost.ToString();
        //    point.GetChild(0).gameObject.SetActive(!isPurchased);
        //    point.GetChild(1).gameObject.SetActive(isPurchased);
        //    point.GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = warehouses[i].GetComponent<Warehouse>().updates[warehouses[i].GetComponent<Warehouse>().currentUpdate-1].Icon;
        //}
    }
    public void UpdateIcon(int warehosueNumber)
    {
        buidlNewPointParent.GetChild(warehosueNumber).GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = warehouses[warehosueNumber].GetComponent<Warehouse>().updates[warehouses[warehosueNumber].GetComponent<Warehouse>().currentUpdate - 2].Icon;
        Debug.Log("Name "+ warehouses[warehosueNumber].GetComponent<Warehouse>().updates[warehouses[warehosueNumber].GetComponent<Warehouse>().currentUpdate - 2].UpdateName);
        buidlNewPointParent.GetChild(warehosueNumber).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = warehouses[warehosueNumber].GetComponent<Warehouse>().updates[warehouses[warehosueNumber].GetComponent<Warehouse>().currentUpdate - 2].UpdateName;
    }
    public void UpdateWarehoueUI(int n)
    {

        currentSelectedObject = n;
        Debug.Log("currentSelectedObject "+ currentSelectedObject);
        for (int i = 0; i < buildDeliveryPointParent.childCount; i++)
        {
            buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(4).gameObject.SetActive(true);
            buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
        }
        Warehouse selectedWarehouse = warehouses[currentSelectedObject].GetComponent<Warehouse>();
        int WarehouseCurrentupdate = selectedWarehouse.currentUpdate-1;

        if ((WarehouseCurrentupdate) < selectedWarehouse.updates.Count)
        {
            // Debug.Log(selectedWarehouse.warehouseName + " "+ currentSelectedObject+" "+ canUpgrade);
            buildDeliveryPointParent.transform.GetChild(selectedWarehouse.currentUpdate-1 ).GetChild(0).GetChild(4).gameObject.SetActive(false);
            buildDeliveryPointParent.transform.GetChild(selectedWarehouse.currentUpdate-1).GetChild(0).GetChild(1).GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = selectedWarehouse.cost.ToString("F0");
            buildDeliveryPointParent.transform.GetChild(selectedWarehouse.currentUpdate-1 ).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.RemoveAllListeners();
            buildDeliveryPointParent.transform.GetChild(selectedWarehouse.currentUpdate-1 ).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectedWarehouse.UpdateWarehouse();
                });
        }
        SoundManager.Instance.PlayButtonClick();
    }

    public void UpdateCostText(int i)
    {
        Warehouse selectedWarehouse = warehouses[i].GetComponent<Warehouse>();
        if (selectedWarehouse.currentUpdate < warehouses.Length&& selectedWarehouse.currentUpdate < buildDeliveryPointParent.transform.childCount)
        {      
            Debug.Log("currentUpdate" + selectedWarehouse.currentUpdate);
            buildDeliveryPointParent.transform.GetChild(selectedWarehouse.currentUpdate).GetChild(0).GetChild(1).GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = selectedWarehouse.cost.ToString("F0");    
        }
    }
   
    private void Update()
    {
        //Debug.Log("here");
        Vector2 offset = BeltMat.mainTextureOffset;
        offset += scrollSpeed * Time.deltaTime;
        BeltMat.mainTextureOffset = offset;
    }
}
