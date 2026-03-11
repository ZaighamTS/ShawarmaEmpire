using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>Single achievement row for the achievements list. Assign refs in prefab or scene.</summary>
public class AchievementRowUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text progressText;
    public TMP_Text rewardText;
    public TMP_Text statusText;
    public Button claimButton;

    private string _achievementId;
    
    [Header("Status Colors")]
    [SerializeField] private Color lockedColor = new Color(0.65f, 0.65f, 0.65f, 1f);
    [SerializeField] private Color claimColor = new Color(0.22f, 0.80f, 0.45f, 1f);
    [SerializeField] private Color claimedColor = new Color(0.95f, 0.77f, 0.24f, 1f);

    public void SetData(string achievementId, string title, string description, string progress, string reward, bool unlocked, bool claimed, Action<string> onClaim)
    {
        _achievementId = achievementId;
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;
        if (progressText != null) progressText.text = progress;
        if (rewardText != null) rewardText.text = reward;
        if (statusText != null)
        {
            if (claimed)
            {
                statusText.text = "Claimed";
                statusText.color = claimedColor;
            }
            else if (unlocked)
            {
                statusText.text = "Claim";
                statusText.color = claimColor;
            }
            else
            {
                statusText.text = "Locked";
                statusText.color = lockedColor;
            }
        }
        if (claimButton != null)
        {
            claimButton.gameObject.SetActive(unlocked && !claimed);
            claimButton.interactable = unlocked && !claimed;
            claimButton.onClick.RemoveAllListeners();
            if (onClaim != null)
                claimButton.onClick.AddListener(() => onClaim(_achievementId));
        }
    }
}
