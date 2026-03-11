using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>Phase 4: Boost shop panel. Lists available boosts and active boosts with remaining time.</summary>
public class BoostShopUI : MonoBehaviour
{
    [Header("Available boosts")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject rowPrefab;

    [Header("Active boosts")]
    [SerializeField] private Transform activeBoostsParent;
    [SerializeField] private TMP_Text activeBoostTemplate;
    [SerializeField] private float activeBoostsRefreshInterval = 1f;

    private List<BoostRowUI> _rows = new List<BoostRowUI>();
    private float _activeRefreshAccum;

    private void OnEnable()
    {
        if (BoostManager.Instance != null)
            BoostManager.Instance.OnBoostsChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (BoostManager.Instance != null)
            BoostManager.Instance.OnBoostsChanged -= Refresh;
    }

    private void Start()
    {
        if (contentParent != null && rowPrefab != null && BoostManager.Instance != null)
        {
            EnsureRowCount(BoostManager.Instance.Definitions.Count);
            for (int i = 0; i < _rows.Count; i++)
            {
                int index = i;
                if (_rows[i].ActivateButton != null)
                    _rows[i].ActivateButton.onClick.RemoveAllListeners();
                if (_rows[i].ActivateButton != null)
                    _rows[i].ActivateButton.onClick.AddListener(() => OnActivateClicked(index));
            }
        }
        Refresh();
    }

    private void Update()
    {
        if (BoostManager.Instance == null) return;
        _activeRefreshAccum += Time.deltaTime;
        if (_activeRefreshAccum >= activeBoostsRefreshInterval)
        {
            _activeRefreshAccum = 0f;
            RefreshRowTimersOnly();
            RefreshActiveBoostsList();
        }
    }

    public void Refresh()
    {
        if (BoostManager.Instance == null) return;
        var defs = BoostManager.Instance.Definitions;
        if (contentParent != null && rowPrefab != null)
        {
            EnsureRowCount(defs.Count);
            RefreshRowsFull();
        }
        RefreshActiveBoostsList();
    }

    private void RefreshRowsFull()
    {
        if (BoostManager.Instance == null) return;
        var defs = BoostManager.Instance.Definitions;
        float gold = GameManager.gameManagerInstance != null ? GameManager.gameManagerInstance.GetGold() : 0f;
        for (int i = 0; i < defs.Count && i < _rows.Count; i++)
        {
            var d = defs[i];
            bool isActive = BoostManager.Instance.IsBoostActive(d.id);
            string remaining = BoostManager.Instance.GetRemainingTime(d.id);
            bool canAfford = d.costType != BoostCostType.Gold || gold >= d.costAmount;
            _rows[i].SetData(d.id, d.displayName, d.description, d.costType, d.costAmount, canAfford, isActive, remaining);
        }
    }

    private void RefreshRowTimersOnly()
    {
        if (BoostManager.Instance == null) return;
        var defs = BoostManager.Instance.Definitions;
        float gold = GameManager.gameManagerInstance != null ? GameManager.gameManagerInstance.GetGold() : 0f;
        for (int i = 0; i < defs.Count && i < _rows.Count; i++)
        {
            var d = defs[i];
            bool isActive = BoostManager.Instance.IsBoostActive(d.id);
            string remaining = BoostManager.Instance.GetRemainingTime(d.id);
            bool canAfford = d.costType != BoostCostType.Gold || gold >= d.costAmount;
            // Re-apply interactable + timer; keep text fields consistent too.
            _rows[i].SetData(d.id, d.displayName, d.description, d.costType, d.costAmount, canAfford, isActive, remaining);
        }
    }

    private void EnsureRowCount(int count)
    {
        while (_rows.Count < count)
        {
            var go = Instantiate(rowPrefab, contentParent);
            var row = go.GetComponent<BoostRowUI>();
            if (row == null) row = go.AddComponent<BoostRowUI>();
            _rows.Add(row);
        }
        while (_rows.Count > count && _rows.Count > 0)
        {
            var last = _rows[_rows.Count - 1];
            _rows.RemoveAt(_rows.Count - 1);
            if (last != null && last.gameObject != null) Destroy(last.gameObject);
        }
    }

    private void OnActivateClicked(int rowIndex)
    {
        if (BoostManager.Instance == null || rowIndex < 0 || rowIndex >= _rows.Count) return;
        string boostId = _rows[rowIndex].BoostId;
        var def = GetDef(boostId);
        if (def == null) return;
        if (def.costType == BoostCostType.Gold)
        {
            if (BoostManager.Instance.TryActivate(boostId))
                Refresh();
        }
        else
        {
            // Watch Ad: in a real build you would show an ad and call ActivateAfterAd in the ad-complete callback.
            // Placeholder: activate immediately (simulate ad watched).
            BoostManager.Instance.ActivateAfterAd(boostId);
            Refresh();
        }
    }

    private BoostDefinition GetDef(string id)
    {
        if (BoostManager.Instance == null) return null;
        foreach (var d in BoostManager.Instance.Definitions)
            if (d.id == id) return d;
        return null;
    }

    private void RefreshActiveBoostsList()
    {
        if (BoostManager.Instance == null || activeBoostsParent == null) return;
        var active = BoostManager.Instance.ActiveBoosts;
        int count = 0;
        foreach (var b in active)
        {
            string remaining = BoostManager.Instance.GetRemainingTime(b.boostId);
            if (string.IsNullOrEmpty(remaining)) continue;
            var def = GetDef(b.boostId);
            string line = def != null ? $"{def.displayName}: {remaining}" : $"{b.boostId}: {remaining}";
            TMP_Text text = GetOrCreateActiveSlot(count);
            if (text != null) text.text = line;
            count++;
        }
        // Hide unused slots
        for (int i = count; i < activeBoostsParent.childCount; i++)
        {
            var c = activeBoostsParent.GetChild(i);
            if (c != null && c.gameObject != null)
                c.gameObject.SetActive(false);
        }
    }

    private TMP_Text GetOrCreateActiveSlot(int index)
    {
        if (activeBoostTemplate == null) return null;
        while (index >= activeBoostsParent.childCount)
        {
            var go = Instantiate(activeBoostTemplate.gameObject, activeBoostsParent);
            go.SetActive(true);
        }
        var child = activeBoostsParent.GetChild(index);
        if (child != null) child.gameObject.SetActive(true);
        return child != null ? child.GetComponent<TMP_Text>() : null;
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
