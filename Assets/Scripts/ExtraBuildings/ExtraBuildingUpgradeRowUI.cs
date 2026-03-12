using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExtraBuildingUpgradeRowUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text netPerHourText;
    [SerializeField] private TMP_Text upgradeCostText;
    [SerializeField] private Button upgradeButton;

    public void SetData(string name, Sprite icon, int level, float netPerHour, bool canUpgrade, float upgradeCost)
    {
        if (iconImage != null) iconImage.sprite = icon;
        if (nameText != null) nameText.text = name;
        if (levelText != null) levelText.text = $"Level {level}/10";
        if (netPerHourText != null) netPerHourText.text = $"${netPerHour:N0}/hr";
        if (upgradeCostText != null)
            upgradeCostText.text = canUpgrade ? $"${upgradeCost:N0}" : "MAX";
        if (upgradeButton != null)
        {
            upgradeButton.interactable = canUpgrade && PlayerProgress.Instance != null && PlayerProgress.Instance.PlayerCash >= upgradeCost;
            upgradeButton.gameObject.SetActive(canUpgrade);
        }
    }

    public Button UpgradeButton => upgradeButton;
}

