using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress : ISaveable
{
    public static PlayerProgress Instance;
    public string SaveKey => "player_progress";
    PlayerProgress()
    {

    }
    private int playerCash;
    private bool isDirty;

    public int PlayerCash
    {
        get => playerCash;
        set
        {
            playerCash = value;
            isDirty = true;
        }
    }
    #region Save/Load
    public object CaptureState()
    {
        return new PlayerProgress
        {
            PlayerCash = PlayerCash,
        };
    }

    public void RestoreState(object state)
    {
        if (state is not PlayerProgressData data)
            return;
        playerCash = data.playerCash;
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
}
