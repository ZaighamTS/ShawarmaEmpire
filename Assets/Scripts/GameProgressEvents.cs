using System;

/// <summary>
/// Central place for progress tracking used by Challenges, Achievements, and Statistics.
/// Phase 0: Foundation for doc-aligned features (challenges, achievements, stats screen).
/// </summary>
public static class GameProgressEvents
{
    /// <summary>Fired when a delivery van completes a delivery. Args: shawarma count, cash earned.</summary>
    public static event Action<int, float> OnDeliveryCompleted;

    /// <summary>Fired when a catering van completes an order. Args: shawarma count, cash earned.</summary>
    public static event Action<int, float> OnCateringCompleted;

    /// <summary>Fired when the player earns cash (any source). Args: amount.</summary>
    public static event Action<float> OnCashEarned;

    /// <summary>Fired when the player purchases an upgrade. Args: upgrade type, cost spent.</summary>
    public static event Action<UpgradeType, float> OnUpgradePurchased;

    /// <summary>Fired when a shawarma is produced (spawned). Args: count produced this call.</summary>
    public static event Action<int> OnShawarmaProduced;

    /// <summary>Earnings accumulated in the current second (reset every second). For "Earn $X in one second" achievements.</summary>
    public static float EarningsThisSecond => _earningsThisSecond;
    private static float _earningsThisSecond;

    /// <summary>Called by GameManager each second to snapshot and reset earnings-this-second.</summary>
    public static void TickEarningsThisSecond()
    {
        float snapshot = _earningsThisSecond;
        _earningsThisSecond = 0f;
        if (snapshot > 0f)
            OnEarningsThisSecondChecked?.Invoke(snapshot);
    }

    /// <summary>Fired every second with total cash earned in that second. For achievements like "Earn $500 in one second".</summary>
    public static event Action<float> OnEarningsThisSecondChecked;

    /// <summary>Add to the current second's earnings. Call from GameManager.AddCash.</summary>
    public static void AddEarningsThisSecond(float amount)
    {
        _earningsThisSecond += amount;
    }

    /// <summary>Call from GameManager.AddCash when cash is earned. Updates earnings-this-second and fires OnCashEarned.</summary>
    public static void NotifyCashEarned(float amount)
    {
        AddEarningsThisSecond(amount);
        OnCashEarned?.Invoke(amount);
    }

    /// <summary>Record a completed delivery and update lifetime stats. Call from DeliveryVan when cash is added.</summary>
    public static void RecordDelivery(int shawarmaCount, float cashEarned)
    {
        if (PlayerProgress.Instance == null) return;
        PlayerProgress.Instance.IncrementTotalDeliveries();
        OnDeliveryCompleted?.Invoke(shawarmaCount, cashEarned);
    }

    /// <summary>Record a completed catering order and update lifetime stats. Call from CateringVan when cash is added.</summary>
    public static void RecordCatering(int shawarmaCount, float cashEarned)
    {
        if (PlayerProgress.Instance == null) return;
        PlayerProgress.Instance.IncrementTotalCateringOrders();
        OnCateringCompleted?.Invoke(shawarmaCount, cashEarned);
    }

    /// <summary>Record an upgrade purchase. Call from Delivery/Warehouse/Kitchen/Catering after SpendCash(cost).</summary>
    public static void RecordUpgrade(UpgradeType type, float costSpent)
    {
        if (PlayerProgress.Instance == null) return;
        PlayerProgress.Instance.RecordUpgradePurchase(costSpent);
        OnUpgradePurchased?.Invoke(type, costSpent);
    }

    /// <summary>Record shawarmas produced. Call from ShawarmaSpawner when a shawarma is created. Fires OnShawarmaProduced.</summary>
    public static void RecordShawarmaProduced(int count)
    {
        OnShawarmaProduced?.Invoke(count);
    }
}
