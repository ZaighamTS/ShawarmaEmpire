using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CateringManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(StartDilivering());
    }
    IEnumerator StartDilivering()
    {
        while (true)
        {
            yield return CaterShwarma();
            yield return new WaitForSeconds(20);
        }
    }
    IEnumerator CaterShwarma()
    {
        yield break;
    }
}
