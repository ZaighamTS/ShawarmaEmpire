using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

using Random = UnityEngine.Random;
[DefaultExecutionOrder(10)]
public class ShawarmaSpawner : MonoBehaviour
{

    public static event Action<UIUpdateType, float> onShawarmaCreated;
    public static event Action<int> onStoreShawarma;
    PlayerProgress playerProgress;

    public ObjectPool objectPool;
    [SerializeField]
    public List<Target> targets = new List<Target>();
    public bool CanGenShawarma = true;
    public static ShawarmaSpawner Instance;
    private Coroutine genRoutine;
    [Range(0, 1)] public float Delay;
    public SliderController sliderController;
   // public WarehouseManager wareHouseManager;

    [Header("Upgardes/Qulifiers")]
    private int qualityBonus = 1;
    private float generationBonus = .05f;


    [Header("Tapping Settings")]
    public float tapMultiplier = 1.0f;
    public float maxMultiplier = 1.5f;
    public float multiplierDecayRate = 0.5f;
    public float multiplierIncreasePerTap = 0.05f;
    public float timeBetweenTapsThreshold = 0.3f; // seconds for faster tapping

    private float lastTapTime;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        playerProgress = PlayerProgress.Instance;
    }
    

    public void OnTapButtonPressed()
    {
        ShawarmaGenFun();
    }
    public void ShawarmaGenFun()
    {
        if (CanGenShawarma && sliderController.slider.value > sliderController.decreaseSpeed)
        {
            sliderController?.DecreaseSliderOnTap();
            Target currentTarget = GetAvailableTarget();
            GameObject obj = objectPool.GetPooledObject();
            var shawarma = obj.transform.GetComponent<Shawarma>();
            shawarma.SetTarget(currentTarget.targetPoint);
            currentTarget.AddObject();
            currentTarget.WareHouseMainObject.GetComponent<Warehouse>().OnShwarmaGen();
            currentTarget.WareHouseMainObject.GetComponent<Warehouse>().CheckWaring();
           // WarehouseManager.Instance.UpdateSliderCurrentValue(currentTarget.WareHouseMainObject.GetComponent<Warehouse>().id, 0, currentTarget.WareHouseMainObject.GetComponent<Warehouse>().currentCapacity, currentTarget.WareHouseMainObject.GetComponent<Warehouse>().currentLoad);
            obj.SetActive(true);
            //Below Logic to check avaiblity to accept shawarma in all warerhouse
            //int n = 0;
            if (!targets.Any(t => t.HasSpace()))
            {
                CanGenShawarma = false;
            }
            var shawarmaType = shawarma.shawarmaType;
            var shawarmaValue = UpgradeCosts.GetShawarmaValue(qualityBonus);
            var generationReward = shawarmaValue * generationBonus;
            var shawarmaCount = 1;
            MultiplierFunctionality();
            //Add TapMultiplier Here,
            GameManager.gameManagerInstance.AddTotalShawarama(1);
           // Debug.Log("generationReward " + generationReward);
            GameManager.gameManagerInstance.AddCash(generationReward);
            onShawarmaCreated?.Invoke(UIUpdateType.Cash, generationReward);
            onShawarmaCreated?.Invoke(UIUpdateType.Storage, shawarmaCount);
            onShawarmaCreated?.Invoke(UIUpdateType.Multiplier, 1);


        }
    }
    private void Update()
    {
        if (Time.time - lastTapTime > timeBetweenTapsThreshold && tapMultiplier > 1.0f)
        {
            tapMultiplier = Mathf.Max(1.0f, tapMultiplier - multiplierDecayRate * Time.deltaTime);
            
            onShawarmaCreated?.Invoke(UIUpdateType.Multiplier, 1);

        }
    }
    public float GetMultiplier()
    {
        return tapMultiplier;
    }
    public void MultiplierFunctionality()
    {

        float timeSinceLastTap = Time.time - lastTapTime;
        lastTapTime = Time.time;

        if (timeSinceLastTap < timeBetweenTapsThreshold)
        {
            tapMultiplier = Mathf.Min(maxMultiplier, tapMultiplier + multiplierIncreasePerTap);
        }
        else
        {
            tapMultiplier = 1.0f; // Reset multiplier on slow tap
        }
    }
    private Target GetAvailableTarget()
    {
        List<Target> availableTargets = new List<Target>();
        foreach (Target t in targets)
        {
            if (t.HasSpace())
            {
                availableTargets.Add(t);
            }
            if (!t.HasSpace())
            {
                t.CanEnter = false;     //Sets indiviuals status of target if there capacity is full
            }
        }
        if (availableTargets.Count == 0)
        {
            return null;
        }
        int randomIndex = Random.Range(0, availableTargets.Count);
        return availableTargets[randomIndex];
    }

    public void AddNewTarget(int index, int capacity, Transform targetPosition, GameObject WarehouseObject, int load)
    {
        if (load<capacity)
        {
            CanGenShawarma = true;
        }
        
        targets.Add(new Target(index, capacity, targetPosition, WarehouseObject,load));
    }
    public void UpdateCapacity(GameObject obj,int newCapacity)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].WareHouseMainObject == obj)
            { 
                targets[i].Capacity =  newCapacity;
                targets[i].CurrentLoad = 0;
                if (targets[i].Capacity > 0)
                {
                    CanGenShawarma = true;
                }
            }
        }
    }
    public void UpdateRecord(int i)
    {
        if (targets[i].CurrentLoad < targets[i].Capacity)
        {
            targets[i].CanEnter = true;
            CanGenShawarma=true;
        }
    }

}
[System.Serializable]
public class Target
{
    public GameObject WareHouseMainObject;
    public int Index;
    public int Capacity;
    public int CurrentLoad;
    public Transform targetPoint;
    public bool CanEnter = true;
    public Target(int index, int capacity, Transform point, GameObject wareHouse,int load)
    {
        Capacity = capacity;
        targetPoint = point;
        CurrentLoad = load;
        Index = index;
        WareHouseMainObject = wareHouse;
    }

