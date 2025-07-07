using UnityEngine;

public static class DataBase
{
    // Player Cash
    public static int PlayerCash
    {
        get => PlayerPrefs.GetInt("PlayerCash", 0);
        set => PlayerPrefs.SetInt("PlayerCash", value);
    }

    // Total Shawarmas Made
    public static int TotalShawarmas
    {
        get => PlayerPrefs.GetInt("TotalShawarmas", 0);
        set => PlayerPrefs.SetInt("TotalShawarmas", value);
    }

    // Get upgrade level of a delivery point by index
    public static int GetDeliveryPointLevel(int index)
    {
        return PlayerPrefs.GetInt($"DeliveryPoint_Level_{index}", 0);
    }

    public static void SetDeliveryPointLevel(int index, int level)
    {
        PlayerPrefs.SetInt($"DeliveryPoint_Level_{index}", level);
    }

    // Get current shawarma count at a delivery point (optional)
    public static int GetDeliveryPointCount(int index)
    {
        return PlayerPrefs.GetInt($"DeliveryPoint_Count_{index}", 0);
    }

    public static void SetDeliveryPointCount(int index, int count)
    {
        PlayerPrefs.SetInt($"DeliveryPoint_Count_{index}", count);
    }

    // Save all changes
    public static void Save() => PlayerPrefs.Save();

    // Reset all game data (use with caution)
    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
