using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>Phase 6: Panel listing shawarma types; unlock condition and Select for current type.</summary>
public class ShawarmaTypesPanelUI : MonoBehaviour
{
    public static ShawarmaTypesPanelUI Instance { get; private set; }

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject rowPrefab;

    private List<ShawarmaTypeRowUI> _rows = new List<ShawarmaTypeRowUI>();

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
        var defs = ShawarmaTypes.Definitions;
        if (contentParent == null || rowPrefab == null) return;

        while (_rows.Count < defs.Count)
        {
            var go = Instantiate(rowPrefab, contentParent);
            var row = go.GetComponent<ShawarmaTypeRowUI>();
            if (row == null) row = go.AddComponent<ShawarmaTypeRowUI>();
            _rows.Add(row);
        }
        while (_rows.Count > defs.Count && _rows.Count > 0)
        {
            var last = _rows[_rows.Count - 1];
            _rows.RemoveAt(_rows.Count - 1);
            if (last != null && last.gameObject != null) Destroy(last.gameObject);
        }

        string currentId = PlayerProgress.Instance != null ? PlayerProgress.Instance.CurrentShawarmaTypeId : ShawarmaTypes.IdClassic;
        for (int i = 0; i < defs.Count && i < _rows.Count; i++)
        {
            var d = defs[i];
            bool unlocked = ShawarmaTypes.IsUnlocked(d.id);
            bool isCurrent = d.id == currentId;
            string unlockDesc = ShawarmaTypes.GetUnlockDescription(d.id);
            // Show actual income per shawarma using current upgrades + Chef Stars.
            // Use qualityBonus=0 so this is the baseline without any quality multiplier.
            float income = UpgradeCosts.GetShawarmaValueForBase(d.baseValue, 0);
            _rows[i].SetData(d.id, d.displayName, income, unlockDesc, unlocked, isCurrent);
            int index = i;
            if (_rows[i].SelectButton != null)
            {
                _rows[i].SelectButton.onClick.RemoveAllListeners();
                _rows[i].SelectButton.onClick.AddListener(() => OnSelectClicked(index));
            }
        }
    }

    private void OnSelectClicked(int index)
    {
        var defs = ShawarmaTypes.Definitions;
        if (index < 0 || index >= defs.Count || PlayerProgress.Instance == null) return;
        var d = defs[index];
        if (!ShawarmaTypes.IsUnlocked(d.id)) return;
        PlayerProgress.Instance.CurrentShawarmaTypeId = d.id;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);
            UIManager.Instance.UpdateUI(UIUpdateType.Multiplier);
        }
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
