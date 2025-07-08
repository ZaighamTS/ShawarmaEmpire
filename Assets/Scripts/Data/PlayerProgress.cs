using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress : ISaveable
{
    public static PlayerProgress Instance;
    public string SaveKey => "";
    PlayerProgress()
    {

    }
    private int playerCash;
    private bool isDirty;

    public int PLayerCash
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
        return null;
    }

    public void RestoreState(object state)
    {
        if (state is not  PlayerProgressData data)
            return;
    }

    public void SetInitialData()
    {
        //Assign initial values if anything goes wrong
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

}
