using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraBuildingFunctionality : MonoBehaviour
{
    public BuildingType currentBuildingType;
    [Tooltip("Optional: index in BuildingUnlockManager.buildings list. If set, uses Phase 8 levels.")]
    public int buildingIndex = -1;

    private bool useLevelSystem;
    private float netPerSecond;
    private float accumulator;

    void Start()
    {
        useLevelSystem = BuildingUnlockManager.Instance != null && buildingIndex >= 0;
        if (!useLevelSystem)
        {
            // Legacy fallback (no level system wired): do nothing rather than applying outdated reward/expense loops
            enabled = false;
            return;
        }

        RefreshNetRate();
    }

    private void Update()
    {
        if (!useLevelSystem || BuildingUnlockManager.Instance == null) return;

        // Refresh when level changes (cheap check)
        // (If you want it event-driven later, we can add an event on level change.)
        RefreshNetRate();

        accumulator += netPerSecond * Time.deltaTime;
        if (Mathf.Abs(accumulator) >= 1f)
        {
            float whole = Mathf.Floor(Mathf.Abs(accumulator)) * Mathf.Sign(accumulator);
            accumulator -= whole;
            GameManager.gameManagerInstance.AddCash(whole);
            if (UIManager.Instance != null) UIManager.Instance.UpdateUI(UIUpdateType.Cash);
        }
    }

    private void RefreshNetRate()
    {
        if (BuildingUnlockManager.Instance == null) return;
        if (buildingIndex < 0) return;
        if (BuildingUnlockManager.Instance.buildingTypes == null || buildingIndex >= BuildingUnlockManager.Instance.buildingTypes.Length) return;

        int level = BuildingUnlockManager.Instance.GetExtraBuildingLevel(buildingIndex);
        ExtraBuildingType type = BuildingUnlockManager.Instance.buildingTypes[buildingIndex];
        float netPerHour = ExtraBuildingLevelSystem.GetNetIncomePerHour(type, level);
        netPerSecond = netPerHour / 3600f;
    }
}
public enum BuildingType
{
    ingrediants,
    merchandise,
    park,
    gasStation,
    management,
    shawarmaLounge,
    juicePoint,
    dessertPoint
}