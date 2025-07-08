using System.Collections;
using UnityEngine;

public class DeliveryVan : MonoBehaviour
{
    public float speed = 5f;
    public float waitTimeAtPoint = 3f;
    public Transform exitOffset;
    public Transform[] wheels; // Assign 4 wheels in Inspector
    public float wheelRotationSpeed = 360f; // degrees per second

    private Vector3 targetPosition;
    private bool isExiting = false;

    public void MoveTo(Vector3 deliveryPoint)
    {
        targetPosition = deliveryPoint;
        StartCoroutine(MoveVan());
    }

    IEnumerator MoveVan()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
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
            yield return new WaitForSeconds(waitTimeAtPoint);
            targetPosition = exitOffset.position;
            isExiting = true;
            StartCoroutine(MoveVan()); // Move again toward exit
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
