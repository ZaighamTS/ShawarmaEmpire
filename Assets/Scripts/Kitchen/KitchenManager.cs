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
            Kitchens[0].GetComponent<Kitchen>().currentUpdate = 2;
           
        }
    }
    public void DelayOnStart()
   // public async UniTask DelayOnStart()
   
    {
        ActionPerformedOneTime();
        //await UniTask.NextFrame();
        for (int i = 0; i < Kitchens.Length; i++)
        {
            if (Kitchens[i].GetComponent<Kitchen>().currentUpdate > 1)
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
            bool isPurchased;

            Transform point = buidlNewPointParent.GetChild(i);
            if (Kitchens[i].GetComponent<Kitchen>().currentUpdate > 1)
            {
                isPurchased = true;
               // Debug.Log("aa " + (Kitchens[i].GetComponent<Kitchen>().currentUpdate - 2).ToString());
                point.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = Kitchens[i].GetComponent<Kitchen>().updates[Kitchens[i].GetComponent<Kitchen>().currentUpdate - 2].UpdateName;
                point.GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = Kitchens[i].GetComponent<Kitchen>().updates[Kitchens[i].GetComponent<Kitchen>().currentUpdate - 2].Icon;
            }
            else
            {
                isPurchased = false;
            }

            point.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = Kitchens[i].GetComponent<Kitchen>().cost.ToString();
            point.GetChild(0).gameObject.SetActive(!isPurchased);
            point.GetChild(1).gameObject.SetActive(isPurchased);


        }
    }
    public void UpdateIcon(int KitchenNumber)
    {
        buidlNewPointParent.GetChild(KitchenNumber).GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = Kitchens[KitchenNumber].GetComponent<Kitchen>().updates[Kitchens[KitchenNumber].GetComponent<Kitchen>().currentUpdate - 2].Icon;
        buidlNewPointParent.GetChild(KitchenNumber).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = Kitchens[KitchenNumber].GetComponent<Kitchen>().updates[Kitchens[KitchenNumber].GetComponent<Kitchen>().currentUpdate - 2].UpdateName;
    }
    public void AddKitchenButtonClicked(int n)
    {
        SoundManager.Instance.PlayButtonClick();
        currentSelectedObject = n;
        if (placedKitchens.Count < Kitchens.Length && Kitchens[currentSelectedObject].GetComponent<Kitchen>().cost < PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(Kitchens[currentSelectedObject].GetComponent<Kitchen>().cost);
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);

            Kitchens[currentSelectedObject].GetComponent<Kitchen>().currentUpdate++;
            Kitchens[currentSelectedObject].GetComponent<Kitchen>().cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Kitchen, Kitchens[currentSelectedObject].GetComponent<Kitchen>().currentUpdate);

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
        int KitchenCurrentupdate = selectedKitchen.currentUpdate-1;

        if ((KitchenCurrentupdate) < selectedKitchen.updates.Count)
        {
            buildDeliveryPointParent.transform.GetChild(selectedKitchen.currentUpdate-1).GetChild(0).GetChild(4).gameObject.SetActive(false);
            buildDeliveryPointParent.transform.GetChild(selectedKitchen.currentUpdate-1).GetChild(0).GetChild(1).GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = selectedKitchen.cost.ToString("F0");
            buildDeliveryPointParent.transform.GetChild(selectedKitchen.currentUpdate-1).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.RemoveAllListeners();
            buildDeliveryPointParent.transform.GetChild(selectedKitchen.currentUpdate-1).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.AddListener(() =>
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
        kitchenObj.transform.GetChild(kitchenObj.GetComponent<Kitchen>().currentUpdate - 2).gameObject.SetActive(true);
        //ShawarmaSpawner.Instance.AddNewTarget(WareHouse.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().currentCapacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentSelectedObject]);
        kitchenObj.name = "Kitchen" + (currentSelectedObject + 1);// For changing gameobject name to see in hierarchy (optional)
        //kitchenObj.GetComponent<Kitchen>().SetKitchenIsPurchased();
        kitchenObj.GetComponent<Kitchen>().cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Kitchen, kitchenObj.GetComponent<Kitchen>().currentUpdate);
        placedKitchens.Add(kitchenObj);
        currentKitchenCount++;
        kitchenObj.GetComponent<Kitchen>().MakePersistent(currentKitchenCount);

    }
}

