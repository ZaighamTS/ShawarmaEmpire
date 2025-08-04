using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        Cost = (Level + 1) * 10;
        obj.transform.GetChild(0).GetChild(5).GetComponent<TextMeshProUGUI>().text = Level + "/10";
        if (Level < 10)
        {
            obj.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
            Debug.Log("cost " + Level);
            obj.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = Cost.ToString();
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
        Cost = (Level + 1) * 10;

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
    #endregion


}
