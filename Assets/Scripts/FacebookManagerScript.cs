using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookManagerScript : MonoBehaviour
{

	private void Awake()
	{
		if (!FB.IsInitialized)
		{
			FB.Init(() =>
			{
				if (FB.IsInitialized)
					FB.ActivateApp();
				else
					Debug.Log("Error");
			},
			isGameShown =>
			{
				if (!isGameShown)
					Time.timeScale = 0;
				else
					Time.timeScale = 1;
			});
		}
		else
			FB.ActivateApp();
	}

	#region Login / Logout
	public void FacebookLogin()
	{
		var permissions = new List<string>() { "public_profile", "email", "user_friends" };
		FB.LogInWithReadPermissions(permissions);
	}

	public void FacebookLogout()
	{
		FB.LogOut();
	}
	#endregion

	//Shares game
	public void FacebookShare()
	{
		FB.ShareLink(
			contentURL: new System.Uri("https://play.google.com/store/apps/details?id=com.SkyGamesDevelopment.SpikeJump&hl"),
			contentTitle: "Spike Jump game!",
			contentDescription: "Join to me in new awesome game Spike Jump!",
			photoURL: new System.Uri("https://lh3.googleusercontent.com/lamVciFIKFi0d8VL1XewyfnlPY2PX4b99_StjGwQcFvx5gwh33gINDpjW-oA93IaJoW_=s180"),
			callback: ShareCallBack
			);
	}

	private void ShareCallBack(IShareResult result)
	{
		if (result.Error != null || result.Cancelled)
			StartCoroutine(this.GetComponent<MenuManagerScript>().ToggleInfoPanel("Something went wrong while posting... :(", true));
		else
		{
			PlayerSaveData psd = SaveManager.LoadData();
			if (!psd.isSharedPost)
			{
				psd.isSharedPost = true;
				psd.specialMoney += 100;
				GPSManager.UnlockAchievement(GPGSIds.achievement_have_at_least_100_crystals);
				this.GetComponent<MenuManagerScript>().UpdateMoneyInMenu(psd.money, psd.specialMoney);
				SaveManager.SaveData(psd);
				StartCoroutine(this.GetComponent<MenuManagerScript>().ToggleInfoPanel("Successfully got 100 crystals!", true));
			}
		}
	}
	private void SharePointsCallBack(IShareResult result)
	{
		if (result.Error != null || result.Cancelled)
			StartCoroutine(this.GetComponent<MenuManagerScript>().ToggleInfoPanel("Something went wrong while posting... :(", true));
	}

	#region Inviting
	public void FacebookGameRequest()
	{
		FB.AppRequest("Join to me in this awesome game!", title: "Spike Jump game");
	}

	public void FacebookInvite()
	{
		FB.Mobile.AppInvite(new System.Uri("https://play.google.com/store/apps/details?id=com.SkyGamesDevelopment.SpikeJump&hl"));
	}
	#endregion
	
	/*
	public void GetFriendsPlayingThisGame()
	{
		string query = "/me/friends";
		FB.API(query, HttpMethod.GET, result =>
		{
			var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
			var friendsList = (List<object>)dictionary["data"];
			FriendsText.text = string.Empty;
			foreach (var dict in friendsList)
				FriendsText.text += ((Dictionary<string, object>)dict)["name"];
		});
	}
	*/
}