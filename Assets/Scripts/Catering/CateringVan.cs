using System.Collections;
using UnityEngine;

public class CateringVan : MonoBehaviour
{
    WarehouseManager storageManager;

    public float speed = 5f;
    public float waitTimeAtPoint = 3f;
    public Transform exitOffset;
    public Transform[] wheels; // Assign 4 wheels in Inspector
    public float wheelRotationSpeed = 360f; // degrees per second

    internal float deliveryCapacity;
    private Vector3 targetPosition;
    private bool isExiting = false;
    public float detectionDistance = 2f;
    public LayerMask vanLayerMask; // Assign layer for vans in Inspector

    private void Start()
    {
        storageManager = WarehouseManager.Instance;
    }
    public void MoveTo(Vector3 deliveryPoint)
    {
        targetPosition = deliveryPoint;
        StartCoroutine(MoveVan());
    }

    IEnumerator MoveVan()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, detectionDistance, vanLayerMask))
            {
                // If another van is detected in front, stop moving
                yield return null;
                continue;
            }

            // Move forward
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            RotateWheels();
            yield return null;
        }

        if (isExiting)
        {
            Destroy(gameObject);
        }
        else
        {
            var totalShawarmas = storageManager.GetWholeLoad();
            if (totalShawarmas > 0)
            {
                yield return new WaitForSeconds(waitTimeAtPoint / 2);
                
                // FIXED: Respect capacity limit and actually deduct shawarmas from warehouses
                // Calculate how many shawarmas to take (respecting capacity)
                int shawarmasToTake = Mathf.Min(totalShawarmas, (int)deliveryCapacity);
                
                // Deduct shawarmas from warehouses (distribute across all warehouses)
                DeductShawarmasFromWarehouses(shawarmasToTake);
                
                var shawarmaValue = UpgradeCosts.GetShawarmaValue(1);
                // EXTENDED GAMEPLAY: Increased tax rate from 20% to 30% to extend gameplay to 1+ week
                // Before: 0.80 (20% tax) - upgrades unlocked too quickly
                // After: 0.70 (30% tax) - slower income = upgrades take 10-20 minutes
                var totalRewards = shawarmaValue * shawarmasToTake * 0.70f;
                Debug.Log($"Catering delivery: {shawarmasToTake} shawarmas, ${totalRewards:F0} earned");
                
                GameManager.gameManagerInstance.AddCash(totalRewards);
                UIManager.Instance.UpdateUI(UIUpdateType.Cash);
                yield return new WaitForSeconds(waitTimeAtPoint / 2);
            }

        

            // Go to exit
            targetPosition = exitOffset.position;
            isExiting = true;
            StartCoroutine(MoveVan()); // Start again to move toward exit
        }
    }

    void RotateWheels()
    {
        if (wheels == null || wheels.Length == 0) return;

        float rotationAmount = wheelRotationSpeed * Time.deltaTime;

        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(Vector3.right, rotationAmount, Space.Self);
        }
    }
    
    // FIXED: Deduct shawarmas from warehouses when catering van picks up
    private void DeductShawarmasFromWarehouses(int amountToTake)
    {
        if (storageManager == null || storageManager.placedWarehouses == null) return;
        
        int remaining = amountToTake;
        
        // Distribute shawarmas across all warehouses (take from each until capacity is met)
        foreach (GameObject warehouseObj in storageManager.placedWarehouses)
        {
            if (remaining <= 0) break;
            
            Warehouse warehouse = warehouseObj.GetComponent<Warehouse>();
            if (warehouse != null && warehouse.currentLoad > 0)
            {
                int takeFromThis = Mathf.Min(warehouse.currentLoad, remaining);
                warehouse.currentLoad -= takeFromThis;
                remaining -= takeFromThis;
                
                // Update warehouse state
                warehouse.CheckWaring();
                
                // Update target record by warehouse GameObject (safer than using warehouse.id)
                ShawarmaSpawner.Instance?.UpdateRecordByWarehouse(warehouseObj);
            }
        }
        
        Debug.Log($"Catering van took {amountToTake - remaining} shawarmas from warehouses");
    }
}
