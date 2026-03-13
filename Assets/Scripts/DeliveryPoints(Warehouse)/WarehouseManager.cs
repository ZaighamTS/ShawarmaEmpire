using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Header("Upgrade Availability Indicator")]
    public GameObject upgradeBadgePrefab; // Badge to show on tab button (optional)
    public Button storageTabButton; // Tab button for storage upgrades (optional - for badge display)


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
    
    /// <summary>
    /// Checks if all warehouses are at full capacity
    /// Returns true only when at least one warehouse has capacity and all such warehouses are full.
    /// Warehouses with 0 capacity (unpurchased) are not considered "full".
    /// </summary>
    public bool AreAllWarehousesFull()
    {
        if (placedWarehouses == null || placedWarehouses.Count == 0)
            return false; // No warehouses placed yet
        
        bool hasAnyWithCapacity = false;
        foreach (var warehouse in placedWarehouses)
        {
            if (warehouse == null) continue;
            var warehouseComponent = warehouse.GetComponent<Warehouse>();
            if (warehouseComponent == null) continue;
            if (warehouseComponent.currentCapacity <= 0) continue; // Unpurchased, don't count as full
            hasAnyWithCapacity = true;
            if (warehouseComponent.currentLoad < warehouseComponent.currentCapacity)
                return false; // At least one warehouse with capacity has space
        }
        return hasAnyWithCapacity; // All warehouses with capacity are full (or none have capacity)
    }
    
    /// <summary>
    /// Gets the number of warehouses that are currently full (capacity > 0 and load >= capacity).
    /// </summary>
    public int GetFullWarehouseCount()
    {
        if (placedWarehouses == null || placedWarehouses.Count == 0)
            return 0;
        
        int fullCount = 0;
        foreach (var warehouse in placedWarehouses)
        {
            if (warehouse == null) continue;
            var warehouseComponent = warehouse.GetComponent<Warehouse>();
            if (warehouseComponent != null && warehouseComponent.currentCapacity > 0 && warehouseComponent.currentLoad >= warehouseComponent.currentCapacity)
                fullCount++;
        }
        return fullCount;
    }
    internal void DeliverShawarma(int value, int current)
    {
        if (placedWarehouses[current].transform.GetComponent<Warehouse>().currentLoad >= value)
        {
            placedWarehouses[current].transform.GetComponent<Warehouse>().currentLoad -= value;

        }
        else
        {
            placedWarehouses[current].transform.GetComponent<Warehouse>().currentLoad = 0;
        }
       // UpdateSliderCurrentValue(placedWarehouses[current].transform.GetComponent<Warehouse>().id,0, placedWarehouses[current].transform.GetComponent<Warehouse>().currentCapacity, placedWarehouses[current].transform.GetComponent<Warehouse>().currentLoad);
        ShawarmaSpawner.Instance.targets[current].CurrentLoad = placedWarehouses[current].transform.GetComponent<Warehouse>().currentLoad;
        ShawarmaSpawner.Instance.UpdateRecord(current);
        placedWarehouses[current].transform.GetComponent<Warehouse>().CheckWaring();
        ShawarmaSpawner.Instance.targets[current].CurrentLoad = placedWarehouses[current].transform.GetComponent<Warehouse>().currentLoad;
    }
    public async UniTask DelayOnStart()
    {
        await UniTask.NextFrame();
        ActionPerformedOneTime();
        for (int i = 0; i < warehouses.Length; i++)
        {
            
            if (warehouses[i].GetComponent<Warehouse>().currentUpdate >1)
            {
              //  Debug.Log("cHECK");
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
        WareHouse.transform.GetChild(WareHouse.GetComponent<Warehouse>().currentUpdate - 2).transform.GetComponent<DOTweenAnimation>().DOPlay();
        WareHouse.transform.GetChild(8).gameObject.SetActive(true);
        DisableEffect(WareHouse).Forget();
        
        Warehouse warehouseComponent = WareHouse.GetComponent<Warehouse>();
        
        // Set warehouse ID before adding to list (will be 1-based index)
        int warehouseIndex = placedWarehouses.Count; // Index before adding (0-based)
        warehouseComponent.MakePersistent(warehouseIndex + 1); // Set id to 1-based index
        
        // Ensure capacity is set correctly based on current update level BEFORE adding target
        int correctCapacity = (int)UpgradeCosts.GetDeliveryCapacity(CapacityType.Storage, warehouseComponent.currentUpdate);
        warehouseComponent.currentCapacity = correctCapacity;
        
        // Now add target with correct capacity and id
        ShawarmaSpawner.Instance.AddNewTarget(warehouseComponent.id, warehouseComponent.currentCapacity, warehouseComponent.TargetPosition, warehouses[currentSelectedObject], warehouseComponent.currentLoad);
       // UpdateSliderCurrentValue(WareHouse.GetComponent<Warehouse>().id, 0, WareHouse.GetComponent<Warehouse>().currentCapacity, WareHouse.GetComponent<Warehouse>().currentLoad);
        Tracks[currentWarehouseCount].SetActive(true);
        WareHouse.name = "warehouse" + (currentSelectedObject + 1);// For changing gameobject name to see in hierarchy (optional)
       // WareHouse.GetComponent<Warehouse>().SetHouseIsPurchased();
        
        warehouseComponent.cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Storage, warehouseComponent.currentUpdate);
        placedWarehouses.Add(WareHouse);
       
        currentWarehouseCount++;
        UpdateSlider(warehouseComponent.id, warehouseComponent.updates.Count, warehouseComponent.currentUpdate - 1);
        
        // Check warning state when warehouse is placed
        warehouseComponent.CheckWaring();
    }

    public void ShowAnimationEffect()
    {
        UIManager.Instance.DisableGameplayPanel();
        CameraSwipeController.instance.LerpCamera(warehouses[currentSelectedObject].transform.position.x + 10, warehouses[currentSelectedObject].transform.position.z + 15);
    }

    public async UniTask DisableEffect(GameObject WareHouse)
    {
        await UniTask.Delay(2000);
        WareHouse.transform.GetChild(8).gameObject.SetActive(false);

    }
    public void AddWarehouseButtonClicked(int n)
    {
        SoundManager.Instance.PlayButtonClick();
        currentSelectedObject = n;
        
        // EXTENDED GAMEPLAY: Calculate purchase cost based on how many warehouses are already placed
        // Uses 3.5x scaling: Warehouse 1 = $5000, Warehouse 2 = $17,500, Warehouse 3 = $61,250, etc.
        int existingWarehouseCount = placedWarehouses.Count;
        float purchaseCost = UpgradeCosts.GetPurchaseCost(UpgradeType.Storage, existingWarehouseCount);
        
        if (placedWarehouses.Count < warehouses.Length && purchaseCost <= PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(purchaseCost);
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);
            warehouses[currentSelectedObject].GetComponent<Warehouse>().currentUpdate++;
            //warehouses[currentSelectedObject].GetComponent<Warehouse>().IsDirty = true;
            warehouses[currentSelectedObject].GetComponent<Warehouse>().cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Storage, warehouses[currentSelectedObject].GetComponent<Warehouse>().currentUpdate);

            PlaceNewWarehouse();
           
            OnUpgradeItem();
            ShowAnimationEffect();





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


    public void UpdateSlider(int warehosueNumber, int maxValue, int currentValue)
    {
        buidlNewPointParent.GetChild(warehosueNumber).GetChild(1).GetChild(3).GetComponent<Slider>().maxValue = maxValue;
        buidlNewPointParent.GetChild(warehosueNumber).GetChild(1).GetChild(3).GetComponent<Slider>().value = currentValue;
    }

    public void UpdateIcon(int warehosueNumber)
    {
        buidlNewPointParent.GetChild(warehosueNumber).GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = warehouses[warehosueNumber].GetComponent<Warehouse>().updates[warehouses[warehosueNumber].GetComponent<Warehouse>().currentUpdate - 2].Icon;
        Debug.Log("Name "+ warehouses[warehosueNumber].GetComponent<Warehouse>().updates[warehouses[warehosueNumber].GetComponent<Warehouse>().currentUpdate - 2].UpdateName);
        buidlNewPointParent.GetChild(warehosueNumber).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = warehouses[warehosueNumber].GetComponent<Warehouse>().updates[warehouses[warehosueNumber].GetComponent<Warehouse>().currentUpdate - 2].UpdateName;
       // UpdateSliderCurrentValue(warehosueNumber,0, warehouses[warehosueNumber].GetComponent<Warehouse>().currentCapacity, warehouses[warehosueNumber].GetComponent<Warehouse>().currentLoad);
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
           // UpdateSliderCurrentValue(selectedWarehouse.currentUpdate - 1, 0, warehouses[selectedWarehouse.currentUpdate - 1].GetComponent<Warehouse>().currentCapacity, warehouses[selectedWarehouse.currentUpdate - 1].GetComponent<Warehouse>().currentLoad);
            // Debug.Log(selectedWarehouse.warehouseName + " "+ currentSelectedObject+" "+ canUpgrade);
            buildDeliveryPointParent.transform.GetChild(selectedWarehouse.currentUpdate-1 ).GetChild(0).GetChild(4).gameObject.SetActive(false);
            
            // FIXED: Show purchase cost for unpurchased warehouses, upgrade cost for purchased ones
            float costToDisplay;
            if (selectedWarehouse.currentUpdate <= 1)
            {
                // Unpurchased warehouse - show purchase cost
                int existingCount = placedWarehouses.Count;
                costToDisplay = UpgradeCosts.GetPurchaseCost(UpgradeType.Storage, existingCount);
            }
            else
            {
                // Purchased warehouse - show upgrade cost
                costToDisplay = selectedWarehouse.cost;
            }
            buildDeliveryPointParent.transform.GetChild(selectedWarehouse.currentUpdate-1).GetChild(0).GetChild(1).GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = costToDisplay.ToString("F0");
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
    
    /// <summary>
    /// Counts how many storage upgrades are currently affordable
    /// </summary>
    public int GetAvailableUpgradeCount()
    {
        int count = 0;
        float playerCash = PlayerProgress.Instance.PlayerCash;
        
        if (warehouses == null) return 0;
        
        foreach (var warehouse in warehouses)
        {
            if (warehouse == null) continue;
            
            Warehouse w = warehouse.GetComponent<Warehouse>();
            if (w == null) continue;
            
            // Check if can purchase new warehouse
            if (w.currentUpdate <= 1)
            {
                int existingCount = placedWarehouses.Count;
                float cost = UpgradeCosts.GetPurchaseCost(UpgradeType.Storage, existingCount);
                if (cost <= playerCash) count++;
            }
            // Check if can upgrade existing warehouse
            else if (w.currentUpdate <= w.updates.Count && w.cost <= playerCash)
            {
                count++;
            }
        }
        
        return count;
    }
    
    /// <summary>
    /// Navigates to the first affordable storage upgrade and highlights it
    /// </summary>
    public void NavigateToFirstAffordableUpgrade()
    {
        if (warehouses == null) return;
        
        float playerCash = PlayerProgress.Instance.PlayerCash;
        
        // Find first affordable warehouse upgrade
        for (int i = 0; i < warehouses.Length; i++)
        {
            if (warehouses[i] == null) continue;
            
            Warehouse w = warehouses[i].GetComponent<Warehouse>();
            if (w == null) continue;
            
            float cost = w.currentUpdate <= 1 
                ? UpgradeCosts.GetPurchaseCost(UpgradeType.Storage, placedWarehouses.Count)
                : w.cost;
            
            if (cost <= playerCash)
            {
                // Select this warehouse and show its upgrade UI
                UpdateWarehoueUI(i);
                
                // Highlight the upgrade button with pulsing animation
                if (buildDeliveryPointParent != null && w.currentUpdate - 1 >= 0 && w.currentUpdate - 1 < buildDeliveryPointParent.childCount)
                {
                    Transform upgradeButton = buildDeliveryPointParent.GetChild(w.currentUpdate - 1);
                    if (upgradeButton != null)
                    {
                        HighlightUpgradeButton(upgradeButton);
                    }
                }
                break;
            }
        }
    }
    
    /// <summary>
    /// Adds a pulsing highlight effect to an upgrade button
    /// </summary>
    private void HighlightUpgradeButton(Transform buttonTransform)
    {
        if (buttonTransform == null) return;
        
        Button button = buttonTransform.GetComponentInChildren<Button>();
        if (button != null)
        {
            // Stop any existing animations
            DG.Tweening.DOTween.Kill(button.transform);
            
            // Add pulsing scale animation
            button.transform.DOScale(1.15f, 0.5f)
                .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
                .SetEase(DG.Tweening.Ease.InOutSine);
        }
    }
}
