using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraBuildingFunctionality : MonoBehaviour
{
    public BuildingType currentBuildingType;
    private record Parameters(int Reward, int rewardDelay, int expense, int expenseDelay);
    Dictionary<BuildingType, Parameters> parametersList = new();
    private bool isPaused = false;

    void Start()
    {
        // FIXED: Increased expenses to make cash reduction noticeable
        // Buildings should have meaningful operating costs
        // Expenses increased 3-5x to create visible cash drain
        parametersList = new()
        {
            // Reward, rewardDelay(sec), expense, expenseDelay(sec)
            {BuildingType.juicePoint,new(2,5,3,10) },        // $2 every 5s, $3 expense every 10s = $720/hr net (was $1,440)
            {BuildingType.dessertPoint,new(2,5,3,10) },      // $2 every 5s, $3 expense every 10s = $720/hr net (was $1,440)
            {BuildingType.merchandise,new(3,5,5,10) },        // $3 every 5s, $5 expense every 10s = $780/hr net (was $2,160)
            {BuildingType.ingrediants,new(3,5,8,10) },       // $3 every 5s, $8 expense every 10s = -$360/hr net (operating cost)
            {BuildingType.shawarmaLounge,new(5,5,10,10) },    // $5 every 5s, $10 expense every 10s = $0/hr net (break even)
            {BuildingType.park,new(2,10,5,15) },             // $2 every 10s, $5 expense every 15s = -$600/hr net (decorative cost)
            {BuildingType.gasStation,new(5,5,12,10) },        // $5 every 5s, $12 expense every 10s = -$1,320/hr net (operating cost)
            {BuildingType.management,new(10,5,15,10) },       // $10 every 5s, $15 expense every 10s = -$1,800/hr net (premium cost)
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
                // Check if building is paused
                if (isPaused)
                {
                    // Check every 5 seconds if player has enough cash to resume
                    yield return new WaitForSeconds(5f);
                    if (PlayerProgress.Instance.PlayerCash >= CurrenetParam.expense)
                    {
                        isPaused = false;
                        Debug.Log($"Building {Building_Type} resumed operations");
                    }
                    continue;
                }

                yield return new WaitForSeconds(CurrenetParam.rewardDelay);
                DoIncreamentinCash(CurrenetParam.Reward);
                
                yield return new WaitForSeconds(CurrenetParam.expenseDelay);
                bool expensePaid = DecreamentCash(CurrenetParam.expense);
                
                if (!expensePaid)
                {
                    isPaused = true;
                    Debug.Log($"Building {Building_Type} paused - insufficient funds for expenses");
                }
            }
        }
        
        void DoIncreamentinCash(int val)
        {
            GameManager.gameManagerInstance.AddCash(val);
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);
        }
        
        bool DecreamentCash(int val)
        {
            bool success = GameManager.gameManagerInstance.SpendCash(val);
            if (!success)
            {
                Debug.Log($"Cannot pay ${val} expense for {currentBuildingType} - building paused");
            }
            UIManager.Instance.UpdateUI(UIUpdateType.Cash);
            return success;
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