using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManagerScript : MonoBehaviour {

	#region variables
	[SerializeField] private GameObject player;
	[SerializeField] private float deadLineStartSpeedValue;
	[SerializeField] private float deadLineIncrementValue;
	[SerializeField] private float deadLinePointsToIncrement;
	[SerializeField] private float deadLineMaxSpeedValue;
	public float deadLineSpeed = 0f;
	[SerializeField] private GameObject[] easyPatterns, hardPatterns, easyMoneyPatterns, hardMoneyPatterns;
	[SerializeField] private int hardPatternsStartLevel;
	[SerializeField] private int moneyPatternsFrequency;
	public float patternsSpawnDistance;
	[SerializeField] private GameObject[] boosts;
	[SerializeField] private GameObject menuManager, audioManager;
	public GameObject deadLinePerfab, recordLinePerfab;
	private GameObject deadLine, recordLine;
	public GameObject wallPerfab, backgroundPerfab;
	private GameObject[] wallsList, backgroundsList;
	private GameObject[] patternsList;
	public float distanceTolerance;
	public float wallsSpawnDistance;
	public float boostChance;
	private float patternsSpawnCount = 0;
	private float wallsSpawnCount = 0;
	public int minBoostSpace;
	public int currentBoostSpace = 0;
	private int lastPattern, lastMoneyPattern;
	public int money;
	[HideInInspector] public bool isGameStarted = false;
	public int score;
	#endregion

	private void Start ()
	{
		//Declaring how many objects can contian walls array and backgrounds array
		backgroundsList = new GameObject[3];
		wallsList = new GameObject[6];
		//Declaring how many objects can constain patterns array
		patternsList = new GameObject[5];
		//Loading game
		LoadGame();
		//Setting values
		deadLineSpeed = deadLineStartSpeedValue;
	}
	private void Update ()
	{
		if (isGameStarted)
		{
			//If player is too far from deadLine move it
			if ((player.transform.position.y - deadLine.transform.position.y) > distanceTolerance)
			{
				deadLine.transform.position = new Vector3(0, player.transform.position.y - distanceTolerance, 0);
			}
			//Does needed to spawn next walls ?
			if (wallsSpawnCount * wallsSpawnDistance <= player.transform.position.y + 12f)
			{
				SpawnWalls();
			}
			//Does needed to spawn next pattern ?
			if (patternsSpawnCount * patternsSpawnDistance <= player.transform.position.y + patternsSpawnDistance)
			{
				SpawnPattern();
				score++;
				menuManager.GetComponent<MenuManagerScript>().UpdateScore(score);
			}
			//DeadLine section
			if (deadLineSpeed < deadLineStartSpeedValue + deadLineIncrementValue * Mathf.FloorToInt(patternsSpawnCount / deadLinePointsToIncrement) && deadLineSpeed <= deadLineMaxSpeedValue)
			{
				deadLineSpeed += deadLineIncrementValue;
				if (deadLineSpeed > deadLineMaxSpeedValue)
					deadLineSpeed = deadLineMaxSpeedValue;
			}
			//Moving up deadLine
			deadLine.transform.position = new Vector3(0, deadLine.transform.position.y + (deadLineSpeed * Time.deltaTime), 0);
		}
	}
	//Setuping game for new play
	public void PrepareGame ()
	{
		//Reseting all values to default
		patternsSpawnCount = 2;
		wallsSpawnCount = 0;
		currentBoostSpace = 0;
		score = 0;
		deadLineSpeed = deadLineStartSpeedValue;
		menuManager.GetComponent<MenuManagerScript>().isAdWatched = false;
		//Spawning walls
		SpawnWalls();
		//Spawnning deadLine
		if (!deadLine)
			deadLine = Instantiate(deadLinePerfab, new Vector3(0, -6f, 0), Quaternion.identity);
		//Spawning record line
		if (!recordLine)
		{
			PlayerSaveData psd = SaveManager.LoadData();
			if (psd.bestScore != 0)
				recordLine = Instantiate(recordLinePerfab, new Vector3(0, (psd.bestScore + 1) * patternsSpawnDistance, 0), Quaternion.identity);
		}
		//In game text updating
		menuManager.GetComponent<MenuManagerScript>().UpdateMoneyInGame(money);
		menuManager.GetComponent<MenuManagerScript>().UpdateScore(0);
		menuManager.GetComponent<MenuManagerScript>().ChangeScoreColor(true);
		//Changing player main color
		player.GetComponent<PlayerScript>().SetPlayerColor(menuManager.GetComponent<CharactersDataScript>().characterColor[menuManager.GetComponent<CharactersDataScript>().choosedCharacterID]);
	}
	//Reseting game after new game
	public void EndGame ()
	{
		//Teleporting player to starting position
		player.transform.position = new Vector3(0,0,0);
		player.transform.eulerAngles = new Vector3(0,0,0);
		//Preparing main menu to open
		PlayerSaveData psd = SaveManager.LoadData();
		menuManager.GetComponent<MenuManagerScript>().UpdateBestScore(psd.bestScore);
		menuManager.GetComponent<MenuManagerScript>().UpdateMoneyInMenu(psd.money, psd.specialMoney);
	}
	//Spawning new walls on top
	private void SpawnWalls ()
	{
		//WALLS
		//Incrementing count
		wallsSpawnCount++;
		//Left
		wallsList[4] = Instantiate(wallPerfab, new Vector3(-2.75f, (wallsSpawnCount - 1) * wallsSpawnDistance, 0), Quaternion.identity);
		//Right
		wallsList[5] = Instantiate(wallPerfab, new Vector3(2.75f, (wallsSpawnCount - 1) * wallsSpawnDistance, 0), Quaternion.identity);
		//Deleting old walls
		Destroy(wallsList[0]);
		Destroy(wallsList[1]);
		//Changing indexes of new walls
		wallsList[0] = wallsList[2];
		wallsList[1] = wallsList[3];
		wallsList[2] = wallsList[4];
		wallsList[3] = wallsList[5];
		//BACKGROUNDS
		backgroundsList[2] = Instantiate(backgroundPerfab, new Vector3(0, (wallsSpawnCount - 1) * wallsSpawnDistance, 1f), Quaternion.identity);
		//Deleting old backgrounds
		Destroy(backgroundsList[0]);
		//Changing indexes of new walls
		backgroundsList[0] = backgroundsList[1];
		backgroundsList[1] = backgroundsList[2];
	}
	//Spawning patterns
	private void SpawnPattern()
	{
		//Incrementing patternSpawnCount variable
		patternsSpawnCount++;
		//Destroying old one pattern from map
		Destroy(patternsList[(patternsList.Length - 1)]);
		//Changing patterns indexes to one up
		for (int i = (patternsList.Length - 1); i > 0; i--)
		{
			patternsList[i] = patternsList[i - 1];
		}
		//Rolling a boost chance
		int rand2 = UnityEngine.Random.Range(0, 100);
		//If everything seems to be okay, spawn boost
		if (rand2 < boostChance && currentBoostSpace >= minBoostSpace)
		{
			//Rolling a new number of boost to take
			int rand = UnityEngine.Random.Range(0, boosts.Length);
			//Spawning
			lastPattern = -1;
			patternsList[0] = Instantiate(boosts[rand], new Vector3(0, (patternsSpawnCount - 1) * patternsSpawnDistance, 0), Quaternion.identity);
			//Reseting currentBoostSpace
			currentBoostSpace = 0;
		}
		//If not, spawn pattern
		else
		{
			//There the easy and hard patterns starts to be spawned
			if (patternsSpawnCount >= hardPatternsStartLevel)
			{
				//Checks for money pattern to be spawned
				if ((patternsSpawnCount - 5) % moneyPatternsFrequency == 0)
				{
					//Rolling a new number of pattern to take
					int rand = UnityEngine.Random.Range(0, easyMoneyPatterns.Length + hardMoneyPatterns.Length);
					//If has the same id like old one rolling a new one
					while (rand == lastMoneyPattern)
					{
						rand = UnityEngine.Random.Range(0, easyMoneyPatterns.Length + hardMoneyPatterns.Length);
					}
					//Spawning
					lastMoneyPattern = rand;
					if (rand < easyMoneyPatterns.Length)
						patternsList[0] = Instantiate(easyMoneyPatterns[rand], new Vector3(0, (patternsSpawnCount - 1) * patternsSpawnDistance, 0), Quaternion.identity);
					else
						patternsList[0] = Instantiate(hardMoneyPatterns[rand - easyMoneyPatterns.Length], new Vector3(0, (patternsSpawnCount - 1) * patternsSpawnDistance, 0), Quaternion.identity);
					//Incrementing currentBoostSpace
					currentBoostSpace++;
				}
				else
				{
					//Rolling a new number of pattern to take
					int rand = UnityEngine.Random.Range(0, easyPatterns.Length + hardPatterns.Length);
					//If has the same id like old one rolling a new one
					while (rand == lastPattern)
					{
						rand = UnityEngine.Random.Range(0, easyPatterns.Length + hardPatterns.Length);
					}
					//Spawning
					lastPattern = rand;
					if (rand < easyPatterns.Length)
						patternsList[0] = Instantiate(easyPatterns[rand], new Vector3(0, (patternsSpawnCount - 1) * patternsSpawnDistance, 0), Quaternion.identity);
					else
						patternsList[0] = Instantiate(hardPatterns[rand - easyPatterns.Length], new Vector3(0, (patternsSpawnCount - 1) * patternsSpawnDistance, 0), Quaternion.identity);
					//Incrementing currentBoostSpace
					currentBoostSpace++;
				}
			}
			//There the easy patterns starts to be spawned
			else
			{
				if ((patternsSpawnCount - 5) % moneyPatternsFrequency == 0)
				{
					//Rolling a new number of pattern to take
					int rand = UnityEngine.Random.Range(0, easyMoneyPatterns.Length);
					//If has the same id like old one rolling a new one
					while (rand == lastMoneyPattern)
					{
						rand = UnityEngine.Random.Range(0, easyMoneyPatterns.Length);
					}
					//Spawning
					lastMoneyPattern = rand;
					patternsList[0] = Instantiate(easyMoneyPatterns[rand], new Vector3(0, (patternsSpawnCount - 1) * patternsSpawnDistance, 0), Quaternion.identity);
					//Incrementing currentBoostSpace
					currentBoostSpace++;
				}
				else
				{
					//Rolling a new number of pattern to take
					int rand = UnityEngine.Random.Range(0, easyPatterns.Length);
					//If has the same id like old one rolling a new one
					while (rand == lastPattern)
					{
						rand = UnityEngine.Random.Range(0, easyPatterns.Length);
					}
					//Spawning
					lastPattern = rand;
					patternsList[0] = Instantiate(easyPatterns[rand], new Vector3(0, (patternsSpawnCount - 1) * patternsSpawnDistance, 0), Quaternion.identity);
					//Incrementing currentBoostSpace
					currentBoostSpace++;
				}
			}
		}
	}
	//Clearing map from patterns and walls
	public void ClearMap ()
	{
		//Destroy all backgrounds
		foreach (GameObject obj in backgroundsList)
		{
			Destroy(obj);
		}
		//Destroy all walls
		foreach (GameObject obj in wallsList)
		{
			Destroy(obj);
		}
		//Destroy all patterns
		foreach (GameObject obj in patternsList)
		{
			Destroy(obj);
		}
		//Destroying deadLine
		Destroy(deadLine);
		//Destroy best score line
		Destroy(recordLine);
	}
	//Adding money to sum
	public void AddMoney (bool is3)
	{
		if (UnityEngine.Random.Range(0,2) == 0)
			audioManager.GetComponent<AudioManagerScript>().CollectCoin.Play();
		else
			audioManager.GetComponent<AudioManagerScript>().CollectCoin2.Play();

		if (is3 == true)
			money += 2;
		else
			money++;
		menuManager.GetComponent<MenuManagerScript>().UpdateMoneyInGame(money);
	}
	//Saving data
	public void SaveGame ()
	{
		//Saving score to leaderboard
		menuManager.GetComponent<GPSManager>().PublishScoreToLeaderboard(score);
		//Loading data and this same creating a save object
		PlayerSaveData psd = SaveManager.LoadData();
		//Checking is new record set
		if (score > psd.bestScore)
		{
			//Saving score to save file
			psd.bestScore = score;
		}
		//Updating money value
		psd.money = money;
		psd.gamesPlayed++;
		//Saving data
		SaveManager.SaveData(psd);
		CheckGamesPlayed(psd.gamesPlayed);
		if (psd.money >= 100)
			GPSManager.UnlockAchievement(GPGSIds.achievement_have_at_least_100_bucks);

		try
		{
			this.GetComponent<GPSManager>().OpenSave(true);
		}
		catch(Exception e)
		{

		}
	}
	//Load all data
	public void LoadGame ()
	{
		//Loading game to variable
		PlayerSaveData psd = SaveManager.LoadData();
		//Setting all variables in RAM to new values
		money = psd.money;
		menuManager.GetComponent<MenuManagerScript>().UpdateBestScore(psd.bestScore);
		menuManager.GetComponent<MenuManagerScript>().UpdateMoneyInMenu(psd.money, psd.specialMoney);
		menuManager.GetComponent<CharactersDataScript>().choosedCharacterID = psd.choosedCharacter;
		menuManager.GetComponent<CharactersDataScript>().ownedCharacters = psd.ownedCharacters;
		menuManager.GetComponent<MenuManagerScript>().freePrizeDate = psd.freePrizeCollectedDate;
		audioManager.GetComponent<AudioManagerScript>().ChangeVolume(true, psd.isMusicMuted);
		audioManager.GetComponent<AudioManagerScript>().ChangeVolume(false, psd.isSoundMuted);
		//Setting player sprite
		GameObject go = GameObject.Find("Player");
		go.GetComponent<SpriteRenderer>().sprite = menuManager.GetComponent<CharactersDataScript>().characterSprite[psd.choosedCharacter];

	}
	//Adding and saving money after purchase
	public void AddPurchasedMoney (int ammount, bool isSpecial)
	{
		audioManager.GetComponent<AudioManagerScript>().IAPbuy.Play();

		PlayerSaveData psd = SaveManager.LoadData();

		if (isSpecial)
		{
			psd.specialMoney += ammount;
			if (psd.specialMoney >= 100)
				GPSManager.UnlockAchievement(GPGSIds.achievement_have_at_least_100_crystals);
		}
		else
		{
			psd.money += ammount;
			if (psd.specialMoney >= 100)
				GPSManager.UnlockAchievement(GPGSIds.achievement_have_at_least_100_bucks);
		}

		//Saving data
		SaveManager.SaveData(psd);

		menuManager.GetComponent<MenuManagerScript>().UpdateMoneyInMenu(psd.money, psd.specialMoney);

		try
		{
			this.GetComponent<GPSManager>().OpenSave(true);
		}
		catch (Exception e)
		{

		}
	}
	//Creatning first save file
	public PlayerSaveData FirstSave ()
	{
		PlayerSaveData psd = new PlayerSaveData(0, 0, 0, menuManager.GetComponent<CharactersDataScript>().ownedCharacters, 0, DateTime.Now.AddMinutes(-menuManager.GetComponent<MenuManagerScript>().freePrizeTimeDelayInMinutes), false, false, false, 0, false, false);
		return psd;
	}
	//Checks achievements
	public void CheckGamesPlayed(int gamesPlayed)
	{
		if (gamesPlayed >= 100)
		{
			GPSManager.UnlockAchievement(GPGSIds.achievement_play_100_games);
		}
		else if (gamesPlayed >= 10)
		{
			GPSManager.UnlockAchievement(GPGSIds.achievement_play_10_games);
		}
		if (gamesPlayed >= 1)
		{
			GPSManager.UnlockAchievement(GPGSIds.achievement_play_1_game);
		}

	}
}
