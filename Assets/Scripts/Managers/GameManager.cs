using Cysharp.Threading.Tasks;
using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(50)]
public class GameManager : MonoBehaviour
{
    PlayerProgress playerProgress;
    public static GameManager gameManagerInstance;

    private float chefStars;
    [SerializeField] private int maxSubscribersCount = 11;
    [SerializeField] private int subscribersCount = 0;
    
    // Automatic earning system: Each spawned shawarma adds 0.01 per second
    private float automaticEarningRate = 0f; // Earnings per second (base rate)
    private float automaticEarningMultiplier = 1f; // Multiplier from prestige and upgrades
    private float automaticEarningAccumulator = 0f; // Accumulated earnings since last update
    private float lastAutomaticEarningUpdate = 0f;
    private const float AUTOMATIC_EARNING_UPDATE_INTERVAL = 1f; // Update every second

    // Phase 0: Play time accumulated this second (flushed every AUTOMATIC_EARNING_UPDATE_INTERVAL)
    private float playTimeAccumulatorThisInterval = 0f;

    // Offline timer: refresh CurrentDateTime periodically so force-close doesn't leave a stale timestamp.
    // Grace period ensures we don't overwrite before CheckOfflineEarning has run (it runs after load + 500ms).
    private float lastOfflineTimestampSave = 0f;
    private const float OFFLINE_TIMESTAMP_SAVE_INTERVAL = 8f;
    private const float OFFLINE_TIMESTAMP_GRACE_PERIOD = 20f; // Don't refresh in Update() for this many seconds after start

    private void Awake()
    {
        if (gameManagerInstance == null)
            gameManagerInstance = this;
    }
    private void Start()
    {
        playerProgress = PlayerProgress.Instance;
        SaveLoadManager.saveLoadManagerInstance.Register(PlayerProgress.Instance);
        RecordPersistentRegistrations().Forget();
        // ExtraBuildingsPlacement placement = FindObjectOfType<ExtraBuildingsPlacement>();
        // placement.CurrentLevel = 1;
        if (TransitionEffectScript.PrestigeDone)
        {
           
          
            UIManager.Instance.StartPanel.SetActive(false);
            
        }
        Invoke("DelayOnStart", 1.1f);
        
        // Initialize automatic earning system
        lastAutomaticEarningUpdate = Time.time;
        lastOfflineTimestampSave = Time.time;
        UpdateAutomaticEarningRate();
    }
    public void DelayOnStart()
    {
        UIManager.Instance.UpdateUI(UIUpdateType.Cash);
        UIManager.Instance.UpdateUI(UIUpdateType.Gold);
        UIManager.Instance.UpdateChefStarsText();
        UIManager.Instance.UpdateUI(UIUpdateType.Storage);
       // Debug.Log("here");
        // GameObject.FindObjectByType(TransitionEffectScript );
        if (TransitionEffectScript.PrestigeDone)
        {
            TransitionEffectScript.PrestigeDone = false;
            FindObjectOfType<TransitionEffectScript>().transform.GetChild(0).gameObject.SetActive(false);
            UIManager.Instance.GameplayPanel.SetActive(true);
            // FindFirstObjectByType<TransitionEffectScript>().transform.GetChild(0).gameObject.SetActive(false);
        }


      

        // UIManager.Instance.Up
    }

