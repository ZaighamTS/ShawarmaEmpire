using Cysharp.Threading.Tasks;
using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEditor;
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
        if (TransitionEffectScript.PrestigeDone)
        {
           
          
            UIManager.Instance.StartPanel.SetActive(false);
            
        }
        Invoke("DelayOnStart", 1.1f);
    }
    public void DelayOnStart()
    {
        UIManager.Instance.UpdateUI(UIUpdateType.Cash);
        UIManager.Instance.UpdateUI(UIUpdateType.Gold);
        UIManager.Instance.UpdateChefStarsText();
        UIManager.Instance.UpdateUI(UIUpdateType.Storage);
       // Debug.Log("here");
        // GameObject.FindObjectByType(TransitionEffectScript );
        if (TransitionEffectScript.PrestigeDone)
        {
            TransitionEffectScript.PrestigeDone = false;
            FindObjectOfType<TransitionEffectScript>().transform.GetChild(0).gameObject.SetActive(false);
            UIManager.Instance.GameplayPanel.SetActive(true);
            // FindFirstObjectByType<TransitionEffectScript>().transform.GetChild(0).gameObject.SetActive(false);
        }


        if (PlayerPrefs.GetInt("RewardCount") > 0)
        {
            Debug.Log("RewardCount" + PlayerPrefs.GetInt("RewardCount"));
            if (DateTime.TryParse(PlayerPrefs.GetString("CurrentDateTime"), null, DateTimeStyles.RoundtripKind, out DateTime savedTime))
            {

                TimeSpan elapsed = DateTime.UtcNow - savedTime;
              

                double secondsElapsed = elapsed.TotalSeconds;
                Debug.Log(secondsElapsed.ToString());
                double amount = (PlayerPrefs.GetInt("RewardCount") * 100) + secondsElapsed;
                PlayerPrefs.SetInt("RewardCount", 0);
                AddCash((int)amount);
                // int cfuel = (int)secondsElapsed / (int)currentWaitTime;
                //for (int i = 0; i < trucksToRefuel; i++)
                //{
                //    Do();
                //}
            }
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
            SaveLoadManager.saveLoadManagerInstance.LoadGame().Forget();
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
    internal float GetGold()
    { 
        return playerProgress.Gold;
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
            UIManager.Instance.UpdateEarningSlider();
        }


        CheckChefStars(playerProgress.TotalEarnings);
    }
    internal void AddGold(float value)
    { 
        playerProgress.Gold+= value;
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
    internal bool SpendGold(float Value)
    {
        if (Value <= playerProgress.Gold)
        {
            playerProgress.Gold -= Value;
            return true;
        }
        return false;
    }

    private void CheckChefStars(float TotalEarning)
    {
        var newStars = UpgradeCosts.GetChefStars(TotalEarning);
       // Debug.Log("newStars " + newStars);
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
        playerProgress.ChefStars++;
        SaveLoadManager.saveLoadManagerInstance.ResetAllISaveables();
    }
    [RuntimeInitializeOnLoadMethod]
    static void RegisterQuitCallback()
    {
        Application.wantsToQuit += OnAppWantsToQuit;
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
#endif
    }
    static bool OnAppWantsToQuit()
    {
        SaveLoadManager.saveLoadManagerInstance?.SaveGame();
        return true;
    }
#if UNITY_EDITOR
    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            SaveLoadManager.saveLoadManagerInstance?.SaveGame();
        }
    }
#endif

private void OnApplicationQuit()
{
    SaveLoadManager.saveLoadManagerInstance.SaveGame();
    PlayerPrefs.SetString("CurrentDateTime", DateTime.UtcNow.ToString());

}
int i;
void OnApplicationPause(bool pauseStatus)
{
    i++;
        if (pauseStatus)
        {
            SaveLoadManager.saveLoadManagerInstance.SaveGame();
            Debug.Log("pause Integer " + i);
        }
   
}
private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
          //  SaveLoadManager.saveLoadManagerInstance.SaveGame();
           // Debug.Log("focus Integer " + i);
        }
        
    }
}
