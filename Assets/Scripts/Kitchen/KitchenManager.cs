using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KitchenManager : MonoBehaviour
{
    public static KitchenManager Instance;
    public GameObject[] Kitchens; // Assign Kitchena GameObjects (only 1 active at start)
    int currentKitchenCount;
    int currentSelectedObject;
   // [SerializeField] int CurrentCash; // Temporary cash
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
         Invoke("DelayOnStart", 1.1f);
       // DelayOnStart().Forget();
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
    public void DelayOnStart()
   // public async UniTask DelayOnStart()
   
    {
        //await UniTask.NextFrame();
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
            bool isPurchased = Kitchens[i].GetComponent<Kitchen>().KitchenIsPurchased /*&& CheckCanUpgrade(warehouses[i].GetComponent<Warehouse>().ScriptableWarehouseData.WarehouseName, i)*/;
            Transform point = buidlNewPointParent.GetChild(i);
            point.GetChild(0).gameObject.SetActive(!isPurchased);
            point.GetChild(1).gameObject.SetActive(isPurchased);
            point.GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = Kitchens[i].GetComponent<Kitchen>().updates[Kitchens[i].GetComponent<Kitchen>().currentUpdate - 1].Icon;
            point.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = Kitchens[i].GetComponent<Kitchen>().updates[Kitchens[i].GetComponent<Kitchen>().currentUpdate - 1].UpdateName;
        }
    }
    public void UpdateIcon(int KitchenNumber)
    {
        buidlNewPointParent.GetChild(KitchenNumber).GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = Kitchens[KitchenNumber].GetComponent<Kitchen>().updates[Kitchens[KitchenNumber].GetComponent<Kitchen>().currentUpdate - 1].Icon;
        buidlNewPointParent.GetChild(KitchenNumber).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = Kitchens[KitchenNumber].GetComponent<Kitchen>().updates[Kitchens[KitchenNumber].GetComponent<Warehouse>().currentUpdate - 1].UpdateName;
    }
    public void AddKitchenButtonClicked(int n)
    {
        SoundManager.Instance.PlayButtonClick();
        currentSelectedObject = n;
        if (placedKitchens.Count < Kitchens.Length && Kitchens[currentSelectedObject].GetComponent<Kitchen>().cost < PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(Kitchens[currentSelectedObject].GetComponent<Kitchen>().cost);
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);
            PlaceNewKitchen();
            UpdateBuildNewKitchenUI();
        }
        else
        {
            UIManager.Instance.lowCashPromt.SetActive(true);
            Debug.Log("Low CAsh");
        }
    }


    public void UpdateKitchenUI(int n)
    {
        currentSelectedObject = n;
        Debug.Log("currentSelectedObject " + currentSelectedObject);
        for (int i = 0; i < buildDeliveryPointParent.childCount; i++)
        {
            buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(4).gameObject.SetActive(true);
            buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
        }
        Kitchen selectedKitchen = Kitchens[currentSelectedObject].GetComponent<Kitchen>();
        int KitchenCurrentupdate = selectedKitchen.currentUpdate;

        if ((KitchenCurrentupdate) < selectedKitchen.updates.Count)
        {
            buildDeliveryPointParent.transform.GetChild(selectedKitchen.currentUpdate).GetChild(0).GetChild(4).gameObject.SetActive(false);
            buildDeliveryPointParent.transform.GetChild(selectedKitchen.currentUpdate).GetChild(0).GetChild(1).GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = selectedKitchen.cost.ToString("F0");
            buildDeliveryPointParent.transform.GetChild(selectedKitchen.currentUpdate).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.RemoveAllListeners();
            buildDeliveryPointParent.transform.GetChild(selectedKitchen.currentUpdate).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedKitchen.UpdateKitchen();
            });
        }
        SoundManager.Instance.PlayButtonClick();
    }

    public void UpdateCostText(int i)
    {
        Kitchen selectedKitchen = Kitchens[i].GetComponent<Kitchen>();
        if (selectedKitchen.currentUpdate < Kitchens.Length && selectedKitchen.currentUpdate < buildDeliveryPointParent.transform.childCount)
        {
            Debug.Log("currentUpdate" + selectedKitchen.currentUpdate);
            buildDeliveryPointParent.transform.GetChild(selectedKitchen.currentUpdate).GetChild(0).GetChild(1).GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = selectedKitchen.cost.ToString("F0");
        }
    }


    public void PlaceNewKitchen()
    {
        GameObject kitchenObj = Kitchens[currentSelectedObject];
        kitchenObj.SetActive(true); 
        kitchenObj.transform.GetChild(kitchenObj.GetComponent<Kitchen>().currentUpdate - 1).gameObject.SetActive(true);
        //ShawarmaSpawner.Instance.AddNewTarget(WareHouse.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().currentCapacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentSelectedObject]);
        kitchenObj.name = "Kitchen" + (currentSelectedObject + 1);// For changing gameobject name to see in hierarchy (optional)
        kitchenObj.GetComponent<Kitchen>().SetKitchenIsPurchased();
        placedKitchens.Add(kitchenObj);
        currentKitchenCount++;

    }
}

