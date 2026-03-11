using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Cysharp.Threading.Tasks;

public enum BoostEffectType
{
    EarningsMult,
    ProductionMult,
    AllBoostsMult,
    ChefStarMult,
    /// <summary>One-time: grant cash = multiplier (e.g. 0.1) × TotalEarnings when activated. Not added to active list.</summary>
    OneTimeCashGrant
}

public enum BoostCostType
{
    Gold,
    WatchAd
}

[Serializable]
public class BoostDefinition
{
    public string id;
    public string displayName;
    public string description;
    public BoostEffectType effectType;
    public float multiplier;
    public float durationSeconds;
    public BoostCostType costType;
    public float costAmount;
}

[Serializable]
public class ActiveBoostState
{
    public string boostId;
    public string endTimeUtc;
}

[Serializable]
public class BoostsSaveData
{
    public List<ActiveBoostState> activeBoosts = new List<ActiveBoostState>();
}

/// <summary>Phase 4: Boost shop and active boosts. Apply multipliers in GameManager.</summary>
public class BoostManager : MonoBehaviour, ISaveable
{
    public static BoostManager Instance { get; private set; }

    private const string SaveKeyBoosts = "boosts";
    private List<BoostDefinition> definitions = new List<BoostDefinition>();
    private List<ActiveBoostState> activeBoosts = new List<ActiveBoostState>();
    private bool isDirty;

    public string SaveKey => SaveKeyBoosts;
    public IReadOnlyList<BoostDefinition> Definitions => definitions;
    public IReadOnlyList<ActiveBoostState> ActiveBoosts => activeBoosts;

    public event Action OnBoostsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        BuildDefinitionsFromDoc();
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

    private void Update()
    {
        RemoveExpiredBoosts();
    }

    private void BuildDefinitionsFromDoc()
    {
        definitions.Clear();
        // Doc: "Unlimited production for 10min" – use very high multiplier to approximate unlimited
        definitions.Add(new BoostDefinition { id = "quantum_kitchen", displayName = "Quantum Kitchen Boost", description = "Unlimited production for 10min", effectType = BoostEffectType.ProductionMult, multiplier = 100f, durationSeconds = 600f, costType = BoostCostType.WatchAd, costAmount = 0 });
        definitions.Add(new BoostDefinition { id = "chefs_special", displayName = "Chef's Special Recipe", description = "3x earnings for 20min", effectType = BoostEffectType.EarningsMult, multiplier = 3f, durationSeconds = 1200f, costType = BoostCostType.WatchAd, costAmount = 0 });
        definitions.Add(new BoostDefinition { id = "chefs_premium", displayName = "Chef's Premium Recipe", description = "10x earnings for 15min", effectType = BoostEffectType.EarningsMult, multiplier = 10f, durationSeconds = 900f, costType = BoostCostType.WatchAd, costAmount = 0 });
        definitions.Add(new BoostDefinition { id = "chefs_best", displayName = "Chef's Best Recipe", description = "50x earnings for 10min", effectType = BoostEffectType.EarningsMult, multiplier = 50f, durationSeconds = 600f, costType = BoostCostType.Gold, costAmount = 2500f });
        definitions.Add(new BoostDefinition { id = "production_prism", displayName = "Production Prism", description = "10x auto-production for 10min", effectType = BoostEffectType.ProductionMult, multiplier = 10f, durationSeconds = 600f, costType = BoostCostType.WatchAd, costAmount = 0 });
        definitions.Add(new BoostDefinition { id = "large_production_prism", displayName = "Large Production Prism", description = "10x auto-production for 4hr", effectType = BoostEffectType.ProductionMult, multiplier = 10f, durationSeconds = 14400f, costType = BoostCostType.Gold, costAmount = 500f });
        definitions.Add(new BoostDefinition { id = "boost_amplifier", displayName = "Boost Amplifier", description = "2x all active boosts for 30min", effectType = BoostEffectType.AllBoostsMult, multiplier = 2f, durationSeconds = 1800f, costType = BoostCostType.Gold, costAmount = 1000f });
        definitions.Add(new BoostDefinition { id = "epic_boost_amplifier", displayName = "Epic Boost Amplifier", description = "10x all active boosts for 10min", effectType = BoostEffectType.AllBoostsMult, multiplier = 10f, durationSeconds = 600f, costType = BoostCostType.Gold, costAmount = 8000f });
        definitions.Add(new BoostDefinition { id = "chef_star_beacon", displayName = "Chef Star Beacon", description = "5x Chef Stars for 30min", effectType = BoostEffectType.ChefStarMult, multiplier = 5f, durationSeconds = 1800f, costType = BoostCostType.Gold, costAmount = 200f });
        // Doc: "+10% of Business Value" – one-time cash grant; Business Value = TotalEarnings (lifetime)
        definitions.Add(new BoostDefinition { id = "business_grant", displayName = "Business Grant", description = "+10% of Business Value (one-time cash)", effectType = BoostEffectType.OneTimeCashGrant, multiplier = 0.1f, durationSeconds = 0f, costType = BoostCostType.Gold, costAmount = 200f });
        definitions.Add(new BoostDefinition { id = "chef_star_2x", displayName = "Chef Star 2x", description = "2x Chef Stars for 10min", effectType = BoostEffectType.ChefStarMult, multiplier = 2f, durationSeconds = 600f, costType = BoostCostType.Gold, costAmount = 100f });
    }

