using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public double money = 0;
   

    private void Awake()
    {
        if (Instance == null) Instance = this;
        LoadData();
    }

    //private void Update()
    //{
    //    moneyText.text = "$" + money.ToString("F0");
    //}

    public void AddMoney(double amount)
    {
        money += amount;
    }

    public void SaveData()
    {
        PlayerPrefs.SetString("Money", money.ToString());
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("Money"))
            double.TryParse(PlayerPrefs.GetString("Money"), out money);
    }
}
