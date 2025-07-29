using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance;
    public GameObject[] Deliverys; // Assign Deliverya GameObjects (only 1 active at start)
    int currentDeliveryCount;
    int currentSelectedObject;
    [SerializeField] int CurrentCash; // Temporary cash
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
        const string oneTimeKey = "OneTimeDelivery";

        if (PlayerPrefs.GetInt(oneTimeKey) != 1)
        {
            PlayerPrefs.SetInt(oneTimeKey, 1);
            string purchaseKey = Deliverys[0].GetComponent<Delivery>().DeliveryName + "Purchased";
            PlayerPrefs.SetInt(purchaseKey, 1);
        }
    }
    public void DelayOnStart()
   // public async UniTask DelayOnStart()
   
    {
        //await UniTask.NextFrame();
        for (int i = 0; i < Deliverys.Length; i++)
        {
            if (PlayerPrefs.GetInt(Deliverys[i].GetComponent<Delivery>().DeliveryName + "Purchased") == 1)
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
            bool isPurchased = Deliverys[i].GetComponent<Delivery>().DeliveryIsPurchased /*&& CheckCanUpgrade(warehouses[i].GetComponent<Warehouse>().ScriptableWarehouseData.WarehouseName, i)*/;
            Transform point = buidlNewPointParent.GetChild(i);
            point.GetChild(0).gameObject.SetActive(!isPurchased);
            point.GetChild(1).gameObject.SetActive(isPurchased);
            point.GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = Deliverys[i].GetComponent<Delivery>().updates[Deliverys[i].GetComponent<Delivery>().currentUpdate - 1].Icon;
        }
    }
    public void UpdateIcon(int DeliveryNumber)
    {
        buidlNewPointParent.GetChild(DeliveryNumber).GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = Deliverys[DeliveryNumber].GetComponent<Delivery>().updates[Deliverys[DeliveryNumber].GetComponent<Delivery>().currentUpdate - 1].Icon;
    }
    public void AddDeliveryButtonClicked(int n)
    {
        SoundManager.Instance.PlayButtonClick();
        currentSelectedObject = n;
        if (placedDeliverys.Count < Deliverys.Length && Deliverys[currentSelectedObject].GetComponent<Delivery>().cost < PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(Deliverys[currentSelectedObject].GetComponent<Delivery>().cost);
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);
            PlaceNewDelivery();
            UpdateBuildNewDeliveryUI();
        }
    }
    public void UpdateDeliveryUI(int n)
    {
        currentSelectedObject = n;
        Debug.Log("currentSelectedObject " + currentSelectedObject);
        for (int i = 0; i < buildDeliveryPointParent.childCount; i++)
        {
            buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(4).gameObject.SetActive(true);
        }
        Delivery selectedDelivery = Deliverys[currentSelectedObject].GetComponent<Delivery>();
        int DeliveryCurrentupdate = selectedDelivery.currentUpdate;

        if ((DeliveryCurrentupdate) < selectedDelivery.updates.Count && selectedDelivery.cost < CurrentCash)
        {
            buildDeliveryPointParent.transform.GetChild(selectedDelivery.currentUpdate).GetChild(0).GetChild(4).gameObject.SetActive(false);
            buildDeliveryPointParent.transform.GetChild(selectedDelivery.currentUpdate).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.RemoveAllListeners();
            buildDeliveryPointParent.transform.GetChild(selectedDelivery.currentUpdate).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedDelivery.UpdateDelivery();
            });
        }
        SoundManager.Instance.PlayButtonClick();
    }
    public void PlaceNewDelivery()
    {
        GameObject DeliveryObj = Deliverys[currentSelectedObject];
        DeliveryObj.SetActive(true);
        for (int i = 0; i < DeliveryObj.GetComponent<Delivery>().currentUpdate; i++)
        {
            DeliveryVanSpawner.Instance.vanPrefab.Add(DeliveryObj.GetComponent<Delivery>().DeliveryVanObjects[i]);
        }
     
        
        DeliveryObj.transform.GetChild(DeliveryObj.GetComponent<Delivery>().currentUpdate - 1).gameObject.SetActive(true);
        //ShawarmaSpawner.Instance.AddNewTarget(WareHouse.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().currentCapacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentSelectedObject]);
        DeliveryObj.name = "Delivery" + (currentSelectedObject + 1);// For changing gameobject name to see in hierarchy (optional)
        DeliveryObj.GetComponent<Delivery>().SetDeliveryIsPurchased();
        placedDeliverys.Add(DeliveryObj);
        currentDeliveryCount++;

    }
}

