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
            Debug.Log("No saved CurrentDateTime found - skipping offline earnings (first launch)");
            return; // First time playing, no offline earnings
        }
        
        if (!DateTime.TryParse(currentDateTimeStr, null, DateTimeStyles.RoundtripKind, out DateTime savedTime))
        {
            Debug.Log("Invalid CurrentDateTime format - skipping offline earnings");
            return; // Invalid date format
        }
        
        TimeSpan elapsed = DateTime.UtcNow - savedTime;
        double secondsElapsed = elapsed.TotalSeconds;
        
        // Require minimum offline time (e.g., 30 seconds) to prevent instant earnings
        const double minimumOfflineSeconds = 30.0;
        if (secondsElapsed < minimumOfflineSeconds)
        {
            Debug.Log($"Offline time too short ({secondsElapsed:F0}s < {minimumOfflineSeconds}s) - skipping offline earnings");
            return; // Too soon, probably same session
        }
        
        // Check if player has meaningful progress before calculating offline earnings
        // Require at least some cash earned or infrastructure built
        bool hasProgress = playerProgress.TotalEarnings > 0 || 
                          (WarehouseManager.Instance != null && WarehouseManager.Instance.placedWarehouses.Count > 0);
        
        if (!hasProgress)
        {
            Debug.Log("No meaningful progress detected - skipping offline earnings (first time player)");
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
            if (WarehouseManager.Instance != null && WarehouseManager.Instance.placedWarehouses.Count > 0)
            {
                // Get total storage capacity
                totalCapacity = WarehouseManager.Instance.GetWholeCapacity();
                
                // Estimate deliveries per minute (conservative estimate)
                // EXTENDED GAMEPLAY: Reduced delivery frequency and size estimate to extend gameplay
                // Before: 4 deliveries/min, 7% capacity (capped at 30)
                // After: 2 deliveries/min, 5% capacity (capped at 20) - slower progression for week-long gameplay
                float deliveriesPerMinute = 2f; // Reduced from 4 to slow down income further
                averageDeliverySize = Mathf.Min(totalCapacity * 0.05f, 20f); // Reduced from 7%/30 to 5%/20
                
                // Calculate earnings per minute (EXTENDED GAMEPLAY: Using 30% tax rate to match delivery system)
                float earningsPerMinute = shawarmaValue * averageDeliverySize * deliveriesPerMinute * 0.70f;
                
                // Convert to per second
                estimatedDeliveryRate = earningsPerMinute / 60f;
            }
            else
            {
                // Fallback: use a base rate if no warehouses
                estimatedDeliveryRate = shawarmaValue * 0.5f; // Very conservative
                totalCapacity = 0;
                averageDeliverySize = 0f;
            }
            
            // Cap offline time at 24 hours (86400 seconds)
            double cappedSeconds = Math.Min(secondsElapsed, 86400.0);
            
            // Calculate total offline earnings
            double potentialEarnings = estimatedDeliveryRate * cappedSeconds;
            
            // Cap maximum offline earnings at a reasonable amount (e.g., 1 hour of active play)
            double maxOfflineEarnings = estimatedDeliveryRate * 3600.0; // 1 hour cap
            double amount = Math.Min(potentialEarnings, maxOfflineEarnings);
            
            // Absolute maximum cap to prevent excessive earnings (e.g., $10M max)
            const double absoluteMaxEarnings = 10000000.0; // $10M absolute maximum
            amount = Math.Min(amount, absoluteMaxEarnings);
            
            // Detailed logging for debugging
            Debug.Log($"=== OFFLINE EARNINGS CALCULATION ===");
            Debug.Log($"Shawarma Value: ${shawarmaValue:F2}");
            Debug.Log($"Total Capacity: {totalCapacity}");
            Debug.Log($"Average Delivery Size: {averageDeliverySize:F1}");
            Debug.Log($"Estimated Delivery Rate: ${estimatedDeliveryRate:F2}/sec (${estimatedDeliveryRate * 60:F2}/min)");
            Debug.Log($"Time Offline: {secondsElapsed:F0} seconds ({secondsElapsed / 3600:F2} hours)");
            Debug.Log($"Capped Time: {cappedSeconds:F0} seconds ({cappedSeconds / 3600:F2} hours)");
            Debug.Log($"Potential Earnings: ${potentialEarnings:F0}");
            Debug.Log($"Max Offline Earnings (1hr cap): ${maxOfflineEarnings:F0}");
            Debug.Log($"Absolute Max Cap: $10,000,000");
            Debug.Log($"Final Amount: ${amount:F0}");
            Debug.Log($"=====================================");
            
            if (amount > 0)
            {
                AddCash((float)amount);
                UIManager.Instance.UpdateUI(UIUpdateType.Cash);
                UIManager.Instance.ShowInfoPopup($"Offline earnings: ${(int)amount:N0} ({(int)(cappedSeconds / 60)} minutes)");
            }
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
                PlayerPrefs.SetString("CurrentDateTime", DateTime.UtcNow.ToString());
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
                    PlayerPrefs.SetString("CurrentDateTime", DateTime.UtcNow.ToString());
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
        PlayerPrefs.SetString("CurrentDateTime", DateTime.UtcNow.ToString());
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
            PlayerPrefs.SetString("CurrentDateTime", DateTime.UtcNow.ToString());
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
