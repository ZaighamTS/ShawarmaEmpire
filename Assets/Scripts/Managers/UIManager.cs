using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text coinsText;
    public TMP_Text goldText;
    public TMP_Text[] storageText;
    public TMP_Text multiplierText;
    public TMP_Text chefStarsText;
    public GameObject lowCashPromt;
    public TMP_Text TotalEarningTxt;
    public TMP_Text automaticEarningText; // Display for automatic earning rate
    public TMP_Text automaticEarningMultiplierText; // Display for automatic earning multiplier
    public GameObject PrestigeWarning;
    public GameObject PrestigePop;
    public GameObject PrestigeTramsitioneffect;
    public GameObject RewardedAdPopUp;
    public GameObject InappPopup;
    public GameObject StartPanel;
    public GameObject GameplayPanel;
    public GameObject PostprocessingEffect;
    public GameObject InfoPopUp;
    public TMP_Text InfoText;
    public Image TotalEarningImage;
    public Transform[] TabsButton;
    [Header("Upgrade Availability Badges")]
    public GameObject upgradeBadgePrefab; // Simple badge prefab with Image and Text (optional)
    public Button managersPanelButton; // Button that opens the panel with Warehouse/Delivery/Kitchen/Catering managers
    public Button challengesPanelButton; // Button that opens the Challenges panel
    public Button achievementsPanelButton; // Button that opens the Achievements panel
    [Header("Optional overlay panels (block warehouse/world clicks when active)")]
    [Tooltip("Assign any panel that can appear over the main game (e.g. Challenges, Achievements, Boost Shop, Gift Calendar, Statistics) so world clicks are blocked when they're open.")]
    public GameObject[] optionalOverlayPanels;
    private Dictionary<Button, GameObject> upgradeBadges = new Dictionary<Button, GameObject>();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void OnEnable()
    {
        ShawarmaSpawner.onShawarmaCreated += UpdateUI;
        Warehouse.onWarehouseUpgraded += OnUpgradeChanged;
        Catering.onCateringUpgraded += OnUpgradeChanged;
        Kitchen.onKitchenUpgraded += OnUpgradeChanged;
        BuildingUnlockManager.onBuildingUpgraded += UpdateUI;
        Delivery.onDeliveryUpgraded += OnUpgradeChanged;

    }
    private void OnDisable()
    {
        ShawarmaSpawner.onShawarmaCreated -= UpdateUI;
        Warehouse.onWarehouseUpgraded -= OnUpgradeChanged;
        Catering.onCateringUpgraded -= OnUpgradeChanged;
        Kitchen.onKitchenUpgraded -= OnUpgradeChanged;
        BuildingUnlockManager.onBuildingUpgraded -= UpdateUI;
        Delivery.onDeliveryUpgraded -= OnUpgradeChanged;
    }
    
    /// <summary>
    /// Called when upgrades change to update automatic earning multiplier
    /// </summary>
    private void OnUpgradeChanged(UIUpdateType updateType, float value = 0)
    {
        UpdateUI(updateType, value);
        
        // Update automatic earning multiplier when upgrades change
        if (GameManager.gameManagerInstance != null)
        {
            GameManager.gameManagerInstance.OnPrestigeOrUpgradeChanged();
        }
        
        // Update upgrade badges when upgrades change
        UpdateUpgradeBadges();
    }
    
    public void UpdateUI(UIUpdateType updateType, float value = 0)
    {
        switch (updateType)
        {
            case UIUpdateType.Cash:
                {
                    if (coinsText != null)
                    {
                        coinsText.text = $"{GameManager.gameManagerInstance.GetCurrentCash():N0}";
                    }
                    if (TotalEarningTxt)
                    {
                        TotalEarningTxt.text = $"{GameManager.gameManagerInstance.GetTotalEarning():N0}";
                    }
                    
                    // Update upgrade availability badges when cash changes
                    UpdateUpgradeBadges();
                       
                    break;
                }
            case UIUpdateType.Storage:
                {
                    //  if (storageText != null)
                    for (int i = 0; i < storageText.Length; i++)
                    {
                        storageText[i].text = $"{PlayerProgress.Instance.ShwarmaCount}";
                    }
                    break;
                }
            case UIUpdateType.Multiplier:
                {
                    if (multiplierText != null && ShawarmaSpawner.Instance != null)
                    {
                        float tapMultiplier = ShawarmaSpawner.Instance.tapMultiplier;
                        float perSecondEarnings = 0f;
                        
                        // Get per-second earnings from GameManager if available
                        if (GameManager.gameManagerInstance != null)
                        {
                            perSecondEarnings = GameManager.gameManagerInstance.GetAutomaticEarningRate();
                        }
                        
                        // Display both multiplier and per-second earnings
                        if (perSecondEarnings > 0f)
                        {
                            multiplierText.text = $"{tapMultiplier:F2}x (${perSecondEarnings:F2}/sec)";
                        }
                        else
                        {
                            multiplierText.text = $"{tapMultiplier:F2}x";
                        }
                    }
                    //multiplierText.text = $"{ShawarmaSpawner.Instance.GetMultiplier():0.0}x";
                    break;
                }
            case UIUpdateType.Gold:
                {
                    if (goldText != null)
                    {
                        goldText.text = $"{GameManager.gameManagerInstance.GetGold():N0}";
                    }
                    break;
                }
            case UIUpdateType.AutomaticEarning:
                {
                    UpdateAutomaticEarningDisplay(value);
                    break;
                }
            default:
                {
                    print($"No Handler For UpdateType {updateType}");
                    break;
                }
        }

    }
    public void UpdateEarningSlider()
    {
       // Debug.Log("bb "+PlayerProgress.Instance.ChefStars);
      //  Debug.Log("check "+ UpgradeCosts.GetNextPrestigeValue());
      //  Debug.Log("val "+ Mathf.Clamp01(PlayerProgress.Instance.TotalEarnings / UpgradeCosts.GetNextPrestigeValue()));
          TotalEarningImage.fillAmount = Mathf.Clamp01(PlayerProgress.Instance.TotalEarnings / UpgradeCosts.GetNextPrestigeValue());

       // Debug.Log((UpgradeCosts.GetNextPrestigeValue(PlayerProgress.Instance.TotalEarnings) / PlayerProgress.Instance.TotalEarnings));
    }
    public void ShowInfoPopup(string info_text)
    {
        InfoPopUp.SetActive(true);
        InfoText.text = info_text;

    }


     
    //public async UniTask DisablePop()
    //{ 
    //    UniTask.Delay()
    //}
    public void ClickOnRewardBtn()
    {
       // FindObjectOfType<RewardedAdManager>().ShowAd();
    }
    public void ClickOnPrestigeButton()
    { 
        GameManager.gameManagerInstance.ResetPlayerStats();
        // Use SaveLoadManager's reset method to clear both PlayerPrefs and JSON file
        if (SaveLoadManager.saveLoadManagerInstance != null)
        {
            SaveLoadManager.saveLoadManagerInstance.CompleteReset();
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }
        TransitionEffectScript.PrestigeDone = true;
        PrestigeTramsitioneffect.transform.GetChild(0).gameObject.SetActive(true);
       // TransitionEffectScript
        PrestigePop.SetActive(false);
        Invoke("ReloadScene",2);
    }
    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
    public void ShowPrestigeBtn()
    {

        PrestigeWarning.SetActive(true);
    }

    public void UpdateChefStarsText()
    {
        if (chefStarsText != null)
        {
            chefStarsText.text=PlayerProgress.Instance.ChefStars.ToString();
        }
        
        // Update automatic earning multiplier when chef stars change
        if (GameManager.gameManagerInstance != null)
        {
            GameManager.gameManagerInstance.OnPrestigeOrUpgradeChanged();
        }
    }

    public void OnRewardedAdSuccess()
    { 
        RewardedAdPopUp.SetActive(false);
        PlayerPrefs.SetInt("RewardCount", PlayerPrefs.GetInt("RewardCount")+1);
        ShowRewardPop("Reward has been added to your offline earnings").Forget();
        // Debug.Log("RewardCount  "+ PlayerPrefs.GetInt("RewardCount"));
        //  ShowInfoPopup("Reward has been added to your offline earnings");

        //GameManager.gameManagerInstance.AddCash(1000);
        //UpdateUI(UIUpdateType.Cash);

    }
    public async UniTask ShowRewardPop(string _text)
    {
        await UniTask.WaitForSeconds(0.5f);
        InfoPopUp.SetActive(true);
        InfoText.text = _text;

    }
    public void InappPurchaseSuccess()
    {
        GameManager.gameManagerInstance.AddCash(10000);
        UpdateUI(UIUpdateType.Cash);
        InappPopup.SetActive(false);
    }

    void OnUserClickUpgrade(int type)
    {
        UpgradeType upgradeType = (UpgradeType)type;
        switch (upgradeType)
        {
            case UpgradeType.Kitchen:
                {
                    break;
                }
            case UpgradeType.Storage:
                {
                    break;
                }
            case UpgradeType.DeliveryVan:
                {
                    break;
                }
        }
    }

    public void ClickOnQuit()
    { 
        Application.Quit();
    }

    /// <summary>
    /// True when only the main gameplay screen is visible (no popup, overlay, or other screen).
    /// Use this to block world clicks (e.g. warehouses) when any UI is in front.
    /// </summary>
    public bool IsMainScreenOnlyVisible()
    {
        if (Instance == null || GameplayPanel == null) return false;
        if (!GameplayPanel.activeInHierarchy) return false;
        if (InfoPopUp != null && InfoPopUp.activeInHierarchy) return false;
        if (PrestigeWarning != null && PrestigeWarning.activeInHierarchy) return false;
        if (PrestigePop != null && PrestigePop.activeInHierarchy) return false;
        if (PrestigeTramsitioneffect != null && PrestigeTramsitioneffect.activeInHierarchy) return false;
        if (RewardedAdPopUp != null && RewardedAdPopUp.activeInHierarchy) return false;
        if (InappPopup != null && InappPopup.activeInHierarchy) return false;
        if (lowCashPromt != null && lowCashPromt.activeInHierarchy) return false;
        if (optionalOverlayPanels != null)
        {
            for (int i = 0; i < optionalOverlayPanels.Length; i++)
            {
                if (optionalOverlayPanels[i] != null && optionalOverlayPanels[i].activeInHierarchy)
                    return false;
            }
        }
        return true;
    }

    public void DisableGameplayPanel()
    { 
        GameplayPanel.SetActive(false);
        PostprocessingEffect.SetActive(false);
        EnableGameplayPanel().Forget();
    }

    public async UniTask EnableGameplayPanel()
    {
        await UniTask.WaitForSeconds(2.0f);
        PostprocessingEffect.SetActive(true);
        GameplayPanel.SetActive(true);
    }
    public void OnClickTabsButton(Transform ThisButton)
    {
        for (int i = 0; i < TabsButton.Length; i++)
        {
            TabsButton[i].localScale = Vector3.one;
        }
        ThisButton.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }
    
    /// <summary>
    /// Updates the automatic earning display in the UI
    /// Shows earnings per second based on spawned shawarmas and multiplier
    /// Format: "0.01/sec" (number only, no prefix)
    /// Also displays multiplier if multiplierText is assigned
    /// </summary>
    public void UpdateAutomaticEarningDisplay(float earningRatePerSecond, float multiplier = 1f)
    {
        if (automaticEarningText != null)
        {
            // Format: "0.01/sec" - just the number and /sec
            automaticEarningText.text = $"{earningRatePerSecond:F2}/sec";
        }
        
        // Display multiplier separately if text field is assigned
        if (automaticEarningMultiplierText != null)
        {
            // Format: "x1.00" or "x1.25" etc.
            automaticEarningMultiplierText.text = $"x{multiplier:F2}";
        }
        
        // Also update the multiplier text to show per-second earnings
        // This ensures the multiplier display shows both tap multiplier and per-second earnings
        if (multiplierText != null && ShawarmaSpawner.Instance != null)
        {
            float tapMultiplier = ShawarmaSpawner.Instance.tapMultiplier;
            if (earningRatePerSecond > 0f)
            {
                multiplierText.text = $"{tapMultiplier:F2}x (${earningRatePerSecond:F2}/sec)";
            }
            else
            {
                multiplierText.text = $"{tapMultiplier:F2}x";
            }
        }
    }
    
    /// <summary>
    /// Updates upgrade availability badges on tab buttons
    /// Shows count of affordable upgrades for each category
    /// </summary>
    public void UpdateUpgradeBadges()
    {
        // Update badge for Storage tab
        if (WarehouseManager.Instance != null && WarehouseManager.Instance.storageTabButton != null)
        {
            int count = WarehouseManager.Instance.GetAvailableUpgradeCount();
            UpdateBadgeOnButton(WarehouseManager.Instance.storageTabButton, count, () => {
                WarehouseManager.Instance.NavigateToFirstAffordableUpgrade();
            });
        }
        
        // Update badge for Delivery tab
        if (DeliveryManager.Instance != null && DeliveryManager.Instance.deliveryTabButton != null)
        {
            int count = DeliveryManager.Instance.GetAvailableUpgradeCount();
            UpdateBadgeOnButton(DeliveryManager.Instance.deliveryTabButton, count, () => {
                DeliveryManager.Instance.NavigateToFirstAffordableUpgrade();
            });
        }
        
        // Update badge for Kitchen tab
        if (KitchenManager.Instance != null && KitchenManager.Instance.kitchenTabButton != null)
        {
            int count = KitchenManager.Instance.GetAvailableUpgradeCount();
            UpdateBadgeOnButton(KitchenManager.Instance.kitchenTabButton, count, () => {
                KitchenManager.Instance.NavigateToFirstAffordableUpgrade();
            });
        }
        
        // Update badge for Catering tab
        if (CateringManager.Instance != null && CateringManager.Instance.cateringTabButton != null)
        {
            int count = CateringManager.Instance.GetAvailableUpgradeCount();
            UpdateBadgeOnButton(CateringManager.Instance.cateringTabButton, count, () => {
                CateringManager.Instance.NavigateToFirstAffordableUpgrade();
            });
        }
        
        // Update badge for Managers Panel button (shows if ANY of the 4 managers have upgrades)
        if (managersPanelButton != null)
        {
            int totalCount = GetManagersPanelUpgradeCount();
            UpdateBadgeOnButton(managersPanelButton, totalCount, () => {
                NavigateToFirstAvailableManagerUpgrade();
            });
        }
        
        // Update badge for Buildings tab
        BuildingUnlockManager buildingManager = FindObjectOfType<BuildingUnlockManager>();
        if (buildingManager != null && buildingManager.buildingsTabButton != null)
        {
            int count = buildingManager.GetAvailableUpgradeCount();
            UpdateBadgeOnButton(buildingManager.buildingsTabButton, count, () => {
                buildingManager.NavigateToFirstAffordableUpgrade();
            });
        }
        
        // Update badge for Materials tab
        CommonAbilities materials = FindObjectOfType<CommonAbilities>();
        if (materials != null && materials.materialsTabButton != null)
        {
            int count = materials.GetAvailableUpgradeCount();
            UpdateBadgeOnButton(materials.materialsTabButton, count, () => {
                materials.NavigateToFirstAffordableUpgrade();
            });
        }
        
        // Update badge for Challenges panel button (number of claimable challenges)
        if (challengesPanelButton != null && ChallengeManager.Instance != null)
        {
            int count = ChallengeManager.Instance.GetClaimableCount();
            UpdateBadgeOnButton(challengesPanelButton, count, () => {
                var panel = FindObjectOfType<ChallengesPanelUI>();
                if (panel != null)
                    panel.SetPanelVisible(true);
            });
        }
        
        // Update badge for Achievements panel button (number of claimable achievements)
        if (achievementsPanelButton != null && AchievementManager.Instance != null)
        {
            int count = AchievementManager.Instance.GetClaimableCount();
            UpdateBadgeOnButton(achievementsPanelButton, count, () => {
                var panel = FindObjectOfType<AchievementsPanelUI>();
                if (panel != null)
                    panel.SetPanelVisible(true);
            });
        }
    }
    
    /// <summary>
    /// Gets the total count of available upgrades across all 4 managers (Warehouse, Delivery, Kitchen, Catering)
    /// </summary>
    private int GetManagersPanelUpgradeCount()
    {
        int totalCount = 0;
        
        if (WarehouseManager.Instance != null)
        {
            totalCount += WarehouseManager.Instance.GetAvailableUpgradeCount();
        }
        
        if (DeliveryManager.Instance != null)
        {
            totalCount += DeliveryManager.Instance.GetAvailableUpgradeCount();
        }
        
        if (KitchenManager.Instance != null)
        {
            totalCount += KitchenManager.Instance.GetAvailableUpgradeCount();
        }
        
        if (CateringManager.Instance != null)
        {
            totalCount += CateringManager.Instance.GetAvailableUpgradeCount();
        }
        
        return totalCount;
    }
    
    /// <summary>
    /// Navigates to the first available upgrade from any of the 4 managers
    /// Priority: Warehouse -> Delivery -> Kitchen -> Catering
    /// </summary>
    private void NavigateToFirstAvailableManagerUpgrade()
    {
        // Try Warehouse first
        if (WarehouseManager.Instance != null && WarehouseManager.Instance.GetAvailableUpgradeCount() > 0)
        {
            WarehouseManager.Instance.NavigateToFirstAffordableUpgrade();
            return;
        }
        
        // Try Delivery second
        if (DeliveryManager.Instance != null && DeliveryManager.Instance.GetAvailableUpgradeCount() > 0)
        {
            DeliveryManager.Instance.NavigateToFirstAffordableUpgrade();
            return;
        }
        
        // Try Kitchen third
        if (KitchenManager.Instance != null && KitchenManager.Instance.GetAvailableUpgradeCount() > 0)
        {
            KitchenManager.Instance.NavigateToFirstAffordableUpgrade();
            return;
        }
        
        // Try Catering last
        if (CateringManager.Instance != null && CateringManager.Instance.GetAvailableUpgradeCount() > 0)
        {
            CateringManager.Instance.NavigateToFirstAffordableUpgrade();
            return;
        }
    }
    
    /// <summary>
    /// Updates or creates a badge on a button showing upgrade count
    /// </summary>
    private void UpdateBadgeOnButton(Button button, int upgradeCount, System.Action onBadgeClick)
    {
        if (button == null) return;
        
        // Remove existing badge if count is 0
        if (upgradeCount == 0)
        {
            if (upgradeBadges.ContainsKey(button) && upgradeBadges[button] != null)
            {
                Destroy(upgradeBadges[button]);
                upgradeBadges.Remove(button);
            }
            return;
        }
        
        // Create badge if it doesn't exist
        if (!upgradeBadges.ContainsKey(button) || upgradeBadges[button] == null)
        {
            GameObject badge;
            
            // Use prefab if available, otherwise create simple badge
            if (upgradeBadgePrefab != null)
            {
                badge = Instantiate(upgradeBadgePrefab, button.transform);
            }
            else
            {
                // Create simple badge programmatically
                badge = new GameObject("UpgradeBadge");
                badge.transform.SetParent(button.transform, false);
                
                // Add Image component for background
                Image bg = badge.AddComponent<Image>();
                bg.color = new Color(1f, 0.2f, 0.2f, 1f); // Red badge
                
                // Create a circular sprite programmatically
                Sprite circleSprite = CreateCircleSprite(70, 70);
                if (circleSprite != null)
                {
                    bg.sprite = circleSprite;
                    bg.type = Image.Type.Simple;
                }
                else
                {
                    // Fallback: try to use Unity's built-in sprites
                    circleSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
                    if (circleSprite != null)
                    {
                        bg.sprite = circleSprite;
                        bg.type = Image.Type.Simple;
                    }
                }
                
                RectTransform rect = badge.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 1f); // Top-left anchor
                rect.anchorMax = new Vector2(0f, 1f); // Top-left anchor
                rect.anchoredPosition = new Vector2(-50f, 50f); // Position from top-left
                rect.sizeDelta = new Vector2(70f, 70f);
                
                // Add Text for count
                GameObject textObj = new GameObject("CountText");
                textObj.transform.SetParent(badge.transform, false);
                TMP_Text countText = textObj.AddComponent<TextMeshProUGUI>();
                countText.text = upgradeCount > 9 ? "9+" : upgradeCount.ToString();
                countText.fontSize = 32; // Larger font for 70x70 badge
                countText.color = Color.white;
                countText.alignment = TextAlignmentOptions.Center;
                countText.fontStyle = FontStyles.Bold;
                
                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                textRect.anchoredPosition = Vector2.zero;
                
                // Ensure text fits properly
                countText.enableAutoSizing = true;
                countText.fontSizeMin = 16;
                countText.fontSizeMax = 40;
            }
            
            // Make badge clickable to navigate to upgrade
            Button badgeButton = badge.GetComponent<Button>();
            if (badgeButton == null)
            {
                badgeButton = badge.AddComponent<Button>();
            }
            
            badgeButton.onClick.RemoveAllListeners();
            badgeButton.onClick.AddListener(() => {
                if (onBadgeClick != null) onBadgeClick();
            });
            
            upgradeBadges[button] = badge;
        }
        
        // Update badge count text
        TMP_Text text = upgradeBadges[button].GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = upgradeCount > 9 ? "9+" : upgradeCount.ToString();
        }
    }
    
    /// <summary>
    /// Creates a circular sprite programmatically
    /// </summary>
    private Sprite CreateCircleSprite(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(width / 2f, height / 2f);
        float radius = width / 2f - 2f; // Slight padding from edges
        
        Color[] pixels = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                
                if (distance <= radius)
                {
                    pixels[y * width + x] = Color.white; // Opaque white (color will be set by Image component)
                }
                else
                {
                    pixels[y * width + x] = Color.clear; // Transparent
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Create sprite from texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
        return sprite;
    }
}
public enum UIUpdateType
{
    Cash,
    Storage,
    Multiplier,
    Gold,
    AutomaticEarning
}