    public void CheckOfflineEarning()
    {
        // Check if CurrentDateTime exists and is valid
        string currentDateTimeStr = PlayerPrefs.GetString("CurrentDateTime", "");
        if (string.IsNullOrEmpty(currentDateTimeStr))
        {
            Debug.Log("[Offline] No saved CurrentDateTime found - skipping (first launch)");
            return; // First time playing, no offline earnings
        }
        
        // Parse saved time: prefer exact "o" (round-trip) format, then legacy formats
        DateTime savedTime;
        bool parsed = DateTime.TryParseExact(currentDateTimeStr, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out savedTime)
            || DateTime.TryParse(currentDateTimeStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out savedTime)
            || DateTime.TryParse(currentDateTimeStr, null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out savedTime);
        if (!parsed)
        {
            Debug.Log($"[Offline] Invalid CurrentDateTime format: '{currentDateTimeStr}' - skipping");
            return;
        }
        if (savedTime.Kind == DateTimeKind.Unspecified)
            savedTime = DateTime.SpecifyKind(savedTime, DateTimeKind.Utc);
        
        TimeSpan elapsed = DateTime.UtcNow - savedTime;
        double secondsElapsed = elapsed.TotalSeconds;
        
        // Require minimum offline time (e.g., 30 seconds) to prevent instant earnings
        const double minimumOfflineSeconds = 30.0;
        if (secondsElapsed < minimumOfflineSeconds)
        {
            Debug.Log($"[Offline] Time too short: {secondsElapsed:F0}s (min {minimumOfflineSeconds}s) - skipping");
            SaveCurrentDateTimeIfNeeded(); // Refresh so next time we have accurate "last active"
            return; // Too soon, probably same session
        }
        
        // Check if player has meaningful progress before calculating offline earnings
        // Require at least some cash earned or infrastructure built
        bool hasProgress = playerProgress.TotalEarnings > 0 || 
                          (WarehouseManager.Instance != null && WarehouseManager.Instance.placedWarehouses.Count > 0);
        
        if (!hasProgress)
        {
            Debug.Log("[Offline] No progress (TotalEarnings=0, no warehouses) - skipping");
            SaveCurrentDateTimeIfNeeded();
            return; // First time player, no offline earnings
        }
            
            // FIXED: Offline earnings now based on actual game state
            // Calculate based on storage capacity and delivery rate
            float shawarmaValue = UpgradeCosts.GetShawarmaValue(1);
            
            // Estimate delivery rate based on available systems
            float estimatedDeliveryRate = 0f;
            int totalCapacity = 0;
            float averageDeliverySize = 0f;
            
            // Calculate based on warehouse capacity and delivery intervals
            int totalStored = 0;
            if (WarehouseManager.Instance != null && WarehouseManager.Instance.placedWarehouses.Count > 0)
            {
                // Get total storage capacity and actual stored shawarmas (from save)
                totalCapacity = WarehouseManager.Instance.GetWholeCapacity();
                totalStored = WarehouseManager.Instance.GetWholeLoad();
                
                // Offline earnings are based on stored shawarmas only; no stored = minimum possible
                if (totalStored > 0)
                {
                    float deliveriesPerMinute = 1.2f;
                    float capacityBasedSize = Mathf.Min(totalCapacity * 0.05f, 20f);
                    averageDeliverySize = Mathf.Min(capacityBasedSize, totalStored);
                    float earningsPerMinute = shawarmaValue * averageDeliverySize * deliveriesPerMinute * 0.70f;
                    estimatedDeliveryRate = earningsPerMinute / 60f;
                }
                else
                {
                    // No shawarmas stored in any warehouse: minimum possible (zero rate)
                    estimatedDeliveryRate = 0f;
                    averageDeliverySize = 0f;
                }
            }
            else
            {
                // No warehouses: minimum possible
                estimatedDeliveryRate = 0f;
                totalCapacity = 0;
                averageDeliverySize = 0f;
            }
            
            // Cap offline time at 24 hours; effective earning time capped at 30 minutes
            double cappedSeconds = Math.Min(secondsElapsed, 86400.0);
            const double maxOfflineSecondsCap = 1800.0; // 30 minutes
            double effectiveSeconds = Math.Min(cappedSeconds, maxOfflineSecondsCap);
            
            // 1) Delivery-based: from stored shawarmas (capped by inventory)
            double deliveryEarnings = estimatedDeliveryRate * effectiveSeconds;
            if (WarehouseManager.Instance != null && WarehouseManager.Instance.placedWarehouses.Count > 0 && totalStored > 0)
            {
                double maxFromInventory = totalStored * shawarmaValue * 0.70;
                if (deliveryEarnings > maxFromInventory)
                    deliveryEarnings = maxFromInventory;
            }
            
            // 2) Automatic earning: from spawned shawarmas (same formula as in-game, for up to 30 min)
            float automaticRate = playerProgress.ShwarmaCount * 0.01f * UpgradeCosts.GetAutomaticEarningMultiplier();
            if (BoostManager.Instance != null)
                automaticRate *= BoostManager.Instance.GetProductionMultiplier();
            double automaticEarnings = automaticRate * effectiveSeconds;
            
            double amount = deliveryEarnings + automaticEarnings;
            const double absoluteMaxEarnings = 10000000.0;
            amount = Math.Min(amount, absoluteMaxEarnings);
            
            // Detailed logging for debugging (single block + per-line)
            double logMaxFromInventory = totalStored > 0 ? totalStored * shawarmaValue * 0.70 : 0;
            Debug.Log($"[Offline] === DETAIL ===\n" +
                $"  Time away: {secondsElapsed:F0}s ({FormatOfflineTimeAway(secondsElapsed)}), effective: {effectiveSeconds:F0}s (30 min max)\n" +
                $"  Shawarma value: ${shawarmaValue:F2} | Capacity: {totalCapacity} | Stored: {totalStored}\n" +
                $"  Delivery: size {averageDeliverySize:F1}, rate ${estimatedDeliveryRate:F2}/s → ${deliveryEarnings:F0}\n" +
                $"  Automatic: rate ${automaticRate:F2}/s (from {playerProgress.ShwarmaCount} spawned) → ${automaticEarnings:F0}\n" +
                $"  Max from inventory cap: ${logMaxFromInventory:F0} | Total amount: ${amount:F0}");
            Debug.Log($"=== OFFLINE EARNINGS CALCULATION ===");
            Debug.Log($"Time: {secondsElapsed:F0}s away, effective: {effectiveSeconds:F0}s (30 min max)");
            Debug.Log($"Shawarma Value: ${shawarmaValue:F2} | Capacity: {totalCapacity}, Stored: {totalStored}");
            Debug.Log($"Delivery: avg size {averageDeliverySize:F1}, rate ${estimatedDeliveryRate:F2}/s → ${deliveryEarnings:F0}");
            Debug.Log($"Automatic: rate ${automaticRate:F2}/s (ShwarmaCount={playerProgress.ShwarmaCount}) → ${automaticEarnings:F0}");
            Debug.Log($"Final Amount (delivery + automatic): ${amount:F0}");
            Debug.Log($"=====================================");
            
            if (amount > 0)
            {
                AddCash((float)amount);
                UIManager.Instance.UpdateUI(UIUpdateType.Cash);
                string timeAwayText = FormatOfflineTimeAway(secondsElapsed);
                UIManager.Instance.ShowInfoPopup($"Offline earnings: ${(int)amount:N0} ({timeAwayText})");
            }
            else
            {
                // Still show time away so player knows offline check ran (e.g. no stored shawarmas to earn from)
                string timeAwayText = FormatOfflineTimeAway(secondsElapsed);
                UIManager.Instance.ShowInfoPopup($"Welcome back! You were away for {timeAwayText}.");
            }
        // Always refresh timestamp after check so next session uses accurate "last active" time
        SaveCurrentDateTimeIfNeeded();
    }

