using UnityEngine.Advertisements;
using UnityEngine;
using System;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField]
    string _androidGameId = "4377207";

    [SerializeField]
    string _iOsGameId = "4377206";

    [SerializeField]
    bool _testMode = true;

    [SerializeField]
    bool _enablePerPlacementMode = true;

    private string _gameId;


    [SerializeField]
    string _androidAdUnitId = "Rewarded_Android";

    [SerializeField]
    string _iOsAdUnitId = "Rewarded_iOS";

    string _adUnitId;



    private EReward _reward = EReward.continue_game;

    private bool _is_ad_loaded = false;

    private bool _is_ads_enabled = false;



    private event Action<bool> OnIsAdLoadedChanged;

    public event Action<EReward> OnReward;

    public event Action<bool> OnCanShowAdChanged;



    public bool CanShowAd => _is_ads_enabled && _is_ad_loaded;

    public bool IsAdsEnabled
    {
        set
        {
            _is_ads_enabled = value;

            OnCanShowAdChanged(CanShowAd);
        }
    }



    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOsGameId : _androidGameId;

        Advertisement.Initialize(_gameId, _testMode, _enablePerPlacementMode, this);

        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOsAdUnitId : _androidAdUnitId;

        OnIsAdLoadedChanged += (b) =>
        {
            _is_ad_loaded = b;

            OnCanShowAdChanged(CanShowAd);
        };
    }



    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");

        LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }



    public void LoadAd()
    {
        Debug.Log("Loading Ad: " + _adUnitId);

        Advertisement.Load(_adUnitId, this);
    }


    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId))
        {
            OnIsAdLoadedChanged(true);
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }



    public void ShowAd(EReward reward)
    {
        _reward = reward;

        OnIsAdLoadedChanged.Invoke(false);

        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");

            OnReward.Invoke(_reward);

            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }



    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }



    public enum EReward
    {
        continue_game,
    }
}