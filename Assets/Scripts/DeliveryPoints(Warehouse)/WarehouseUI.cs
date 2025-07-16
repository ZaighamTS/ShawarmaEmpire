using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WarehouseUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text title;
    public TMP_Text costText;
    public Button buildButton;

    private Warehouse data;
    private WarehouseManager uiManager;

    public void Setup(Warehouse data, WarehouseManager manager)
    {
        this.data = data;
        this.uiManager = manager;

       // icon.sprite = data.icon;
        title.text = data.warehouseName;
       // costText.text = $"${data.cost}";

        buildButton.onClick.AddListener(OnBuildClicked);
    }

    private void OnBuildClicked()
    {
        //if (EconomyManager.Instance.TrySpend(data.buildCost))
        //{
        //    Debug.Log($"{data.pointName} Built!");
        //    uiManager.AddBuiltPoint(data);
        //    Destroy(gameObject); // Remove build option after building
        //}
        //else
        //{
        //    Debug.Log("Not enough cash.");
        //}
    }
}
