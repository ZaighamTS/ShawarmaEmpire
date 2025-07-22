using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCamera;
    public float updateInterval = 0.02f; // update every 20 milliseconds (50 FPS)

    void Start()
    {
        mainCamera = Camera.main;
       // StartCoroutine(LookAtCameraRoutine());
    }
    void OnEnable()
    {
        StartCoroutine(LookAtCameraRoutine());
    }

    IEnumerator LookAtCameraRoutine()
    {
        while (true)
        {
            if (mainCamera != null)
            {
                // Make emoji look at the camera
                transform.LookAt(mainCamera.transform);

                // Optional: Fix rotation if emoji looks reversed
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + 180f, 0f);
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }
}

