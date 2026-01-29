using Cysharp.Threading.Tasks;
using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text coinsText;
    public TMP_Text goldText;
    public TMP_Text[] storageText;
    public TMP_Text multiplierText;
    public TMP_Text chefStarsText;
    public GameObject lowCashPromt;
    public TMP_Text TotalEarningTxt;
    public TMP_Text automaticEarningText; // Display for automatic earning rate
    public TMP_Text automaticEarningMultiplierText; // Display for automatic earning multiplier
    public GameObject PrestigeWarning;
    public GameObject PrestigePop;
    public GameObject PrestigeTramsitioneffect;
    public GameObject RewardedAdPopUp;
    public GameObject InappPopup;
    public GameObject StartPanel;
    public GameObject GameplayPanel;
    public GameObject PostprocessingEffect;
    public GameObject InfoPopUp;
    public TMP_Text InfoText;
    public Image TotalEarningImage;
    public Transform[] TabsButton;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void OnEnable()
    {
        ShawarmaSpawner.onShawarmaCreated += UpdateUI;
        Warehouse.onWarehouseUpgraded += OnUpgradeChanged;
        Catering.onCateringUpgraded += OnUpgradeChanged;
        Kitchen.onKitchenUpgraded += OnUpgradeChanged;
        BuildingUnlockManager.onBuildingUpgraded += UpdateUI;
        Delivery.onDeliveryUpgraded += OnUpgradeChanged;

    }
    private void OnDisable()
    {
        ShawarmaSpawner.onShawarmaCreated -= UpdateUI;
        Warehouse.onWarehouseUpgraded -= OnUpgradeChanged;
        Catering.onCateringUpgraded -= OnUpgradeChanged;
        Kitchen.onKitchenUpgraded -= OnUpgradeChanged;
        BuildingUnlockManager.onBuildingUpgraded -= UpdateUI;
        Delivery.onDeliveryUpgraded -= OnUpgradeChanged;
    }
    
    /// <summary>
    /// Called when upgrades change to update automatic earning multiplier
    /// </summary>
    private void OnUpgradeChanged(UIUpdateType updateType, float value = 0)
    {
        UpdateUI(updateType, value);
        
        // Update automatic earning multiplier when upgrades change
        if (GameManager.gameManagerInstance != null)
        {
            GameManager.gameManagerInstance.OnPrestigeOrUpgradeChanged();
        }
    }
    
    public void UpdateUI(UIUpdateType updateType, float value = 0)
    {
        switch (updateType)
        {
            case UIUpdateType.Cash:
                {
                    if (coinsText != null)
                    {
                        coinsText.text = $"{GameManager.gameManagerInstance.GetCurrentCash():N0}";
                    }
                    if (TotalEarningTxt)
                    {
                        TotalEarningTxt.text = $"{GameManager.gameManagerInstance.GetTotalEarning():N0}";
                    }
                       
                    break;
                }
            case UIUpdateType.Storage:
                {
                    //  if (storageText != null)
                    for (int i = 0; i < storageText.Length; i++)
                    {
                        storageText[i].text = $"{PlayerProgress.Instance.ShwarmaCount}";
                    }
                    break;
                }
            case UIUpdateType.Multiplier:
                {
                    if (multiplierText != null && ShawarmaSpawner.Instance != null)
                        multiplierText.text = ShawarmaSpawner.Instance.tapMultiplier.ToString("f2")+ "x";
                    //multiplierText.text = $"{ShawarmaSpawner.Instance.GetMultiplier():0.0}x";
                    break;
                }
            case UIUpdateType.Gold:
                {
                    if (goldText != null)
                    {
                        goldText.text = $"{GameManager.gameManagerInstance.GetGold():N0}";
                    }
                    break;
                }
            case UIUpdateType.AutomaticEarning:
                {
                    UpdateAutomaticEarningDisplay(value);
                    break;
                }
            default:
                {
                    print($"No Handler For UpdateType {updateType}");
                    break;
                }
        }

    }
    public void UpdateEarningSlider()
    {
       // Debug.Log("bb "+PlayerProgress.Instance.ChefStars);
      //  Debug.Log("check "+ UpgradeCosts.GetNextPrestigeValue());
      //  Debug.Log("val "+ Mathf.Clamp01(PlayerProgress.Instance.TotalEarnings / UpgradeCosts.GetNextPrestigeValue()));
          TotalEarningImage.fillAmount = Mathf.Clamp01(PlayerProgress.Instance.TotalEarnings / UpgradeCosts.GetNextPrestigeValue());

       // Debug.Log((UpgradeCosts.GetNextPrestigeValue(PlayerProgress.Instance.TotalEarnings) / PlayerProgress.Instance.TotalEarnings));
    }
    public void ShowInfoPopup(string info_text)
    {
        InfoPopUp.SetActive(true);
        InfoText.text = info_text;

    }


     
    //public async UniTask DisablePop()
    //{ 
    //    UniTask.Delay()
    //}
    public void ClickOnRewardBtn()
    {
       // FindObjectOfType<RewardedAdManager>().ShowAd();
    }
    public void ClickOnPrestigeButton()
    { 
        GameManager.gameManagerInstance.ResetPlayerStats();
        // Use SaveLoadManager's reset method to clear both PlayerPrefs and JSON file
        if (SaveLoadManager.saveLoadManagerInstance != null)
        {
            SaveLoadManager.saveLoadManagerInstance.CompleteReset();
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }
        TransitionEffectScript.PrestigeDone = true;
        PrestigeTramsitioneffect.transform.GetChild(0).gameObject.SetActive(true);
       // TransitionEffectScript
        PrestigePop.SetActive(false);
        Invoke("ReloadScene",2);
    }
    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
    public void ShowPrestigeBtn()
    {

        PrestigeWarning.SetActive(true);
    }

    public void UpdateChefStarsText()
    {
        if (chefStarsText != null)
        {
            chefStarsText.text=PlayerProgress.Instance.ChefStars.ToString();
        }
        
        // Update automatic earning multiplier when chef stars change
        if (GameManager.gameManagerInstance != null)
        {
            GameManager.gameManagerInstance.OnPrestigeOrUpgradeChanged();
        }
    }

    public void OnRewardedAdSuccess()
    { 
        RewardedAdPopUp.SetActive(false);
        PlayerPrefs.SetInt("RewardCount", PlayerPrefs.GetInt("RewardCount")+1);
        ShowRewardPop("Reward has been added to your offline earnings").Forget();
        // Debug.Log("RewardCount  "+ PlayerPrefs.GetInt("RewardCount"));
        //  ShowInfoPopup("Reward has been added to your offline earnings");

        //GameManager.gameManagerInstance.AddCash(1000);
        //UpdateUI(UIUpdateType.Cash);

    }
    public async UniTask ShowRewardPop(string _text)
    {
        await UniTask.WaitForSeconds(0.5f);
        InfoPopUp.SetActive(true);
        InfoText.text = _text;

    }
    public void InappPurchaseSuccess()
    {
        GameManager.gameManagerInstance.AddCash(10000);
        UpdateUI(UIUpdateType.Cash);
        InappPopup.SetActive(false);
    }

    void OnUserClickUpgrade(int type)
    {
        UpgradeType upgradeType = (UpgradeType)type;
        switch (upgradeType)
        {
            case UpgradeType.Kitchen:
                {
                    break;
                }
            case UpgradeType.Storage:
                {
                    break;
                }
            case UpgradeType.DeliveryVan:
                {
                    break;
                }
        }
    }

    public void ClickOnQuit()
    { 
        Application.Quit();
    }

    public void DisableGameplayPanel()
    { 
        GameplayPanel.SetActive(false);
        PostprocessingEffect.SetActive(false);
        EnableGameplayPanel().Forget();
    }

    public async UniTask EnableGameplayPanel()
    {
        await UniTask.WaitForSeconds(2.0f);
        PostprocessingEffect.SetActive(true);
        GameplayPanel.SetActive(true);
    }
    public void OnClickTabsButton(Transform ThisButton)
    {
        for (int i = 0; i < TabsButton.Length; i++)
        {
            TabsButton[i].localScale = Vector3.one;
        }
        ThisButton.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }
    
    /// <summary>
    /// Updates the automatic earning display in the UI
    /// Shows earnings per second based on spawned shawarmas and multiplier
    /// Format: "0.01/sec" (number only, no prefix)
    /// Also displays multiplier if multiplierText is assigned
    /// </summary>
    public void UpdateAutomaticEarningDisplay(float earningRatePerSecond, float multiplier = 1f)
    {
        if (automaticEarningText != null)
        {
            // Format: "0.01/sec" - just the number and /sec
            automaticEarningText.text = $"{earningRatePerSecond:F2}/sec";
        }
        
        // Display multiplier separately if text field is assigned
        if (automaticEarningMultiplierText != null)
        {
            // Format: "x1.00" or "x1.25" etc.
            automaticEarningMultiplierText.text = $"x{multiplier:F2}";
        }
    }
}
public enum UIUpdateType
{
    Cash,
    Storage,
    Multiplier,
    Gold,
    AutomaticEarning
}