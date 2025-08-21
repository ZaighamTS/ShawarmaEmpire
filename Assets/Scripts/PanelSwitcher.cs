using UnityEngine;
using UnityEngine.UI;

public class PanelSwitcher : MonoBehaviour
{


    public bool isUp,singlePanel;
    public float defaultHeight, effectHeight;
    public Vector3 defaultScale, effectScale;
    public ButtonPanelPair[] buttonPanelPairs;

  
    void Start()
    {
  
        AddBtnListener();
    }

    private void OnEnable()
    {
        if (singlePanel)
            SetDefaultForSinglePanel();
        else
            SetDefault();
    }



    // Method to add listeners to each button
    private void AddBtnListener()
    {
        foreach (ButtonPanelPair pair in buttonPanelPairs)
        {
            Button button = pair.button;
            GameObject panel = pair.panel;

            if (button != null && panel != null)
            {
                button.onClick.AddListener(() =>
                {
                    ManagePanels(panel);
                    ButtonEffect(button);
                    });
            }
        }
    }

    // Method to manage the visibility of panels
    private void ManagePanels(GameObject panelObj)
    {
        foreach (ButtonPanelPair pair in buttonPanelPairs)
        {
            GameObject panel = pair.panel;
            if (panel != null)
            {
                panel.SetActive(panel == panelObj);
            }
        }

    }

    //Method for button click effect
    private void ButtonEffect(Button clickedButton)
    {

        foreach (ButtonPanelPair pair in buttonPanelPairs)
        {
            RectTransform rectTransform = pair.button.transform.GetChild(0).GetComponent<RectTransform>();
            Image img = pair.button.GetComponent<Image>();

            if (pair.button == clickedButton)
            {
                rectTransform.anchoredPosition = new Vector3(0, isUp ? effectHeight : -effectHeight, 0);
                rectTransform.localScale = effectScale;
                img.enabled = true;
                img.color = new Color32(255, 20, 0, 120);
            }
            else
            {
                rectTransform.anchoredPosition = new Vector3(0, defaultHeight, 0);
                
                img.color = new Color32(255, 20, 0, 0);
                rectTransform.localScale = defaultScale;
            }
        }
    }

    //Set the default states for the menu 
    public void SetDefault()
    {
        for (int i = 0; i < buttonPanelPairs.Length; i++)
        {
            if (i == 0)
            {
                RectTransform rectTransform = buttonPanelPairs[i].button.transform.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector3(0, isUp ? effectHeight : -effectHeight, 0);
                Image img = buttonPanelPairs[i].button.GetComponent<Image>();
                img.color = new Color32(255, 20, 0, 120);
                buttonPanelPairs[i].panel.SetActive(true);
            }
            else
            {
                RectTransform rectTransform = buttonPanelPairs[i].button.transform.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector3(0, defaultHeight, 0);
                Image img = buttonPanelPairs[i].button.GetComponent<Image>();
                img.color = new Color32(255, 20, 0, 0);
                buttonPanelPairs[i].panel.SetActive(false);
            }
           
        }
    }

    //set the default state for collectionMenu as it shares one single panel
    private void SetDefaultForSinglePanel()
    {
        for (int i = 0; i < buttonPanelPairs.Length; i++)
        {
            if (i == 0)
            {
                RectTransform rectTransform = buttonPanelPairs[i].button.transform.GetChild(0).GetComponent<RectTransform>();
               
                rectTransform.anchoredPosition = new Vector3(0, isUp ? effectHeight : -effectHeight, 0);
                
            }
            else
            {
                RectTransform rectTransform = buttonPanelPairs[i].button.transform.GetChild(0).GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector3(0, defaultHeight, 0);
               

            }
             buttonPanelPairs[i].panel.SetActive(true);
        }
    }
}



[System.Serializable]
public class ButtonPanelPair
{
    public string Name;
    public Button button;
    public GameObject panel;
}