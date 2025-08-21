using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Upgdradable : MonoBehaviour
{
    [SerializeField] private protected Transform buidlNewPointParent;
    [SerializeField] private protected GameObject[] warehouses;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     protected virtual void OnUpgradeItem()
    {
        for (int i = 0; i < buidlNewPointParent.childCount; i++)
        {
            bool isPurchased;

            Transform point = buidlNewPointParent.GetChild(i);
            if (warehouses[i].GetComponent<Warehouse>().currentUpdate > 1)
            {
                isPurchased = true;
               // Debug.Log("aa " + (warehouses[i].GetComponent<Warehouse>().currentUpdate - 2).ToString());
                point.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = warehouses[i].GetComponent<Warehouse>().updates[warehouses[i].GetComponent<Warehouse>().currentUpdate - 2].UpdateName;
                point.GetChild(1).GetChild(1).GetChild(0).GetChild(0).transform.GetComponent<Image>().sprite = warehouses[i].GetComponent<Warehouse>().updates[warehouses[i].GetComponent<Warehouse>().currentUpdate - 2].Icon;
            }
            else
            {
                isPurchased = false;
            }
           
            point.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = warehouses[i].GetComponent<Warehouse>().cost.ToString();
            point.GetChild(0).gameObject.SetActive(!isPurchased);
            point.GetChild(1).gameObject.SetActive(isPurchased);
            
           
        }
    }
}
