using UnityEngine;

public static class GameDataManager
{
    private const string CashKey = "GameCash";
   

    public static int GetCash()
    {
        return PlayerPrefs.GetInt(CashKey, 1000); // default starting cash
    }

    public static void SetCash(int amount)
    {
        PlayerPrefs.SetInt(CashKey, amount);
    }

    public static bool IsBuildingPurchased(int index)
    {
        return PlayerPrefs.GetInt("building_purchased_" + index, 0) == 1;
    }

    public static void SaveBuildingPurchase(int index, bool state)
    {
        PlayerPrefs.SetInt("building_purchased_" + index, state ? 1 : 0);
    }

    public static void SaveAll()
    {
        PlayerPrefs.Save();
    }
}
