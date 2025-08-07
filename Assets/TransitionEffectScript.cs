using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionEffectScript : MonoBehaviour
{
    public static bool PrestigeDone;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    public void OnTransitionComplete()
    { 
        transform.gameObject.SetActive(false);
    }
}
