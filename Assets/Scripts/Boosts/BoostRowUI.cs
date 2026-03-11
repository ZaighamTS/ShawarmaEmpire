using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>One row in the boost shop: name, description, cost, activate button.</summary>
public class BoostRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button activateButton;

    private string _boostId;

    public void SetData(string boostId, string title, string description, BoostCostType costType, float costAmount, bool canAfford, bool isActive, string remainingTime)
    {
        _boostId = boostId;
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;
        if (costText != null)
            costText.text = costType == BoostCostType.Gold ? $"{costAmount:N0} Gold" : "Watch Ad";
        if (timerText != null)
            timerText.text = isActive && !string.IsNullOrEmpty(remainingTime) ? remainingTime : "";
        if (activateButton != null)
            activateButton.interactable = !isActive && (costType == BoostCostType.WatchAd || canAfford);
    }

    public string BoostId => _boostId;
    public Button ActivateButton => activateButton;
}
