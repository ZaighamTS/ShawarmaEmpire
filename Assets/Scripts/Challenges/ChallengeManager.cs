using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>Goal type for a challenge. Progress is updated from GameProgressEvents.</summary>
public enum ChallengeGoalType
{
    DeliverCount,   // Delivery + catering completions
    EarnCash,       // Total cash earned since challenge started
    ProduceCount,   // Shawarmas produced since challenge started
    UpgradeCount    // Upgrades purchased since challenge started
}

/// <summary>Reward type when challenge is claimed.</summary>
public enum ChallengeRewardType
{
    Cash,
    Gold,
    ChefStar
}

/// <summary>Challenge category for refresh (Daily = new each day, etc.).</summary>
public enum ChallengeCategory
{
    Daily,
    Weekly,
    Special
}

[Serializable]
public class ActiveChallengeState
{
    public ChallengeGoalType goalType;
    public float targetValue;
    public float currentProgress;
    public ChallengeRewardType rewardType;
    public float rewardValue;
    public string description;
    public bool completed;
    public bool claimed;
    public ChallengeCategory category;

    public string GetProgressText()
    {
        if (goalType == ChallengeGoalType.EarnCash)
            return $"${currentProgress:N0} / ${targetValue:N0}";
        return $"{(int)currentProgress} / {(int)targetValue}";
    }

    public string GetRewardText()
    {
        switch (rewardType)
        {
            case ChallengeRewardType.Cash: return $"${rewardValue:N0}";
            case ChallengeRewardType.Gold: return $"{rewardValue:N0} Gold";
            case ChallengeRewardType.ChefStar: return $"{rewardValue} Chef Star(s)";
            default: return "";
        }
    }
}

/// <summary>Editable challenge template. DifficultyTier 0=Starter, 4=Veteran; challenges are offered when player tier >= this tier. Use -1 for "any tier".</summary>
[Serializable]
public class ChallengeDefinition
{
    [Tooltip("0=Starter, 1=Growing, 2=Established, 3=Expanding, 4=Veteran. -1 = any tier. Player tier is based on total earnings.")]
    public int difficultyTier = 0;

    [Tooltip("Daily / Weekly / Special")]
    public ChallengeCategory category = ChallengeCategory.Daily;

    [Tooltip("What the player must do")]
    public ChallengeGoalType goalType = ChallengeGoalType.DeliverCount;

    [Tooltip("Target value (e.g. 10 deliveries, 5000 cash)")]
    public float targetValue = 10f;

    [Tooltip("Leave empty to auto-generate from goal type and target")]
    public string description = "";

    [Tooltip("Reward type")]
    public ChallengeRewardType rewardType = ChallengeRewardType.Cash;

    [Tooltip("Reward amount (e.g. 500 cash, 10 gold, 1 chef star)")]
    public float rewardValue = 500f;

    public string GetDescription()
    {
        if (!string.IsNullOrWhiteSpace(description)) return description.Trim();
        switch (goalType)
        {
            case ChallengeGoalType.DeliverCount: return $"Complete {(int)targetValue} deliveries";
            case ChallengeGoalType.EarnCash: return $"Earn ${targetValue:N0}";
            case ChallengeGoalType.ProduceCount: return $"Produce {(int)targetValue} shawarmas";
            case ChallengeGoalType.UpgradeCount: return $"Buy {(int)targetValue} upgrade(s)";
            default: return $"Complete {(int)targetValue}";
        }
    }
}

[Serializable]
public class ChallengesSaveData
{
    public List<ActiveChallengeState> activeChallenges = new List<ActiveChallengeState>();
    public string lastDailyRefreshUtc = "";
    public string lastWeeklyRefreshUtc = "";
}

/// <summary>Phase 1: Manages 3 active challenges, progress from events, and claim rewards. Persisted via ISaveable.</summary>
public class ChallengeManager : MonoBehaviour, ISaveable
{
    public static ChallengeManager Instance { get; private set; }

    public const int MaxActiveChallenges = 3;
    private const string SaveKeyChallenges = "challenges";

    [Tooltip("Add your own challenges here (optional). If empty, uses doc-inspired pool scaled to player progress (total earnings).")]
    [SerializeField] private List<ChallengeDefinition> customDefinitions = new List<ChallengeDefinition>();

    [Tooltip("If true, only show challenges whose difficulty tier <= player tier. If false, custom list ignores tier.")]
    [SerializeField] private bool scaleCustomDefinitionsByTier = true;

