using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CateringVehicle : MonoBehaviour
{
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



    [SerializeField]
    internal int baseCateringCapacity;
    internal int cpacityIcrement;
    internal int GetCateringCapacity()
    {
        return baseCateringCapacity + cpacityIcrement;
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
            yield return new WaitForSeconds(waitTimeAtPoint / 2);

            //var shwarmas = storageManager.GetStoredShawarmas();
            //if (shwarmas >= deliveryCapacity)
            //{
            //    storageManager.DeliverShawarma(shwarmas);
            //    var shawarmaValue = UpgradeCosts.GetShawarmaValue(1);
            //    var totalRewards = (shawarmaValue * shwarmas) * 0.95f;
            //    PlayerProgress.Instance.PlayerCash += totalRewards;
            //}

            yield return new WaitForSeconds(waitTimeAtPoint / 2);

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
