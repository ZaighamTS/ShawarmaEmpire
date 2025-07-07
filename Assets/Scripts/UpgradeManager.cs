using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public double upgradeCost = 100;
    public double incomeMultiplier = 1;
    public Text upgradeText;

    private void Start()
    {
        UpdateUI();
    }

    public void BuyUpgrade()
    {
        if (GameManager.Instance.money >= upgradeCost)
        {
            GameManager.Instance.money -= upgradeCost;
            incomeMultiplier += 1;
            upgradeCost *= 2;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        upgradeText.text = "Upgrade Income x" + incomeMultiplier + " ($" + upgradeCost + ")";
    }

    public double GetCurrentMultiplier()
    {
        return incomeMultiplier;
    }
}
