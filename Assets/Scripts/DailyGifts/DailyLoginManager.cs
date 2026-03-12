using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Cysharp.Threading.Tasks;

public enum DailyGiftRewardType
{
    Cash,
    Gold,
    Boost
}

[Serializable]
public class DailyGiftReward
{
    public DailyGiftRewardType rewardType;
    public float amount; // Cash/Gold
    public string boostId; // for Boost
    public string title;
}

[Serializable]
public class DailyLoginSaveData
{
    public int dayIndex; // 0..6
    public string lastClaimDateUtc; // yyyy-MM-dd
}

/// <summary>Phase 5: 7-day daily gift calendar manager.</summary>
public class DailyLoginManager : MonoBehaviour, ISaveable
{
    public static DailyLoginManager Instance { get; private set; }

    private const string SaveKeyDailyLogin = "daily_login";

    [Header("Rewards (7 days)")]
    [SerializeField] private List<DailyGiftReward> rewards = new List<DailyGiftReward>(7);

    [Header("Rules")]
    [Tooltip("If true, missing a day resets dayIndex to 0. If false, player continues where they left off.")]
    [SerializeField] private bool resetStreakOnMissedDay = false;

    private int dayIndex;
    private string lastClaimDateUtc;
    private bool isDirty;

    public string SaveKey => SaveKeyDailyLogin;
    public bool IsDirty => isDirty;

    public event Action OnDailyGiftChanged;

    public int DayIndex => Mathf.Clamp(dayIndex, 0, 6);
    public IReadOnlyList<DailyGiftReward> Rewards => rewards;
    public string LastClaimDateUtc => lastClaimDateUtc ?? "";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        EnsureDefaultRewards();
        if (SaveLoadManager.saveLoadManagerInstance != null)
        {
            SaveLoadManager.saveLoadManagerInstance.Register(this);
            GameManager.gameManagerInstance?.RecordPersistentRegistrations().Forget();
        }
    }

    private void OnDestroy()
    {
        if (SaveLoadManager.saveLoadManagerInstance != null)
            SaveLoadManager.saveLoadManagerInstance.Unregister(this);
        if (Instance == this) Instance = null;
    }

    private static string TodayUtcDateString()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static int DaysBetweenUtcDates(string fromYmd, string toYmd)
    {
        if (!DateTime.TryParseExact(fromYmd, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var from))
            return 0;
        if (!DateTime.TryParseExact(toYmd, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var to))
            return 0;
        return (int)(to.Date - from.Date).TotalDays;
    }

    public bool HasClaimedToday()
    {
        string today = TodayUtcDateString();
        return string.Equals(lastClaimDateUtc, today, StringComparison.Ordinal);
    }

    public bool CanClaimToday() => !HasClaimedToday();

    public DailyGiftReward GetTodayReward()
    {
        EnsureDefaultRewards();
        if (rewards == null || rewards.Count == 0) return null;
        return rewards[Mathf.Clamp(dayIndex, 0, rewards.Count - 1)];
    }

    public bool TryClaimToday()
    {
        EnsureDefaultRewards();
        if (!CanClaimToday()) return false;

        // Missed-day handling: if player comes back after multiple days, optionally reset
        if (!string.IsNullOrEmpty(lastClaimDateUtc))
        {
            int gapDays = DaysBetweenUtcDates(lastClaimDateUtc, TodayUtcDateString());
            if (gapDays >= 2 && resetStreakOnMissedDay)
                dayIndex = 0;
        }

        var reward = GetTodayReward();
        if (reward == null) return false;

        GrantReward(reward);

        lastClaimDateUtc = TodayUtcDateString();
        dayIndex = (dayIndex + 1) % 7;
        isDirty = true;
        OnDailyGiftChanged?.Invoke();
        return true;
    }

    private void GrantReward(DailyGiftReward reward)
    {
        if (reward == null) return;
        if (GameManager.gameManagerInstance == null) return;

        switch (reward.rewardType)
        {
            case DailyGiftRewardType.Cash:
                GameManager.gameManagerInstance.AddCash(reward.amount);
                if (UIManager.Instance != null) UIManager.Instance.UpdateUI(UIUpdateType.Cash);
                break;
            case DailyGiftRewardType.Gold:
                GameManager.gameManagerInstance.AddGold(reward.amount);
                if (UIManager.Instance != null) UIManager.Instance.UpdateUI(UIUpdateType.Gold);
                break;
            case DailyGiftRewardType.Boost:
                if (BoostManager.Instance != null && !string.IsNullOrEmpty(reward.boostId))
                    BoostManager.Instance.ActivateFree(reward.boostId);
                break;
        }
    }

    private void EnsureDefaultRewards()
    {
        if (rewards == null) rewards = new List<DailyGiftReward>();
        if (rewards.Count == 7) return;

        rewards.Clear();
        // Default 7-day rewards (lower values)
        rewards.Add(new DailyGiftReward { rewardType = DailyGiftRewardType.Cash, amount = 1000, title = "1,000 Coins" });
        rewards.Add(new DailyGiftReward { rewardType = DailyGiftRewardType.Gold, amount = 1, title = "1 Gold" });
        rewards.Add(new DailyGiftReward { rewardType = DailyGiftRewardType.Cash, amount = 5000, title = "5,000 Coins" });
        rewards.Add(new DailyGiftReward { rewardType = DailyGiftRewardType.Gold, amount = 5, title = "5 Gold" });
        rewards.Add(new DailyGiftReward { rewardType = DailyGiftRewardType.Cash, amount = 10000, title = "10,000 Coins" });
        rewards.Add(new DailyGiftReward { rewardType = DailyGiftRewardType.Gold, amount = 5, title = "5 Gold" });
        rewards.Add(new DailyGiftReward { rewardType = DailyGiftRewardType.Boost, boostId = "chefs_special", title = "Chef's Special (10m)" });
        isDirty = true;
    }

    public void TryAutoShowCalendar()
    {
        if (HasClaimedToday()) return;
        // UI may not exist yet; wait a couple frames.
        AutoShowAsync().Forget();
    }

    private async UniTask AutoShowAsync()
    {
        await UniTask.NextFrame();
        await UniTask.NextFrame();
        if (GiftCalendarUI.Instance != null)
        {
            GiftCalendarUI.Instance.SetPanelVisible(true);
        }
    }

    #region ISaveable
    public void ClearDirty() => isDirty = false;

    public object CaptureState()
    {
        return new DailyLoginSaveData
        {
            dayIndex = Mathf.Clamp(dayIndex, 0, 6),
            lastClaimDateUtc = lastClaimDateUtc ?? ""
        };
    }

    public void RestoreState(object state)
    {
        if (state is not DailyLoginSaveData data) return;
        dayIndex = Mathf.Clamp(data.dayIndex, 0, 6);
        lastClaimDateUtc = data.lastClaimDateUtc ?? "";
        isDirty = false;
        OnDailyGiftChanged?.Invoke();
        TryAutoShowCalendar();
    }

    public void SetInitialData()
    {
        dayIndex = 0;
        lastClaimDateUtc = "";
        EnsureDefaultRewards();
        isDirty = true;
        OnDailyGiftChanged?.Invoke();
        TryAutoShowCalendar();
    }
    #endregion
}

