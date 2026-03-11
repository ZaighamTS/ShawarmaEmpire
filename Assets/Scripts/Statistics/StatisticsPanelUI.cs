using System;
using System.Globalization;
using UnityEngine;
using TMPro;

/// <summary>Phase 3: Statistics screen. Reads from PlayerProgress and GameManager; assign text refs in Inspector.</summary>
public class StatisticsPanelUI : MonoBehaviour
{
    [Header("Production")]
    [SerializeField] private TMP_Text totalProducedText;
    [SerializeField] private TMP_Text productionRatePerHourText;

    [Header("Delivery")]
    [SerializeField] private TMP_Text totalDeliveriesText;
    [SerializeField] private TMP_Text totalCateringOrdersText;

    [Header("Earnings")]
    [SerializeField] private TMP_Text totalEarningsText;
    [SerializeField] private TMP_Text earningsPerHourText;

    [Header("Upgrades")]
    [SerializeField] private TMP_Text totalUpgradesText;
    [SerializeField] private TMP_Text moneySpentOnUpgradesText;

    [Header("Time")]
    [SerializeField] private TMP_Text totalPlayTimeText;
    [SerializeField] private TMP_Text lastLoginText;

    [Header("Panel")]
    [SerializeField] private GameObject statisticsPanelRoot;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (PlayerProgress.Instance == null) return;

        var p = PlayerProgress.Instance;
        double playTimeSeconds = p.TotalPlayTimeSeconds;
        double playTimeHours = playTimeSeconds > 0 ? playTimeSeconds / 3600.0 : 0;

        if (totalProducedText != null)
            totalProducedText.text = "Total produced: " + p.ShwarmaCount.ToString("N0");

        if (productionRatePerHourText != null)
        {
            if (playTimeHours > 0)
                productionRatePerHourText.text = "Production rate/hr: " + (p.ShwarmaCount / playTimeHours).ToString("N0");
            else
                productionRatePerHourText.text = "Production rate/hr: —";
        }

        if (totalDeliveriesText != null)
            totalDeliveriesText.text = "Total deliveries: " + p.TotalDeliveriesCompleted.ToString("N0");

        if (totalCateringOrdersText != null)
            totalCateringOrdersText.text = "Total catering orders: " + p.TotalCateringOrdersCompleted.ToString("N0");

        if (totalEarningsText != null)
            totalEarningsText.text = "Total earned: $" + p.TotalEarnings.ToString("N0");

        if (earningsPerHourText != null)
        {
            if (playTimeHours > 0)
                earningsPerHourText.text = "Earnings/hr: $" + (p.TotalEarnings / (float)playTimeHours).ToString("N0");
            else
                earningsPerHourText.text = "Earnings/hr: —";
        }

        if (totalUpgradesText != null)
            totalUpgradesText.text = "Total upgrades: " + p.TotalUpgradesPurchased.ToString("N0");

        if (moneySpentOnUpgradesText != null)
            moneySpentOnUpgradesText.text = "Money spent on upgrades: $" + p.TotalMoneySpentOnUpgrades.ToString("N0");

        if (totalPlayTimeText != null)
            totalPlayTimeText.text = "Total play time: " + FormatPlayTime(playTimeSeconds);

        if (lastLoginText != null)
            lastLoginText.text = "Last login: " + FormatLastLogin(p.LastLoginUtc);
    }

    private static string FormatPlayTime(double totalSeconds)
    {
        if (totalSeconds <= 0) return "—";
        var ts = TimeSpan.FromSeconds(totalSeconds);
        if (ts.TotalHours >= 1)
            return $"{(int)ts.TotalHours}h {ts.Minutes}m";
        if (ts.TotalMinutes >= 1)
            return $"{(int)ts.TotalMinutes}m {ts.Seconds}s";
        return $"{(int)ts.TotalSeconds}s";
    }

    private static string FormatLastLogin(string lastLoginUtc)
    {
        if (string.IsNullOrEmpty(lastLoginUtc)) return "—";
        if (DateTime.TryParse(lastLoginUtc, null, DateTimeStyles.RoundtripKind, out var dt))
            return dt.ToLocalTime().ToString("g");
        return lastLoginUtc;
    }

    public void SetPanelVisible(bool visible)
    {
        if (statisticsPanelRoot != null)
            statisticsPanelRoot.SetActive(visible);
        if (visible)
            Refresh();
    }

    public void TogglePanel()
    {
        if (statisticsPanelRoot != null)
            SetPanelVisible(!statisticsPanelRoot.activeSelf);
        else
            SetPanelVisible(!gameObject.activeSelf);
    }
}
