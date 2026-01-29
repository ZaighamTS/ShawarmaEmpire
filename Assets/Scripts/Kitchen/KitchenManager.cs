using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    [Header("Upgrade Availability Indicator")]
    public Button kitchenTabButton; // Tab button for kitchen upgrades (optional - for badge display)
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
    }
    void Start()
    {
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
    public async UniTask DelayOnStart()
   // public async UniTask DelayOnStart()
   
    {
        await UniTask.NextFrame();
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
                point.GetChild(1).GetChild(1).GetChild(0).GetChild(0).transform.GetComponent<Image>().sprite = Kitchens[i].GetComponent<Kitchen>().updates[Kitchens[i].GetComponent<Kitchen>().currentUpdate - 2].Icon;
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
    public void UpdateSlider(int KitchenNumber, int maxValue, int currentValue)
    {
        buidlNewPointParent.GetChild(KitchenNumber).GetChild(1).GetChild(3).GetComponent<Slider>().maxValue = maxValue;
        buidlNewPointParent.GetChild(KitchenNumber).GetChild(1).GetChild(3).GetComponent<Slider>().value = currentValue;
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
            ShowAnimationEffect();
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
    public void ShowAnimationEffect()
    {
        UIManager.Instance.DisableGameplayPanel();
        CameraSwipeController.instance.LerpCamera(26.9f, 62.9f);
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

        kitchenObj.transform.GetChild(kitchenObj.GetComponent<Kitchen>().currentUpdate - 2).transform.GetComponent<DOTweenAnimation>().DOPlay();
        kitchenObj.transform.GetChild(4).gameObject.SetActive(true);
        DisableEffect(kitchenObj).Forget();
        //ShawarmaSpawner.Instance.AddNewTarget(WareHouse.GetComponent<Warehouse>().id, WareHouse.GetComponent<Warehouse>().currentCapacity, WareHouse.GetComponent<Warehouse>().TargetPosition, warehouses[currentSelectedObject]);
        kitchenObj.name = "Kitchen" + (currentSelectedObject + 1);// For changing gameobject name to see in hierarchy (optional)
        //kitchenObj.GetComponent<Kitchen>().SetKitchenIsPurchased();
        kitchenObj.GetComponent<Kitchen>().cost = UpgradeCosts.GetUpgradeCost(UpgradeType.Kitchen, kitchenObj.GetComponent<Kitchen>().currentUpdate);
        placedKitchens.Add(kitchenObj);
        currentKitchenCount++;
        UpdateSlider(kitchenObj.GetComponent<Kitchen>().id, kitchenObj.GetComponent<Kitchen>().updates.Count, kitchenObj.GetComponent<Kitchen>().currentUpdate - 1);
        kitchenObj.GetComponent<Kitchen>().MakePersistent(currentKitchenCount);

    }
    public async UniTask DisableEffect(GameObject Kitchen)
    {
        await UniTask.Delay(2000);
        Kitchen.transform.GetChild(4).gameObject.SetActive(false);

    }
    
    /// <summary>
    /// Counts how many kitchen upgrades are currently affordable
    /// </summary>
    public int GetAvailableUpgradeCount()
    {
        int count = 0;
        float playerCash = PlayerProgress.Instance.PlayerCash;
        
        if (Kitchens == null) return 0;
        
        foreach (var kitchen in Kitchens)
        {
            if (kitchen == null) continue;
            
            Kitchen k = kitchen.GetComponent<Kitchen>();
            if (k == null) continue;
            
            if (k.currentUpdate <= k.updates.Count && k.cost <= playerCash)
            {
                count++;
            }
        }
        
        return count;
    }
    
    /// <summary>
    /// Navigates to the first affordable kitchen upgrade and highlights it
    /// </summary>
    public void NavigateToFirstAffordableUpgrade()
    {
        if (Kitchens == null) return;
        
        float playerCash = PlayerProgress.Instance.PlayerCash;
        
        // Find first affordable kitchen upgrade
        for (int i = 0; i < Kitchens.Length; i++)
        {
            if (Kitchens[i] == null) continue;
            
            Kitchen k = Kitchens[i].GetComponent<Kitchen>();
            if (k == null) continue;
            
            if (k.cost <= playerCash && k.currentUpdate <= k.updates.Count)
            {
                // Select this kitchen and show its upgrade UI
                UpdateKitchenUI(i);
                
                // Highlight the upgrade button
                if (buildDeliveryPointParent != null && k.currentUpdate - 1 >= 0 && k.currentUpdate - 1 < buildDeliveryPointParent.childCount)
                {
                    Transform upgradeButton = buildDeliveryPointParent.GetChild(k.currentUpdate - 1);
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

