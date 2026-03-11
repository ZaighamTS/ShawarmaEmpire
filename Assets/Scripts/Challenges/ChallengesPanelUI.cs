using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class ChallengeSlotRefs
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text progressText;
    public Image progressFillImage;
    public Button claimButton;
    public GameObject completedLabel;
}

/// <summary>Phase 1: UI for the Challenges panel. Assign 3 slots in the inspector; panel shows progress and Claim buttons.</summary>
public class ChallengesPanelUI : MonoBehaviour
{
    [SerializeField] private ChallengeSlotRefs[] slots = new ChallengeSlotRefs[3];
    [SerializeField] private GameObject challengesPanelRoot;

    private void OnEnable()
    {
        if (ChallengeManager.Instance != null)
            ChallengeManager.Instance.OnChallengesChanged += RefreshDisplay;
        RefreshDisplay();
    }

    private void OnDisable()
    {
        if (ChallengeManager.Instance != null)
            ChallengeManager.Instance.OnChallengesChanged -= RefreshDisplay;
    }

    private void Start()
    {
        for (int i = 0; i < slots.Length && i < 3; i++)
        {
            int index = i;
            if (slots[i].claimButton != null)
                slots[i].claimButton.onClick.AddListener(() => OnClaimClicked(index));
        }
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (ChallengeManager.Instance == null) return;
        if (ChallengeManager.Instance.ActiveChallenges.Count == 0)
            ChallengeManager.Instance.EnsureChallenges();
        var list = ChallengeManager.Instance.ActiveChallenges;
        for (int i = 0; i < slots.Length && i < ChallengeManager.MaxActiveChallenges; i++)
        {
            var slot = slots[i];
            if (slot.titleText == null) continue;
            if (i >= list.Count)
            {
                slot.titleText.text = "—";
                if (slot.descriptionText != null) slot.descriptionText.text = "";
                if (slot.progressText != null) slot.progressText.text = "";
                if (slot.progressFillImage != null) slot.progressFillImage.fillAmount = 0f;
                if (slot.claimButton != null) slot.claimButton.gameObject.SetActive(false);
                if (slot.completedLabel != null) slot.completedLabel.SetActive(false);
                continue;
            }
            var c = list[i];
            slot.titleText.text = c.category.ToString();
            if (slot.descriptionText != null) slot.descriptionText.text = c.description;
            if (slot.progressText != null) slot.progressText.text = c.GetProgressText();
            float fill = c.targetValue > 0 ? Mathf.Clamp01(c.currentProgress / c.targetValue) : 0f;
            if (slot.progressFillImage != null) slot.progressFillImage.fillAmount = fill;
            bool canClaim = c.completed && !c.claimed;
            if (slot.claimButton != null)
            {
                slot.claimButton.gameObject.SetActive(canClaim);
                slot.claimButton.interactable = canClaim;
            }
            if (slot.completedLabel != null)
                slot.completedLabel.SetActive(c.completed && !c.claimed);
        }
    }

    private void OnClaimClicked(int index)
    {
        if (ChallengeManager.Instance == null) return;
        if (ChallengeManager.Instance.Claim(index))
            RefreshDisplay();
    }

    /// <summary>Show or hide the challenges panel. Call from a button that opens challenges.</summary>
    public void SetPanelVisible(bool visible)
    {
        if (challengesPanelRoot != null)
            challengesPanelRoot.SetActive(visible);
        if (visible)
            RefreshDisplay();
    }

    public void TogglePanel()
    {
        if (challengesPanelRoot != null)
            SetPanelVisible(!challengesPanelRoot.activeSelf);
    }
}
