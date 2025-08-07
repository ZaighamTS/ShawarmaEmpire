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
        Debug.Log("here");
        // GameObject.FindObjectByType(TransitionEffectScript );
        if (TransitionEffectScript.PrestigeDone)
        {
            TransitionEffectScript.PrestigeDone = false;
            FindObjectOfType<TransitionEffectScript>().transform.GetChild(0).gameObject.SetActive(false);
           // FindFirstObjectByType<TransitionEffectScript>().transform.GetChild(0).gameObject.SetActive(false);
        }
      

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
    internal float GetTotalEarning()
    {
        return playerProgress.TotalEarnings;
    }
    //private void Update()
    //{
    //    moneyText.text = "$" + money.ToString("F0");
    //}
    internal void AddCash(float value)
    {
       
        playerProgress.PlayerCash += value;
        if (value > 0)
        {
            playerProgress.TotalEarnings += value;
        }
        
        
        CheckChefStars(playerProgress.TotalEarnings);
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

    private void CheckChefStars(float TotalEarning)
    {
        var newStars = UpgradeCosts.GetChefStars(TotalEarning);
        Debug.Log("newStars "+newStars);
       // Debug.Log("chefStars " + chefStars);
        if (newStars > playerProgress.ChefStars)
        {
            
            // ResetPlayerStats();
            UIManager.Instance.ShowPrestigeBtn();
        }
    }
    public void ResetPlayerStats()
    {
        //playerProgress.PlayerCash = 0;
        playerProgress.ChefStars ++;
        SaveLoadManager.saveLoadManagerInstance.ResetAllISaveables();
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
      //  Debug.Log("pppppppppp "+i);
    }
}
