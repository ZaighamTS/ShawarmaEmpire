using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(50)]
public class GameManager : MonoBehaviour
{
    PlayerProgress playerProgress;
    public static GameManager gameManagerInstance;

    private float chefStars;
    [SerializeField] private int maxSubscribersCount = 7;
    [SerializeField] private int subscribersCount = 0;

    private void Awake()
    {
        if (gameManagerInstance == null)
            gameManagerInstance = this;
    }
    private void Start()
    {
        playerProgress = PlayerProgress.Instance;
        SaveLoadManager.saveLoadManagerInstance.Register(PlayerProgress.Instance);
        RecordPersistentRegistrations().Forget();
        // ExtraBuildingsPlacement placement = FindObjectOfType<ExtraBuildingsPlacement>();
        // placement.CurrentLevel = 1;
        Invoke("DelayOnStart",1.1f);
    }
    public void DelayOnStart()
    {
        UIManager.Instance.UpdateUI(UIUpdateType.Cash);
        UIManager.Instance.UpdateChefStarsText();
        UIManager.Instance.UpdateUI(UIUpdateType.Storage);

        // UIManager.Instance.Up
    }

    private void OnDestroy()
    {
        SaveLoadManager.saveLoadManagerInstance.Unregister(PlayerProgress.Instance);
    }
    internal async UniTask RecordPersistentRegistrations()
    {
        subscribersCount++;
        if (subscribersCount == maxSubscribersCount)
        {
            await UniTask.NextFrame();
            SaveLoadManager.saveLoadManagerInstance.LoadGame();
        }
    }
    internal float GetCurrentCash()
    {
        return playerProgress.PlayerCash;
    }
    //private void Update()
    //{
    //    moneyText.text = "$" + money.ToString("F0");
    //}
    internal void AddCash(float value)
    {
       
        playerProgress.PlayerCash += value;
       
        playerProgress.TotalEarnings += value;
        
        CheckChefStars(playerProgress.TotalEarnings, KitchenManager.Instance.Kitchens[0].GetComponent<Kitchen>().currentUpdate);
    }
    internal void AddTotalShawarama(int value)
    {

        playerProgress.ShwarmaCount += value;

       
    }

    internal bool SpendCash(float Value)
    {
        if (Value <= playerProgress.PlayerCash)
        {
            playerProgress.PlayerCash -= Value;
            return true;
        }
        return false;
    }

    private void CheckChefStars(float playerCash, int level)
    {
        var newStars = UpgradeCosts.GetChefStars(playerCash,level);
        if (newStars > chefStars)
        {
            playerProgress.ChefStars = newStars;
            ResetPlayerStats();
        }
    }
    private void ResetPlayerStats()
    {
        playerProgress.PlayerCash = 0;
    }
    private void OnApplicationQuit()
    {
        SaveLoadManager.saveLoadManagerInstance.SaveGame();
    }
    int i;
    void OnApplicationPause()
    {
        i++;

      //  SaveLoadManager.saveLoadManagerInstance.SaveGame();
    }
    private void OnApplicationFocus(bool focus)
    {
        Debug.Log("pppppppppp "+i);
    }
}
