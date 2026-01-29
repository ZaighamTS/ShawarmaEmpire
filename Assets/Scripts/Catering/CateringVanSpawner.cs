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
   
    void SpawnVan()
    {
        // FIXED: Set catering van capacity based on current catering level
        int cateringLevel = GetCurrentCateringLevel();
        float cateringCapacity = UpgradeCosts.GetDeliveryCapacity(CapacityType.Catering, cateringLevel);
        
        int n = Random.Range(0, vanPrefab.Length);
        GameObject van = Instantiate(vanPrefab[n], spawnPoint.position, spawnPoint.rotation);
        van.transform.SetParent(transform);
        CateringVan cateringVan = van.GetComponent<CateringVan>();
        cateringVan.exitOffset = Exit_point;
        cateringVan.deliveryCapacity = cateringCapacity; // Set capacity based on level
        
        //Pass Unit To Van To Deduct At Delivery
        cateringVan.MoveTo(deliveryPoint.position);
    }
    
    // Helper method to get current catering level
    private int GetCurrentCateringLevel()
    {
        if (CateringManager.Instance != null && CateringManager.Instance.placedCatering.Count > 0)
        {
            // Get the highest level catering building
            int maxLevel = 1;
            foreach (GameObject catering in CateringManager.Instance.placedCatering)
            {
                int level = catering.GetComponent<Catering>().currentUpdate - 1; // currentUpdate starts at 2 for level 1
                if (level > maxLevel) maxLevel = level;
            }
            return maxLevel;
        }
        return 1; // Default level if no catering buildings
    }
  
}