    public bool HasSpace()
    {
        return CurrentLoad < Capacity;
    }

    public void AddObject()
    {
        if (CurrentLoad < Capacity)
        {
            CurrentLoad++;
           
            CanEnter = true;
        }
        if (CurrentLoad >= Capacity)
        {
            CanEnter = false;
        }
    }

}
//public void CheckBeltMat()
//{
//    for (int i = 0; i < targets.Count; i++)
//    {
//        if (!targets[i].HasSpace())
//        {
//            wareHouseManager.Tracks[i].GetComponent<ScrollMaterial>().ChangeBeltMat(false);
//        }
//        else
//        {
//            wareHouseManager.Tracks[i].GetComponent<ScrollMaterial>().ChangeBeltMat(true);
//        }
//    }
//}
//public void OnPointerDown(PointerEventData eventData)
//{
//    StartGenerating();
//}

//public void OnPointerUp(PointerEventData eventData)
//{
//    StopGenerating();
//}

//public void StartGenerating()
//{
//    if (sliderController.slider.value > sliderController.refillSpeed)
//    {
//        if (genRoutine == null)
//        {
//            genRoutine = StartCoroutine(GenerateShawarma());


//        }
//    }        
//}

//public void StopGenerating()
//{
//    if (genRoutine != null)
//    {
//        StopCoroutine(genRoutine);
//        genRoutine = null;
//        sliderController?.StartRefilling();
//    }
//}

//private IEnumerator GenerateShawarma()
//{
//    while (true)
//    {
//        OnTapButtonPressed();
//        yield return new WaitForSeconds(Delay);
//    }
//}
//foreach (Target t in _Targets)
//{

//    if (t.HasSpace())
//    {
//        return t;
//    }
//}
//return null;

/// <summary>
/// Will be used Coomon Arg for Updaes Once Finalized
/// </summary>
public class ShawarmaUpdate
{

}
