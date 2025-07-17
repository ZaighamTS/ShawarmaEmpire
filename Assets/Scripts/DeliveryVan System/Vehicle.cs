using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    PlayerProgress playerProgress;
    private int id;
    private int capacity;
    private int kitchenLevel = 0;
    internal string VehicleName;
    public int CurrentUpdate;
    public bool VehicleIsPurchased;
   
    [Header("This Class References")]
    private bool isDirty = false;
    public string SaveKey => "Vehicle" + id;

    public List<KitchenUpdateDetails> updates = new List<KitchenUpdateDetails>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
