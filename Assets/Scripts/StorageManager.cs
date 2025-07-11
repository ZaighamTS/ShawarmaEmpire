using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;
[System.Serializable]

public class StorageUnit
{
    private int crrentLevel = 0;
    private int currentCapacity;
    [SerializeField]
    private int maxUpgradeLevel;
    // Adjustable per level in Inspector
    public float[] upgradeCosts;
    public int[] capacityPerLevelList;

    public int GetCapacity()
    {
        //if (capacityPerLevelList != null && level < capacityPerLevelList.Length)
        //    return capacityPerLevelList[level];
        //else if (capacityPerLevelList != null && capacityPerLevelList.Length > 0)
        //    return capacityPerLevelList[capacityPerLevelList.Length - 1];
        //else
        //    return 500; // fallback default
        return currentCapacity;
    }

    public float GetUpgradeCost()
    {
        //if (upgradeCosts != null && level < upgradeCosts.Length)
        //    return upgradeCosts[level];
        //else if (upgradeCosts != null && upgradeCosts.Length > 0)
        //    return upgradeCosts[upgradeCosts.Length - 1];
        //else
        //    return 100f; // fallback default
        return UpgradeCosts.GetUpgradeCost(UpgradeType.Storage, crrentLevel);
    }

    public bool CanUpgrade()
    {
        return crrentLevel < maxUpgradeLevel && crrentLevel < capacityPerLevelList.Length;
    }

    public void Upgrade()
    {
        crrentLevel++;
    }
}

// --- STORAGE MANAGER ---
public class StorageManager : MonoBehaviour
{
    public static StorageManager storageManagerInstance;

    public List<StorageUnit> storageUnits = new List<StorageUnit>();
    public int currentShawarmas = 0;

    private void Awake()
    {
        if (storageManagerInstance == null)
        {
            storageManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        ShawarmaSpawner.onStoreShawarma += StoreShawarma;
    }
    private void OnDisable()
    {
        ShawarmaSpawner.onStoreShawarma -= StoreShawarma;
    }
    public void StoreShawarma(int value)
    {
        if (currentShawarmas < GetStorageCapacity())
        {
            currentShawarmas += value;
        }
    }
    internal void DeliverShawarma(int value)
    {
        if (value >= GetStorageCapacity())
        {
            currentShawarmas -= value;
        }
    }
    public void ClearStorage()
    {
        currentShawarmas = 0;
    }

    public int GetStorageCapacity()
    {
        return storageUnits.Sum(unit => unit.GetCapacity()); ;
    }

    public int GetStoredShawarmas()
    {
        return currentShawarmas;
    }

    public void UpgradeStorage(int index)
    {
        if (index < 0 || index >= storageUnits.Count) return;

        StorageUnit unit = storageUnits[index];

        if (!unit.CanUpgrade()) return;
        float cost = unit.GetUpgradeCost();
        if (GameManager.gameManagerInstance.SpendCash(cost))
        {
            unit.Upgrade();
        }
    }
}