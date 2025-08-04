using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraBuildingFunctionality : MonoBehaviour
{
    public BuildingType currentBuildingType;
    private record Parameters(int Reward, int rewardDelay, int expense, int expenseDelay);
    Dictionary<BuildingType, Parameters> parametersList = new();

    // Start is called before the first frame update
    void Start()
    {
        parametersList = new()
        {
            {BuildingType.ingrediants,new(10,1,3,2) },
            {BuildingType.merchandise,new(10,1,2,2) },
            {BuildingType.shawarmaLounge,new(10,1,4,2) },
            {BuildingType.juicePoint,new(10,1,2,2) },
            {BuildingType.dessertPoint,new(10,1,4,2) },
            {BuildingType.management,new(10,1,1,2) },
            {BuildingType.park,new(10,1,3,2) },
            {BuildingType.gasStation,new(10,1,3,2) },
        };

        StartCoroutine(DoRewardFunctionality(currentBuildingType));
    }
   
    IEnumerator DoRewardFunctionality(BuildingType Building_Type)
    {

        yield return new WaitForSeconds(2);
        if (parametersList.TryGetValue(Building_Type, out var CurrenetParam))
        {
            while (true)
            {
                yield return new WaitForSeconds(CurrenetParam.rewardDelay);
                DoIncreamentinCash(CurrenetParam.Reward);
                yield return new WaitForSeconds(CurrenetParam.expenseDelay);
                DecreamentCash(-CurrenetParam.expense);
            }

        }
        void DoIncreamentinCash(int val)
        {
            GameManager.gameManagerInstance.AddCash(val);
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);
        }
        void DecreamentCash(int val)
        {
            GameManager.gameManagerInstance.AddCash(val);
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);
        }

    }
}
public enum BuildingType
{
    ingrediants,
    merchandise,
    park,
    gasStation,
    management,
    shawarmaLounge,
    juicePoint,
    dessertPoint
}