    private static string FormatOfflineTimeAway(double totalSeconds)
    {
        if (totalSeconds < 60)
            return $"{(int)totalSeconds} seconds";
        int minutes = (int)(totalSeconds / 60);
        int secs = (int)Math.Floor(totalSeconds % 60);
        if (secs == 0)
            return $"{minutes} minute{(minutes == 1 ? "" : "s")}";
        return $"{minutes} min {secs} sec";
    }

    /// <summary>
    /// Writes current UTC time to PlayerPrefs for offline-earnings calculation.
    /// Call periodically during gameplay so force-close doesn't leave a stale timestamp.
    /// </summary>
    private void SaveCurrentDateTimeIfNeeded()
    {
        if (playerProgress == null) return;
        bool hasProgress = playerProgress.TotalEarnings > 0 ||
            (WarehouseManager.Instance != null && WarehouseManager.Instance.placedWarehouses != null && WarehouseManager.Instance.placedWarehouses.Count > 0);
        if (hasProgress)
            PlayerPrefs.SetString("CurrentDateTime", DateTime.UtcNow.ToString("o"));
        else
            PlayerPrefs.DeleteKey("CurrentDateTime");
    }

    private void OnDestroy()
    {
        SaveLoadManager.saveLoadManagerInstance.Unregister(PlayerProgress.Instance);
    }
    internal async UniTask RecordPersistentRegistrations()
    {
        subscribersCount++;
        if (subscribersCount == maxSubscribersCount)
        {
            await UniTask.NextFrame();
            SaveLoadManager.saveLoadManagerInstance.LoadGame().Forget();
        }
    }
    internal float GetCurrentCash()
    {
        return playerProgress.PlayerCash;
    }
    internal float GetTotalEarning()
    {
        return playerProgress.TotalEarnings;
    }
    internal float GetGold()
    { 
        return playerProgress.Gold;
    }
    //private void Update()
    //{
    //    moneyText.text = "$" + money.ToString("F0");
    //}
    internal void AddCash(float value)
    {
        float mult = BoostManager.Instance != null ? BoostManager.Instance.GetEarningsMultiplier() : 1f;
        value *= mult;
        playerProgress.PlayerCash += value;
        if (value > 0)
        {
            playerProgress.TotalEarnings += value;
            GameProgressEvents.NotifyCashEarned(value);
            UIManager.Instance.UpdateEarningSlider();
        }
        CheckChefStars(playerProgress.TotalEarnings);
    }
    internal void AddGold(float value)
    { 
        playerProgress.Gold+= value;
    }
    internal void AddTotalShawarama(int value)
    {
        playerProgress.ShwarmaCount += value;
    }

