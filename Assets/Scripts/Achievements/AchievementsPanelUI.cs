using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>Phase 2: Achievements panel. Fills a scroll list with achievement rows (from prefab or existing children).</summary>
public class AchievementsPanelUI : MonoBehaviour
{
    [Tooltip("Parent transform under the scroll content. Rows will be added here (if using row prefab) or existing children will be used.")]
    [SerializeField] private Transform contentParent;
    [Tooltip("Prefab for one achievement row. If set, we instantiate one per achievement. If null, we use contentParent's existing children (AchievementRowUI) up to count.")]
    [SerializeField] private GameObject rowPrefab;
    [Tooltip("If no prefab, we use contentParent's first N children as rows. Ignored if rowPrefab is set.")]
    [SerializeField] private int maxRowsWithoutPrefab = 30;

    private List<AchievementRowUI> _rows = new List<AchievementRowUI>();

    private void OnEnable()
    {
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.OnAchievementsChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.OnAchievementsChanged -= Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (AchievementManager.Instance == null || contentParent == null) return;

        if (rowPrefab != null)
        {
            EnsureRowCount(AchievementManager.Instance.Definitions.Count);
            for (int i = 0; i < AchievementManager.Instance.Definitions.Count; i++)
                SetRow(_rows[i], AchievementManager.Instance.Definitions[i]);
        }
        else
        {
            _rows.Clear();
            for (int i = 0; i < contentParent.childCount && i < maxRowsWithoutPrefab; i++)
            {
                var row = contentParent.GetChild(i).GetComponent<AchievementRowUI>();
                if (row != null) _rows.Add(row);
            }
            for (int i = 0; i < _rows.Count && i < AchievementManager.Instance.Definitions.Count; i++)
                SetRow(_rows[i], AchievementManager.Instance.Definitions[i]);
        }
    }

    private void EnsureRowCount(int count)
    {
        while (_rows.Count < count)
        {
            var go = Instantiate(rowPrefab, contentParent);
            var row = go.GetComponent<AchievementRowUI>();
            if (row == null) row = go.AddComponent<AchievementRowUI>();
            _rows.Add(row);
        }
        while (_rows.Count > count && _rows.Count > 0)
        {
            var last = _rows[_rows.Count - 1];
            _rows.RemoveAt(_rows.Count - 1);
            if (last != null && last.gameObject != null) Destroy(last.gameObject);
        }
    }

    private void SetRow(AchievementRowUI row, AchievementDefinition def)
    {
        if (row == null) return;
        bool claimed = AchievementManager.Instance.IsClaimed(def.id);
        bool unlocked = AchievementManager.Instance.IsUnlocked(def.id);
        string progress = AchievementManager.Instance.GetProgressText(def);
        string reward = AchievementManager.Instance.GetRewardText(def);
        row.SetData(def.id, def.title, def.description, progress, reward, unlocked, claimed, OnClaimClicked);
    }

    private void OnClaimClicked(string achievementId)
    {
        if (AchievementManager.Instance != null && AchievementManager.Instance.Claim(achievementId))
            Refresh();
    }

    public void SetPanelVisible(bool visible)
    {
        gameObject.SetActive(visible);
        if (visible) Refresh();
    }

    public void TogglePanel()
    {
        SetPanelVisible(!gameObject.activeSelf);
    }
}
