using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
public class BuildingUnlockManager : MonoBehaviour, ISaveable
{
  
    public List<Building> buildings;
   
    public Transform buildingListParent;
    public Text cashText; // Assign in inspector

    private List<Transform> buildingButtons = new List<Transform>();
    private int playerCash;
    private int playerGold;
    private bool isDirty = false;
    public string SaveKey => "building";
    public static event Action<UIUpdateType, float> onBuildingUpgraded;
    [HideInInspector]public int currentUpdate;
    [HideInInspector]public int cost;
    
    [Header("Extra Building Configuration")]
    [Tooltip("Map each building in the 'buildings' list to its ExtraBuildingType. Set size to match buildings list.")]
    public ExtraBuildingType[] buildingTypes; // Set in Unity Inspector - map to buildings list
    void Start()
    {
        SaveLoadManager.saveLoadManagerInstance.Register(this);
        GameManager.gameManagerInstance.RecordPersistentRegistrations().Forget();
        Invoke("DelayOnStart", 1.1f);
    } 
    private void OnDestroy()
    {
        SaveLoadManager.saveLoadManagerInstance?.Unregister(this);
    }
    public void DelayOnStart()
    {
       
        LoadPurchaseStatus();
        GenerateBuildingUI();
        UpdateUI();
    }
    void LoadPurchaseStatus()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].isPurchased = IsBuildingPurchased(i);
        }
    }
    void GenerateBuildingUI()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            Building b = buildings[i];
            Transform btn = buildingListParent.GetChild(i);
            Image iconImage = btn.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            TMP_Text costText = btn.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
            TMP_Text costTextGold = btn.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
            //iconImage.sprite = b.icon;
            
            // EXTENDED GAMEPLAY: Calculate dynamic cost for extra buildings
            if (buildingTypes != null && i < buildingTypes.Length)
            {
                int existingCount = GetPurchasedCountOfType(buildingTypes[i]);
                float dynamicCost = UpgradeCosts.GetExtraBuildingCost(buildingTypes[i], existingCount);
                int roundedCost = Mathf.RoundToInt(dynamicCost);
                costText.text = roundedCost.ToString();
                // Store calculated cost for purchase check
                buildings[i].cost = roundedCost;
            }
            else
            {
                // Fallback to inspector value if no type assigned
                costText.text = b.cost.ToString();
            }
            
            costTextGold.text = b.goldCost.ToString();
            int index = i;
            btn.GetChild(0).GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            btn.GetChild(0).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
            btn.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => TryUnlockBuilding(index));
            btn.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() => TryUnlockBuildingGold(index));
            buildingButtons.Add(btn);
        }
    }
    
    /// <summary>
    /// Helper method to count purchased buildings of a specific type
    /// </summary>
    private int GetPurchasedCountOfType(ExtraBuildingType buildingType)
    {
        int count = 0;
        if (buildingTypes == null) return 0;
        
        for (int i = 0; i < buildings.Count && i < buildingTypes.Length; i++)
        {
            if (buildingTypes[i] == buildingType && buildings[i].isPurchased)
            {
                count++;
            }
        }
        return count;
    }

    void TryUnlockBuilding(int index)
    {
        playerCash = (int)PlayerProgress.Instance.PlayerCash;
        if (buildings[index].isPurchased)
            return;

        Building b = buildings[index];
        
        // EXTENDED GAMEPLAY: Recalculate dynamic cost before purchase
        if (buildingTypes != null && index < buildingTypes.Length)
        {
            int existingCount = GetPurchasedCountOfType(buildingTypes[index]);
            float dynamicCost = UpgradeCosts.GetExtraBuildingCost(buildingTypes[index], existingCount);
            b.cost = Mathf.RoundToInt(dynamicCost);
        }

        if (playerCash >= b.cost)
        {
            GameManager.gameManagerInstance.SpendCash(b.cost);
           // playerCash -= b.cost;
           // SetCash(playerCash);
            buildings[index].isPurchased = true;
            SaveBuildingPurchase(index, true);
            onBuildingUpgraded?.Invoke(UIUpdateType.Cash, PlayerProgress.Instance.PlayerCash);
            UpdateUI();
            UIManager.Instance.DisableGameplayPanel();
            CameraSwipeController.instance.LerpCamera(b.BuildingObject.transform.position.x, b.BuildingObject.transform.position.z);
            b.Particle.SetActive(true);
            
            // Refresh UI to show updated costs for other buildings of the same type
            GenerateBuildingUI();
        }
        else
        {
            UIManager.Instance.lowCashPromt.SetActive(true);
        }
    }


    void TryUnlockBuildingGold(int index)
    {
        playerGold = (int)PlayerProgress.Instance.Gold;
        if (buildings[index].isPurchased)
            return;

        Building b = buildings[index];

        if (playerGold >= b.goldCost)
        {
            GameManager.gameManagerInstance.SpendGold(b.goldCost);
            // playerCash -= b.cost;
            // SetCash(playerCash);
            buildings[index].isPurchased = true;
            SaveBuildingPurchase(index, true);
            onBuildingUpgraded?.Invoke(UIUpdateType.Gold, PlayerProgress.Instance.Gold);
            UpdateUI();
            UIManager.Instance.DisableGameplayPanel();
            CameraSwipeController.instance.LerpCamera(b.BuildingObject.transform.position.x, b.BuildingObject.transform.position.z);
            b.Particle.SetActive(true);
        }
        else
        {
            UIManager.Instance.lowCashPromt.SetActive(true);
        }
    }



    void UpdateUI()
    {
        //cashText.text = "Cash: $" + playerCash;

        for (int i = 0; i < buildingButtons.Count; i++)
        {
            Transform btn = buildingListParent.GetChild(i);
            Image icon = btn.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            TMP_Text costText = btn.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();

            bool isPurchased = buildings[i].isPurchased;

            // If already purchased, disable button and show faded icon
            btn.GetChild(0).GetChild(1).GetComponent<Button>().interactable = !isPurchased;
            btn.GetChild(0).GetChild(2).GetComponent<Button>().interactable = !isPurchased;
           // icon.color = isPurchased ? new Color(1, 1, 1, 0.4f) : Color.white;
            buildings[i].BuildingObject.SetActive(isPurchased);
            btn.transform.GetChild(0).GetChild(5).gameObject.SetActive(isPurchased);
           // costText.gameObject.SetActive(!isPurchased);
        }
    }

    public bool IsBuildingPurchased(int index)
    {
        return PlayerPrefs.GetInt("building_purchased_" + index, 0) == 1;
    }

    public void SaveBuildingPurchase(int index, bool state)
    {
        PlayerPrefs.SetInt("building_purchased_" + index, state ? 1 : 0);
    }

    #region Save/Load
    public bool IsDirty => isDirty;
    public object CaptureState()
    {
        return new BuildingsData
        {    
            currentUpdate = currentUpdate,
           // cost = cost
        };
    }
    public void RestoreState(object state)
    {
        if (state is not BuildingsData data)
            return;
       
        currentUpdate = data.currentUpdate;
      //  cost = data.cost;
        isDirty = false;
    }
    public void SetInitialData()
    {
       // currentUpdate = 1;
        cost = (int)UpgradeCosts.GetUpgradeCost(UpgradeType.Kitchen, currentUpdate);
        isDirty = true;

    }
    public void ClearDirty()
    {
        isDirty = false;
    }
    #endregion
    
}
[System.Serializable]
public class Building
{
    public string name;
   // public Sprite icon;
    public int cost;
    public bool isPurchased;
    public GameObject BuildingObject;
    public GameObject Particle;
    public int goldCost;
}
public class BuildingsData
{
    public int id;
    public int currentUpdate;
   // public int cost;
   

}