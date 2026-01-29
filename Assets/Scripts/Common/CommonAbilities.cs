using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CommonAbilities : MonoBehaviour
{
  

    [Header("AutoChef")]
    public GameObject AutoChefObject;
    [Header("Chicken")]
    public GameObject ChickenObject;
    [Header("Bread")]
    public GameObject BreadObject;
    [Header("Machine")]
    public GameObject MachineObject;
    [Header("Sause")]
    public GameObject SauceObject;
    [Header("Upgrade Availability Indicator")]
    public Button materialsTabButton; // Tab button for material upgrades (optional - for badge display)
    int Cost;
    int Level;
    // Start is called before the first frame update
    void Start()
    {
        CheckAvaibility("Chef", AutoChefObject);
        CheckAvaibility("Chicken", ChickenObject);
        CheckAvaibility("Bread", BreadObject);
        CheckAvaibility("Machine", MachineObject);
        CheckAvaibility("Sause", SauceObject);
    }
  
    #region Genaric
    public void CheckAvaibility(string PlayerPrefName, GameObject obj)
    {
        Level = PlayerPrefs.GetInt(PlayerPrefName);
        // EXTENDED GAMEPLAY: Increased material upgrade costs by 5x to extend gameplay
        // Before: (Level + 1) * 100 (material upgrades completed in ~3 hours)
        // After: (Level + 1) * 500 (material upgrades take ~15-20 hours)
        // COST REDUCTION: Reduced by 25%: 500 → 375
        Cost = (Level + 1) * 375;
        obj.transform.GetChild(0).GetChild(5).GetComponent<TextMeshProUGUI>().text = Level + "/10";
        if (Level < 10)
        {
            obj.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
           // Debug.Log("cost " + Level);
            obj.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = Cost.ToString();
        }
        else
        {
            obj.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
        }
        if (Level > 0)
        {
            if (PlayerPrefName == "Chef")
            {
                StartCoroutine(DoChefFunctionality(Level));
            }
            else if (PlayerPrefName == "Chicken")
            {
                StartCoroutine(DoChickenFunctionality(Level));
            }
            else if (PlayerPrefName == "Bread")
            {
                StartCoroutine(DoBreadFunctionality(Level));
            }
            else if (PlayerPrefName == "Sause")
            {
                StartCoroutine(DoSauseFunctionality(Level));
            }
            else if (PlayerPrefName == "Machine")
            {
                StartCoroutine(DoMachineFunctionality(Level));
            }
            else
            {
                Debug.Log("wrong");
            }
        }
    }

    public void ClickOnUpdateBtn(string PlayerPrefName)
    {
        Level = PlayerPrefs.GetInt(PlayerPrefName);
        // EXTENDED GAMEPLAY: Increased material upgrade costs by 5x to extend gameplay
        // Before: (Level + 1) * 100 (material upgrades completed in ~3 hours)
        // After: (Level + 1) * 500 (material upgrades take ~15-20 hours)
        // COST REDUCTION: Reduced by 25%: 500 → 375
        Cost = (Level + 1) * 375;

        if (Cost < PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(Cost);
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);
            PlayerPrefs.SetInt(PlayerPrefName, PlayerPrefs.GetInt(PlayerPrefName) + 1);
            if (PlayerPrefName == "Chef")
            {
                CheckAvaibility(PlayerPrefName, AutoChefObject);
            }
            else if (PlayerPrefName == "Chicken")
            {
                CheckAvaibility(PlayerPrefName, ChickenObject);
            }
            else if (PlayerPrefName == "Bread")
            {
                CheckAvaibility(PlayerPrefName, BreadObject);
            }
            else if (PlayerPrefName == "Sause")
            {
                CheckAvaibility(PlayerPrefName, SauceObject);
            }
            else if (PlayerPrefName == "Machine")
            {
                CheckAvaibility(PlayerPrefName, MachineObject);
            }
            else
            {
                Debug.Log("wrong");
            }


        }
        else
        {
            UIManager.Instance.lowCashPromt.SetActive(true);
        }

    }
    IEnumerator DoChefFunctionality(int level)
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            DoIncreamentCash(1);
            float maxWait = 4f;
            float minWait = 1f;
            float t = (Level - 1) / 9f;
            float waitTime = Mathf.Lerp(maxWait, minWait, t);
            yield return new WaitForSeconds(waitTime);
        }
    }
    IEnumerator DoChickenFunctionality(int level)
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            
            DoIncreamentCash(UpgradeCosts.GetChickenUpgradeValue(level));
            float maxWait = 4f;
            float minWait = 1f;
            float t = (Level - 1) / 9f;
            float waitTime = Mathf.Lerp(maxWait, minWait, t);
            yield return new WaitForSeconds(waitTime);
        }
    }
    IEnumerator DoBreadFunctionality(int level)
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            DoIncreamentCash(UpgradeCosts.GetBreadUpgradeValue(level));
            float maxWait = 4f;
            float minWait = 1f;
            float t = (Level - 1) / 9f;
            float waitTime = Mathf.Lerp(maxWait, minWait, t);
            yield return new WaitForSeconds(waitTime);
        }
    }
    IEnumerator DoSauseFunctionality(int level)
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            DoIncreamentCash(UpgradeCosts.GetSauceUpgradeValue(level));
            float maxWait = 4f;
            float minWait = 1f;
            float t = (Level - 1) / 9f;
            float waitTime = Mathf.Lerp(maxWait, minWait, t);
            yield return new WaitForSeconds(waitTime);
        }
    }
    IEnumerator DoMachineFunctionality(int level)
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            DoIncreamentCash(UpgradeCosts.GetMachineUpgradeCookRate(level));
            float maxWait = 4f;
            float minWait = 1f;
            float t = (Level - 1) / 9f;
            float waitTime = Mathf.Lerp(maxWait, minWait, t);
            yield return new WaitForSeconds(waitTime);
        }
    }
    void DoIncreamentCash(float amount)
    {
        GameManager.gameManagerInstance.AddCash(amount);
        UIManager.Instance.UpdateUI(UIUpdateType.Cash);
    }
    
    /// <summary>
    /// Counts how many material upgrades are currently affordable
    /// </summary>
    public int GetAvailableUpgradeCount()
    {
        int count = 0;
        float playerCash = PlayerProgress.Instance.PlayerCash;
        
        string[] materialTypes = { "Bread", "Chicken", "Sause", "Chef", "Machine" };
        
        foreach (string type in materialTypes)
        {
            int level = PlayerPrefs.GetInt(type);
            if (level < 10)
            {
                float cost = (level + 1) * 375f;
                if (cost <= playerCash) count++;
            }
        }
        
        return count;
    }
    
    /// <summary>
    /// Navigates to the first affordable material upgrade and highlights it
    /// </summary>
    public void NavigateToFirstAffordableUpgrade()
    {
        float playerCash = PlayerProgress.Instance.PlayerCash;
        
        // Check each material type in order
        string[] materialTypes = { "Bread", "Chicken", "Sause", "Chef", "Machine" };
        GameObject[] materialObjects = { BreadObject, ChickenObject, SauceObject, AutoChefObject, MachineObject };
        
        for (int i = 0; i < materialTypes.Length && i < materialObjects.Length; i++)
        {
            if (materialObjects[i] == null) continue;
            
            int level = PlayerPrefs.GetInt(materialTypes[i]);
            if (level < 10)
            {
                float cost = (level + 1) * 375f;
                if (cost <= playerCash)
                {
                    // Highlight this material upgrade button
                    Transform materialButton = materialObjects[i].transform.GetChild(0).GetChild(1);
                    if (materialButton != null)
                    {
                        HighlightUpgradeButton(materialButton);
                    }
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Adds a pulsing highlight effect to a material upgrade button
    /// </summary>
    private void HighlightUpgradeButton(Transform buttonTransform)
    {
        if (buttonTransform == null) return;
        
        Button button = buttonTransform.GetComponent<Button>();
        if (button != null)
        {
            // Stop any existing animations
            DG.Tweening.DOTween.Kill(button.transform);
            
            // Add pulsing scale animation
            button.transform.DOScale(1.1f, 0.5f)
                .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
                .SetEase(DG.Tweening.Ease.InOutSine);
        }
    }
    #endregion


}
