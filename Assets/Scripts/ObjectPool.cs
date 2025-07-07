using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;       // Prefab to pool
    public int poolSize = 10;       // Initial pool size
    public Transform[] ShwarmaSpawnPoints;
    private List<GameObject> pool;

    void Start()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.name = "Shawarma" + i;
            int spawnIndex = i % ShwarmaSpawnPoints.Length;
            obj.transform.position = ShwarmaSpawnPoints[spawnIndex].position;
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            
            pool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // Optional: expand pool if all are in use
        GameObject newObj = Instantiate(prefab);
        newObj.SetActive(false);
        pool.Add(newObj);
        return newObj;
    }
}
