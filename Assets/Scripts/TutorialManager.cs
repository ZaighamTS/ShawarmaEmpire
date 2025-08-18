using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [System.Serializable]
    public class TutorialStep
    {
        public string key;           
        public GameObject panel;    
    }

    [Header("Tutorial Steps")]
    public List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    private bool isShowing = false;
    private TutorialStep currentStep;

    private void Awake()
    {
      
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

   
    public void ShowTutorial(string key)
    {

        if (isShowing) return;

        
        if (PlayerPrefs.GetInt("Tutorial_" + key, 0) == 1)
            return;

        Debug.Log("key "+key);
        currentStep = tutorialSteps.Find(step => step.key == key);

        if (currentStep != null && currentStep.panel != null)
        {
            currentStep.panel.SetActive(true);
            isShowing = true;
           // Time.timeScale = 0f; 
        }
    }

   
    public void CloseCurrentTutorial()
    {
        if (currentStep != null && currentStep.panel != null)
        {
            currentStep.panel.SetActive(false);
            PlayerPrefs.SetInt("Tutorial_" + currentStep.key, 1);
            PlayerPrefs.Save();
        }

        currentStep = null;
        isShowing = false;
      //  Time.timeScale = 1f; 
    }
}
