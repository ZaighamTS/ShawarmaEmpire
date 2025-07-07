using UnityEngine;
using System;

public class ScrollMaterial : MonoBehaviour
{
    public Renderer[] targetRenderer;
    public Material BeltMove, BeltStatic;

    // Public static event so other scripts can trigger it
    public static Action<bool> OnBeltStateChanged;

    void OnEnable()
    {
        OnBeltStateChanged += ChangeBeltMat;
    }

    void OnDisable()
    {
        OnBeltStateChanged -= ChangeBeltMat;
    }

    public void ChangeBeltMat(bool isMoving)
    {
        for (int i = 0; i < targetRenderer.Length; i++)
        {
            targetRenderer[i].material = isMoving ? BeltMove : BeltStatic;
        }
    }
}
