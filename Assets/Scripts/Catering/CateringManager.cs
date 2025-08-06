using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CateringManager : MonoBehaviour
{
    public static CateringManager Instance;
    public GameObject[] Caterings; 
    int currentCateringCount;
    int currentSelectedObject;
    //[SerializeField] int CurrentCash; // Temporary cash
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
       // ActionPerformedOneTime();
    }
    void Start()
    {
        //Invoke("DelayOnStart",1);
        Invoke("DelayOnStart", 1.1f);
    }
    public void ActionPerformedOneTime()
    {
        const string oneTimeKey = "OneTimeCatering";

        if (PlayerPrefs.GetInt(oneTimeKey) != 1)
        {
            PlayerPrefs.SetInt(oneTimeKey, 1);
            Caterings[0].GetComponent<Catering>().currentUpdate = 2;
        }
    }
    public void DelayOnStart()
    {
        ActionPerformedOneTime();
        for (int i = 0; i < Caterings.Length; i++)
        {
            if (Caterings[i].GetComponent<Catering>().currentUpdate > 1)
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
            bool isPurchased;

            Transform point = buidlNewPointParent.GetChild(i);
            if (Caterings[i].GetComponent<Catering>().currentUpdate > 1)
            {
                isPurchased = true;
                Debug.Log("aa " + (Caterings[i].GetComponent<Catering>().currentUpdate - 2).ToString());
                point.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = Caterings[i].GetComponent<Catering>().updates[Caterings[i].GetComponent<Catering>().currentUpdate - 2].UpdateName;
                point.GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = Caterings[i].GetComponent<Catering>().updates[Caterings[i].GetComponent<Catering>().currentUpdate - 2].Icon;
            }
            else
            {
                isPurchased = false;
            }

            point.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = Caterings[i].GetComponent<Catering>().cost.ToString();
            point.GetChild(0).gameObject.SetActive(!isPurchased);
            point.GetChild(1).gameObject.SetActive(isPurchased);


        }
    }
    public void UpdateIcon(int CateringNumber)
    {
        buidlNewPointParent.GetChild(CateringNumber).GetChild(1).GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = Caterings[CateringNumber].GetComponent<Catering>().updates[Caterings[CateringNumber].GetComponent<Catering>().currentUpdate - 2].Icon;
        buidlNewPointParent.GetChild(CateringNumber).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = Caterings[CateringNumber].GetComponent<Catering>().updates[Caterings[CateringNumber].GetComponent<Catering>().currentUpdate - 2].UpdateName;
    }
    public void AddCateringButtonClicked(int n)
    {
        SoundManager.Instance.PlayButtonClick();
        currentSelectedObject = n;
        if (placedCatering.Count < Caterings.Length && Caterings[currentSelectedObject].GetComponent<Catering>().cost < PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(Caterings[currentSelectedObject].GetComponent<Catering>().cost);
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);

            Caterings[currentSelectedObject].GetComponent<Catering>().currentUpdate++;
            Caterings[currentSelectedObject].GetComponent<Catering>().cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Catering, Caterings[currentSelectedObject].GetComponent<Catering>().currentUpdate);

            PlaceNewCatering();
            UpdateBuildNewCateringUI();
        }
        else
        {
            UIManager.Instance.lowCashPromt.SetActive(true);
            Debug.Log("Low CAsh");
        }
    }
    public void UpdateCateringUI(int n)
    {
        currentSelectedObject = n;
        Debug.Log("currentSelectedObject " + currentSelectedObject);
        for (int i = 0; i < buildDeliveryPointParent.childCount; i++)
        {
            buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(4).gameObject.SetActive(true);
            buildDeliveryPointParent.transform.GetChild(i).GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
        }
        Catering selectedCatering = Caterings[currentSelectedObject].GetComponent<Catering>();
        int CateringCurrentupdate = selectedCatering.currentUpdate-1;

        if ((CateringCurrentupdate) < selectedCatering.updates.Count)
        {
            buildDeliveryPointParent.transform.GetChild(selectedCatering.currentUpdate-1).GetChild(0).GetChild(4).gameObject.SetActive(false);
            buildDeliveryPointParent.transform.GetChild(selectedCatering.currentUpdate-1).GetChild(0).GetChild(1).GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = selectedCatering.cost.ToString("F0");
            buildDeliveryPointParent.transform.GetChild(selectedCatering.currentUpdate-1).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.RemoveAllListeners();
            buildDeliveryPointParent.transform.GetChild(selectedCatering.currentUpdate-1).GetChild(0).GetChild(1).transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedCatering.UpdateCatering();
            });
        }
        SoundManager.Instance.PlayButtonClick();
    }

    public void UpdateCostText(int i)
    {
        Catering selectedCatering = Caterings[i].GetComponent<Catering>();
        if (selectedCatering.currentUpdate < Caterings.Length && selectedCatering.currentUpdate < buildDeliveryPointParent.transform.childCount)
        {
            Debug.Log("currentUpdate" + selectedCatering.currentUpdate);
            buildDeliveryPointParent.transform.GetChild(selectedCatering.currentUpdate).GetChild(0).GetChild(1).GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = selectedCatering.cost.ToString("F0");
        }
    }

    public void PlaceNewCatering()
    {
        GameObject CateringObj = Caterings[currentSelectedObject];
        CateringObj.SetActive(true);

        CateringVanSpawner.Instance.spawnInterval = UpgradeCosts.GetCateringInterval(CateringObj.GetComponent<Catering>().currentUpdate - 1);

        CateringObj.transform.GetChild(CateringObj.GetComponent<Catering>().currentUpdate - 2).gameObject.SetActive(true);
        //ShawarmaSpawner.Instance.AddNewTarget(WareHouse.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().currentCapacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentSelectedObject]);
        CateringObj.name = "Catering" + (currentSelectedObject + 1);// For changing gameobject name to see in hierarchy (optional)
        //CateringObj.GetComponent<Catering>().SetCateringIsPurchased();
        CateringObj.GetComponent<Catering>().cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Catering, CateringObj.GetComponent<Catering>().currentUpdate);
        placedCatering.Add(CateringObj);
        currentCateringCount++;
        CateringObj.GetComponent<Catering>().MakePersistent(currentCateringCount);

    }
   
}

