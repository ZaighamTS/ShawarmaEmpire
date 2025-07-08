using System.Collections;
using UnityEngine;

public class DeliveryVanSpawner : MonoBehaviour
{
    public GameObject vanPrefab;
    public Transform spawnPoint;
    public Transform deliveryPoint;
    public Transform Exit_point;
    public float spawnInterval = 10f;

    private void Start()
    {
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

    void SpawnVan()
    {
        GameObject van = Instantiate(vanPrefab, spawnPoint.position, spawnPoint.rotation);
        van.transform.SetParent(transform);
        DeliveryVan deliveryVan = van.GetComponent<DeliveryVan>();
        deliveryVan.exitOffset = Exit_point;
        deliveryVan.MoveTo(deliveryPoint.position);
    }
}
