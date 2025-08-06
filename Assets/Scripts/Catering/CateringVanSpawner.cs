using Cysharp.Threading.Tasks;
using System.Collections;
using System.Linq;

using UnityEngine;


public class CateringVanSpawner : MonoBehaviour
{
    PlayerProgress playerProgress;
    public static CateringVanSpawner Instance;
    public GameObject[] vanPrefab;
    public Transform spawnPoint;
    public Transform deliveryPoint;
    public Transform Exit_point;


    public float spawnInterval = 10f;
    private float deliveryCapacity;
    private int currentLevel;
    private bool isDirty = false;

    private void Start()
    {
        if (Instance == null)
        { 
            Instance = this;
        }
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
        yield return new WaitForSecondsRealtime(5);
        while (true)
        { 
            SpawnVan();
            yield return new WaitForSecondsRealtime(spawnInterval);
        }
    }
    internal void UpgradeVan()
    {
        float upgradeCost = UpgradeCosts.GetUpgradeCost(UpgradeType.DeliveryVan, currentLevel);
        if (upgradeCost <= playerProgress.PlayerCash)
        {
            playerProgress.PlayerCash -= upgradeCost;
            currentLevel++;
            deliveryCapacity = UpgradeCosts.GetDeliveryCapacity(CapacityType.Delivery, currentLevel);
            spawnInterval = UpgradeCosts.GetDeliveryInterval(currentLevel);
            isDirty = true;
        }
        else
        {
            //Open Store
        }
    }
    void SpawnVan()
    {
        int n=Random.Range(0, vanPrefab.Length);
        GameObject van = Instantiate(vanPrefab[n], spawnPoint.position, spawnPoint.rotation);
        van.transform.SetParent(transform);
        CateringVan cateringVan = van.GetComponent<CateringVan>();
        cateringVan.exitOffset = Exit_point;
        //if (WarehouseManager.Instance.placedWarehouses.Count > 0)
        //{
        //    int RandomNumber = Random.Range(0, WarehouseManager.Instance.placedWarehouses.Count);
        //    Debug.Log("" + WarehouseManager.Instance.placedWarehouses.Count);

        //    // deliveryVan.MoveTo(WarehouseManager.Instance.placedWarehouses[RandomNumber].transform.GetComponent<Warehouse>().DeliveryPosition.localPosition);
        //    deliveryPoint.position = WarehouseManager.Instance.placedWarehouses[RandomNumber].transform.GetComponent<Warehouse>().DeliveryPosition.position;

        //}
        //Pass Unit To Van To Deduct At Delivery
        cateringVan.MoveTo(deliveryPoint.position);

    }
  
}

