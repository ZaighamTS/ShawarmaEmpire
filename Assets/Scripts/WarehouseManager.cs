using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WarehouseManager : MonoBehaviour
{
    public GameObject[] warehouses; // Assign 4 warehouse GameObjects (only 1 active at start)
    public GameObject[] Tracks;
    
    //WarehouseData CurrentWareHouseData;
    int currentWarehouseCount;
    int CurrentWarehouseId;
    public static WarehouseManager Instance;
    private List<GameObject> placedWarehouses = new List<GameObject>();

    [Header("UI Setup")]
    public GameObject panel; // Assign the warehouse UI panel
    public Transform warehouseListParent; // ScrollView Content to hold buttons
    public GameObject warehouseItemPrefab; // UI item showing warehouse info
    public Button addWarehouseButton;
    
    [Header("Belt Settings")]
    public Material BeltMat;
    public Vector2 scrollSpeed = new Vector2(0, 1f);
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        currentWarehouseCount = 0;
        //Invoke("DelayOnStart",1);
        DelayOnStart();


    }
    public void DelayOnStart()
    {
        
        for (int i = 0; i < warehouses.Length; i++)
        {
            Debug.Log("qqqqqq");
            if (warehouses[i].GetComponent<Warehouse>().ScriptableWarehouseData.HouseIsPurchased)
            {
                Debug.Log("www");
                PlaceNewWarehouse();
                UpdateUI();
            }
        }
    }
    public void PlaceNewWarehouse()
    {
        Debug.Log("oooo");
        GameObject WareHouse = warehouses[currentWarehouseCount];
        WareHouse.SetActive(true);
        WareHouse.transform.GetChild(WareHouse.GetComponent<Warehouse>().CurrentUpdate).gameObject.SetActive(true);
        ShawarmaSpawner.Instance.AddNewTarget(WareHouse.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().Capacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentWarehouseCount]);
        Tracks[currentWarehouseCount].SetActive(true);
        WareHouse.name = "Warehouse" + (currentWarehouseCount + 1);// For changing gameobject name to see in hierarchy (optional)
        //WareHouse.GetComponent<Warehouse>().Init("Warehouse" + (currentWarehouseCount + 1));
        WareHouse.GetComponent<Warehouse>().ScriptableWarehouseData.SetUpdateDetails(WareHouse.name, 0);
        WareHouse.GetComponent<Warehouse>().ScriptableWarehouseData.SetHouseIsPurchased();
        placedWarehouses.Add(WareHouse);
        currentWarehouseCount++;
    }

    public void BuyNewWarehouse()
    { 
        
    
    }

    public void AddWarehouseButtonClicked()
    {
        if (placedWarehouses.Count < warehouses.Length)
        {
            PlaceNewWarehouse();
            UpdateUI();
        }
    }
    void UpdateUI()
    {
        // Clear existing UI items
        foreach (Transform child in warehouseListParent)
            Destroy(child.gameObject);

        // Populate panel with current warehouses
        for (int i = 0; i < placedWarehouses.Count; i++)
        {
            GameObject item = Instantiate(warehouseItemPrefab, warehouseListParent);
            Warehouse wh = placedWarehouses[i].GetComponent<Warehouse>();
            item.GetComponentInChildren<Text>().text = wh.warehouseName;

            int index = i; // avoid closure issue
            item.transform.Find("UpdateButton").GetComponent<Button>().onClick.AddListener(() => {
                wh.UpdateWarehouse(); // call your update logic here
            });
        }

        // Add warehouse button visibility
        addWarehouseButton.gameObject.SetActive(placedWarehouses.Count < warehouses.Length);
    }
    private void Update()
    {
        

        Vector2 offset = BeltMat.mainTextureOffset;
        offset += scrollSpeed * Time.deltaTime;
        BeltMat.mainTextureOffset = offset;
    }

    public void ShowUpgradePanel(Warehouse warehouse)
    {
        panel.SetActive(true);
        //upgradeTitleText.text = "Warehouse ID: " + warehouse.data.id + "\nCapacity: " + warehouse.data.capacity;
    }

    public void HideUpgradePanel()
    {
        panel.SetActive(false);
    }
}
