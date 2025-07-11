using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CateringVehicle : MonoBehaviour
{
    [SerializeField]
    internal int baseCateringCapacity;
    internal int cpacityIcrement;
    internal int GetCateringCapacity()
    {
        return baseCateringCapacity + cpacityIcrement;
    }
}
