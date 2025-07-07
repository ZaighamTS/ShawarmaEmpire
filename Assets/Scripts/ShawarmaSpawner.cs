using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ShawarmaSpawner : MonoBehaviour
{
    
    public ObjectPool objectPool; 
    [SerializeField]
    public List<Target> _Targets = new List<Target>();
    public bool CanGenShawarma=true;
    public static ShawarmaSpawner Instance;
    private Coroutine genRoutine;
    [Range(0,1)]public float Delay;
    public SliderController sliderController;
    public WarehouseManager _WareHouseManager;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void CheckBeltMat()
    {
        for (int i = 0; i < _Targets.Count; i++)
        {
            if (!_Targets[i].HasSpace())
            {
                _WareHouseManager.Tracks[i].GetComponent<ScrollMaterial>().ChangeBeltMat(false);
            }
            else
            {
                _WareHouseManager.Tracks[i].GetComponent<ScrollMaterial>().ChangeBeltMat(true);
            }
        }
    }
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

    public void OnTapButtonPressed()
    {
        ShawarmaGenFun();
     
    }
    public void ShawarmaGenFun()
    {
        if (CanGenShawarma&& sliderController.slider.value> sliderController.decreaseSpeed)
        {
            sliderController?.DecreaseSliderOnTap();
            Target currentTarget = GetAvailableTarget();
            GameObject obj = objectPool.GetPooledObject();
            obj.transform.GetComponent<Shawarma>().SetTarget(currentTarget.targetPoint);
            currentTarget.AddObject();
            currentTarget.WareHouseMainObject.GetComponent<Warehouse>().OnShwarmaGen();
            obj.SetActive(true);
            //Below Logic to check avaiblity to accept shawarma in all warerhouse
            int n = 0;
            foreach (Target t in _Targets)
            {
                if (t.HasSpace())
                {
                    n++;
                    break;
                }
            }
            if (n == 0)
            {
                CanGenShawarma = false;
            }

            //CheckBeltMat();
        }
    }
    private Target GetAvailableTarget()
    {
        List<Target> availableTargets = new List<Target>();

        foreach (Target t in _Targets)
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

    public void AddNewTarget(int index, int capacity, Transform targetPosition,GameObject WarehouseObject)
    {
        CanGenShawarma = true;
        _Targets.Add(new Target(index, capacity, targetPosition, WarehouseObject));
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
    public bool CanEnter=true;
    public Target(int index,int capacity, Transform point,GameObject wareHouse)
    {
        Capacity = capacity;
        targetPoint = point;
        CurrentLoad = 0;
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
//foreach (Target t in _Targets)
//{

//    if (t.HasSpace())
//    {
//        return t;
//    }
//}
//return null;