    private List<ActiveChallengeState> activeChallenges = new List<ActiveChallengeState>();
    private string lastDailyRefreshUtc = "";
    private string lastWeeklyRefreshUtc = "";
    private bool isDirty;

    public string SaveKey => SaveKeyChallenges;

    public IReadOnlyList<ActiveChallengeState> ActiveChallenges => activeChallenges;

    public event Action OnChallengesChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        GameProgressEvents.OnDeliveryCompleted += OnDeliveryCompleted;
        GameProgressEvents.OnCateringCompleted += OnCateringCompleted;
        GameProgressEvents.OnCashEarned += OnCashEarned;
        GameProgressEvents.OnUpgradePurchased += OnUpgradePurchased;
        GameProgressEvents.OnShawarmaProduced += OnShawarmaProduced;
    }

    private void OnDisable()
    {
        GameProgressEvents.OnDeliveryCompleted -= OnDeliveryCompleted;
        GameProgressEvents.OnCateringCompleted -= OnCateringCompleted;
        GameProgressEvents.OnCashEarned -= OnCashEarned;
        GameProgressEvents.OnUpgradePurchased -= OnUpgradePurchased;
        GameProgressEvents.OnShawarmaProduced -= OnShawarmaProduced;
    }

    private void Start()
    {
        if (SaveLoadManager.saveLoadManagerInstance != null)
        {
            SaveLoadManager.saveLoadManagerInstance.Register(this);
            GameManager.gameManagerInstance?.RecordPersistentRegistrations().Forget();
        }
        EnsureChallenges();
    }

    private void OnDestroy()
    {
        if (SaveLoadManager.saveLoadManagerInstance != null)
            SaveLoadManager.saveLoadManagerInstance.Unregister(this);
        if (Instance == this)
            Instance = null;
    }

    private void OnDeliveryCompleted(int count, float cash)
    {
        foreach (var c in activeChallenges)
        {
            if (c.claimed || c.goalType != ChallengeGoalType.DeliverCount) continue;
            c.currentProgress += count;
            if (c.currentProgress >= c.targetValue) c.completed = true;
        }
        MarkDirtyAndNotify();
    }

    private void OnCateringCompleted(int count, float cash)
    {
        foreach (var c in activeChallenges)
        {
            if (c.claimed || c.goalType != ChallengeGoalType.DeliverCount) continue;
            c.currentProgress += count;
            if (c.currentProgress >= c.targetValue) c.completed = true;
        }
        MarkDirtyAndNotify();
    }

    private void OnCashEarned(float amount)
    {
        foreach (var c in activeChallenges)
        {
            if (c.claimed || c.goalType != ChallengeGoalType.EarnCash) continue;
            c.currentProgress += amount;
            if (c.currentProgress >= c.targetValue) c.completed = true;
        }
        MarkDirtyAndNotify();
    }

    private void OnUpgradePurchased(UpgradeType type, float cost)
    {
        foreach (var c in activeChallenges)
        {
            if (c.claimed || c.goalType != ChallengeGoalType.UpgradeCount) continue;
            c.currentProgress += 1f;
            if (c.currentProgress >= c.targetValue) c.completed = true;
        }
        MarkDirtyAndNotify();
    }

    private void OnShawarmaProduced(int count)
    {
        foreach (var c in activeChallenges)
        {
            if (c.claimed || c.goalType != ChallengeGoalType.ProduceCount) continue;
            c.currentProgress += count;
            if (c.currentProgress >= c.targetValue) c.completed = true;
        }
        MarkDirtyAndNotify();
    }

    private void MarkDirtyAndNotify()
    {
        isDirty = true;
        OnChallengesChanged?.Invoke();
    }

    /// <summary>Ensure we have up to 3 challenges, refreshing daily/weekly if needed.</summary>
    public void EnsureChallenges()
    {
        var now = DateTime.UtcNow;
        var needNew = false;
        var dailyDate = string.IsNullOrEmpty(lastDailyRefreshUtc) ? (DateTime?)null : ParseUtc(lastDailyRefreshUtc)?.Date;
        var weeklyStart = string.IsNullOrEmpty(lastWeeklyRefreshUtc) ? (DateTime?)null : ParseUtc(lastWeeklyRefreshUtc);
        var currentWeekStart = GetWeekStart(now);

        // Remove claimed challenges so we can fill slots
        activeChallenges.RemoveAll(c => c.claimed);
        if (activeChallenges.Count < MaxActiveChallenges)
            needNew = true;
        if (dailyDate.HasValue && dailyDate.Value < now.Date)
        {
            activeChallenges.RemoveAll(c => c.category == ChallengeCategory.Daily);
            lastDailyRefreshUtc = now.ToString("o");
            needNew = true;
        }
        if (weeklyStart.HasValue && GetWeekStart(weeklyStart.Value) < currentWeekStart)
        {
            activeChallenges.RemoveAll(c => c.category == ChallengeCategory.Weekly);
            lastWeeklyRefreshUtc = now.ToString("o");
            needNew = true;
        }

        int playerTier = GetPlayerTier();
        List<ChallengeDefinition> customPool = null;
        List<ChallengeDefinition> defaultTierPool = null;
        if (customDefinitions != null && customDefinitions.Count > 0 && scaleCustomDefinitionsByTier)
            customPool = FilterByTier(customDefinitions, playerTier);
        else if (customDefinitions == null || customDefinitions.Count == 0)
            defaultTierPool = FilterByTier(GetDefaultChallengePool(), playerTier);

        while (activeChallenges.Count < MaxActiveChallenges)
        {
            var category = activeChallenges.Count == 0 ? ChallengeCategory.Daily :
                           activeChallenges.Count == 1 ? ChallengeCategory.Weekly : ChallengeCategory.Special;
            if (customDefinitions != null && customDefinitions.Count > 0)
            {
                var pool = scaleCustomDefinitionsByTier ? customPool : customDefinitions;
                if (pool == null || pool.Count == 0)
                    pool = customDefinitions;
                if (pool != null && pool.Count > 0)
                    activeChallenges.Add(CreateFromDefinition(pool[UnityEngine.Random.Range(0, pool.Count)], category));
                else
                    activeChallenges.Add(GenerateOneChallenge(category, playerTier));
            }
            else
            {
                if (defaultTierPool != null && defaultTierPool.Count > 0)
                    activeChallenges.Add(CreateFromDefinition(defaultTierPool[UnityEngine.Random.Range(0, defaultTierPool.Count)], category));
                else
                    activeChallenges.Add(GenerateOneChallenge(category, playerTier));
            }
        }
        isDirty = true;
        OnChallengesChanged?.Invoke();
    }

    private static DateTime? ParseUtc(string utc)
    {
        if (string.IsNullOrEmpty(utc)) return null;
        return DateTime.TryParse(utc, null, DateTimeStyles.RoundtripKind, out var d) ? d : (DateTime?)null;
    }

    private static DateTime GetWeekStart(DateTime d) => d.Date.AddDays(-(int)d.DayOfWeek);

    /// <summary>Player tier 0-4 from total earnings (and chef stars). Low income = low tier = easier challenges.</summary>
    private int GetPlayerTier()
    {
        if (PlayerProgress.Instance == null) return 0;
        float total = PlayerProgress.Instance.TotalEarnings;
        int stars = PlayerProgress.Instance.ChefStars;
        int tier = 0;
        if (total >= 10000000f) tier = 4;
        else if (total >= 1000000f) tier = 3;
        else if (total >= 100000f) tier = 2;
        else if (total >= 10000f) tier = 1;
        tier = Mathf.Min(4, tier + Mathf.Min(stars, 2));
        return tier;
    }

    private static List<ChallengeDefinition> FilterByTier(List<ChallengeDefinition> list, int playerTier)
    {
        var result = new List<ChallengeDefinition>();
        foreach (var def in list)
        {
            if (def.difficultyTier < 0 || def.difficultyTier <= playerTier)
                result.Add(def);
        }
        return result;
    }

    /// <summary>Doc-inspired challenge pool: Deliver, Earn, Produce, Upgrade across tiers 0-4 with appropriate targets and rewards.</summary>
    private static List<ChallengeDefinition> GetDefaultChallengePool()
    {
        var pool = new List<ChallengeDefinition>();
        // Tier 0 - Starter (earnings < $10K)
        pool.Add(new ChallengeDefinition { difficultyTier = 0, goalType = ChallengeGoalType.DeliverCount, targetValue = 5, rewardType = ChallengeRewardType.Cash, rewardValue = 500 });
        pool.Add(new ChallengeDefinition { difficultyTier = 0, goalType = ChallengeGoalType.DeliverCount, targetValue = 10, rewardType = ChallengeRewardType.Cash, rewardValue = 1000 });
        pool.Add(new ChallengeDefinition { difficultyTier = 0, goalType = ChallengeGoalType.EarnCash, targetValue = 500, rewardType = ChallengeRewardType.Cash, rewardValue = 750 });
        pool.Add(new ChallengeDefinition { difficultyTier = 0, goalType = ChallengeGoalType.EarnCash, targetValue = 1000, rewardType = ChallengeRewardType.Cash, rewardValue = 1500 });
        pool.Add(new ChallengeDefinition { difficultyTier = 0, goalType = ChallengeGoalType.ProduceCount, targetValue = 50, rewardType = ChallengeRewardType.Cash, rewardValue = 500 });
        pool.Add(new ChallengeDefinition { difficultyTier = 0, goalType = ChallengeGoalType.ProduceCount, targetValue = 200, rewardType = ChallengeRewardType.Cash, rewardValue = 1500 });
        pool.Add(new ChallengeDefinition { difficultyTier = 0, goalType = ChallengeGoalType.UpgradeCount, targetValue = 1, rewardType = ChallengeRewardType.Gold, rewardValue = 5 });
        pool.Add(new ChallengeDefinition { difficultyTier = 0, goalType = ChallengeGoalType.UpgradeCount, targetValue = 2, rewardType = ChallengeRewardType.Gold, rewardValue = 12 });
        // Tier 1 - Growing ($10K - $100K)
        pool.Add(new ChallengeDefinition { difficultyTier = 1, goalType = ChallengeGoalType.DeliverCount, targetValue = 15, rewardType = ChallengeRewardType.Cash, rewardValue = 3000 });
        pool.Add(new ChallengeDefinition { difficultyTier = 1, goalType = ChallengeGoalType.DeliverCount, targetValue = 25, rewardType = ChallengeRewardType.Cash, rewardValue = 5000 });
        pool.Add(new ChallengeDefinition { difficultyTier = 1, goalType = ChallengeGoalType.EarnCash, targetValue = 5000, rewardType = ChallengeRewardType.Cash, rewardValue = 7500 });
        pool.Add(new ChallengeDefinition { difficultyTier = 1, goalType = ChallengeGoalType.EarnCash, targetValue = 10000, rewardType = ChallengeRewardType.Cash, rewardValue = 15000 });
        pool.Add(new ChallengeDefinition { difficultyTier = 1, goalType = ChallengeGoalType.EarnCash, targetValue = 150000, rewardType = ChallengeRewardType.Cash, rewardValue = 150000 });
        pool.Add(new ChallengeDefinition { difficultyTier = 1, goalType = ChallengeGoalType.ProduceCount, targetValue = 500, rewardType = ChallengeRewardType.Cash, rewardValue = 10000 });
        pool.Add(new ChallengeDefinition { difficultyTier = 1, goalType = ChallengeGoalType.ProduceCount, targetValue = 7500, rewardType = ChallengeRewardType.Cash, rewardValue = 50000 });
        pool.Add(new ChallengeDefinition { difficultyTier = 1, goalType = ChallengeGoalType.UpgradeCount, targetValue = 3, rewardType = ChallengeRewardType.Gold, rewardValue = 24 });
        pool.Add(new ChallengeDefinition { difficultyTier = 1, goalType = ChallengeGoalType.ProduceCount, targetValue = 50000, rewardType = ChallengeRewardType.Cash, rewardValue = 1000000 });
        // Tier 2 - Established ($100K - $1M)
        pool.Add(new ChallengeDefinition { difficultyTier = 2, goalType = ChallengeGoalType.DeliverCount, targetValue = 50, rewardType = ChallengeRewardType.Gold, rewardValue = 24 });
        pool.Add(new ChallengeDefinition { difficultyTier = 2, goalType = ChallengeGoalType.DeliverCount, targetValue = 75, rewardType = ChallengeRewardType.Cash, rewardValue = 50000 });
        pool.Add(new ChallengeDefinition { difficultyTier = 2, goalType = ChallengeGoalType.EarnCash, targetValue = 25000, rewardType = ChallengeRewardType.Gold, rewardValue = 24 });
        pool.Add(new ChallengeDefinition { difficultyTier = 2, goalType = ChallengeGoalType.EarnCash, targetValue = 100000, rewardType = ChallengeRewardType.Cash, rewardValue = 150000 });
        pool.Add(new ChallengeDefinition { difficultyTier = 2, goalType = ChallengeGoalType.EarnCash, targetValue = 1000000, rewardType = ChallengeRewardType.Gold, rewardValue = 48 });
        pool.Add(new ChallengeDefinition { difficultyTier = 2, goalType = ChallengeGoalType.ProduceCount, targetValue = 10000, rewardType = ChallengeRewardType.Gold, rewardValue = 24 });
        pool.Add(new ChallengeDefinition { difficultyTier = 2, goalType = ChallengeGoalType.ProduceCount, targetValue = 50000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        pool.Add(new ChallengeDefinition { difficultyTier = 2, goalType = ChallengeGoalType.UpgradeCount, targetValue = 5, rewardType = ChallengeRewardType.Gold, rewardValue = 48 });
        // Tier 3 - Expanding ($1M - $10M)
        pool.Add(new ChallengeDefinition { difficultyTier = 3, goalType = ChallengeGoalType.DeliverCount, targetValue = 100, rewardType = ChallengeRewardType.Cash, rewardValue = 100000 });
        pool.Add(new ChallengeDefinition { difficultyTier = 3, goalType = ChallengeGoalType.DeliverCount, targetValue = 250, rewardType = ChallengeRewardType.Gold, rewardValue = 48 });
        pool.Add(new ChallengeDefinition { difficultyTier = 3, goalType = ChallengeGoalType.EarnCash, targetValue = 5000000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        pool.Add(new ChallengeDefinition { difficultyTier = 3, goalType = ChallengeGoalType.EarnCash, targetValue = 10000000, rewardType = ChallengeRewardType.Gold, rewardValue = 48 });
        pool.Add(new ChallengeDefinition { difficultyTier = 3, goalType = ChallengeGoalType.ProduceCount, targetValue = 100000, rewardType = ChallengeRewardType.Cash, rewardValue = 500000 });
        pool.Add(new ChallengeDefinition { difficultyTier = 3, goalType = ChallengeGoalType.UpgradeCount, targetValue = 8, rewardType = ChallengeRewardType.Gold, rewardValue = 96 });
        // Tier 4 - Veteran ($10M+)
        pool.Add(new ChallengeDefinition { difficultyTier = 4, goalType = ChallengeGoalType.DeliverCount, targetValue = 250, rewardType = ChallengeRewardType.Gold, rewardValue = 48 });
        pool.Add(new ChallengeDefinition { difficultyTier = 4, goalType = ChallengeGoalType.DeliverCount, targetValue = 500000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        pool.Add(new ChallengeDefinition { difficultyTier = 4, goalType = ChallengeGoalType.EarnCash, targetValue = 500000000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        pool.Add(new ChallengeDefinition { difficultyTier = 4, goalType = ChallengeGoalType.EarnCash, targetValue = 1000000000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        pool.Add(new ChallengeDefinition { difficultyTier = 4, goalType = ChallengeGoalType.ProduceCount, targetValue = 1000000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        pool.Add(new ChallengeDefinition { difficultyTier = 4, goalType = ChallengeGoalType.ProduceCount, targetValue = 5000000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        pool.Add(new ChallengeDefinition { difficultyTier = 4, goalType = ChallengeGoalType.UpgradeCount, targetValue = 10, rewardType = ChallengeRewardType.Gold, rewardValue = 250 });
        return pool;
    }

    private static ActiveChallengeState CreateFromDefinition(ChallengeDefinition def, ChallengeCategory category)
    {
        return new ActiveChallengeState
        {
            goalType = def.goalType,
            targetValue = def.targetValue,
            currentProgress = 0,
            rewardType = def.rewardType,
            rewardValue = def.rewardValue,
            description = def.GetDescription(),
            completed = false,
            claimed = false,
            category = category
        };
    }

    private static ActiveChallengeState GenerateOneChallenge(ChallengeCategory category, int tier = 0)
    {
        var rng = new System.Random(DateTime.UtcNow.Millisecond + Environment.TickCount);
        var goalType = (ChallengeGoalType)rng.Next(0, 4);
        float target;
        string desc;
        int t = Mathf.Clamp(tier, 0, 4);
        switch (goalType)
        {
            case ChallengeGoalType.DeliverCount:
                target = t == 0 ? rng.Next(3, 11) : t == 1 ? rng.Next(10, 26) : t == 2 ? rng.Next(25, 51) : t == 3 ? rng.Next(50, 101) : rng.Next(100, 201);
                desc = $"Complete {(int)target} deliveries";
                break;
            case ChallengeGoalType.EarnCash:
                float[] cashMin = { 500f, 2000f, 15000f, 50000f, 200000f };
                float[] cashMax = { 2000f, 15000f, 50000f, 200000f, 1000000f };
                target = (float)(rng.NextDouble() * (cashMax[t] - cashMin[t]) + cashMin[t]);
                desc = $"Earn ${target:N0}";
                break;
            case ChallengeGoalType.ProduceCount:
                target = t == 0 ? rng.Next(20, 81) : t == 1 ? rng.Next(80, 201) : t == 2 ? rng.Next(200, 501) : t == 3 ? rng.Next(500, 2001) : rng.Next(2000, 5001);
                desc = $"Produce {(int)target} shawarmas";
                break;
            case ChallengeGoalType.UpgradeCount:
                target = t == 0 ? rng.Next(1, 3) : t == 1 ? rng.Next(2, 5) : t == 2 ? rng.Next(4, 9) : t == 3 ? rng.Next(6, 12) : rng.Next(8, 15);
                desc = $"Buy {(int)target} upgrade(s)";
                break;
            default:
                target = 10; desc = "Complete 10 deliveries";
                break;
        }
        var rewardType = (ChallengeRewardType)rng.Next(0, 3);
        float rewardValue;
        switch (rewardType)
        {
            case ChallengeRewardType.Cash:
                float[] cashRewardMin = { 500f, 3000f, 15000f, 50000f, 100000f };
                float[] cashRewardMax = { 2000f, 15000f, 100000f, 300000f, 500000f };
                rewardValue = (float)(rng.NextDouble() * (cashRewardMax[t] - cashRewardMin[t]) + cashRewardMin[t]);
                break;
            case ChallengeRewardType.Gold:
                rewardValue = 5 + t * 10 + rng.Next(0, 15);
                break;
            case ChallengeRewardType.ChefStar:
                rewardValue = (t >= 2 && rng.Next(0, 3) == 0) ? 1f : 0f;
                if (rewardValue == 0) { rewardValue = 500 + t * 500; rewardType = ChallengeRewardType.Cash; }
                break;
            default:
                rewardValue = 1000;
                break;
        }
        return new ActiveChallengeState
        {
            goalType = goalType,
            targetValue = target,
            currentProgress = 0,
            rewardType = rewardType,
            rewardValue = rewardValue,
            description = desc,
            completed = false,
            claimed = false,
            category = category
        };
    }

    /// <summary>Claim reward for challenge at index. Grants Cash/Gold/ChefStar via GameManager.</summary>
    public bool Claim(int index)
    {
        if (index < 0 || index >= activeChallenges.Count) return false;
        var c = activeChallenges[index];
        if (c.claimed || !c.completed) return false;

        switch (c.rewardType)
        {
            case ChallengeRewardType.Cash:
                GameManager.gameManagerInstance.AddCash(c.rewardValue);
                break;
            case ChallengeRewardType.Gold:
                GameManager.gameManagerInstance.AddGold(c.rewardValue);
                break;
            case ChallengeRewardType.ChefStar:
                if (PlayerProgress.Instance != null)
                    PlayerProgress.Instance.ChefStars += (int)c.rewardValue;
                if (UIManager.Instance != null)
                    UIManager.Instance.UpdateChefStarsText();
                break;
        }
        c.claimed = true;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateUI(UIUpdateType.Gold);
        isDirty = true;
        OnChallengesChanged?.Invoke();
        EnsureChallenges();
        return true;
    }

    #region ISaveable
    public bool IsDirty => isDirty;
    public void ClearDirty() => isDirty = false;

    public object CaptureState()
    {
        return new ChallengesSaveData
        {
            activeChallenges = new List<ActiveChallengeState>(activeChallenges),
            lastDailyRefreshUtc = lastDailyRefreshUtc,
            lastWeeklyRefreshUtc = lastWeeklyRefreshUtc
        };
    }

    public void RestoreState(object state)
    {
        if (state is not ChallengesSaveData data) return;
        activeChallenges = data.activeChallenges ?? new List<ActiveChallengeState>();
        lastDailyRefreshUtc = data.lastDailyRefreshUtc ?? "";
        lastWeeklyRefreshUtc = data.lastWeeklyRefreshUtc ?? "";
        isDirty = false;
        if (activeChallenges == null || activeChallenges.Count < MaxActiveChallenges)
            EnsureChallenges();
        OnChallengesChanged?.Invoke();
    }

    public void SetInitialData()
    {
        activeChallenges.Clear();
        lastDailyRefreshUtc = "";
        lastWeeklyRefreshUtc = "";
        isDirty = true;
        EnsureChallenges();
    }
    #endregion
}
