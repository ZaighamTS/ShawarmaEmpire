using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommonAbilities : MonoBehaviour
{

    public GameObject AutoChefObject;
    int AutoChefCost;
    int AutoChefLevel;
    // Start is called before the first frame update
    void Start()
    { 
        CheckAutoChefAvaibility();       
    }

    public void CheckAutoChefAvaibility()
    {
        AutoChefLevel = PlayerPrefs.GetInt("AutoChefLevel");
        AutoChefCost = (AutoChefLevel+1) * 10;
        AutoChefObject.transform.GetChild(0).GetChild(5).GetComponent<TextMeshProUGUI>().text= AutoChefLevel+"/10";
        if (AutoChefLevel < 10)
        {
            AutoChefObject.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
            Debug.Log("cost "+ AutoChefCost);
            AutoChefObject.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = AutoChefCost.ToString();
        }
        else
        {
            AutoChefObject.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
        }
        if (AutoChefLevel > 0)
        {
            StartCoroutine(DoAutoChefFunctionality());
        }
    }

    public void ClickOnAutoChefUpdateBtn()
    {

        if (AutoChefCost < PlayerProgress.Instance.PlayerCash)
        {
            GameManager.gameManagerInstance.SpendCash(AutoChefCost);
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);
            PlayerPrefs.SetInt("AutoChefLevel", PlayerPrefs.GetInt("AutoChefLevel") + 1);
            CheckAutoChefAvaibility();
        }
        else
        {
            UIManager.Instance.lowCashPromt.SetActive(true);
        }

    }
    IEnumerator DoAutoChefFunctionality()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            DoIncreamentinCash();
            float maxWait = 4f;
            float minWait = 1f;
            float t = (AutoChefLevel - 1) / 9f; 
            float waitTime = Mathf.Lerp(maxWait, minWait, t);
            yield return new WaitForSeconds(waitTime);
        }
    }
    void DoIncreamentinCash()
    {
        GameManager.gameManagerInstance.AddCash(AutoChefLevel);
        UIManager.Instance.UpdateUI(UIUpdateType.Cash);
    }
}
