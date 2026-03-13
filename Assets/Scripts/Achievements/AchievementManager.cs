using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>Condition type for an achievement. Matches doc achievement table.</summary>
public enum AchievementConditionType
{
    TotalProduced,          // ShwarmaCount >= X
    EarnInOneSecond,        // Earnings in one second >= X
    TotalEarnings,          // TotalEarnings >= X
    StorageCapacity,        // Total warehouse capacity >= X
    DeliveryCapacityPerMin, // Delivery throughput shawarmas/min >= X
    TotalDeliveries,        // TotalDeliveriesCompleted >= X
    TotalCateringOrders,    // TotalCateringOrdersCompleted >= X
    TotalUpgrades,          // TotalUpgradesPurchased >= X
    PlayerCash,             // PlayerCash >= X (money in bank)
    ChefStars,              // ChefStars >= X
    StoredShawarmas         // Current load in warehouses >= X (GetWholeLoad)
}

[Serializable]
public class AchievementDefinition
{
    public string id;
    public string title;
    public string description;
    public AchievementConditionType conditionType;
    public float targetValue;
    public ChallengeRewardType rewardType;
    public float rewardValue;
}

[Serializable]
public class AchievementState
{
    public string achievementId;
    public bool claimed;
}

[Serializable]
public class AchievementsSaveData
{
    public List<AchievementState> claimedAchievements = new List<AchievementState>();
    public float maxEarningsInOneSecondEver;
}

/// <summary>Phase 2: Doc achievement list. Evaluates conditions from GameProgressEvents, grants rewards on claim, persists claimed.</summary>
public class AchievementManager : MonoBehaviour, ISaveable
{
    public static AchievementManager Instance { get; private set; }

    private const string SaveKeyAchievements = "achievements";
    private HashSet<string> claimedIds = new HashSet<string>();
    private List<AchievementDefinition> definitions = new List<AchievementDefinition>();
    private float maxEarningsInOneSecondEver;
    private bool isDirty;