    internal bool SpendCash(float Value)
    {
        if (Value <= playerProgress.PlayerCash)
        {
            playerProgress.PlayerCash -= Value;
            return true;
        }
        return false;
    }
    internal bool SpendGold(float Value)
    {
        if (Value <= playerProgress.Gold)
        {
            playerProgress.Gold -= Value;
            return true;
        }
        return false;
    }

    private void CheckChefStars(float TotalEarning)
    {
        var newStars = UpgradeCosts.GetChefStars(TotalEarning);
        Debug.Log("newStars " + newStars);
        // Debug.Log("chefStars " + chefStars);
        if (newStars > playerProgress.ChefStars)
        {

            // ResetPlayerStats();
            UIManager.Instance.ShowPrestigeBtn();
        }
    }
    public void ResetPlayerStats()
    {
        //playerProgress.PlayerCash = 0;
        float starMult = BoostManager.Instance != null ? BoostManager.Instance.GetChefStarMultiplier() : 1f;
        int starsToAdd = Mathf.Max(1, Mathf.RoundToInt(1f * starMult));
        playerProgress.ChefStars += starsToAdd;
        SaveLoadManager.saveLoadManagerInstance.ResetAllISaveables();
        
        // Update automatic earning multiplier after prestige
        OnPrestigeOrUpgradeChanged();
    }
    [RuntimeInitializeOnLoadMethod]
    static void RegisterQuitCallback()
    {
        Application.wantsToQuit += OnAppWantsToQuit;
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
#endif
    }
    static bool OnAppWantsToQuit()
    {
        SaveLoadManager.saveLoadManagerInstance?.SaveGame();
        
        // Save CurrentDateTime if player has progress (handled in instance method)
        if (gameManagerInstance != null && gameManagerInstance.playerProgress != null)
        {
            if (gameManagerInstance.playerProgress.TotalEarnings > 0 || 
                (WarehouseManager.Instance != null && WarehouseManager.Instance.placedWarehouses.Count > 0))
            {
                PlayerPrefs.SetString("CurrentDateTime", DateTime.UtcNow.ToString("o"));
            }
            else
            {
                PlayerPrefs.DeleteKey("CurrentDateTime");
            }
        }
        return true;
    }
#if UNITY_EDITOR
    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            SaveLoadManager.saveLoadManagerInstance?.SaveGame();
            
            // Save CurrentDateTime if player has progress
            if (gameManagerInstance != null && gameManagerInstance.playerProgress != null)
            {
                if (gameManagerInstance.playerProgress.TotalEarnings > 0 || 
                    (WarehouseManager.Instance != null && WarehouseManager.Instance.placedWarehouses.Count > 0))
                {
                    PlayerPrefs.SetString("CurrentDateTime", DateTime.UtcNow.ToString("o"));
                }
                else
                {
                    PlayerPrefs.DeleteKey("CurrentDateTime");
                }
            }
        }
    }
#endif

