using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CateringManager : MonoBehaviour
{
    [SerializeField]
    private Transform[] cateringVehicles;
    [SerializeField]
    private float vehicleSpeed;
    [SerializeField] private Transform instantiatePoint;
    [SerializeField] private Transform pickUpPoint;
    [SerializeField] private Transform exitPoint;
    void Start()
    {
        StartCoroutine(StartDilivering());
    }
    IEnumerator StartDilivering()
    {
        yield return new WaitForSeconds(30 /*Initial Warm Up*/);
        while (true)
        {
            yield return CaterShwarma();
            yield return new WaitForSeconds(20);
        }
    }
    IEnumerator CaterShwarma()
    {
        var randomIndex = Random.Range(0, cateringVehicles.Length);
        var newVehicle = Instantiate(cateringVehicles[randomIndex], instantiatePoint.position, Quaternion.identity);
        if (!newVehicle.TryGetComponent(out CateringVehicle cateringVehicle))
            yield break;

        int currentCapacity = cateringVehicle.GetCateringCapacity();

        Vector3 direction = (pickUpPoint.position - newVehicle.position);
        newVehicle.LookAt(direction);
        float distanceToReach = Vector3.Distance(pickUpPoint.position, newVehicle.position);

        while (distanceToReach >= .01f)
        {
            direction = (pickUpPoint.position - newVehicle.position);
            newVehicle.transform.position += direction * vehicleSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //Wait For Enough Production
        do
        {
            yield return new WaitForSeconds(2);
        } while (StorageManager.storageManagerInstance.currentShawarmas < currentCapacity);

        //Add To Income like in delivery Van

        distanceToReach = Vector3.Distance(pickUpPoint.position, exitPoint.position);
        while (distanceToReach >= .01f)
        {
            direction = (exitPoint.position - pickUpPoint.position);
            newVehicle.transform.position += direction * vehicleSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //Destroy Vehicle
    }
}
