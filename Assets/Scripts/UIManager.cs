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
                        storageText.text = $"Storage: {StorageManager.Instance.GetStoredShawarmas()} / {StorageManager.Instance.GetStorageCapacity()}";
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
    void SaveProgress()
    {
        //DataBase.PlayerCash = playerCash;
        //DataBase.TotalShawarmas += 1;

        //for (int i = 0; i < deliveryPoints.Length; i++)
        //{
        //    DataBase.SetDeliveryPointLevel(i, deliveryPoints[i].upgradeLevel);
        //    DataBase.SetDeliveryPointCount(i, deliveryPoints[i].currentShawarmas);
        //}

        //DataBase.Save();
    }

    void LoadProgress()
    {
        //playerCash = DataBase.PlayerCash;

        //for (int i = 0; i < deliveryPoints.Length; i++)
        //{
        //    deliveryPoints[i].upgradeLevel = DataBase.GetDeliveryPointLevel(i);
        //    deliveryPoints[i].capacity = deliveryPoints[i].capacityPerLevel[deliveryPoints[i].upgradeLevel];
        //    deliveryPoints[i].currentShawarmas = DataBase.GetDeliveryPointCount(i);
        //}
    }
}
public enum UIUpdateType
{
    Cash,
    Storage,
    Multiplier
}