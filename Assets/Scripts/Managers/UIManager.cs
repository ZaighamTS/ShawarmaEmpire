using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text coinsText;
    public TMP_Text storageText;
    public TMP_Text multiplierText;
    public TMP_Text chefStarsText;
    public GameObject lowCashPromt;
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
                       
                    break;
                }
            case UIUpdateType.Storage:
                {
                    if (storageText != null)
                        storageText.text = $"{PlayerProgress.Instance.ShwarmaCount}";
                    break;
                }
            case UIUpdateType.Multiplier:
                {
                    if (multiplierText != null && ShawarmaSpawner.Instance != null)
                        multiplierText.text = ShawarmaSpawner.Instance.tapMultiplier.ToString("f2");
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

    public void UpdateChefStarsText()
    {
        if (chefStarsText != null)
        {
            chefStarsText.text=PlayerProgress.Instance.ChefStars.ToString();
        }
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
}
public enum UIUpdateType
{
    Cash,
    Storage,
    Multiplier
}