using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobManager : MonoBehaviour
{
    public static AdMobManager Instance;

    private RewardedAd rewardedAd;
    private string rewardedAdUnitId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeAdMob();
        LoadRewardedAd();
    }

    private void InitializeAdMob()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob initialized.");
        });

#if UNITY_ANDROID
        rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917"; // Test Rewarded Ad
#elif UNITY_IPHONE
        rewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313"; // Test Rewarded Ad
#else
        rewardedAdUnitId = "unexpected_platform";
#endif
    }

    public void LoadRewardedAd()
    {
        Debug.Log("Loading Rewarded Ad...");

        var adRequest = new AdRequest();

        RewardedAd.Load(rewardedAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load: " + error);
                return;
            }

            Debug.Log("Rewarded ad loaded.");
            rewardedAd = ad;

            // Register callbacks
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded ad closed. Loading new ad...");
                LoadRewardedAd();
            };

            rewardedAd.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.LogError("Rewarded ad failed to show: " + adError);
                LoadRewardedAd();
            };

            rewardedAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log($"Ad paid: {adValue.Value} {adValue.CurrencyCode}");
            };
        });
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"User rewarded with: {reward.Amount} {reward.Type}");
                GiveReward();
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad not ready, loading again...");
            LoadRewardedAd();
        }
    }

    private void GiveReward()
    {
        // TODO: Add your reward logic (coins, items, etc.)
        Debug.Log("Reward granted to player!");
        UIManager.Instance.OnRewardedAdSuccess();
    }
}
