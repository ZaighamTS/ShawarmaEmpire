using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>One row in the shawarma types panel: name, income per shawarma, unlock text, Select button.</summary>
public class ShawarmaTypeRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text incomeText;
    [SerializeField] private TMP_Text unlockText;
    [SerializeField] private Button selectButton;
    [SerializeField] private GameObject currentLabel;

    public void SetData(string typeId, string displayName, float incomePerShawarma, string unlockDescription, bool unlocked, bool isCurrent)
    {
        if (nameText != null) nameText.text = displayName;
        if (incomeText != null) incomeText.text = $"Income per shawarma: ${incomePerShawarma:N0}";
        if (unlockText != null) unlockText.text = unlockDescription;
        if (selectButton != null)
        {
            selectButton.interactable = unlocked && !isCurrent;
            selectButton.gameObject.SetActive(unlocked);
        }
        if (currentLabel != null) currentLabel.SetActive(isCurrent);
    }

    public Button SelectButton => selectButton;
}
