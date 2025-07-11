using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text coinsText;
    public TMP_Text storageText;
    public TMP_Text multiplierText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void OnEnable()
    {
        ShawarmaSpawner.onShawarmaCreated += UpdateUI;
    }
    private void OnDisable()
    {
        ShawarmaSpawner.onShawarmaCreated -= UpdateUI;
    }
    public void UpdateUI(UIUpdateType updateType, float value = 0)
    {
        switch (updateType)
        {
            case UIUpdateType.Cash:
                {
                    if (coinsText != null)
                        coinsText.text = $"Coins: ${CurrencyManager.Instance.Coins:N0}";
                    break;
                }
            case UIUpdateType.Storage:
                {
                    if (storageText != null)
                        storageText.text = $"Storage: {StorageManager.storageManagerInstance.GetStoredShawarmas()} / {StorageManager.storageManagerInstance.GetStorageCapacity()}";
                    break;
                }
            case UIUpdateType.Multiplier:
                {
                    if (multiplierText != null && ShawarmaProductionSystem.Instance != null)
                        multiplierText.text = $"Multiplier: {ShawarmaProductionSystem.Instance.GetMultiplier():0.0}x";
                    break;
                }
            default:
                {
                    print($"No Handler For UpdateType {updateType}");
                    break;
                }
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