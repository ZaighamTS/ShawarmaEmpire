using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        OnPlayButtonClick();
    }

    public void OnPlayButtonClick()
    {
        Invoke("Delay",3);
    }
    void Delay()
    {
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }
}