    private BoostDefinition GetDefinition(string id)
    {
        foreach (var d in definitions) if (d.id == id) return d;
        return null;
    }

    private static DateTime? ParseUtc(string utc)
    {
        if (string.IsNullOrEmpty(utc)) return null;
        return DateTime.TryParse(utc, null, DateTimeStyles.RoundtripKind, out var d) ? d : (DateTime?)null;
    }

    private void RemoveExpiredBoosts()
    {
        var now = DateTime.UtcNow;
        int removed = activeBoosts.RemoveAll(b => ParseUtc(b.endTimeUtc).HasValue && now >= ParseUtc(b.endTimeUtc).Value);
        if (removed > 0) { isDirty = true; OnBoostsChanged?.Invoke(); }
    }

    private bool IsActive(string boostId)
    {
        var end = GetEndTime(boostId);
        return end.HasValue && DateTime.UtcNow < end.Value;
    }

    public bool IsBoostActive(string boostId) => IsActive(boostId);

    private DateTime? GetEndTime(string boostId)
    {
        foreach (var b in activeBoosts) if (b.boostId == boostId) return ParseUtc(b.endTimeUtc);
        return null;
    }

    public float GetEarningsMultiplier()
    {
        float mult = 1f;
        foreach (var b in activeBoosts)
        {
            var def = GetDefinition(b.boostId);
            if (def == null || def.effectType != BoostEffectType.EarningsMult) continue;
            if (!IsActive(b.boostId)) continue;
            mult *= def.multiplier;
        }
        float allMult = 1f;
        foreach (var b in activeBoosts)
        {
            var def = GetDefinition(b.boostId);
            if (def == null || def.effectType != BoostEffectType.AllBoostsMult) continue;
            if (!IsActive(b.boostId)) continue;
            allMult *= def.multiplier;
        }
        return mult * allMult;
    }

    public float GetProductionMultiplier()
    {
        float mult = 1f;
        foreach (var b in activeBoosts)
        {
            var def = GetDefinition(b.boostId);
            if (def == null || def.effectType != BoostEffectType.ProductionMult) continue;
            if (!IsActive(b.boostId)) continue;
            mult *= def.multiplier;
        }
        float allMult = 1f;
        foreach (var b in activeBoosts)
        {
            var def = GetDefinition(b.boostId);
            if (def == null || def.effectType != BoostEffectType.AllBoostsMult) continue;
            if (!IsActive(b.boostId)) continue;
            allMult *= def.multiplier;
        }
        return mult * allMult;
    }