    public string SaveKey => SaveKeyAchievements;
    public IReadOnlyList<AchievementDefinition> Definitions => definitions;
    public event Action OnAchievementsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        GameProgressEvents.OnDeliveryCompleted += OnProgressEvent;
        GameProgressEvents.OnCateringCompleted += OnProgressEvent;
        GameProgressEvents.OnCashEarned += OnProgressEvent;
        GameProgressEvents.OnUpgradePurchased += OnProgressEvent;
        GameProgressEvents.OnShawarmaProduced += OnProgressEvent;
        GameProgressEvents.OnEarningsThisSecondChecked += OnEarningsThisSecondChecked;
    }

    private void OnDisable()
    {
        GameProgressEvents.OnDeliveryCompleted -= OnProgressEvent;
        GameProgressEvents.OnCateringCompleted -= OnProgressEvent;
        GameProgressEvents.OnCashEarned -= OnProgressEvent;
        GameProgressEvents.OnUpgradePurchased -= OnProgressEvent;
        GameProgressEvents.OnShawarmaProduced -= OnProgressEvent;
        GameProgressEvents.OnEarningsThisSecondChecked -= OnEarningsThisSecondChecked;
    }

    private void Start()
    {
        BuildDefinitionsFromDoc();
        if (SaveLoadManager.saveLoadManagerInstance != null)
        {
            SaveLoadManager.saveLoadManagerInstance.Register(this);
            GameManager.gameManagerInstance?.RecordPersistentRegistrations().Forget();
        }
        EvaluateAll();
    }

    private void OnDestroy()
    {
        if (SaveLoadManager.saveLoadManagerInstance != null)
            SaveLoadManager.saveLoadManagerInstance.Unregister(this);
        if (Instance == this) Instance = null;
    }

    private void OnProgressEvent(int a, float b) { EvaluateAll(); }
    private void OnProgressEvent(UpgradeType t, float c) { EvaluateAll(); }
    private void OnProgressEvent(float amount) { EvaluateAll(); }
    private void OnProgressEvent(int count) { EvaluateAll(); }

    public bool IsClaimed(string achievementId) => claimedIds.Contains(achievementId);

    public bool IsUnlocked(string achievementId)
    {
        var def = GetDefinition(achievementId);
        return def != null && GetCurrentValue(def) >= def.targetValue;
    }

    public float GetCurrentValue(AchievementDefinition def)
    {
        if (PlayerProgress.Instance == null) return 0;
        switch (def.conditionType)
        {
            case AchievementConditionType.TotalProduced: return PlayerProgress.Instance.ShwarmaCount;
            case AchievementConditionType.TotalEarnings: return PlayerProgress.Instance.TotalEarnings;
            case AchievementConditionType.TotalDeliveries: return PlayerProgress.Instance.TotalDeliveriesCompleted;
            case AchievementConditionType.TotalCateringOrders: return PlayerProgress.Instance.TotalCateringOrdersCompleted;
            case AchievementConditionType.TotalUpgrades: return PlayerProgress.Instance.TotalUpgradesPurchased;
            case AchievementConditionType.PlayerCash: return PlayerProgress.Instance.PlayerCash;
            case AchievementConditionType.ChefStars: return PlayerProgress.Instance.ChefStars;
            case AchievementConditionType.StorageCapacity: return WarehouseManager.Instance != null ? WarehouseManager.Instance.GetWholeCapacity() : 0;
            case AchievementConditionType.StoredShawarmas: return WarehouseManager.Instance != null ? WarehouseManager.Instance.GetWholeLoad() : 0;
            case AchievementConditionType.DeliveryCapacityPerMin: return GetDeliveryCapacityPerMin();
            case AchievementConditionType.EarnInOneSecond: return maxEarningsInOneSecondEver;
            default: return 0;
        }
    }

    private float GetDeliveryCapacityPerMin()
    {
        if (DeliveryManager.Instance == null || DeliveryManager.Instance.Deliverys == null) return 0;
        float total = 0f;
        foreach (var go in DeliveryManager.Instance.Deliverys)
        {
            var d = go != null ? go.GetComponent<Delivery>() : null;
            if (d == null || d.currentUpdate <= 1) continue;
            if (d.spawnInterval > 0) total += d.deliverCapacity * (60f / d.spawnInterval);
        }
        return total;
    }

    private AchievementDefinition GetDefinition(string id)
    {
        foreach (var d in definitions) if (d.id == id) return d;
        return null;
    }

    private void BuildDefinitionsFromDoc()
    {
        definitions.Clear();
        // Doc achievement table (implementable without Research)
        definitions.Add(new AchievementDefinition { id = "two_hundred", title = "Two Hundred", description = "Produce 200 shawarmas", conditionType = AchievementConditionType.TotalProduced, targetValue = 200, rewardType = ChallengeRewardType.Cash, rewardValue = 1500 });
        definitions.Add(new AchievementDefinition { id = "get_going", title = "Get Going", description = "Earn $500 in one second", conditionType = AchievementConditionType.EarnInOneSecond, targetValue = 500, rewardType = ChallengeRewardType.Cash, rewardValue = 150000 });
        definitions.Add(new AchievementDefinition { id = "growing_family", title = "Growing Family", description = "Expand warehouses to hold 4,200 shawarmas", conditionType = AchievementConditionType.StorageCapacity, targetValue = 4200, rewardType = ChallengeRewardType.Cash, rewardValue = 500000 });
        definitions.Add(new AchievementDefinition { id = "more_production", title = "More Production", description = "Produce 50,000 shawarmas total", conditionType = AchievementConditionType.TotalProduced, targetValue = 50000, rewardType = ChallengeRewardType.Cash, rewardValue = 1000000 });
        definitions.Add(new AchievementDefinition { id = "big_storage", title = "Big Storage", description = "Expand warehouses to hold 15,000 shawarmas", conditionType = AchievementConditionType.StorageCapacity, targetValue = 15000, rewardType = ChallengeRewardType.Gold, rewardValue = 24 });
        definitions.Add(new AchievementDefinition { id = "supply_chain", title = "Supply Chain", description = "Have delivery capacity of 250 shawarmas/min", conditionType = AchievementConditionType.DeliveryCapacityPerMin, targetValue = 250, rewardType = ChallengeRewardType.Gold, rewardValue = 24 });
        definitions.Add(new AchievementDefinition { id = "rack_it_in", title = "Rack It In", description = "Earn $1 Million in one second", conditionType = AchievementConditionType.EarnInOneSecond, targetValue = 1000000, rewardType = ChallengeRewardType.Gold, rewardValue = 48 });
        definitions.Add(new AchievementDefinition { id = "get_rich_quick", title = "Get Rich Quick", description = "Earn $5 Million in one second", conditionType = AchievementConditionType.EarnInOneSecond, targetValue = 5000000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        definitions.Add(new AchievementDefinition { id = "production_everywhere", title = "Production Everywhere", description = "Produce 50,000 shawarmas total", conditionType = AchievementConditionType.TotalProduced, targetValue = 50000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        definitions.Add(new AchievementDefinition { id = "shawarma_city", title = "Shawarma City", description = "1 Million shawarmas stored in warehouses", conditionType = AchievementConditionType.StoredShawarmas, targetValue = 1000000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        definitions.Add(new AchievementDefinition { id = "yuuge_storage", title = "YUUGE Storage", description = "Expand warehouses to hold 2 Million shawarmas", conditionType = AchievementConditionType.StorageCapacity, targetValue = 2000000, rewardType = ChallengeRewardType.Gold, rewardValue = 500 });
        definitions.Add(new AchievementDefinition { id = "cash_avalanche", title = "Cash Avalanche", description = "Lifetime earnings exceed $500 Million", conditionType = AchievementConditionType.TotalEarnings, targetValue = 500000000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        definitions.Add(new AchievementDefinition { id = "money_vault", title = "Money Vault", description = "$1 Billion in the bank", conditionType = AchievementConditionType.PlayerCash, targetValue = 1000000000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        definitions.Add(new AchievementDefinition { id = "shawarma_metropolis", title = "Shawarma Metropolis", description = "5 Million shawarmas stored in warehouses", conditionType = AchievementConditionType.StoredShawarmas, targetValue = 5000000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        definitions.Add(new AchievementDefinition { id = "epic_storage", title = "Epic Storage", description = "Expand warehouses to hold 50 Million shawarmas", conditionType = AchievementConditionType.StorageCapacity, targetValue = 50000000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        definitions.Add(new AchievementDefinition { id = "shawarma_country", title = "Shawarma Country", description = "50 Million shawarmas stored in warehouses", conditionType = AchievementConditionType.StoredShawarmas, targetValue = 50000000, rewardType = ChallengeRewardType.Gold, rewardValue = 2000 });
        definitions.Add(new AchievementDefinition { id = "soul_search", title = "Soul Search", description = "Collect 50,000 Chef Stars", conditionType = AchievementConditionType.ChefStars, targetValue = 50000, rewardType = ChallengeRewardType.Gold, rewardValue = 1200 });
        definitions.Add(new AchievementDefinition { id = "shawarma_planet", title = "Shawarma Planet", description = "150 Million shawarmas stored in warehouses", conditionType = AchievementConditionType.StoredShawarmas, targetValue = 150000000, rewardType = ChallengeRewardType.ChefStar, rewardValue = 1 });
        definitions.Add(new AchievementDefinition { id = "shawarma_galaxy", title = "Shawarma Galaxy", description = "300 Million shawarmas stored in warehouses", conditionType = AchievementConditionType.StoredShawarmas, targetValue = 300000000, rewardType = ChallengeRewardType.Gold, rewardValue = 5000 });
        definitions.Add(new AchievementDefinition { id = "soul_king", title = "Soul King", description = "Collect 250,000 Chef Stars", conditionType = AchievementConditionType.ChefStars, targetValue = 250000, rewardType = ChallengeRewardType.Gold, rewardValue = 3000 });
        definitions.Add(new AchievementDefinition { id = "bonus_practice", title = "Bonus Practice", description = "Complete 5 bonus deliveries", conditionType = AchievementConditionType.TotalDeliveries, targetValue = 5, rewardType = ChallengeRewardType.Gold, rewardValue = 12 });
        definitions.Add(new AchievementDefinition { id = "production_grind", title = "Production Grind", description = "Produce 7,500 shawarmas total", conditionType = AchievementConditionType.TotalProduced, targetValue = 7500, rewardType = ChallengeRewardType.Gold, rewardValue = 24 });
        definitions.Add(new AchievementDefinition { id = "tons_of_production", title = "Tons of Production", description = "Produce 10,000 shawarmas total", conditionType = AchievementConditionType.TotalProduced, targetValue = 10000, rewardType = ChallengeRewardType.Gold, rewardValue = 24 });
        definitions.Add(new AchievementDefinition { id = "bigger_storage", title = "Bigger Storage", description = "Expand warehouses to hold 50,000 shawarmas", conditionType = AchievementConditionType.StorageCapacity, targetValue = 50000, rewardType = ChallengeRewardType.Gold, rewardValue = 48 });
        definitions.Add(new AchievementDefinition { id = "cash_flow", title = "Cash Flow", description = "Lifetime earnings exceed $10 Million", conditionType = AchievementConditionType.TotalEarnings, targetValue = 10000000, rewardType = ChallengeRewardType.Gold, rewardValue = 30 });
        definitions.Add(new AchievementDefinition { id = "save_dat_money", title = "Save Dat Money", description = "$2 Million in the bank", conditionType = AchievementConditionType.PlayerCash, targetValue = 2000000, rewardType = ChallengeRewardType.Gold, rewardValue = 48 });
    }

    private void EvaluateAll()
    {
        bool anyChanged = false;
        foreach (var def in definitions)
        {
            if (claimedIds.Contains(def.id)) continue;
            if (GetCurrentValue(def) >= def.targetValue) anyChanged = true;
        }
        if (anyChanged) { isDirty = true; OnAchievementsChanged?.Invoke(); }
    }

    private void OnEarningsThisSecondChecked(float amount)
    {
        if (amount > maxEarningsInOneSecondEver)
        {
            maxEarningsInOneSecondEver = amount;
            isDirty = true;
            OnAchievementsChanged?.Invoke();
        }
    }

    /// <summary>
    /// Returns how many achievements are unlocked but not yet claimed.
    /// Used by UI badges (e.g., Achievements panel button).
    /// </summary>
    public int GetClaimableCount()
    {
        int count = 0;
        foreach (var def in definitions)
        {
            if (def == null) continue;
            if (claimedIds.Contains(def.id)) continue;
            if (GetCurrentValue(def) >= def.targetValue)
                count++;
        }
        return count;
    }

    public string GetProgressText(AchievementDefinition def)
    {
        float current = GetCurrentValue(def);
        if (def.conditionType == AchievementConditionType.TotalEarnings || def.conditionType == AchievementConditionType.PlayerCash || def.conditionType == AchievementConditionType.EarnInOneSecond)
            return $"${current:N0} / ${def.targetValue:N0}";
        return $"{(int)current} / {(int)def.targetValue}";
    }

    public string GetRewardText(AchievementDefinition def)
    {
        switch (def.rewardType)
        {
            case ChallengeRewardType.Cash: return $"${def.rewardValue:N0}";
            case ChallengeRewardType.Gold: return $"{def.rewardValue:N0} Gold";
            case ChallengeRewardType.ChefStar: return $"{def.rewardValue} Chef Star(s)";
            default: return "";
        }
    }

    public bool Claim(string achievementId)
    {
        if (claimedIds.Contains(achievementId)) return false;
        var def = GetDefinition(achievementId);
        if (def == null) return false;
        if (GetCurrentValue(def) < def.targetValue) return false;

        switch (def.rewardType)
        {
            case ChallengeRewardType.Cash: GameManager.gameManagerInstance.AddCash(def.rewardValue); break;
            case ChallengeRewardType.Gold: GameManager.gameManagerInstance.AddGold(def.rewardValue); break;
            case ChallengeRewardType.ChefStar:
                if (PlayerProgress.Instance != null) PlayerProgress.Instance.ChefStars += (int)def.rewardValue;
                if (UIManager.Instance != null) UIManager.Instance.UpdateChefStarsText();
                break;
        }
        claimedIds.Add(achievementId);
        if (UIManager.Instance != null) { UIManager.Instance.UpdateUI(UIUpdateType.Cash); UIManager.Instance.UpdateUI(UIUpdateType.Gold); }
        isDirty = true;
        OnAchievementsChanged?.Invoke();
        return true;
    }

    #region ISaveable
    public bool IsDirty => isDirty;
    public void ClearDirty() => isDirty = false;

    public object CaptureState()
    {
        var list = new List<AchievementState>();
        foreach (var id in claimedIds) list.Add(new AchievementState { achievementId = id, claimed = true });
        return new AchievementsSaveData { claimedAchievements = list, maxEarningsInOneSecondEver = maxEarningsInOneSecondEver };
    }

    public void RestoreState(object state)
    {
        claimedIds.Clear();
        if (state is AchievementsSaveData data)
        {
            if (data.claimedAchievements != null)
                foreach (var s in data.claimedAchievements) claimedIds.Add(s.achievementId);
            maxEarningsInOneSecondEver = data.maxEarningsInOneSecondEver;
        }
        isDirty = false;
        OnAchievementsChanged?.Invoke();
    }

    public void SetInitialData()
    {
        claimedIds.Clear();
        isDirty = true;
        OnAchievementsChanged?.Invoke();
    }
    #endregion
}
