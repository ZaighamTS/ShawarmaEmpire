using System.Collections;
using System.Collections.Generic;
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
    private bool isDirty;
    private float totalEarnings;
    public float PlayerCash
    {
        get => playerCash;
        set
        {
            playerCash = value;
            isDirty = true;
        }
    }
    public float TotalEarnings
    {
        get => totalEarnings;
        set
        {
            totalEarnings = value;
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
        return new PlayerProgress
        {
            playerCash = playerCash,
            chefStars = chefStars
        };
    }

    public void RestoreState(object state)
    {
        if (state is not PlayerProgressData data)
            return;
        playerCash = data.playerCash;
        chefStars = data.chefStars;
        isDirty = false;
    }

    public void SetInitialData()
    {
        playerCash = 0;
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
    public int playerCash;
    public int chefStars;
}
