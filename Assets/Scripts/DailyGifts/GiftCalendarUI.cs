using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class GiftDaySlotRefs
{
    public TMP_Text dayLabelText;     // e.g. "Day 1"
    public TMP_Text rewardText;       // e.g. "5 Gold"
    public GameObject claimedOverlay; // shown if already claimed today (or past)
    public GameObject todayHighlight; // shown for current dayIndex
}

/// <summary>Phase 5: 7-day gift calendar panel UI.</summary>
public class GiftCalendarUI : MonoBehaviour
{
    public static GiftCalendarUI Instance { get; private set; }

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private GiftDaySlotRefs[] slots = new GiftDaySlotRefs[7];
    [SerializeField] private Button claimButton;
    [SerializeField] private TMP_Text claimButtonText;
    [SerializeField] private TMP_Text infoText;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void OnEnable()
    {
        if (DailyLoginManager.Instance != null)
            DailyLoginManager.Instance.OnDailyGiftChanged += Refresh;
        if (claimButton != null)
        {
            claimButton.onClick.RemoveListener(OnClaimClicked);
            claimButton.onClick.AddListener(OnClaimClicked);
        }
        Refresh();
    }

    private void OnDisable()
    {
        if (DailyLoginManager.Instance != null)
            DailyLoginManager.Instance.OnDailyGiftChanged -= Refresh;
    }

    public void Refresh()
    {
        if (DailyLoginManager.Instance == null) return;
        var mgr = DailyLoginManager.Instance;

        // "Today" = the slot we can claim (or just claimed) today. After claim, dayIndex advances so today's slot is (dayIndex + 6) % 7.
        int todaySlotIndex = mgr.HasClaimedToday() ? (mgr.DayIndex + 6) % 7 : mgr.DayIndex;

        // Slots
        for (int i = 0; i < slots.Length && i < 7; i++)
        {
            var s = slots[i];
            if (s == null) continue;
            if (s.dayLabelText != null) s.dayLabelText.text = $"Day {i + 1}";
            var reward = (mgr.Rewards != null && mgr.Rewards.Count > i) ? mgr.Rewards[i] : null;
            if (s.rewardText != null) s.rewardText.text = reward != null ? (reward.title ?? reward.rewardType.ToString()) : "—";
            if (s.todayHighlight != null) s.todayHighlight.SetActive(i == todaySlotIndex);

            // claimed overlay: slots before dayIndex (past days this cycle) or the slot we just claimed today
            bool claimed = (i < mgr.DayIndex) || (mgr.HasClaimedToday() && i == (mgr.DayIndex + 6) % 7);
            if (s.claimedOverlay != null) s.claimedOverlay.SetActive(claimed);
        }

        bool canClaim = mgr.CanClaimToday();
        if (claimButton != null)
        {
            claimButton.interactable = canClaim;
        }
        if (claimButtonText != null)
            claimButtonText.text = canClaim ? "CLAIM" : "CLAIMED";

        if (infoText != null)
        {
            var todayReward = mgr.GetTodayReward();
            if (todayReward != null)
                infoText.text = canClaim ? $"Today's gift: {todayReward.title}" : "Come back tomorrow for the next gift.";
        }
    }

    private void OnClaimClicked()
    {
        if (DailyLoginManager.Instance == null) return;
        if (DailyLoginManager.Instance.TryClaimToday())
            Refresh();
    }

    public void SetPanelVisible(bool visible)
    {
        if (panelRoot != null) panelRoot.SetActive(visible);
        else gameObject.SetActive(visible);
        if (visible) Refresh();
    }

    public void TogglePanel()
    {
        bool current = panelRoot != null ? panelRoot.activeSelf : gameObject.activeSelf;
        SetPanelVisible(!current);
    }
}

