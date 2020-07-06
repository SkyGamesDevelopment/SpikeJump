using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Linq;

public class GPSManager : MonoBehaviour
{
	private bool isSaving = false;

	void Awake()
	{
		//Creating a new config builder
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
		//Initializing new config file
		PlayGamesPlatform.InitializeInstance(config);
		//Activating GPS
		PlayGamesPlatform.Activate();
	}
	//Simple log in function
	public void SignIn()
	{
		//Logging in GPS
		Social.localUser.Authenticate((bool success) => { });
	}

	public bool IsAuthenticated()
	{
		return Social.localUser.authenticated;
	}

	#region saved games

	public string GetSaveString()
	{
		PlayerSaveData psd = SaveManager.LoadData();
		string r = "";
		r += psd.money.ToString();
		r += "|";
		r += psd.specialMoney.ToString();
		r += "|";
		r += psd.bestScore.ToString();
		r += "|";
		r += psd.choosedCharacter.ToString();
		r += "|";
		r += psd.freePrizeCollectedDate.ToString();
		r += "|";
		r += psd.isMusicMuted.ToString();
		r += "|";
		r += psd.isSoundMuted.ToString();
		r += "|";
		r += psd.playedBefore.ToString();
		r += "|";
		r += psd.gamesPlayed.ToString();
		r += "|";
		r += psd.developerMode.ToString();
		r += "|";
		r += psd.isSharedPost.ToString();

		return r;
	}

	public string GetSaveCharactersString()
	{
		PlayerSaveData psd = SaveManager.LoadData();
		string r = String.Join("|", psd.ownedCharacters.Select(p => p.ToString()).ToArray());

		return r;
	}

	public void LoadSaveString(string save)
	{
		string[] data = save.Split('|');
		PlayerSaveData psd = new PlayerSaveData
			(
			int.Parse(data[0]),
			int.Parse(data[1]),
			int.Parse(data[2]),
			new bool[this.GetComponent<CharactersDataScript>().characterSprite.Length],
			int.Parse(data[3]),
			Convert.ToDateTime(data[4]),
			Convert.ToBoolean(data[5]),
			Convert.ToBoolean(data[6]),
			Convert.ToBoolean(data[7]),
			int.Parse(data[8]),
			Convert.ToBoolean(data[9]),
			Convert.ToBoolean(data[10])
			);

		SaveManager.SaveData(psd);

		GameObject.Find("GameManager").GetComponent<GameManagerScript>().LoadGame();
	}

	public void LoadSaveStringCharacters(string save)
	{
		string[] data = save.Split('|');
		PlayerSaveData psd = SaveManager.LoadData();
		bool[] x = new bool[this.GetComponent<CharactersDataScript>().characterSprite.Length];
		
		for(int i = 0; i < data.Length; i++)
		{
			x[i] = Convert.ToBoolean(data[i]);
		}
		for(int i = data.Length; i < x.Length; i++)
		{
			x[i] = false;
		}

		psd.ownedCharacters = x;
		SaveManager.SaveData(psd);
		this.GetComponent<CharactersDataScript>().ownedCharacters = psd.ownedCharacters;
	}

	public void OpenSave(bool saving)
	{
		if(Social.localUser.authenticated)
		{
			isSaving = saving;
			((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution
				(
				"SpikeJumpSaveData",
				GooglePlayGames.BasicApi.DataSource.ReadCacheOrNetwork,
				GooglePlayGames.BasicApi.SavedGame.ConflictResolutionStrategy.UseLongestPlaytime,
				SaveGameOpened
				);
		}
	}

	public void OpenSaveCharacters(bool saving)
	{
		isSaving = saving;
		((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution
			(
			"SpikeJumpSaveData2",
			GooglePlayGames.BasicApi.DataSource.ReadCacheOrNetwork,
			GooglePlayGames.BasicApi.SavedGame.ConflictResolutionStrategy.UseLongestPlaytime,
			SaveGameOpenedCharacters
			);
	}

	private void SaveGameOpenedCharacters(SavedGameRequestStatus status, ISavedGameMetadata meta)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			if (isSaving)
			{
				byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(GetSaveCharactersString());
				SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().WithUpdatedDescription("Saved at: " + DateTime.Now.ToString()).Build();

				((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(meta, update, data, SaveUpdated);
			}
			else
			{
				((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(meta, SaveReadCharacters);
			}
		}
	}

	private void SaveGameOpened(SavedGameRequestStatus status, ISavedGameMetadata meta)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			if (isSaving)
			{
				byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(GetSaveString());
				SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().WithUpdatedDescription("Saved at: " + DateTime.Now.ToString()).Build();

				((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(meta, update, data, SaveUpdated);
			}
			else
			{
				((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(meta, SaveRead);
			}
		}
	}

	private void SaveReadCharacters(SavedGameRequestStatus status, byte[] data)
	{
		if(status == SavedGameRequestStatus.Success)
		{
			string saveData = System.Text.ASCIIEncoding.ASCII.GetString(data);
			LoadSaveStringCharacters(saveData);
			StartCoroutine(this.GetComponent<MenuManagerScript>().ToggleInfoPanel("Load success, restart app to avoid problems", true));
		}
		else
			StartCoroutine(this.GetComponent<MenuManagerScript>().ToggleInfoPanel("Load unsuccess", true));
	}

	private void SaveRead(SavedGameRequestStatus status, byte[] data)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			string saveData = System.Text.ASCIIEncoding.ASCII.GetString(data);
			LoadSaveString(saveData);

			OpenSaveCharacters(false);
		}
		else
			StartCoroutine(this.GetComponent<MenuManagerScript>().ToggleInfoPanel("Load unsuccess", true));
	}

	private void SaveUpdated(SavedGameRequestStatus status, ISavedGameMetadata meta)
	{
		OpenSaveCharacters(true);
	}

	private void SaveUpdatedCharacters(SavedGameRequestStatus status, ISavedGameMetadata meta)
	{
		Debug.Log("Success");
	}

	#endregion

	#region achievements
	//Unlocking achievement function
	public static void UnlockAchievement(string id)
	{
		//Reports a progress of achievement of ID... Value of progress...
		Social.ReportProgress(id, 100, success => { });
	}
	//Function that increments achievement progress by some number
	public void IncrementAchievement(string id, int howMuchIncrement)
	{
		//Reports a incremented progress of achievement of ID... Value to increment...
		PlayGamesPlatform.Instance.IncrementAchievement(id, howMuchIncrement, success => { });
	}
	//Simple function that show ups a achievements
	public void ShowAchievementsUI()
	{
		if (Social.localUser.authenticated)
			Social.ShowAchievementsUI();
		else
			SignIn();

	}
	#endregion

	#region leaderboards
	//Simple function that publishing score to leaderboard
	public void PublishScoreToLeaderboard(long score)
	{
		//Score ammount... id of leaderboard...
		if (Social.localUser.authenticated)
			Social.ReportScore(score, GPGSIds.leaderboard_best_score, (bool success) => {  });
	}
	//Simple function that show ups a leaderboards
	public void ShowLeaderboards()
	{
		if (Social.localUser.authenticated)
			Social.ShowLeaderboardUI();
		else
			SignIn();
	}
	#endregion
}