    public float GetChefStarMultiplier()
    {
        float mult = 1f;
        foreach (var b in activeBoosts)
        {
            var def = GetDefinition(b.boostId);
            if (def == null || def.effectType != BoostEffectType.ChefStarMult) continue;
            if (!IsActive(b.boostId)) continue;
            mult *= def.multiplier;
        }
        float allMult = 1f;
        foreach (var b in activeBoosts)
        {
            var def = GetDefinition(b.boostId);
            if (def == null || def.effectType != BoostEffectType.AllBoostsMult) continue;
            if (!IsActive(b.boostId)) continue;
            allMult *= def.multiplier;
        }
        return mult * allMult;
    }

    public bool TryActivate(string boostId, Action onAdWatched = null)
    {
        var def = GetDefinition(boostId);
        if (def == null) return false;
        // Prevent re-activating the same boost while it's already active (no stacking).
        if (def.durationSeconds > 0f && IsActive(def.id)) return false;
        if (def.costType == BoostCostType.Gold)
        {
            if (GameManager.gameManagerInstance == null || !GameManager.gameManagerInstance.SpendGold(def.costAmount)) return false;
        }
        else if (def.costType == BoostCostType.WatchAd)
        {
            if (onAdWatched != null) onAdWatched.Invoke();
            else return false;
        }
        if (def.effectType == BoostEffectType.OneTimeCashGrant)
        {
            ApplyOneTimeCashGrant(def);
            if (UIManager.Instance != null) UIManager.Instance.UpdateUI(UIUpdateType.Gold);
            OnBoostsChanged?.Invoke();
            return true;
        }
        var endUtc = DateTime.UtcNow.AddSeconds(def.durationSeconds).ToString("o");
        activeBoosts.Add(new ActiveBoostState { boostId = def.id, endTimeUtc = endUtc });
        isDirty = true;
        OnBoostsChanged?.Invoke();
        if (UIManager.Instance != null) UIManager.Instance.UpdateUI(UIUpdateType.Gold);
        return true;
    }

    private void ApplyOneTimeCashGrant(BoostDefinition def)
    {
        if (GameManager.gameManagerInstance == null || PlayerProgress.Instance == null) return;
        float businessValue = PlayerProgress.Instance.TotalEarnings;
        float grant = businessValue * def.multiplier;
        if (grant > 0f) GameManager.gameManagerInstance.AddCash(grant);
    }

    public void ActivateAfterAd(string boostId)
    {
        var def = GetDefinition(boostId);
        if (def == null) return;
        // OneTimeCashGrant is Gold-only in doc; no-op if called after "ad" by mistake
        if (def.effectType == BoostEffectType.OneTimeCashGrant) return;
        // Prevent re-activating the same boost while it's already active (no stacking).
        if (def.durationSeconds > 0f && IsActive(def.id)) return;
        var endUtc = DateTime.UtcNow.AddSeconds(def.durationSeconds).ToString("o");
        activeBoosts.Add(new ActiveBoostState { boostId = def.id, endTimeUtc = endUtc });
        isDirty = true;
        OnBoostsChanged?.Invoke();
    }

    public string GetRemainingTime(string boostId)
    {
        var end = GetEndTime(boostId);
        if (!end.HasValue) return "";
        var left = end.Value - DateTime.UtcNow;
        if (left.TotalSeconds <= 0) return "";
        if (left.TotalMinutes >= 1) return $"{(int)left.TotalMinutes}m {left.Seconds}s";
        return $"{(int)left.TotalSeconds}s";
    }

    #region ISaveable
    public bool IsDirty => isDirty;
    public void ClearDirty() => isDirty = false;

    public object CaptureState()
    {
        return new BoostsSaveData { activeBoosts = new List<ActiveBoostState>(activeBoosts) };
    }

    public void RestoreState(object state)
    {
        if (state is BoostsSaveData data && data.activeBoosts != null)
            activeBoosts = data.activeBoosts;
        isDirty = false;
        OnBoostsChanged?.Invoke();
    }

    public void SetInitialData()
    {
        activeBoosts.Clear();
        isDirty = true;
        OnBoostsChanged?.Invoke();
    }
    #endregion
}
