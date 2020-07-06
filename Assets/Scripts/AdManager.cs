using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{
	#region variables
	public static AdManager instance;

	private RewardBasedVideoAd rewardAd;

	private const string appID = "ca-app-pub-9362859609190359~2125151411";
	private const string adID = "ca-app-pub-9362859609190359/6372939069";
	private const string testAdID = "ca-app-pub-3940256099942544/5224354917";
	#endregion

	//This is initialization function
	private void Awake()
	{
		instance = this;
	}
	private void Start()
	{
		rewardAd = RewardBasedVideoAd.Instance;
		rewardAd.OnAdClosed += HandleRewardBasedVideoClosed;
		rewardAd.OnAdRewarded += HandleRewardBasedVideoRewarded;
		rewardAd.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
	}
	//Function that requesting ad
	public void RequestAd()
	{
		AdRequest adRequest = new AdRequest.Builder().AddTestDevice("45FEFFA2F21716EA33D649B6F6CB5449").Build();
		rewardAd.LoadAd(adRequest, adID);
	}
	//Showing ad
	public void ShowAd(GameObject returnObject)
	{
		if (rewardAd.IsLoaded())
		{
			rewardAd.Show();
		}
		else
		{
			returnObject.GetComponent<MenuManagerScript>().canInteract = true;
		}
	}
	//Handlers
	public void HandleRewardBasedVideoRewarded(object sender, Reward args)
	{
		StartCoroutine(GameObject.Find("MenuManager").GetComponent<MenuManagerScript>().AdWatched());
	}
	public void HandleRewardBasedVideoFailedToLoad(object sender, EventArgs args)
	{
		RequestAd();
	}
	public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
	{
		GameObject.Find("MenuManager").GetComponent<MenuManagerScript>().canInteract = true;
	}
}
