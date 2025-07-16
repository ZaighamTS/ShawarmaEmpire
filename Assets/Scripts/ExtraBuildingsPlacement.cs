using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraBuildingsPlacement : MonoBehaviour
{
    public GameObject[] ExtraBuildings;

    private int _currentLevel;

    public int CurrentLevel
    {
        get => _currentLevel;
        set
        {
            _currentLevel = value;
            OnLevelUpgrade();
        }
    }
    public void OnLevelUpgrade()
    {
        for (int i = 0; i < ExtraBuildings.Length; i++)
        {
            ExtraBuildings[i].SetActive(i < _currentLevel);
        }
    }
}
