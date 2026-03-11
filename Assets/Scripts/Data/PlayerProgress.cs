
using UnityEngine;

public class PlayerProgress : ISaveable
{

    private static PlayerProgress instance;
    public static PlayerProgress Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerProgress();
            }
            return instance;
        }
    }
    public string SaveKey => "player_progress";
    PlayerProgress()
    {

    }
    private float playerCash;
    private int chefStars;
    private int shwarmaCount;
    private float totalEarnings;
    private float gold;
    private bool isDirty = false;

    // Phase 0: Progress tracking for challenges, achievements, statistics
    private int totalDeliveriesCompleted;
    private int totalCateringOrdersCompleted;
    private int totalUpgradesPurchased;
    private float totalMoneySpentOnUpgrades;
    private double totalPlayTimeSeconds;
    private string lastLoginUtc;
    
    public float TotalEarnings
    {
        get => totalEarnings;
        set
        {
            totalEarnings = value;
            isDirty = true;
        }
    }
    public int ShwarmaCount
    {
        get => shwarmaCount;
        set
        {
            shwarmaCount = value;
            isDirty = true;
        }
    }
    public float PlayerCash
    {
        get => playerCash;
        set
        {
            playerCash = value;
            isDirty = true;
        }
    }
    public float Gold
    {
        get => gold;
        set
        {
            gold = value;
            isDirty = true;
        }
    }


    public int ChefStars
    {
        get => chefStars;
        set
        {
            chefStars = value;
            isDirty = true;
        }
    }

    public int TotalDeliveriesCompleted => totalDeliveriesCompleted;
    public int TotalCateringOrdersCompleted => totalCateringOrdersCompleted;
    public int TotalUpgradesPurchased => totalUpgradesPurchased;
    public float TotalMoneySpentOnUpgrades => totalMoneySpentOnUpgrades;
    public double TotalPlayTimeSeconds => totalPlayTimeSeconds;
    public string LastLoginUtc => lastLoginUtc;

    public void IncrementTotalDeliveries()
    {
        totalDeliveriesCompleted++;
        isDirty = true;
    }

    public void IncrementTotalCateringOrders()
    {
        totalCateringOrdersCompleted++;
        isDirty = true;
    }

    public void RecordUpgradePurchase(float costSpent)
    {
        totalUpgradesPurchased++;
        totalMoneySpentOnUpgrades += costSpent;
        isDirty = true;
    }

    public void AddPlayTimeSeconds(double seconds)
    {
        totalPlayTimeSeconds += seconds;
        isDirty = true;
    }

    public void SetLastLoginUtc(string utcString)
    {
        lastLoginUtc = utcString;
        isDirty = true;
    }

    public void ResetDataOnPrestigue()
    {
        playerCash = 0;
        // chefStars = 0;
        shwarmaCount = 0;
        //  totalEarnings = 0;

        isDirty = true;
    }
    #region Save/Load
    public object CaptureState()
    {
        return new PlayerProgressData
        {
            playerCash = playerCash,
            gold = gold,
            chefStars = chefStars,
            shwarmaCount = shwarmaCount,
            totalEarnings = totalEarnings,
            totalDeliveriesCompleted = totalDeliveriesCompleted,
            totalCateringOrdersCompleted = totalCateringOrdersCompleted,
            totalUpgradesPurchased = totalUpgradesPurchased,
            totalMoneySpentOnUpgrades = totalMoneySpentOnUpgrades,
            totalPlayTimeSeconds = totalPlayTimeSeconds,
            lastLoginUtc = lastLoginUtc,
        };
    }
   
    public void RestoreState(object state)
    {
        if (state is not PlayerProgressData data)
            return;
        playerCash = data.playerCash;
        chefStars = data.chefStars;
        shwarmaCount = data.shwarmaCount;
        totalEarnings = data.totalEarnings;
        gold = data.gold;
        totalDeliveriesCompleted = data.totalDeliveriesCompleted;
        totalCateringOrdersCompleted = data.totalCateringOrdersCompleted;
        totalUpgradesPurchased = data.totalUpgradesPurchased;
        totalMoneySpentOnUpgrades = data.totalMoneySpentOnUpgrades;
        totalPlayTimeSeconds = data.totalPlayTimeSeconds;
        lastLoginUtc = data.lastLoginUtc ?? "";
        isDirty = false;
    }

    public void SetInitialData()
    {
        playerCash = 0;
        chefStars = 0;
        shwarmaCount = 0;
        gold = 0;
        totalEarnings = 0;
        totalDeliveriesCompleted = 0;
        totalCateringOrdersCompleted = 0;
        totalUpgradesPurchased = 0;
        totalMoneySpentOnUpgrades = 0f;
        totalPlayTimeSeconds = 0;
        lastLoginUtc = "";
        isDirty = true;
    }

    public bool IsDirty => isDirty;

    public void ClearDirty()
    {
        isDirty = false;
    }
    #endregion
}
public class PlayerProgressData
{
    public float playerCash;
    public int chefStars;
    public int shwarmaCount;
    public float totalEarnings;
    public float gold;
    public int totalDeliveriesCompleted;
    public int totalCateringOrdersCompleted;
    public int totalUpgradesPurchased;
    public float totalMoneySpentOnUpgrades;
    public double totalPlayTimeSeconds;
    public string lastLoginUtc;
}
