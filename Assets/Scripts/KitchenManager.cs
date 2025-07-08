using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class KitchenManager : MonoBehaviour
{
    public GameObject[] Kitchens; // Assign 4 Kitchena GameObjects (only 1 active at start)
   
    //WarehouseData CurrentWareHouseData;
    int currentLitchenCount;
    int CurrentKitchenId;
    public static KitchenManager Instance;
    private List<GameObject> placedKitchens = new List<GameObject>();

    [Header("UI Setup")]
    public GameObject KitchenPanel; // Assign the Kitchen UI panel
    public Transform KitchenListParent; // ScrollView Content to hold buttons
    public GameObject KitchenItemPrefab; // UI item showing warehouse info
    public Button addKitchenButton;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        currentLitchenCount = 0;
        //Invoke("DelayOnStart",1);
        DelayOnStart();


    }
    public void DelayOnStart()
    {
        
        for (int i = 0; i < Kitchens.Length; i++)
        {
           
            if (Kitchens[i].GetComponent<Kitchen>().ScriptableKitchenData.KitchenIsPurchased)
            {
                PlaceNewKitchen();
                UpdateUI();
            }
        }
    }
    public void PlaceNewKitchen()
    {
       
        GameObject Kitchen = Kitchens[currentLitchenCount];
        Kitchen.SetActive(true);
       // Kitchen.transform.GetChild(Kitchen.GetComponent<Kitchen>().CurrentUpdate).gameObject.SetActive(true);
        //  ShawarmaSpawner.Instance.AddNewTarget(WareHouse.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().Capacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentWarehouseCount]);

        Kitchen.name = "Kitchen" + (currentLitchenCount + 1);// For changing gameobject name to see in hierarchy (optional)
    
        Kitchen.GetComponent<Kitchen>().ScriptableKitchenData.SetUpdateDetails(Kitchen.name, 0);
        Kitchen.GetComponent<Kitchen>().ScriptableKitchenData.SetHouseIsPurchased();
        placedKitchens.Add(Kitchen);
        currentLitchenCount++;
    }

    public void BuyNewKitchen()
    { 
        
    
    }

    public void AddWarehouseButtonClicked()
    {

        PlaceNewKitchen();
         UpdateUI();
        
    }
    void UpdateUI()
    {
        // Clear existing UI items
        foreach (Transform child in KitchenListParent)
            Destroy(child.gameObject);

        // Populate panel with current kitchen
        for (int i = 0; i < placedKitchens.Count; i++)
        {
            GameObject item = Instantiate(KitchenItemPrefab, KitchenListParent);
            Kitchen K = placedKitchens[i].GetComponent<Kitchen>();
            item.GetComponentInChildren<Text>().text = K.KitchenName;

            int index = i; // avoid closure issue
            item.transform.Find("UpdateButton").GetComponent<Button>().onClick.AddListener(() => {
            K.UpdateKitchen(); // call your update logic here
            });
        }

        // Add Kitchen button visibility
        addKitchenButton.gameObject.SetActive(placedKitchens.Count < Kitchens.Length);
    }
   

    public void ShowUpgradePanel(Warehouse warehouse)
    {
        KitchenPanel.SetActive(true);
        //upgradeTitleText.text = "Warehouse ID: " + warehouse.data.id + "\nCapacity: " + warehouse.data.capacity;
    }

    public void HideUpgradePanel()
    {
        KitchenPanel.SetActive(false);
    }
}
