using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public double Coins { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddCoins(double amount)
    {
        Coins += amount;
        //UIManager.Instance?.UpdateUI();
    }

    public bool SpendCoins(double amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            //UIManager.Instance?.UpdateUI();
            return true;
        }
        return false;
    }

    public void ResetCoins()
    {
        Coins = 0;
        //UIManager.Instance?.UpdateUI();
    }


    public void SaveData()
    {
        PlayerPrefs.SetString("Money", Coins.ToString());
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("Money"))
        {
            double.TryParse(PlayerPrefs.GetString("Money"), out double Coin);
                Coins = Coin;
            
            //PlayerPrefs.GetString("Money");
        }
            
    }
}