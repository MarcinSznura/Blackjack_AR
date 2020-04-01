using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;



public class RewardAdsDisplay : MonoBehaviour
{
    public string myGameIdAndroid = "3535446";
    public string myGameIdIOS = "3535446";
    public string myVideoPlacement = "rewardedVideo";
    public bool adStarted;
    public bool adCompleted;
    private bool testMode = true;

    ShowOptions options = new ShowOptions();
    [SerializeField] Canvas reawardCanvas;

    private void Start()
    {
#if UNITY_IOS
        Advertisement.Initialize(myGameIdIOS, testMode); 
#else
        Advertisement.Initialize(myGameIdAndroid, testMode); 
#endif
    }

    public void ShowAd()
    {
        if (Advertisement.isInitialized && Advertisement.IsReady(myVideoPlacement) && !adStarted)
        {
            options.resultCallback = AdDisplayResultCallback;
            Advertisement.Show(myVideoPlacement, options);
            adStarted = true;
        }
    }

    private void Update()
    {
        if (adCompleted)
        {
            FindObjectOfType<GameMaster>().ChangePlayerBalance(1000);
            adStarted = false;
            adCompleted = false;
            reawardCanvas.enabled = true;
        }
    }

    private void AdDisplayResultCallback(ShowResult result)
    {
        adCompleted = result == ShowResult.Finished;
    }

    public void CloseRewardInfo()
    {
        reawardCanvas.enabled = false;
    }

}
