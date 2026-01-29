using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    public List<GameObject> placedCatering = new List<GameObject>(); // FIXED: Made public so CateringVanSpawner can access it
    [Header("UI References")]
    public Transform buidlNewPointParent;
    public Transform buildDeliveryPointParent;
    [Header("Upgrade Availability Indicator")]
    public Button cateringTabButton; // Tab button for catering upgrades (optional - for badge display)
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
    public async UniTask DelayOnStart()
    {
        await UniTask.NextFrame();
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
               // Debug.Log("aa " + (Caterings[i].GetComponent<Catering>().currentUpdate - 2).ToString());
                point.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = Caterings[i].GetComponent<Catering>().updates[Caterings[i].GetComponent<Catering>().currentUpdate - 2].UpdateName;
                point.GetChild(1).GetChild(1).GetChild(0).GetChild(0).transform.GetComponent<Image>().sprite = Caterings[i].GetComponent<Catering>().updates[Caterings[i].GetComponent<Catering>().currentUpdate - 2].Icon;
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
    public void UpdateSlider(int KitchenNumber, int maxValue, int currentValue)
    {
        buidlNewPointParent.GetChild(KitchenNumber).GetChild(1).GetChild(3).GetComponent<Slider>().maxValue = maxValue;
        buidlNewPointParent.GetChild(KitchenNumber).GetChild(1).GetChild(3).GetComponent<Slider>().value = currentValue;
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
            ShowAnimationEffect();
        }
        else
        {
            UIManager.Instance.lowCashPromt.SetActive(true);
            Debug.Log("Low CAsh");
        }
    }
    public void ShowAnimationEffect()
    {
        UIManager.Instance.DisableGameplayPanel();

        CameraSwipeController.instance.LerpCamera(Caterings[currentSelectedObject].transform.position.x + 5, Caterings[currentSelectedObject].transform.position.z + 5);
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
        CateringObj.transform.GetChild(CateringObj.GetComponent<Catering>().currentUpdate - 2).transform.GetComponent<DOTweenAnimation>().DOPlay();
        CateringObj.transform.GetChild(5).gameObject.SetActive(true);
        DisableEffect(CateringObj).Forget();

        //ShawarmaSpawner.Instance.AddNewTarget(WareHouse.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().currentCapacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentSelectedObject]);
        CateringObj.name = "Catering" + (currentSelectedObject + 1);// For changing gameobject name to see in hierarchy (optional)
        //CateringObj.GetComponent<Catering>().SetCateringIsPurchased();
        CateringObj.GetComponent<Catering>().cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Catering, CateringObj.GetComponent<Catering>().currentUpdate);
        placedCatering.Add(CateringObj);
        currentCateringCount++;
        UpdateSlider(CateringObj.GetComponent<Catering>().id, CateringObj.GetComponent<Catering>().updates.Count, CateringObj.GetComponent<Catering>().currentUpdate - 1);
        CateringObj.GetComponent<Catering>().MakePersistent(currentCateringCount);

    }
    public async UniTask DisableEffect(GameObject Catering)
    {
        await UniTask.Delay(2000);
        Catering.transform.GetChild(5).gameObject.SetActive(false);

    }
    
    /// <summary>
    /// Counts how many catering upgrades are currently affordable
    /// </summary>
    public int GetAvailableUpgradeCount()
    {
        int count = 0;
        float playerCash = PlayerProgress.Instance.PlayerCash;
        
        if (Caterings == null) return 0;
        
        foreach (var catering in Caterings)
        {
            if (catering == null) continue;
            
            Catering c = catering.GetComponent<Catering>();
            if (c == null) continue;
            
            if (c.currentUpdate <= c.updates.Count && c.cost <= playerCash)
            {
                count++;
            }
        }
        
        return count;
    }
    
    /// <summary>
    /// Navigates to the first affordable catering upgrade and highlights it
    /// </summary>
    public void NavigateToFirstAffordableUpgrade()
    {
        if (Caterings == null) return;
        
        float playerCash = PlayerProgress.Instance.PlayerCash;
        
        // Find first affordable catering upgrade
        for (int i = 0; i < Caterings.Length; i++)
        {
            if (Caterings[i] == null) continue;
            
            Catering c = Caterings[i].GetComponent<Catering>();
            if (c == null) continue;
            
            if (c.cost <= playerCash && c.currentUpdate <= c.updates.Count)
            {
                // Select this catering and show its upgrade UI
                UpdateCateringUI(i);
                
                // Highlight the upgrade button
                if (buildDeliveryPointParent != null && c.currentUpdate - 1 >= 0 && c.currentUpdate - 1 < buildDeliveryPointParent.childCount)
                {
                    Transform upgradeButton = buildDeliveryPointParent.GetChild(c.currentUpdate - 1);
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

