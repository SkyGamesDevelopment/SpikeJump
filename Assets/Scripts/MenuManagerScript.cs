using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MenuManagerScript : MonoBehaviour
{

	#region variables
	private const string AdsCommand = "DontLikeAds";
	private const string godCommand = "CheekiBreeki";
	private const string charactersCommand = "LetMeTakeALookAtThis";
	private const string crystals100Command = "GiveMeMy100Crystals";
	private const string bucks100Command = "GiveMeMy100Bucks";
	public Animator mainMenuAnimator, gameHUDAnimator, charactersShopAnimator, playerAnimator, infoPanelAnimator, adPanelAnimator, microtransactionsAnimator, settingsAnimator, backgroundAnimator, tutorialAnimator, tutorialObjectsAnimator;
	public Animator freePrizeMainMenuAnimator;
	public GameObject characterPanel, characterTrash;
	[SerializeField] private GameObject scoreText, inGameMoneyText, mainMenuMoneyText, mainMenuSpecialMoneyText, charactersPanelMoneyText, charactersPanelSpecialMoneyText, bestScoreText, infoPanelText, microtransactionsMoneyText, microtransactionsSpecialMoneyText, freePrizeTimeLeftText;
	[SerializeField] private GameObject price1Text, price2Text, price3Text, price4Text, price5Text, price6Text, price7Text;
	[SerializeField] private GameObject characterButton, microtransactionsMoneyAnimation, microtransactionsSpecialMoneyAnimation;
	[SerializeField] private GameObject gameManager, audioManager;
	[SerializeField] private Sprite musicImage, musicImageMuted, soundImage, soundImageMuted;
	[SerializeField] private GameObject soundButton, musicButton;
	[SerializeField] private GameObject safeObject;
	[SerializeField] private Sprite safeOpen, safeClose;
	[SerializeField] private GameObject bestscoreParticle, camera;
	public float freePrizeTimeDelayInMinutes;
	public DateTime freePrizeDate;
	private GameObject[] buttons;
	public bool canInteract = false;
	[HideInInspector] public bool isAdWatched, isMenuFocused;
	private float timer;
	public bool isPaused = false;
	private TouchScreenKeyboard keyboard;
	private bool aproved = false;
	#endregion

	private void Start()
	{
		StartCoroutine("OpenMainMenu", 0);
		buttons = new GameObject[this.gameObject.GetComponent<CharactersDataScript>().characterSprite.Length];
		this.GetComponent<GPSManager>().SignIn();
	}

	private void Update()
	{
		if (timer <= 0)
		{
			CheckFreePrize();
			timer = 5f;
		}
		timer -= Time.deltaTime;
		//Keyboard manager
		if (TouchScreenKeyboard.visible == false && keyboard != null && keyboard.done)
		{
			switch (keyboard.text)
			{
				case godCommand:

					PlayerSaveData psd = SaveManager.LoadData();
					psd.money = 10000;
					psd.specialMoney = 10000;
					for (int i = 0; i < psd.ownedCharacters.Length; i++)
					{
						psd.ownedCharacters[i] = true;
					}
					SaveManager.SaveData(psd);
					gameManager.GetComponent<GameManagerScript>().money = psd.money;
					this.GetComponent<CharactersDataScript>().ownedCharacters = psd.ownedCharacters;
					Handheld.Vibrate();
					UpdateMoneyInMenu(psd.money, psd.specialMoney);
					break;

				case AdsCommand:

					PlayerSaveData psd2 = SaveManager.LoadData();
					psd2.developerMode = true;
					SaveManager.SaveData(psd2);
					Handheld.Vibrate();
					break;

				case charactersCommand:

					for (int i = 0; i < this.GetComponent<CharactersDataScript>().ownedCharacters.Length; i++)
					{
						this.GetComponent<CharactersDataScript>().ownedCharacters[i] = true;
					}
					Handheld.Vibrate();
					break;

				case crystals100Command:

					PlayerSaveData psd3 = SaveManager.LoadData();
					psd3.specialMoney += 100;
					SaveManager.SaveData(psd3);
					Handheld.Vibrate();
					UpdateMoneyInMenu(psd3.money, psd3.specialMoney);
					break;

				case bucks100Command:

					PlayerSaveData psd4 = SaveManager.LoadData();
					psd4.money += 100;
					SaveManager.SaveData(psd4);
					gameManager.GetComponent<GameManagerScript>().money = psd4.money;
					Handheld.Vibrate();
					UpdateMoneyInMenu(psd4.money, psd4.specialMoney);
					break;
				default:
					StartCoroutine(ToggleInfoPanel("You put wrong code, or you are KGB spy... ", true));
					break;
			}

			keyboard = null;
		}
		//Checking for back button
		if (gameManager.GetComponent<GameManagerScript>().isGameStarted && !isPaused)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				PauseButtonClicked();
			}
		}
	}
	//Opens main menu first time (0) or more (1) or from characters panel (2) or from microtransactions (3)
	public IEnumerator OpenMainMenu(int how)
	{
		if (how == 0)
		{
			//Opening main menu
			yield return new WaitForSeconds(0.1f);
			mainMenuAnimator.SetBool("isOpened", true);
			yield return new WaitForSeconds(0.25f);
			backgroundAnimator.SetFloat("movingFloat", 1f);
			PlayerSaveData psd = SaveManager.LoadData();
			if (!psd.playedBefore)
				StartCoroutine("ToggleTutorialPanel", true);
			else
			{
				canInteract = true;
				isMenuFocused = true;
			}
			CheckFreePrize();
		}
		else if (how == 1)
		{
			//Closing HUD
			gameHUDAnimator.SetBool("isOpened", false);
			//Opening menu
			StartCoroutine(camera.GetComponent<CameraScript>().ZoomIn());
			yield return new WaitForSeconds(1.25f);
			mainMenuAnimator.SetBool("isOpened", true);
			yield return new WaitForSeconds(1f);
			StartCoroutine("SmoothBackground", 1);
			yield return new WaitForSeconds(0.4f);
			canInteract = true;
			isMenuFocused = true;
			CheckFreePrize();
		}
		else if (how == 2)
		{
			//Closing characters menu
			charactersShopAnimator.SetBool("isOpened", false);
			//Opening menu
			mainMenuAnimator.SetTrigger("toggleCharacters");
			//Showing up player
			yield return new WaitForSeconds(0.2f);
			playerAnimator.SetBool("isVisible", true);
			yield return new WaitForSeconds(0.15f);
			canInteract = true;
			isMenuFocused = true;
			CheckFreePrize();
		}
		else if (how == 3)
		{
			//Closing microtransactions
			microtransactionsAnimator.SetBool("isOpened", false);
			//Opening menu
			mainMenuAnimator.SetTrigger("toggleMicrotransactions");
			//Showing up player
			yield return new WaitForSeconds(0.2f);
			playerAnimator.SetBool("isVisible", true);
			yield return new WaitForSeconds(0.15f);
			canInteract = true;
			isMenuFocused = true;
			CheckFreePrize();
		}
	}
	//Opens in game HUD and closing main menu
	public IEnumerator OpenInGameHUD()
	{
		StartCoroutine("SmoothBackground", 0);
		canInteract = false;
		mainMenuAnimator.SetBool("isOpened", false);
		//Waiting for menu close
		yield return new WaitForSeconds(0.3f);
		gameHUDAnimator.SetBool("isOpened", true);
		freePrizeMainMenuAnimator.SetBool("isSpinning", false);
		StartCoroutine(camera.GetComponent<CameraScript>().ZoomOut());
	}
	//Opens characters shop and load data
	public IEnumerator OpenCharactersShop()
	{
		//Fix for updates with new characters
		if (this.GetComponent<CharactersDataScript>().ownedCharacters.Length != this.GetComponent<CharactersDataScript>().characterSprite.Length)
		{
			if (this.GetComponent<CharactersDataScript>().ownedCharacters.Length > this.GetComponent<CharactersDataScript>().characterSprite.Length)
			{
				PlayerSaveData psd = SaveManager.LoadData();
				bool[] x = new bool[this.GetComponent<CharactersDataScript>().characterSprite.Length];
				for (int i = 0; i <= this.GetComponent<CharactersDataScript>().characterSprite.Length - 1; i++)
				{
					x[i] = psd.ownedCharacters[i];
				}
				psd.ownedCharacters = x;
				SaveManager.SaveData(psd);
				this.GetComponent<CharactersDataScript>().ownedCharacters = psd.ownedCharacters;
			}
			else if (this.GetComponent<CharactersDataScript>().ownedCharacters.Length < this.GetComponent<CharactersDataScript>().characterSprite.Length)
			{
				PlayerSaveData psd = SaveManager.LoadData();
				bool[] x = new bool[this.GetComponent<CharactersDataScript>().characterSprite.Length];
				int y = 0;
				for (int i = 0; i <= this.GetComponent<CharactersDataScript>().ownedCharacters.Length - 1; i++)
				{
					x[i] = psd.ownedCharacters[i];
					y++;
				}
				for (int i = y; i <= this.GetComponent<CharactersDataScript>().characterSprite.Length - 1; i++)
				{
					x[i] = false;
				}
				psd.ownedCharacters = x;
				SaveManager.SaveData(psd);
				this.GetComponent<CharactersDataScript>().ownedCharacters = psd.ownedCharacters;
			}
		}
		//Removing all childrens in characters panel to make sure that old one characters buttons is deleted
		foreach (Transform t in characterPanel.transform)
		{
			Destroy(t.gameObject);
		}
		//Initializing all buttons and setting all variables in it
		for (int i = 0; i <= buttons.Length - 1; i++)
		{
			buttons[i] = Instantiate(characterButton, characterTrash.transform);
			buttons[i].GetComponent<CharacterShopButtonScript>().SetID(i);
			buttons[i].GetComponent<CharacterShopButtonScript>().SetSprite(this.GetComponent<CharactersDataScript>().characterSprite[i]);
			buttons[i].GetComponent<CharacterShopButtonScript>().SetState(this.GetComponent<CharactersDataScript>().ownedCharacters[i], this.GetComponent<CharactersDataScript>().isSpecialCharacter[i]);
			buttons[i].GetComponent<CharacterShopButtonScript>().SetCost(this.GetComponent<CharactersDataScript>().characterCost[i]);
		}
		//Setting up owned special characters
		for (int i = 0; i <= buttons.Length - 1; i++)
		{
			int min = 100000;
			int minID = -1;
			for (int j = 0; j <= buttons.Length - 1; j++)
			{
				if (buttons[j].GetComponent<CharacterShopButtonScript>().isOwned && buttons[j].GetComponent<CharacterShopButtonScript>().isSpecial && this.GetComponent<CharactersDataScript>().characterCost[buttons[j].GetComponent<CharacterShopButtonScript>().characterID] < min && !buttons[j].GetComponent<CharacterShopButtonScript>().isSetUp)
				{
					min = this.GetComponent<CharactersDataScript>().characterCost[buttons[j].GetComponent<CharacterShopButtonScript>().characterID];
					minID = j;
				}
			}
			if (minID == -1)
				break;
			else
			{
				buttons[minID].transform.SetParent(characterPanel.transform);
				buttons[minID].GetComponent<CharacterShopButtonScript>().isSetUp = true;
			}
		}
		//Setting up owned non-special characters
		for (int i = 0; i <= buttons.Length - 1; i++)
		{
			int min = 100000;
			int minID = -1;
			for (int j = 0; j <= buttons.Length - 1; j++)
			{
				if (buttons[j].GetComponent<CharacterShopButtonScript>().isOwned && !buttons[j].GetComponent<CharacterShopButtonScript>().isSpecial && this.GetComponent<CharactersDataScript>().characterCost[buttons[j].GetComponent<CharacterShopButtonScript>().characterID] < min && !buttons[j].GetComponent<CharacterShopButtonScript>().isSetUp)
				{
					min = this.GetComponent<CharactersDataScript>().characterCost[buttons[j].GetComponent<CharacterShopButtonScript>().characterID];
					minID = j;
				}
			}
			if (minID == -1)
				break;
			else
			{
				buttons[minID].transform.SetParent(characterPanel.transform);
				buttons[minID].GetComponent<CharacterShopButtonScript>().isSetUp = true;
			}
		}
		//Setting up non-owned special characters
		for (int i = 0; i <= buttons.Length - 1; i++)
		{
			int min = 100000;
			int minID = -1;
			for (int j = 0; j <= buttons.Length - 1; j++)
			{
				if (!buttons[j].GetComponent<CharacterShopButtonScript>().isOwned && buttons[j].GetComponent<CharacterShopButtonScript>().isSpecial && this.GetComponent<CharactersDataScript>().characterCost[buttons[j].GetComponent<CharacterShopButtonScript>().characterID] < min && !buttons[j].GetComponent<CharacterShopButtonScript>().isSetUp && this.GetComponent<CharactersDataScript>().characterCost[buttons[j].GetComponent<CharacterShopButtonScript>().characterID] != -1)
				{
					min = this.GetComponent<CharactersDataScript>().characterCost[buttons[j].GetComponent<CharacterShopButtonScript>().characterID];
					minID = j;
				}
			}
			if (minID == -1)
				break;
			else
			{
				buttons[minID].transform.SetParent(characterPanel.transform);
				buttons[minID].GetComponent<CharacterShopButtonScript>().isSetUp = true;
			}
		}
		//Setting up non-owned non-special characters
		for (int i = 0; i <= buttons.Length - 1; i++)
		{
			int min = 100000;
			int minID = -1;
			for (int j = 0; j <= buttons.Length - 1; j++)
			{
				if (!buttons[j].GetComponent<CharacterShopButtonScript>().isOwned && !buttons[j].GetComponent<CharacterShopButtonScript>().isSpecial && this.GetComponent<CharactersDataScript>().characterCost[buttons[j].GetComponent<CharacterShopButtonScript>().characterID] < min && !buttons[j].GetComponent<CharacterShopButtonScript>().isSetUp)
				{
					min = this.GetComponent<CharactersDataScript>().characterCost[buttons[j].GetComponent<CharacterShopButtonScript>().characterID];
					minID = j;
				}
			}
			if (minID == -1)
				break;
			else
			{
				buttons[minID].transform.SetParent(characterPanel.transform);
				buttons[minID].GetComponent<CharacterShopButtonScript>().isSetUp = true;
			}
		}
		//Changing panel size and position
		characterPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(720, (175 + 50) * Mathf.Ceil((float)(buttons.Length - 1) / 2) - 50);
		characterPanel.GetComponent<RectTransform>().localPosition = new Vector2(0, -100000);
		//Closing main menu and openin characters menu
		playerAnimator.SetBool("isVisible", false);
		yield return new WaitForSeconds(0.15f);
		mainMenuAnimator.SetTrigger("toggleCharacters");
		charactersShopAnimator.SetBool("isOpened", true);
		yield return new WaitForSeconds(0.2f);
		canInteract = true;
		freePrizeMainMenuAnimator.SetBool("isSpinning", false);
	}
	//Opens microtransactions shop
	public IEnumerator OpenMicrotransactions()
	{
		this.GetComponent<IAPManager>().InitializePurchasing();
		//Updating free prize time left text
		CheckFreePrize();
		microtransactionsMoneyText.GetComponent<TextMeshProUGUI>().text = gameManager.GetComponent<GameManagerScript>().money.ToString();
		//Setting products costs
		price1Text.gameObject.GetComponent<TextMeshProUGUI>().text = this.GetComponent<IAPManager>().ProductPrice(1);
		price2Text.gameObject.GetComponent<TextMeshProUGUI>().text = this.GetComponent<IAPManager>().ProductPrice(2);
		price3Text.gameObject.GetComponent<TextMeshProUGUI>().text = this.GetComponent<IAPManager>().ProductPrice(3);
		price4Text.gameObject.GetComponent<TextMeshProUGUI>().text = this.GetComponent<IAPManager>().ProductPrice(4);
		price5Text.gameObject.GetComponent<TextMeshProUGUI>().text = this.GetComponent<IAPManager>().ProductPrice(5);
		price6Text.gameObject.GetComponent<TextMeshProUGUI>().text = this.GetComponent<IAPManager>().ProductPrice(6);
		price7Text.gameObject.GetComponent<TextMeshProUGUI>().text = this.GetComponent<IAPManager>().ProductPrice(7);
		//Closing main menu and opening microtransactions shop
		if (price7Text.gameObject.GetComponent<TextMeshProUGUI>().text != null)
		{
			playerAnimator.SetBool("isVisible", false);
			yield return new WaitForSeconds(0.15f);
			mainMenuAnimator.SetTrigger("toggleMicrotransactions");
			microtransactionsAnimator.SetBool("isOpened", true);
			yield return new WaitForSeconds(0.2f);
			canInteract = true;
			freePrizeMainMenuAnimator.SetBool("isSpinning", false);
		}
		else
		{
			GameObject go = GameObject.Find("MenuManager");
			StartCoroutine(go.GetComponent<MenuManagerScript>().ToggleInfoPanel("Error, check your internet connection or wait for a while :<", true));
			canInteract = true;
		}
	}
	//Toggles info panel
	public IEnumerator ToggleInfoPanel(string x, bool open)
	{
		if (open)
		{
			isMenuFocused = false;
			canInteract = false;
			infoPanelText.GetComponent<TextMeshProUGUI>().text = x;
			infoPanelAnimator.SetBool("isOpened", true);
			yield return new WaitForSeconds(0.25f);
			canInteract = true;
		}
		else
		{
			canInteract = false;
			infoPanelAnimator.SetBool("isOpened", false);
			yield return new WaitForSeconds(0.25f);
			canInteract = true;
			isMenuFocused = true;
		}
	}
	//Toggles ad panel
	public IEnumerator ToggleAdPanel(bool open)
	{
		if (open)
		{
			this.GetComponent<AdManager>().RequestAd();
			isMenuFocused = false;
			canInteract = false;
			adPanelAnimator.SetBool("isOpened", true);
			yield return new WaitForSeconds(0.25f);
			canInteract = true;
		}
		else
		{
			canInteract = false;
			adPanelAnimator.SetBool("isOpened", false);
			yield return new WaitForSeconds(0.25f);
		}
	}
	//Toggles settings panel
	public IEnumerator ToggleSettingsPanel(bool open)
	{
		if (open)
		{
			aproved = false;
			isMenuFocused = false;
			canInteract = false;
			PlayerSaveData psd = SaveManager.LoadData();

			if (psd.isMusicMuted)
				musicButton.GetComponent<Image>().sprite = musicImageMuted;
			else
				musicButton.GetComponent<Image>().sprite = musicImage;
			if (psd.isSoundMuted)
				soundButton.GetComponent<Image>().sprite = soundImageMuted;
			else
				soundButton.GetComponent<Image>().sprite = soundImage;

			settingsAnimator.SetBool("isOpened", true);
			yield return new WaitForSeconds(0.4f);
			canInteract = true;
		}
		else
		{
			canInteract = false;
			settingsAnimator.SetBool("isOpened", false);
			yield return new WaitForSeconds(0.4f);
			canInteract = true;
			isMenuFocused = true;
		}
	}
	//Toggles tutorial panel
	public IEnumerator ToggleTutorialPanel(bool open)
	{
		canInteract = false;
		if (open)
		{
			isMenuFocused = false;
			tutorialAnimator.SetBool("isOpened", true);
			yield return new WaitForSeconds(0.25f);
			canInteract = true;
			tutorialObjectsAnimator.SetBool("isOpened", true);
		}
		else
		{
			tutorialAnimator.SetBool("isOpened", false);
			yield return new WaitForSeconds(0.25f);
			tutorialObjectsAnimator.SetBool("isOpened", false);
			canInteract = true;
			isMenuFocused = true;

			PlayerSaveData psd = SaveManager.LoadData();
			if(!psd.playedBefore)
			{
				psd.playedBefore = true;
				SaveManager.SaveData(psd);
				StartCoroutine(ToggleInfoPanel("If you want to download savegame from cloud please go to settings and click GPS button!", true));
			}
		}
	}
	//Adding nice slide fade from moving background to centre position
	public IEnumerator SmoothBackground(int whatToDo)
	{
		if (whatToDo == 0)
		{
			for (int i = 0; i < 40; i++)
			{
				yield return new WaitForSeconds(0.005f);
				backgroundAnimator.SetFloat("movingFloat", 1f - (float)(i + 1) / 20);
			}
		}
		else
		{
			for (int i = 0; i < 40; i++)
			{
				yield return new WaitForSeconds(0.015f);
				backgroundAnimator.SetFloat("movingFloat", -1f + (float)(i + 1) / 20);
			}
		}
	}
	//Updates score value in HUD
	public void UpdateScore(int x)
	{
		scoreText.GetComponent<TextMeshProUGUI>().text = x.ToString();
	}
	//Updates money
	public void UpdateMoneyInGame(int x)
	{
		inGameMoneyText.GetComponent<TextMeshProUGUI>().text = x.ToString();
	}
	//Updates money
	public void UpdateMoneyInMenu(int normal, int special)
	{
		mainMenuMoneyText.GetComponent<TextMeshProUGUI>().text = normal.ToString();
		charactersPanelMoneyText.GetComponent<TextMeshProUGUI>().text = normal.ToString();
		microtransactionsMoneyText.GetComponent<TextMeshProUGUI>().text = normal.ToString();

		mainMenuSpecialMoneyText.GetComponent<TextMeshProUGUI>().text = special.ToString();
		charactersPanelSpecialMoneyText.GetComponent<TextMeshProUGUI>().text = special.ToString();
		microtransactionsSpecialMoneyText.GetComponent<TextMeshProUGUI>().text = special.ToString();
	}
	//Update best score
	public void UpdateBestScore(int x)
	{
		bestScoreText.GetComponent<TextMeshProUGUI>().text = x.ToString();
	}
	//Changing in game score text to white/gold
	public void ChangeScoreColor(bool white)
	{
		if (white)
			scoreText.GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 120);
		else
		{
			bestscoreParticle.SetActive(false);
			bestscoreParticle.SetActive(true);
			scoreText.GetComponent<TextMeshProUGUI>().color = new Color32(255, 225, 0, 120);
		}
	}
	//Deciding what to do after logging in
	public void LoginCallback(bool success)
	{
		if (success)
			StartCoroutine(ToggleInfoPanel("Succesfully logged in! :>", true));
		else
			StartCoroutine(ToggleInfoPanel("Cant log in... :C", true));
	}
	//Button click sound
	private void ButtonClickSound()
	{
		audioManager.GetComponent<AudioManagerScript>().ButtonClick.Play();
	}

	#region Free prize
	//Returns is prize ready
	public bool IsPrizeReady()
	{
		//Checks is time to next free prize is overdone
		if (DateTime.Now >= freePrizeDate.AddMinutes(freePrizeTimeDelayInMinutes))
			return true;
		else
			return false;
	}
	//Sets timer of free prize
	public void SetFreePrizeTimer(double timeInMinutes)
	{
		//Checks is prize is ready
		if (timeInMinutes == 0)
		{
			freePrizeTimeLeftText.GetComponent<TextMeshProUGUI>().text = "Get prize!";
		}
		else
		{
			//Changing minutes to hours and minutes
			int minutesLeft = (int)timeInMinutes + 1;
			int hoursLeft = 0;
			while (minutesLeft >= 60)
			{
				minutesLeft -= 60;
				hoursLeft++;
			}
			//Setting prize text to minutes and hours
			freePrizeTimeLeftText.GetComponent<TextMeshProUGUI>().text = hoursLeft + "h " + minutesLeft + "min";
		}
	}
	//Checking that is free prize is ready to get
	public void CheckFreePrize()
	{
		//If prize is ready set timer to "Ready" or something else
		if (IsPrizeReady())
		{
			safeObject.GetComponent<Image>().sprite = safeClose;
			SetFreePrizeTimer(0);
			freePrizeMainMenuAnimator.SetBool("isSpinning", true);
		}
		else
		{
			safeObject.GetComponent<Image>().sprite = safeOpen;
			TimeSpan delay = freePrizeDate.AddMinutes(freePrizeTimeDelayInMinutes) - DateTime.Now;
			SetFreePrizeTimer(delay.TotalMinutes);
			freePrizeMainMenuAnimator.SetBool("isSpinning", false);
		}
	}
	//Gets prize
	public void GetPrize()
	{
		ButtonClickSound();
		if (canInteract && IsPrizeReady())
		{
			audioManager.GetComponent<AudioManagerScript>().IAPbuy.Play();
			canInteract = false;
			microtransactionsAnimator.SetTrigger("getPrize");
			PlayerSaveData psd = SaveManager.LoadData();
			psd.freePrizeCollectedDate = DateTime.Now;

			microtransactionsSpecialMoneyAnimation.SetActive(false);
			microtransactionsSpecialMoneyAnimation.SetActive(true);
			microtransactionsMoneyAnimation.SetActive(false);
			microtransactionsMoneyAnimation.SetActive(true);

			psd.specialMoney += UnityEngine.Random.Range(1, 3);
			psd.money += UnityEngine.Random.Range(5, 11);

			freePrizeDate = DateTime.Now;
			SaveManager.SaveData(psd);
			UpdateMoneyInMenu(psd.money, psd.specialMoney);
			gameManager.GetComponent<GameManagerScript>().money = psd.money;
			UpdateMoneyInGame(psd.money);
			CheckFreePrize();
			canInteract = true;

			if (psd.money >= 100)
				GPSManager.UnlockAchievement(GPGSIds.achievement_have_at_least_100_bucks);
			if (psd.specialMoney >= 100)
				GPSManager.UnlockAchievement(GPGSIds.achievement_have_at_least_100_crystals);

			try
			{
				this.GetComponent<GPSManager>().OpenSave(true);
			}
			catch (Exception e)
			{

			}
		}
	}
	#endregion

	#region IAP panel
	//On menu button click
	public void IAPBackToMenu()
	{
		if (canInteract)
		{
			ButtonClickSound();
			canInteract = false;
			StartCoroutine("OpenMainMenu", 3);
		}
	}
	//Buying product of id...
	public void IAPBuyProduct(int id)
	{
		ButtonClickSound();
		this.GetComponent<IAPManager>().BuyProduct(id);
	}
	//Callback when you cant purchase item
	public void IAPCallbackError()
	{
		StartCoroutine(ToggleInfoPanel("You cant purchase this item :/", true));
	}
	//Callback when initialization is not finished or went wrong
	public void IAPInitializeError()
	{
		StartCoroutine(ToggleInfoPanel("Initialization eror, try again or restart App :O", true));
	}
	#endregion

	#region Characters shop buttons
	//When player pressed button to buy/select character
	public void CharactersButtonClicked(int id, bool owned, bool special)
	{
		if (canInteract)
		{
			ButtonClickSound();
			canInteract = false;
			int cost = this.GetComponent<CharactersDataScript>().characterCost[id];
			//Checking that is player having that character
			if (owned)
			{
				//Finding player
				GameObject player = GameObject.Find("Player");
				//Setting a new sprite
				player.GetComponent<SpriteRenderer>().sprite = this.GetComponent<CharactersDataScript>().characterSprite[id];
				//Saving new selected player id
				this.GetComponent<CharactersDataScript>().choosedCharacterID = id;
				PlayerSaveData psd = SaveManager.LoadData();
				psd.choosedCharacter = id;
				SaveManager.SaveData(psd);
				StartCoroutine("OpenMainMenu", 2);

				try
				{
					this.GetComponent<GPSManager>().OpenSave(true);
				}
				catch (Exception e)
				{

				}
			}
			else
			{
				if (special)
				{
					PlayerSaveData psd = SaveManager.LoadData();
					//Checking is player have enought money to buy character
					if (psd.specialMoney >= cost)
					{
						//Finding player
						GameObject player = GameObject.Find("Player");
						//Setting a new sprite
						player.GetComponent<SpriteRenderer>().sprite = this.GetComponent<CharactersDataScript>().characterSprite[id];
						//Saving new selected player id and money
						this.GetComponent<CharactersDataScript>().choosedCharacterID = id;
						this.GetComponent<CharactersDataScript>().ownedCharacters[id] = true;
						psd.specialMoney -= cost;
						psd.choosedCharacter = id;
						psd.ownedCharacters[id] = true;
						SaveManager.SaveData(psd);
						UpdateMoneyInMenu(psd.money, psd.specialMoney);
						StartCoroutine("OpenMainMenu", 2);
						GPSManager.UnlockAchievement(GPGSIds.achievement_get_first_special_character);

						try
						{
							this.GetComponent<GPSManager>().OpenSave(true);
						}
						catch (Exception e)
						{

						}
					}
					else
						canInteract = true;
				}
				else
				{
					PlayerSaveData psd = SaveManager.LoadData();
					//Checking is player have enought money to buy character
					if (psd.money >= cost)
					{
						//Finding player
						GameObject player = GameObject.Find("Player");
						//Setting a new sprite
						player.GetComponent<SpriteRenderer>().sprite = this.GetComponent<CharactersDataScript>().characterSprite[id];
						//Saving new selected player id and money
						this.GetComponent<CharactersDataScript>().choosedCharacterID = id;
						this.GetComponent<CharactersDataScript>().ownedCharacters[id] = true;
						psd.money -= cost;
						psd.choosedCharacter = id;
						psd.ownedCharacters[id] = true;

						int y = 0;
						foreach (bool x in psd.ownedCharacters)
						{
							if (x)
								y++;
						}
						if (y >= 10)
							GPSManager.UnlockAchievement(GPGSIds.achievement_get_10_characters);
						else if (y >= 2)
							GPSManager.UnlockAchievement(GPGSIds.achievement_get_first_character);

						SaveManager.SaveData(psd);
						gameManager.GetComponent<GameManagerScript>().money = psd.money;
						UpdateMoneyInMenu(psd.money, psd.specialMoney);
						StartCoroutine("OpenMainMenu", 2);

						try
						{
							this.GetComponent<GPSManager>().OpenSave(true);
						}
						catch (Exception e)
						{

						}
					}
					else
					{
						canInteract = true;
					}
				}
			}
		}
	}
	//Back to main menu button
	public void CharactersMenuButton()
	{
		if (canInteract)
		{
			ButtonClickSound();
			canInteract = false;
			StartCoroutine("OpenMainMenu", 2);
		}
	}
	#endregion

	#region Main menu buttons
	//When console button clicked
	public void ConsoleClick()
	{
		if (canInteract)
			keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
	}
	//When tutorial button clicked
	public void TutorialClick()
	{
		if (canInteract)
		{
			ButtonClickSound();
			canInteract = false;
			StartCoroutine("ToggleTutorialPanel", true);
		}
	}
	//When setting button clicked
	public void SettingsClick()
	{
		//Opens setting panel
		if (canInteract)
		{
			ButtonClickSound();
			canInteract = false;
			StartCoroutine("ToggleSettingsPanel", true);
		}
	}
	//When leaderboards button clicked
	public void LeaderboardsClick()
	{
		//Opens leaderboards
		if (canInteract)
		{
			ButtonClickSound();
			this.GetComponent<GPSManager>().ShowLeaderboards();
		}
	}
	//When achievements button clicked
	public void AchievementsClick()
	{
		//Opens achievements
		if (canInteract)
		{
			ButtonClickSound();
			this.GetComponent<GPSManager>().ShowAchievementsUI();
		}
	}
	//When characters shop clicked
	public void CharactersClick()
	{
		if (canInteract)
		{
			ButtonClickSound();
			canInteract = false;
			StartCoroutine("OpenCharactersShop");
		}
	}
	//When microtransactions shop clicked
	public void ShopClick()
	{
		if (canInteract)
		{
			ButtonClickSound();
			canInteract = false;
			StartCoroutine("OpenMicrotransactions");
		}
	}
	#endregion

	#region Information panel buttons
	//When panel button is clicked
	public void InfoPanelButtonClicked()
	{
		ButtonClickSound();
		StartCoroutine(ToggleInfoPanel("", false));
	}

	#endregion

	#region Ad panel
	//Closing ad panel
	public void CloseAdPanel()
	{
		if (canInteract)
		{
			ButtonClickSound();
			canInteract = false;
			StartCoroutine("ToggleAdPanel", false);
			GameObject go = GameObject.Find("Player");
			go.GetComponent<PlayerScript>().StartCoroutine("KillPlayer");
		}
	}
	//Watching ad
	public void WatchAd()
	{
		if (!isAdWatched && canInteract)
		{
			ButtonClickSound();
			PlayerSaveData psd = SaveManager.LoadData();
			if (psd.developerMode)
				StartCoroutine(AdWatched());
			else
			{
				canInteract = false;
				AdManager.instance.ShowAd(this.gameObject);
			}
		}
	}
	//If player ended watching ad
	public IEnumerator AdWatched()
	{
		isAdWatched = true;
		StartCoroutine("ToggleAdPanel", false);
		yield return new WaitForSeconds(0.25f);
		GameObject go = GameObject.Find("Player");
		go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + 1f, 0f);
		go.GetComponent<PlayerScript>().Respawn();
	}
	//If ad watching failed
	public void AdFailed()
	{
		StartCoroutine(ToggleInfoPanel("Watching ad failed. Try again! ;/", true));
	}
	#endregion

	#region Settings panel
	//GPS button clicked
	public void GPSButtonClicked()
	{
		if (canInteract)
		{
			if (aproved)
			{
				try
				{
					this.GetComponent<GPSManager>().OpenSave(false);
				}
				catch (Exception e)
				{

				}
			}
			else
			{
				StartCoroutine(ToggleInfoPanel("If you click again all your data will be replaced with cloud data", true));
				aproved = true;
			}
		}
	}
	//Closing setting panel
	public void SettingsMenuButton()
	{
		if (canInteract)
		{
			ButtonClickSound();
			StartCoroutine("ToggleSettingsPanel", false);
		}
	}
	//Reseting data
	public void ResetData()
	{
		if (canInteract)
		{
			ButtonClickSound();
			bool[] x = this.GetComponent<CharactersDataScript>().ownedCharacters;
			x[0] = true;
			for (int i = 1; i < x.Length; i++)
			{
				x[i] = false;
			}
			PlayerSaveData psd = SaveManager.LoadData();
			PlayerSaveData psd2 = new PlayerSaveData(0, 0, 0, x, 0, DateTime.Now.AddMinutes(-freePrizeTimeDelayInMinutes), false, false, false, 0, psd.developerMode, false);
			SaveManager.SaveData(psd2);
			Application.Quit();
		}
	}
	//Sound changing
	public void ChangeSound()
	{
		if (canInteract)
		{
			ButtonClickSound();
			PlayerSaveData psd = SaveManager.LoadData();
			audioManager.GetComponent<AudioManagerScript>().ChangeVolume(false, !psd.isSoundMuted);

			if (!psd.isSoundMuted)
				soundButton.GetComponent<Image>().sprite = soundImageMuted;
			else
				soundButton.GetComponent<Image>().sprite = soundImage;
		}
	}
	//Music changing
	public void ChangeMusic()
	{
		if (canInteract)
		{
			ButtonClickSound();
			PlayerSaveData psd = SaveManager.LoadData();
			audioManager.GetComponent<AudioManagerScript>().ChangeVolume(true, !psd.isMusicMuted);

			if (!psd.isMusicMuted)
				musicButton.GetComponent<Image>().sprite = musicImageMuted;
			else
				musicButton.GetComponent<Image>().sprite = musicImage;
		}
	}
	#endregion

	#region Tutorial panel

	public void TutorialMenuButtonClicked()
	{
		if (canInteract)
		{
			ButtonClickSound();
			canInteract = false;
			PlayerSaveData psd = SaveManager.LoadData();
			GPSManager.UnlockAchievement(GPGSIds.achievement_beginner_is_here);
			StartCoroutine("ToggleTutorialPanel", false);
		}
	}

	#endregion

	#region In game panel
	//Pause button clicked
	public void PauseButtonClicked()
	{
		ButtonClickSound();
		if (isPaused)
		{
			if (canInteract)
				StartCoroutine(UnpauseGame());
		}
		else
		{
			if (canInteract)
			{
				isPaused = true;
				Time.timeScale = 0f;
				gameHUDAnimator.SetBool("isPaused", true);
				scoreText.GetComponent<TextMeshProUGUI>().text = "3";
			}
		}
	}
	private IEnumerator UnpauseGame()
	{
		canInteract = false;
		for (int i = 3; i >= 1; i--)
		{
			scoreText.GetComponent<TextMeshProUGUI>().text = i.ToString();
			yield return new WaitForSecondsRealtime(1f);
		}
		scoreText.GetComponent<TextMeshProUGUI>().text = gameManager.GetComponent<GameManagerScript>().score.ToString();
		Time.timeScale = 1f;
		canInteract = true;
		isPaused = false;
		gameHUDAnimator.SetBool("isPaused", false);
	}

	#endregion
}
