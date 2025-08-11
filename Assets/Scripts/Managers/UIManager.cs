using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text coinsText;
    public TMP_Text[] storageText;
    public TMP_Text multiplierText;
    public TMP_Text chefStarsText;
    public GameObject lowCashPromt;
    public TMP_Text TotalEarningTxt;
    public GameObject PrestigeWarning;
    public GameObject PrestigePop;
    public GameObject PrestigeTramsitioneffect;
    public GameObject RewardedAdPopUp;
    public GameObject InappPopup;
    public GameObject StartPanel;
    public GameObject GameplayPanel;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void OnEnable()
    {
        ShawarmaSpawner.onShawarmaCreated += UpdateUI;
        Warehouse.onWarehouseUpgraded += UpdateUI;
        Catering.onCateringUpgraded += UpdateUI;
        Kitchen.onKitchenUpgraded += UpdateUI;
        BuildingUnlockManager.onBuildingUpgraded += UpdateUI;
        Delivery.onDeliveryUpgraded += UpdateUI;

    }
    private void OnDisable()
    {
        ShawarmaSpawner.onShawarmaCreated -= UpdateUI;
        Warehouse.onWarehouseUpgraded -= UpdateUI;
        Catering.onCateringUpgraded -= UpdateUI;
        Kitchen.onKitchenUpgraded -= UpdateUI;
        BuildingUnlockManager.onBuildingUpgraded -= UpdateUI;
        Delivery.onDeliveryUpgraded -= UpdateUI;
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
            default:
                {
                    print($"No Handler For UpdateType {updateType}");
                    break;
                }
        }

    }
    public void ClickOnRewardBtn()
    {
       // FindObjectOfType<RewardedAdManager>().ShowAd();
    }
    public void ClickOnPrestigeButton()
    { 
        GameManager.gameManagerInstance.ResetPlayerStats();
        PlayerPrefs.DeleteAll();
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
    }

    public void OnRewardedAdSuccess()
    { 
        RewardedAdPopUp.SetActive(false);
        PlayerPrefs.SetInt("RewardCount", PlayerPrefs.GetInt("RewardCount")+1);
        Debug.Log("RewardCount  "+ PlayerPrefs.GetInt("RewardCount"));
        //GameManager.gameManagerInstance.AddCash(1000);
        //UpdateUI(UIUpdateType.Cash);
       
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
}
public enum UIUpdateType
{
    Cash,
    Storage,
    Multiplier
}