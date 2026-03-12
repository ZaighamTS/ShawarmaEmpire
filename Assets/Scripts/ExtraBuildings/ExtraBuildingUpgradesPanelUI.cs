using System.Collections.Generic;
using UnityEngine;

/// <summary>Phase 8: Panel that lists purchased extra buildings and allows upgrading levels 0..10.</summary>
public class ExtraBuildingUpgradesPanelUI : MonoBehaviour
{
    public static ExtraBuildingUpgradesPanelUI Instance { get; private set; }

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject rowPrefab;

    private readonly List<ExtraBuildingUpgradeRowUI> rows = new();
    private readonly List<int> rowToBuildingIndex = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (BuildingUnlockManager.Instance == null || BuildingUnlockManager.Instance.buildings == null) return;
        if (contentParent == null || rowPrefab == null) return;

        rowToBuildingIndex.Clear();
        for (int i = 0; i < BuildingUnlockManager.Instance.buildings.Count; i++)
        {
            if (BuildingUnlockManager.Instance.buildings[i] != null && BuildingUnlockManager.Instance.buildings[i].isPurchased)
                rowToBuildingIndex.Add(i);
        }

        EnsureRowCount(rowToBuildingIndex.Count);

        for (int r = 0; r < rowToBuildingIndex.Count; r++)
        {
            int idx = rowToBuildingIndex[r];
            var b = BuildingUnlockManager.Instance.buildings[idx];
            var type = (BuildingUnlockManager.Instance.buildingTypes != null && idx < BuildingUnlockManager.Instance.buildingTypes.Length)
                ? BuildingUnlockManager.Instance.buildingTypes[idx]
                : ExtraBuildingType.JuicePoint;

            int level = BuildingUnlockManager.Instance.GetExtraBuildingLevel(idx);
            float netPerHour = ExtraBuildingLevelSystem.GetNetIncomePerHour(type, level);
            bool canUpgrade = level < 10;
            float nextCost = canUpgrade ? ExtraBuildingLevelSystem.GetUpgradeCost(type, level + 1) : 0f;

            rows[r].SetData(b.name, b.icon, level, netPerHour, canUpgrade, nextCost);

            int rowIndex = r;
            if (rows[r].UpgradeButton != null)
            {
                rows[r].UpgradeButton.onClick.RemoveAllListeners();
                rows[r].UpgradeButton.onClick.AddListener(() => OnUpgradeClicked(rowIndex));
            }
        }
    }

    private void EnsureRowCount(int count)
    {
        while (rows.Count < count)
        {
            var go = Instantiate(rowPrefab, contentParent);
            var row = go.GetComponent<ExtraBuildingUpgradeRowUI>();
            if (row == null) row = go.AddComponent<ExtraBuildingUpgradeRowUI>();
            rows.Add(row);
        }
        while (rows.Count > count && rows.Count > 0)
        {
            var last = rows[rows.Count - 1];
            rows.RemoveAt(rows.Count - 1);
            if (last != null && last.gameObject != null) Destroy(last.gameObject);
        }
    }

    private void OnUpgradeClicked(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= rowToBuildingIndex.Count) return;
        int buildingIndex = rowToBuildingIndex[rowIndex];
        if (BuildingUnlockManager.Instance != null)
            BuildingUnlockManager.Instance.TryUpgradeExtraBuilding(buildingIndex);
        Refresh();
    }

    public void SetPanelVisible(bool visible)
    {
        if (panelRoot != null) panelRoot.SetActive(visible);
        else gameObject.SetActive(visible);
        if (visible) Refresh();
    }

    public void TogglePanel()
    {
        bool current = panelRoot != null ? panelRoot.activeSelf : gameObject.activeSelf;
        SetPanelVisible(!current);
    }
}

