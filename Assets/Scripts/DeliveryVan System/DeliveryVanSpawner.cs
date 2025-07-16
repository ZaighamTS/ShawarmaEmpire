using Cysharp.Threading.Tasks;
using System.Collections;
using System.Linq;

using UnityEngine;


public class DeliveryVanSpawner : MonoBehaviour, ISaveable
{
    PlayerProgress playerProgress;
    public string SaveKey => "delivery_van";
    public GameObject vanPrefab;
    public Transform spawnPoint;
    public Transform deliveryPoint;
    public Transform Exit_point;


    private float spawnInterval = 10f;
    private float deliveryCapacity;
    private int currentLevel;
    private bool isDirty = false;

    private void Start()
    {
        playerProgress = PlayerProgress.Instance;
        SaveLoadManager.saveLoadManagerInstance.Register(this);
        StartSpawning().Forget();
    }
    void OnDestroy()
    {
        SaveLoadManager.saveLoadManagerInstance.Unregister(this);
    }
    async UniTask StartSpawning()
    {
        await UniTask.WaitUntil(() => StorageManager.storageManagerInstance != null);
        StartCoroutine(SpawnVanLoop());
    }
    IEnumerator SpawnVanLoop()
    {
        while (true)
        {
            SpawnVan();
            yield return new WaitForSeconds(spawnInterval);
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
        GameObject van = Instantiate(vanPrefab, spawnPoint.position, spawnPoint.rotation);
        van.transform.SetParent(transform);
        DeliveryVan deliveryVan = van.GetComponent<DeliveryVan>();
        deliveryVan.exitOffset = Exit_point;
        if(WarehouseManager.Instance.placedWarehouses.Count>0)
        {
            int RandomNumber = Random.Range(0, WarehouseManager.Instance.placedWarehouses.Count);
            Debug.Log("" + WarehouseManager.Instance.placedWarehouses.Count);
            
            // deliveryVan.MoveTo(WarehouseManager.Instance.placedWarehouses[RandomNumber].transform.GetComponent<Warehouse>().DeliveryPosition.localPosition);
            deliveryPoint.position = WarehouseManager.Instance.placedWarehouses[RandomNumber].transform.GetComponent<Warehouse>().DeliveryPosition.position;

        }
        //Pass Unit To Van To Deduct At Delivery
        deliveryVan.MoveTo(deliveryPoint.position);

    }
    #region Save/Load
    public bool IsDirty => isDirty;
    public object CaptureState()
    {
        return new DeliveryVanData
        {
            spawnInterval = spawnInterval,
            currentLevel = currentLevel,
            deliveryCapacity = deliveryCapacity,
        };
    }
    public void RestoreState(object state)
    {
        if (state is not DeliveryVanData data)
            return;
        spawnInterval = data.spawnInterval;
        deliveryCapacity = data.deliveryCapacity;
        currentLevel = data.currentLevel;
        isDirty = false;
    }
    public void SetInitialData()
    {

    }
    public void ClearDirty()
    {
        isDirty = false;
    }
    #endregion
}
public class DeliveryVanData
{
    public float spawnInterval;
    public float deliveryCapacity;
    public int currentLevel;
}
