using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;


public class DeliveryVanSpawner : MonoBehaviour
{
    PlayerProgress playerProgress;
    
    public  List<GameObject> vanPrefab;
    public Transform spawnPoint;
    public Transform deliveryPoint;
    public Transform Exit_point;


    public float spawnInterval = 10f;
   
 
    public static DeliveryVanSpawner Instance;
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
       
        StartSpawning().Forget();
    }
    void OnDestroy()
    {
       
    }
    async UniTask StartSpawning()
    {
        await UniTask.WaitUntil(() => WarehouseManager.Instance != null);
        StartCoroutine(SpawnVanLoop());
    }
    IEnumerator SpawnVanLoop()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            SpawnVan();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
   
    void SpawnVan()
    {
        if (vanPrefab.Count > 0)
        {
            int n = Random.Range(0, vanPrefab.Count);
            GameObject van = Instantiate(vanPrefab[n], spawnPoint.position, spawnPoint.rotation);
            van.transform.SetParent(transform);
            DeliveryVan deliveryVan = van.GetComponent<DeliveryVan>();
            deliveryVan.exitOffset = Exit_point;
            int RandomNumber = 0;
            if (WarehouseManager.Instance.placedWarehouses.Count > 0)
            {
                //for (int i = 0; i < WarehouseManager.Instance.placedWarehouses.Count; i++)
                //{
                //    if (WarehouseManager.Instance.placedWarehouses[i].transform)
                //}



                RandomNumber = Random.Range(0, WarehouseManager.Instance.placedWarehouses.Count);
              //  Debug.Log("" + WarehouseManager.Instance.placedWarehouses.Count);

                // deliveryVan.MoveTo(WarehouseManager.Instance.placedWarehouses[RandomNumber].transform.GetComponent<Warehouse>().DeliveryPosition.localPosition);
                deliveryPoint.position = WarehouseManager.Instance.placedWarehouses[RandomNumber].transform.GetComponent<Warehouse>().DeliveryPosition.position;

            }
            //Pass Unit To Van To Deduct At Delivery
            deliveryVan.MoveTo(deliveryPoint.position, RandomNumber);
        }
        

    }
   
}

