
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
    private bool isDirty=false;
    
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
   
    
    public int ChefStars
    {
        get => chefStars;
        set
        {
            chefStars = value;
            isDirty = true;
        }
    }
    #region Save/Load
    public object CaptureState()
    {
        return new PlayerProgressData
        {
            playerCash = playerCash,
            chefStars = chefStars,
            shwarmaCount = shwarmaCount,
            totalEarnings = totalEarnings,
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

        isDirty = false;
    }

    public void SetInitialData()
    {
        playerCash = 0;
        chefStars = 0;
        shwarmaCount = 0;
        totalEarnings = 0;

        isDirty=true;
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
}
