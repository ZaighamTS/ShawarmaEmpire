using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class CateringManager : MonoBehaviour
{
    public static CateringManager Instance;
    public GameObject[] Caterings; // Assign Kitchena GameObjects (only 1 active at start)
    int currentCateringCount;
    int currentSelectedObject;
    [SerializeField] int CurrentCash; // Temporary cash
    private List<GameObject> placedCatering = new List<GameObject>();
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
        const string oneTimeKey = "OneTimeCatering";

        if (PlayerPrefs.GetInt(oneTimeKey) != 1)
        {
            PlayerPrefs.SetInt(oneTimeKey, 1);
            string purchaseKey = Caterings[0].GetComponent<Catering>().CateringName + "Purchased";
            PlayerPrefs.SetInt(purchaseKey, 1);
        }
    }
    public async UniTask DelayOnStart()
    {
        await UniTask.NextFrame();
        for (int i = 0; i < Caterings.Length; i++)
        {
            if (PlayerPrefs.GetInt(Caterings[i].GetComponent<Catering>().CateringName + "Purchased") == 1)
            {
                currentSelectedObject = i;
                PlaceNewCatering();
                UpdateBuildNewCateringUI();
            }
        }
    }
    public void UpdateBuildNewCateringUI()
    {
        for (int i = 0; i < buidlNewPointParent.childCount; i++)
        {
            bool isPurchased = Caterings[i].GetComponent<Catering>().CateringIsPurchased /*&& CheckCanUpgrade(warehouses[i].GetComponent<Warehouse>().ScriptableWarehouseData.WarehouseName, i)*/;
            Transform point = buidlNewPointParent.GetChild(i);
            point.GetChild(0).gameObject.SetActive(!isPurchased);
            point.GetChild(1).gameObject.SetActive(isPurchased);
        }
    }
    public void UpdateIcon(int CateringNumber)
    {
        buidlNewPointParent.GetChild(CateringNumber).GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = Caterings[CateringNumber].GetComponent<Catering>().updates[Caterings[CateringNumber].GetComponent<Catering>().currentUpdate - 1].Icon;
    }
    public void AddCateringButtonClicked(int n)
    {
        SoundManager.Instance.PlayButtonClick();
        currentSelectedObject = n;
        if (placedCatering.Count < Caterings.Length)
        {
            PlaceNewCatering();
            UpdateBuildNewCateringUI();
        }
    }
    public void UpdateCateringUI(int n)
    {
        currentSelectedObject = n;
        Debug.Log("currentSelectedObject " + currentSelectedObject);
        for (int i = 0; i < buildDeliveryPointParent.childCount; i++)
        {
            buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(4).gameObject.SetActive(true);
        }
        Catering selectedCatering = Caterings[currentSelectedObject].GetComponent<Catering>();
        int CateringCurrentupdate = selectedCatering.currentUpdate;

        if ((CateringCurrentupdate) < 3 && selectedCatering.cost < CurrentCash)
        {
            buildDeliveryPointParent.transform.GetChild(selectedCatering.currentUpdate).GetChild(0).GetChild(4).gameObject.SetActive(false);
            buildDeliveryPointParent.transform.GetChild(selectedCatering.currentUpdate).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.RemoveAllListeners();
            buildDeliveryPointParent.transform.GetChild(selectedCatering.currentUpdate).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedCatering.UpdateCatering();
            });
        }
        SoundManager.Instance.PlayButtonClick();
    }
    public void PlaceNewCatering()
    {
        GameObject CateringObj = Caterings[currentSelectedObject];
        CateringObj.SetActive(true);
        CateringObj.transform.GetChild(CateringObj.GetComponent<Catering>().currentUpdate - 1).gameObject.SetActive(true);
        //ShawarmaSpawner.Instance.AddNewTarget(WareHouse.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().currentCapacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentSelectedObject]);
        CateringObj.name = "Catering" + (currentSelectedObject + 1);// For changing gameobject name to see in hierarchy (optional)
        CateringObj.GetComponent<Catering>().SetCateringIsPurchased();
        placedCatering.Add(CateringObj);
        currentCateringCount++;

    }
}

