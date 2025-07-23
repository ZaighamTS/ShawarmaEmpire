using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUnlockManager : MonoBehaviour
{
    [System.Serializable]
    public class Building
    {
        public string name;
        public Sprite icon;
        public int cost;
        public bool isPurchased;
        public GameObject BuildingObject;
    }

    public List<Building> buildings;
   
    public Transform buildingListParent;
    public Text cashText; // Assign in inspector

    private List<Transform> buildingButtons = new List<Transform>();
    private int playerCash;

    void Start()
    {
        playerCash = GameDataManager.GetCash();
        LoadPurchaseStatus();
        GenerateBuildingUI();
        UpdateUI();
    }

    void LoadPurchaseStatus()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].isPurchased = GameDataManager.IsBuildingPurchased(i);
        }
    }

    void GenerateBuildingUI()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            Building b = buildings[i];
          //  GameObject btnObj = Instantiate(buildingButtonPrefab, buildingListParent);
            Transform btn = buildingListParent.GetChild(i);

            Image iconImage = btn.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            Text costText = btn.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>();

            iconImage.sprite = b.icon;
            costText.text = b.cost.ToString();
           // b.BuildingObject.SetActive(b.isPurchased);
            int index = i;
            btn.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => TryUnlockBuilding(index));
            buildingButtons.Add(btn);
        }
    }

    void TryUnlockBuilding(int index)
    {
        if (buildings[index].isPurchased)
            return;

        Building b = buildings[index];

        if (playerCash >= b.cost)
        {
            playerCash -= b.cost;
            GameDataManager.SetCash(playerCash);
            buildings[index].isPurchased = true;
            GameDataManager.SaveBuildingPurchase(index, true);
            GameDataManager.SaveAll();
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough cash!");
        }
    }

    void UpdateUI()
    {
        //cashText.text = "Cash: $" + playerCash;

        for (int i = 0; i < buildingButtons.Count; i++)
        {
            Transform btn = buildingListParent.GetChild(i);
            Image icon = btn.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            Text costText = btn.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>();

            bool isPurchased = buildings[i].isPurchased;

            // If already purchased, disable button and show faded icon
            btn.GetChild(0).GetChild(1).GetComponent<Button>().interactable = !isPurchased;
            icon.color = isPurchased ? new Color(1, 1, 1, 0.4f) : Color.white;
            buildings[i].BuildingObject.SetActive(isPurchased);
            costText.gameObject.SetActive(!isPurchased);
        }
    }

   
}
