using System.Collections;
using UnityEngine;

public class DeliveryVan : MonoBehaviour
{
    WarehouseManager warehouseManager;

    public float speed = 5f;
    public float waitTimeAtPoint = 3f;
    public Transform exitOffset;
    public Transform[] wheels; // Assign 4 wheels in Inspector
    public float wheelRotationSpeed = 360f; // degrees per second

    public int deliveryCapacity;
    private Vector3 targetPosition;
    private bool isExiting = false;
    public float detectionDistance = 2f;
    public LayerMask vanLayerMask; // Assign layer for vans in Inspector
    int CurrentStop;
    private void Start()
    {
        warehouseManager = WarehouseManager.Instance;
    }
    public void MoveTo(Vector3 deliveryPoint,int n)
    {
        targetPosition = deliveryPoint;
        StartCoroutine(MoveVan());
        CurrentStop = n;
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
            var shwarmas = warehouseManager.placedWarehouses[CurrentStop].transform.GetComponent<Warehouse>().currentLoad;
            if (shwarmas >0)
            {
                yield return new WaitForSeconds(waitTimeAtPoint / 2);



                //if (shwarmas <= deliveryCapacity)
                {
                    var n = 0;
                    if (shwarmas < deliveryCapacity)
                    {
                        n = shwarmas;
                    }
                    else if (shwarmas > deliveryCapacity)
                    {
                        n = deliveryCapacity;
                    }
                    warehouseManager.DeliverShawarma(n, CurrentStop);
                    var shawarmaValue = UpgradeCosts.GetShawarmaValue(1);
                    var totalRewards = (shawarmaValue + n) * 0.95f;
                    Debug.Log("totalRewards " + totalRewards);
                    PlayerProgress.Instance.PlayerCash += totalRewards;
                    UIManager.Instance.UpdateUI(UIUpdateType.Cash);
                }

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
}