private void OnApplicationQuit()
{
    SaveLoadManager.saveLoadManagerInstance.SaveGame();
    
    // Only save CurrentDateTime if player has meaningful progress
    // This prevents offline earnings on first launch
    if (playerProgress != null && (playerProgress.TotalEarnings > 0 || 
        (WarehouseManager.Instance != null && WarehouseManager.Instance.placedWarehouses.Count > 0)))
    {
        PlayerPrefs.SetString("CurrentDateTime", DateTime.UtcNow.ToString("o"));
        Debug.Log("Saved CurrentDateTime for offline earnings");
    }
    else
    {
        // Clear CurrentDateTime if no progress (first launch scenario)
        PlayerPrefs.DeleteKey("CurrentDateTime");
        Debug.Log("No progress detected - cleared CurrentDateTime");
    }
}
int i;
void OnApplicationPause(bool pauseStatus)
{
    i++;
    if (pauseStatus)
    {
        SaveLoadManager.saveLoadManagerInstance.SaveGame();
        
        // Only save CurrentDateTime if player has meaningful progress
        if (playerProgress != null && (playerProgress.TotalEarnings > 0 || 
            (WarehouseManager.Instance != null && WarehouseManager.Instance.placedWarehouses.Count > 0)))
        {
            PlayerPrefs.SetString("CurrentDateTime", DateTime.UtcNow.ToString("o"));
            Debug.Log($"Pause - Saved CurrentDateTime for offline earnings (pause count: {i})");
        }
        else
        {
            PlayerPrefs.DeleteKey("CurrentDateTime");
            Debug.Log($"Pause - No progress detected - cleared CurrentDateTime (pause count: {i})");
        }
    }
}
private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
          //  SaveLoadManager.saveLoadManagerInstance.SaveGame();
           // Debug.Log("focus Integer " + i);
        }
        
    }
    
    private void Update()
    {
        // Update automatic earning every second
        float currentTime = Time.time;
        if (currentTime - lastAutomaticEarningUpdate >= AUTOMATIC_EARNING_UPDATE_INTERVAL)
        {
            float deltaTime = currentTime - lastAutomaticEarningUpdate;
            UpdateAutomaticEarningRate();
            ProcessAutomaticEarnings(deltaTime);
            // Phase 0: Tick earnings-this-second for "Earn $X in one second" achievements
            GameProgressEvents.TickEarningsThisSecond();
            // Phase 0: Persist play time (once per second to avoid dirty every frame)
            playTimeAccumulatorThisInterval += deltaTime;
            playerProgress.AddPlayTimeSeconds(playTimeAccumulatorThisInterval);
            playTimeAccumulatorThisInterval = 0f;
            lastAutomaticEarningUpdate = currentTime;
        }
        // Refresh offline timestamp periodically so force-close doesn't leave a stale time.
        // Only after grace period so we don't overwrite before CheckOfflineEarning has run on cold start.
        if (currentTime >= OFFLINE_TIMESTAMP_GRACE_PERIOD && currentTime - lastOfflineTimestampSave >= OFFLINE_TIMESTAMP_SAVE_INTERVAL)
        {
            lastOfflineTimestampSave = currentTime;
            SaveCurrentDateTimeIfNeeded();
        }
    }
    
    /// <summary>
    /// Updates the automatic earning rate based on spawned shawarmas and multiplier
    /// Formula: Base rate = ShwarmaCount * 0.01 per second
    /// Final rate = Base rate * Multiplier (from prestige and upgrades)
    /// </summary>
    private void UpdateAutomaticEarningRate()
    {
        // Base earning rate: 0.01 per second per spawned shawarma
        float baseRate = playerProgress.ShwarmaCount * 0.01f;
        
        // Get multiplier from prestige and upgrades
        automaticEarningMultiplier = UpgradeCosts.GetAutomaticEarningMultiplier();
        
        // Apply multiplier to base rate (prestige/upgrades)
        automaticEarningRate = baseRate * automaticEarningMultiplier;
        // Phase 4: production boosts (e.g. Production Prism)
        float productionBoost = BoostManager.Instance != null ? BoostManager.Instance.GetProductionMultiplier() : 1f;
        automaticEarningRate *= productionBoost;
        
        // Update UI to show automatic earning rate and multiplier
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateAutomaticEarningDisplay(automaticEarningRate, automaticEarningMultiplier);
        }
    }
    
    /// <summary>
    /// Processes automatic earnings and adds them to player cash
    /// </summary>
    private void ProcessAutomaticEarnings(float deltaTime)
    {
        if (automaticEarningRate > 0f)
        {
            float earnings = automaticEarningRate * deltaTime;
            if (earnings > 0f)
            {
                AddCash(earnings);
                // Update UI less frequently to avoid spam
                UIManager.Instance.UpdateUI(UIUpdateType.Cash);
            }
        }
    }
    
    /// <summary>
    /// Gets the current automatic earning rate per second (with multiplier applied)
    /// </summary>
    public float GetAutomaticEarningRate()
    {
        return automaticEarningRate;
    }
    
    /// <summary>
    /// Gets the current automatic earning multiplier
    /// </summary>
    public float GetAutomaticEarningMultiplier()
    {
        return automaticEarningMultiplier;
    }
    
    /// <summary>
    /// Called when a shawarma is spawned to update automatic earning rate
    /// </summary>
    public void OnShawarmaSpawned()
    {
        UpdateAutomaticEarningRate();
    }
    
    /// <summary>
    /// Called when prestige or upgrades change to update automatic earning multiplier
    /// </summary>
    public void OnPrestigeOrUpgradeChanged()
    {
        UpdateAutomaticEarningRate();
    }
}
