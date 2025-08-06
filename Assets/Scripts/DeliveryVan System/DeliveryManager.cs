using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance;
    public GameObject[] Deliverys; // Assign Deliverya GameObjects (only 1 active at start)
    int currentDeliveryCount;
    int currentSelectedObject;
   // [SerializeField] int CurrentCash; // Temporary cash
    private List<GameObject> placedDeliverys = new List<GameObject>();
    [Header("UI References")]
    public Transform buidlNewPointParent;
    public Transform buildDeliveryPointParent;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
       // ActionPerformedOneTime();
    }
    void Start()
    {
        //Invoke("DelayOnStart",1);
         Invoke("DelayOnStart", 1.1f);
       // DelayOnStart().Forget();
    }
    public void ActionPerformedOneTime()
    {
        const string oneTimeKey = "OneTimeDelivery";

        if (PlayerPrefs.GetInt(oneTimeKey) != 1)
        {
            PlayerPrefs.SetInt(oneTimeKey, 1);
            Deliverys[0].GetComponent<Delivery>().currentUpdate = 2;
        }
    }
    public void DelayOnStart()
   // public async UniTask DelayOnStart()
   
    {
        ActionPerformedOneTime();
        //await UniTask.NextFrame();
        for (int i = 0; i < Deliverys.Length; i++)
        {
            if (Deliverys[i].GetComponent<Delivery>().currentUpdate > 1)
            {
                currentSelectedObject = i;
                PlaceNewDelivery();
                UpdateBuildNewDeliveryUI();
            }
        }
    }
    public void UpdateBuildNewDeliveryUI()
    {


        for (int i = 0; i < buidlNewPointParent.childCount; i++)
        {
            bool isPurchased;

            Transform point = buidlNewPointParent.GetChild(i);
            if (Deliverys[i].GetComponent<Delivery>().currentUpdate > 1)
            {
                isPurchased = true;
                Debug.Log("aa " + (Deliverys[i].GetComponent<Delivery>().currentUpdate - 2).ToString());
                point.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = Deliverys[i].GetComponent<Delivery>().updates[Deliverys[i].GetComponent<Delivery>().currentUpdate - 2].UpdateName;
                point.GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = Deliverys[i].GetComponent<Delivery>().updates[Deliverys[i].GetComponent<Delivery>().currentUpdate - 2].Icon;
            }
            else
            {
                isPurchased = false;
            }

            point.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = Deliverys[i].GetComponent<Delivery>().cost.ToString();
            point.GetChild(0).gameObject.SetActive(!isPurchased);
            point.GetChild(1).gameObject.SetActive(isPurchased);

        }
    }
    public void UpdateIcon(int DeliveryNumber)
    {
        buidlNewPointParent.GetChild(DeliveryNumber).GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = Deliverys[DeliveryNumber].GetComponent<Delivery>().updates[Deliverys[DeliveryNumber].GetComponent<Delivery>().currentUpdate - 2].Icon;
        buidlNewPointParent.GetChild(DeliveryNumber).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = Deliverys[DeliveryNumber].GetComponent<Delivery>().updates[Deliverys[DeliveryNumber].GetComponent<Delivery>().currentUpdate - 2].UpdateName;
    }
    public void AddDeliveryButtonClicked(int n)
    {
        SoundManager.Instance.PlayButtonClick();
        currentSelectedObject = n;
        if (placedDeliverys.Count < Deliverys.Length && Deliverys[currentSelectedObject].GetComponent<Delivery>().cost < PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(Deliverys[currentSelectedObject].GetComponent<Delivery>().cost);
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);

            Deliverys[currentSelectedObject].GetComponent<Delivery>().currentUpdate++;
            Deliverys[currentSelectedObject].GetComponent<Delivery>().cost = UpgradeCosts.GetUpgradeCost(UpgradeType.DeliveryVan, Deliverys[currentSelectedObject].GetComponent<Delivery>().currentUpdate);

            PlaceNewDelivery();
            UpdateBuildNewDeliveryUI();
        }
        else
        {
            UIManager.Instance.lowCashPromt.SetActive(true);
            Debug.Log("Low CAsh");
        }
    }
    public void UpdateDeliveryUI(int n)
    {
        currentSelectedObject = n;
        Debug.Log("currentSelectedObject " + currentSelectedObject);
        for (int i = 0; i < buildDeliveryPointParent.childCount; i++)
        {
            buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(4).gameObject.SetActive(true);
            buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
        }
        Delivery selectedDelivery = Deliverys[currentSelectedObject].GetComponent<Delivery>();
        int DeliveryCurrentupdate = selectedDelivery.currentUpdate-1;
       
        if ((DeliveryCurrentupdate) < selectedDelivery.updates.Count )
        {
            buildDeliveryPointParent.transform.GetChild(selectedDelivery.currentUpdate-1).GetChild(0).GetChild(4).gameObject.SetActive(false);
            buildDeliveryPointParent.transform.GetChild(selectedDelivery.currentUpdate-1).GetChild(0).GetChild(1).GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = selectedDelivery.cost.ToString("F0");
            buildDeliveryPointParent.transform.GetChild(selectedDelivery.currentUpdate-1).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.RemoveAllListeners();
            buildDeliveryPointParent.transform.GetChild(selectedDelivery.currentUpdate-1).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedDelivery.UpdateDelivery();
            });
        }
        SoundManager.Instance.PlayButtonClick();

    }
    public void UpdateCostText(int i)
    {
        Delivery selectedDelivery = Deliverys[i].GetComponent<Delivery>();
        if (selectedDelivery.currentUpdate < Deliverys.Length && selectedDelivery.currentUpdate < buildDeliveryPointParent.transform.childCount)
        {
            Debug.Log("currentUpdate" + selectedDelivery.currentUpdate);
            buildDeliveryPointParent.transform.GetChild(selectedDelivery.currentUpdate).GetChild(0).GetChild(1).GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = selectedDelivery.cost.ToString("F0");
        }
    }
    public void PlaceNewDelivery()
    {
        GameObject DeliveryObj = Deliverys[currentSelectedObject];
        DeliveryObj.SetActive(true);
       
        for (int i = 0; i < DeliveryObj.GetComponent<Delivery>().currentUpdate-1; i++)
        {
          
            DeliveryVanSpawner.Instance.vanPrefab.Add(DeliveryObj.GetComponent<Delivery>().DeliveryVanObjects[i]);
            
        }
        DeliveryVanSpawner.Instance.spawnInterval = UpgradeCosts.GetDeliveryInterval(DeliveryObj.GetComponent<Delivery>().currentUpdate - 1);

        DeliveryObj.transform.GetChild(DeliveryObj.GetComponent<Delivery>().currentUpdate - 2).gameObject.SetActive(true);
        //ShawarmaSpawner.Instance.AddNewTarget(WareHouse.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().currentCapacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentSelectedObject]);
        DeliveryObj.name = "Delivery" + (currentSelectedObject + 1);// For changing gameobject name to see in hierarchy (optional)
       // DeliveryObj.GetComponent<Delivery>().SetDeliveryIsPurchased();
        DeliveryObj.GetComponent<Delivery>().cost = UpgradeCosts.GetUpgradeCost(UpgradeType.DeliveryVan, DeliveryObj.GetComponent<Delivery>().currentUpdate);
        placedDeliverys.Add(DeliveryObj);
        currentDeliveryCount++;

    }
}

