//using System;
//using System.Security.Cryptography;
//using UnityEngine;
//using UnityEngine.Advertisements;
//using UnityEngine.UI;

//public class Unity_Ads : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
//{

//    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
//    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
//    string _adUnitId = null; // This will remain null for unsupported platforms
//    public string id;
//    public bool Test;
//    public Button _showAdButton;
//    public Action RewardVideoCompletedAction;


//    void Awake()
//    {

//        // Get the Ad Unit ID for the current platform:
//#if UNITY_IOS
//        _adUnitId = _iOSAdUnitId;
//#elif UNITY_ANDROID
//        _adUnitId = _androidAdUnitId;
//#else
//        _adUnitId = _androidAdUnitId;
//#endif
//        // Disable the button until the ad is ready to show:
//        // _showAdButton.interactable = false;
//        LoadSDK();
//    }
//    public void LoadSDK()
//    {
        
//        if (!Advertisement.isInitialized && Advertisement.isSupported)
//        {
//            Advertisement.Initialize(id, Test, this);
//            Debug.Log("1");
           
//        }
//    }

//    public void OnInitializationComplete()
//    {
//        LoadAd();
//        Debug.Log("Unity Ads initialization complete.");
//    }

//    public void LoadAd()
//    {
       
//        Debug.Log("Loading Ad: " + _adUnitId);
//        Advertisement.Load(_adUnitId, this);
//    }

//    // If the ad successfully loads, add a listener to the button and enable it:
//    public void OnUnityAdsAdLoaded(string adUnitId)
//    {
//        Debug.Log("Ad Loaded: " + adUnitId);

//        if (adUnitId.Equals(_adUnitId))
//        {
//            // Configure the button to call the ShowAd() method when clicked:
//             _showAdButton.onClick.AddListener(ShowRewardedAd);
//            // Enable the button for users to click:
//             _showAdButton.interactable = true;
//        }
//    }

//    // Implement a method to execute when the user clicks the button:
//    public void ShowRewardedAd()
//    {
//        // Disable the button:
//      //  _showAdButton.interactable = false;
//        // Then show the ad:
//        Advertisement.Show(_adUnitId, this);
//    }

//    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
//    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
//    {
//        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
//        {
//            //AdsManager.Instance.GiveReward();
//            Debug.Log("Unity Ads Rewarded Ad Completed");
//            UIManager.Instance.OnRewardedAdSuccess();
//            LoadAd();
//            // Grant a reward.
//        }
//    }

//    // Implement Load and Show Listener error callbacks:
//    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
//    {
//        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
//        // _DisplayUIElements.ShowPopUp("Failed to load Ad.");
//        // Use the error details to determine whether to try to load another ad.
//    }

//    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
//    {
//        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
//        // _DisplayUIElements.ShowPopUp("Failed to load Ad.");
//        // Use the error details to determine whether to try to load another ad.
//    }

//    public void OnUnityAdsShowStart(string adUnitId) { }

//    public void OnUnityAdsShowClick(string adUnitId)
//    {
//        LoadAd();
//    }

//    //public void OnInitializationComplete()
//    //{
//    //    // throw new System.NotImplementedException();
//    //}

//    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
//    {
//        // throw new System.NotImplementedException();
//    }
//}





