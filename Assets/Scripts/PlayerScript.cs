using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

	#region variables
	[HideInInspector] public Rigidbody2D playerRb;
	[SerializeField] private float jumpForce;
	[SerializeField] private float jumpDelay;
	[SerializeField] private float rotateForce;
	[SerializeField] private GameObject playerTrailObject;
	[SerializeField] private GameObject gameManager;
	[SerializeField] private GameObject menuManager, audioManager;
	[SerializeField] private Animator cameraAnimator;
	[SerializeField] private GameObject deathParticle;
	[SerializeField] private GameObject[] boostDestroyParticles;
	[SerializeField] private GameObject hitParticle;
	[SerializeField] private GameObject jumpParticle;
	[SerializeField] private GameObject jumpBoostParticleEffect, magnetBoostParticleEffect, multiplerBoostParticleEffect,shieldShortBoostParticleEffect, shieldBoostParticleEffect;
	[SerializeField] private GameObject shieldBoostPlayerEffectDestroy;
	[SerializeField] private GameObject coinDestroyEffect, crystalDestroyEffect, scoreLaneDestroyEffect;
	public float jumpBoostDelay, magnetBoostDelay, multiplerBoostDelay, shieldBoostDelay, shieldShortBoostDelay;
	public Gradient mainPlayerColor;
	private float jumpDelayTimer;
	private GameObject currentBoostObject;
	private float shakeTimer;
	public float jumpBoostForce;
	[HideInInspector] public int activeBoostID;
	private bool pressed = false;
	private bool canStart = false;
	public bool isAndroid;
	#endregion

	private void Start ()
	{
		//Setting jumpDelayTimer to 0 to allow player jump on start
		jumpDelayTimer = 0f;
		//Setting up shakeTimer;
		shakeTimer = 0f;
		//Initiating player rigidbody
		playerRb = this.GetComponent<Rigidbody2D>();
		//Setting gravity scale to 0
		playerRb.gravityScale = 0f;
	}
	private void Update ()
	{
		if (canStart)
		{
			//Checking is game started
			if (gameManager.GetComponent<GameManagerScript>().isGameStarted)
			{
				//Substracting timers by deltaTime
				jumpDelayTimer -= Time.deltaTime;
				shakeTimer -= Time.deltaTime;
				//Checking that is needed to add force to player when jumoBoost is active
				if (activeBoostID == 1)
					playerRb.velocity = new Vector3(0, jumpBoostForce, 0);
			}
			else if ((!gameManager.GetComponent<GameManagerScript>().isGameStarted && canStart) && (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && !menuManager.GetComponent<MenuManagerScript>().isPaused)
			{
				if (!IsPointerOnPause())
				{
					audioManager.GetComponent<AudioManagerScript>().Jump1.Play();
					//Setting jump delay timer
					jumpDelayTimer = jumpDelay * 1.5f;
					//Making sure that player gravity is set properly and is simulated = true
					playerRb.gravityScale = 2.2f;
					//Setting game to started for make sure
					gameManager.GetComponent<GameManagerScript>().isGameStarted = true;
					//Setting is pressed to true to avoid player jumping down to see under map
					pressed = true;
					//Launching player a little bit to up
					playerRb.velocity = new Vector3(0, 1 * jumpForce * 2, 0);
					//Changing particle color to color of player
					var edit = jumpParticle.GetComponent<ParticleSystem>().main;
					edit.startColor = mainPlayerColor;
					Instantiate(jumpParticle, new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
					menuManager.GetComponent<MenuManagerScript>().canInteract = true;
				}
			}
			//Checking what device is using right now
			if (isAndroid && gameManager.GetComponent<GameManagerScript>().isGameStarted && activeBoostID != 1)
			{
				//Checking that person is pressing anywhere on screen and is jumpDelayTimer is not on cooldown and is player stopped pressing since last time
				if (Input.touchCount > 0 && jumpDelayTimer <= 0 && pressed == false && !menuManager.GetComponent<MenuManagerScript>().isPaused)
				{
					if (!IsPointerOnPause())
					{
						if (UnityEngine.Random.Range(0, 2) == 0)
							audioManager.GetComponent<AudioManagerScript>().Jump1.Play();
						else
							audioManager.GetComponent<AudioManagerScript>().Jump2.Play();
						//Reseting cooldown
						jumpDelayTimer = jumpDelay;
						//Getting point from screen to game
						Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
						//Reseting z position for no problems
						touchPosition.z = 0f;
						//Geting normalized direction of input and player
						Vector3 direction = (touchPosition - this.transform.position).normalized;
						//Adding force to player rigidbody velocity
						playerRb.velocity = (direction * -jumpForce);
						//Checking to what direction add rotation
						if (direction.x >= 0)
							playerRb.angularVelocity = rotateForce;
						else
							playerRb.angularVelocity = -rotateForce;
						//Changing particle color to color of player
						var edit = jumpParticle.GetComponent<ParticleSystem>().main;
						edit.startColor = mainPlayerColor;
						Instantiate(jumpParticle, new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
					}
				}
				//Checking which one state of pressing is
				if (Input.touchCount == 0)
					pressed = false;
				else
					pressed = true;
			}
			else if (!isAndroid && gameManager.GetComponent<GameManagerScript>().isGameStarted && activeBoostID != 1)
			{
				//Checking that person is pressing anywhere on screen and is jumpDelayTimer is not on cooldown and is player stopped pressing since last time
				if (Input.GetMouseButtonDown(0) && jumpDelayTimer <= 0 && !menuManager.GetComponent<MenuManagerScript>().isPaused)
				{
					if (!IsPointerOnPause())
					{
						if (UnityEngine.Random.Range(0, 2) == 0)
							audioManager.GetComponent<AudioManagerScript>().Jump1.Play();
						else
							audioManager.GetComponent<AudioManagerScript>().Jump2.Play();
						//Reseting cooldown
						jumpDelayTimer = jumpDelay;
						//Getting point from screen to game
						Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
						//Reseting z position for no problems
						touchPosition.z = 0f;
						//Geting normalized direction of input and player
						Vector3 direction = (touchPosition - this.transform.position).normalized;
						//Adding force to player rigidbody velocity
						playerRb.velocity = (direction * -jumpForce);
						//Checking to what direction add rotation
						if (direction.x >= 0)
							playerRb.angularVelocity = rotateForce;
						else
							playerRb.angularVelocity = -rotateForce;
						//Changing particle color to color of player
						var edit = jumpParticle.GetComponent<ParticleSystem>().main;
						edit.startColor = mainPlayerColor;
						Instantiate(jumpParticle, new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
					}
				}
			}
		}
	}
	//This function calls when clicking on player collision
	public void OnMouseDown ()
	{
		if (!gameManager.GetComponent<GameManagerScript>().isGameStarted && !canStart && playerRb.simulated && menuManager.GetComponent<MenuManagerScript>().canInteract && menuManager.GetComponent<MenuManagerScript>().isMenuFocused)
		{
			menuManager.GetComponent<MenuManagerScript>().canInteract = false;
			audioManager.GetComponent<AudioManagerScript>().ButtonClick.Play();
			StartCoroutine("StartGame");
		}
	}
	//Function killing player
	public IEnumerator KillPlayer ()
	{
		//Reseting stats
		gameManager.GetComponent<GameManagerScript>().isGameStarted = false;
		canStart = false;
		menuManager.GetComponent<MenuManagerScript>().canInteract = false;
		playerRb.simulated = false;
		playerRb.gravityScale = 0f;
		playerRb.velocity = new Vector3(0,0,0);
		playerRb.angularVelocity = 0f;
		pressed = false;
		activeBoostID = 0;
		this.GetComponent<Animator>().enabled = true;
		this.GetComponent<Animator>().SetTrigger("Vanish");
		this.GetComponent<Animator>().SetBool("isVisible", false);
		playerTrailObject.SetActive(false);
		//Changing particle color to color of player
		var edit = deathParticle.GetComponent<ParticleSystem>().main;
		edit.startColor = mainPlayerColor;
		Instantiate(deathParticle, new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
		//Destroying an boost object if exists
		if (currentBoostObject)
			Destroy(currentBoostObject);
		//Opens main menu
		menuManager.GetComponent<MenuManagerScript>().StartCoroutine("OpenMainMenu", 1);
		//Saving data in gamemanager TODO
		gameManager.GetComponent<GameManagerScript>().SaveGame();
		//Wait for estetic
		yield return new WaitForSeconds(1.25f);
		//Clearing map
		gameManager.GetComponent<GameManagerScript>().ClearMap();
		//Setting up game
		gameManager.GetComponent<GameManagerScript>().EndGame();
		//Waiting for camera to centre on player
		yield return new WaitForSeconds(1.25f);
		//Showing up player
		this.GetComponent<Animator>().SetBool("isVisible", true);
		//Starting simulating player simulation
		playerRb.simulated = true;
	}
	//Function shaking camera
	private void Shake ()
	{
		/*
		//Rolling random int
		int rand = Random.Range(0,2);
		//Choosing shake type
		if (rand == 0)
			cameraAnimator.SetTrigger("Shake1");
		else
			cameraAnimator.SetTrigger("Shake2");
		//Spawning particle effect
		*/
		//Changing color of diffrent players choosed
		var edit = hitParticle.GetComponent<ParticleSystem>().main;
		edit.startColor = mainPlayerColor;
		Instantiate(hitParticle, new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
	}
	//Function collecting coin
	private void CollectCoin (GameObject obj)
	{
		//Spawning coin destroy effect and deleting coin
		Instantiate(coinDestroyEffect, obj.transform.position, Quaternion.identity);
		Destroy(obj);
		//Sending how much to add to function in game manager
		gameManager.GetComponent<GameManagerScript>().AddMoney(activeBoostID == 3);
	}
	//Function collecting crystal
	private void CollectCrystal (GameObject obj)
	{
		//Spawning crystal destroy effect and deleting coin
		Instantiate(crystalDestroyEffect, obj.transform.position, Quaternion.identity);
		Destroy(obj);
		PlayerSaveData psd = SaveManager.LoadData();
		psd.specialMoney++;
		SaveManager.SaveData(psd);
		audioManager.GetComponent<AudioManagerScript>().CollectCoin.Play();
	}
	//Function starting game
	private IEnumerator StartGame()
	{
		//Preparing map
		gameManager.GetComponent<GameManagerScript>().PrepareGame();
		//Enabling trail
		playerTrailObject.SetActive(true);
		//Setting PP color
		var edit = playerTrailObject.GetComponent<ParticleSystem>().main;
		edit.startColor = mainPlayerColor;
		//Checking is that beta character
		PlayerSaveData psd = SaveManager.LoadData();
		if (psd.choosedCharacter == 21)
			mainPlayerColor = menuManager.GetComponent<CharactersDataScript>().characterColor[5];
		//Turning off menu and opening HUD
		menuManager.GetComponent<MenuManagerScript>().StartCoroutine("OpenInGameHUD");
		//Wait for animations to end
		yield return new WaitForSeconds(0.5f);
		//Disabling animator
		this.GetComponent<Animator>().enabled = false;
		//Enabling starting
		canStart = true;
	}
	//Functions respawning player after watching ad
	public void Respawn ()
	{
		//Reseting boost counter to avoid spawning next boost when finished jump boost
		gameManager.GetComponent<GameManagerScript>().currentBoostSpace = 0;
		//Setting jumpboost
		StartCoroutine(BoostJump(this.GetComponent<Collider2D>(), true));
		//Setting stats
		menuManager.GetComponent<MenuManagerScript>().canInteract = true;
		gameManager.GetComponent<GameManagerScript>().isGameStarted = true;
		canStart = true;
		playerRb.simulated = true;
		playerRb.gravityScale = 2.2f;
		playerRb.velocity = new Vector3(0, 0, 0);
		playerRb.angularVelocity = 0f;
		pressed = false;
	}
	//Sets player color
	public void SetPlayerColor (Gradient x)
	{
		mainPlayerColor = x;
	}
	//Checks that is player pressed anywhere not on pause button
	private bool IsPointerOnPause ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 touchPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			if (touchPosition.x <= 0.18f && touchPosition.y >= 0.92f)
				return true;
			else
				return false;
		}
		else if (Input.touchCount > 0)
		{
			Vector2 touchPosition = Camera.main.ScreenToViewportPoint(Input.GetTouch(0).position);
			if (touchPosition.x <= 0.18f && touchPosition.y >= 0.92f)
				return true;
			else
				return false;
		}
		else
			return false;
	}

	#region Boost section
	//Boost jump section
	private IEnumerator BoostJump (Collider2D obj, bool respawning)
	{
		//Destroying boost
		if (!respawning)
		{
			Instantiate(boostDestroyParticles[0], new Vector3(obj.transform.position.x, obj.transform.position.y, 0f), Quaternion.identity);
			Destroy(obj.gameObject);
		}
		//Adding boost particle effect on player
		currentBoostObject = Instantiate(jumpBoostParticleEffect, new Vector3(this.transform.position.x, this.transform.position.y - 0.4f, 0), Quaternion.identity);
		currentBoostObject.GetComponent<ObjectTrackScript>().yOffset = -0.4f;
		currentBoostObject.GetComponent<ObjectTrackScript>().trackObject = this.gameObject;
		currentBoostObject.GetComponent<ObjectTrackScript>().effectDelay = jumpBoostDelay + shieldShortBoostDelay + 0.5f;
		//Sdding boost effect on player
		activeBoostID = 1;
		yield return new WaitForSeconds(jumpBoostDelay);
		//Change active boost to shield
		activeBoostID = 4;
		yield return new WaitForSeconds(shieldShortBoostDelay);
		if (activeBoostID == 4)
			activeBoostID = 0;
	}
	//Boost magnet section
	private IEnumerator BoostMagnet (Collider2D obj)
	{
		//Spawning boost destroy particle effect
		Instantiate(boostDestroyParticles[1], new Vector3(obj.transform.position.x, obj.transform.position.y, 0f), Quaternion.identity);
		Destroy(obj.gameObject);
		//Change active boost to magnet
		currentBoostObject = Instantiate(magnetBoostParticleEffect, new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
		currentBoostObject.GetComponent<ObjectTrackScript>().trackObject = this.gameObject;
		currentBoostObject.GetComponent<ObjectTrackScript>().effectDelay = magnetBoostDelay + 1f;
		activeBoostID = 2;
		yield return new WaitForSeconds(magnetBoostDelay);
		if (activeBoostID == 2)
			activeBoostID = 0;
	}
	//Boost multipler section
	private IEnumerator BoostMultipler (Collider2D obj)
	{
		//Spawning boost destroy particle effect
		Instantiate(boostDestroyParticles[2], new Vector3(obj.transform.position.x, obj.transform.position.y, 0f), Quaternion.identity);
		Destroy(obj.gameObject);
		//Change active boost to multipler
		currentBoostObject = Instantiate(multiplerBoostParticleEffect, new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
		currentBoostObject.GetComponent<ObjectTrackScript>().trackObject = this.gameObject;
		currentBoostObject.GetComponent<ObjectTrackScript>().effectDelay = multiplerBoostDelay + 0.4f;
		activeBoostID = 3;
		yield return new WaitForSeconds(multiplerBoostDelay);
		if (activeBoostID == 3)
			activeBoostID = 0;
	}
	//Boost shield section
	private IEnumerator BoostShield (Collider2D obj)
	{
		//Spawning boost destroy particle effect
		Instantiate(boostDestroyParticles[3], new Vector3(obj.transform.position.x, obj.transform.position.y, 0f), Quaternion.identity);
		Destroy(obj.gameObject);
		//Change active boost to shield
		currentBoostObject = Instantiate(shieldBoostParticleEffect, new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
		currentBoostObject.GetComponent<ObjectTrackScript>().trackObject = this.gameObject;
		currentBoostObject.GetComponent<ObjectTrackScript>().effectDelay = shieldBoostDelay;
		activeBoostID = 4;
		yield return new WaitForSeconds(shieldBoostDelay);
		if (activeBoostID == 4)
			activeBoostID = 0;
	}
	#endregion

	#region Collision section
	//Detecting collision with trigger
	void OnTriggerEnter2D(Collider2D other)
	{
		//Checking is that deadLine
		if (other.CompareTag("DeadLine"))
		{
			audioManager.GetComponent<AudioManagerScript>().DeathLane.Play();
			GPSManager.UnlockAchievement(GPGSIds.achievement_get_burnt);
			//Handheld.Vibrate();
			StartCoroutine("KillPlayer");
		}
		//Checking is that boost
		else if (other.CompareTag("BoostJump") && activeBoostID == 0)
		{
			audioManager.GetComponent<AudioManagerScript>().BubbleDestroy.Play();
			GPSManager.UnlockAchievement(GPGSIds.achievement_boost_up);
			StartCoroutine(BoostJump(other, false));
		}
		else if (other.CompareTag("BoostMagnet") && activeBoostID == 0)
		{
			audioManager.GetComponent<AudioManagerScript>().BubbleDestroy.Play();
			GPSManager.UnlockAchievement(GPGSIds.achievement_boost_up);
			StartCoroutine("BoostMagnet", other);
		}
		else if (other.CompareTag("BoostMultipler") && activeBoostID == 0)
		{
			audioManager.GetComponent<AudioManagerScript>().BubbleDestroy.Play();
			GPSManager.UnlockAchievement(GPGSIds.achievement_boost_up);
			StartCoroutine("BoostMultipler", other);
		}
		else if (other.CompareTag("BoostShield") && activeBoostID == 0)
		{
			audioManager.GetComponent<AudioManagerScript>().BubbleDestroy.Play();
			GPSManager.UnlockAchievement(GPGSIds.achievement_boost_up);
			StartCoroutine("BoostShield", other);
		}
		else if (other.CompareTag("Coin"))
			CollectCoin(other.gameObject);
		else if (other.CompareTag("Crystal"))
			CollectCrystal(other.gameObject);
		//Checking is that best score lane
		else if (other.CompareTag("ScoreLane"))
		{
			audioManager.GetComponent<AudioManagerScript>().RecordLane.Play();
			Instantiate(scoreLaneDestroyEffect, new Vector3(0f, other.gameObject.transform.position.y, 0f), Quaternion.identity);
			Destroy(other.gameObject);
			Handheld.Vibrate();
			menuManager.GetComponent<MenuManagerScript>().ChangeScoreColor(false);
		}

	}
	//Detecting collision with map collider
	void OnCollisionEnter2D(Collision2D other)
	{
		//Checking that jumpBoost is working
		if (activeBoostID == 1 && !other.gameObject.CompareTag("Wall"))
		{
			audioManager.GetComponent<AudioManagerScript>().WallHit.Play();
			var edit = deathParticle.GetComponent<ParticleSystem>().main;
			edit.startColor = mainPlayerColor;
			Instantiate(deathParticle, new Vector3(this.transform.position.x, this.transform.position.y + 1.1f, 0), Quaternion.identity);
			Destroy(other.transform.parent.gameObject);
		}
		//Checking that shieldBoost is working
		else if (activeBoostID == 4 && other.gameObject.CompareTag("Obstacle"))
		{
			Destroy(other.transform.parent.gameObject);
			playerRb.velocity = new Vector3(0, 0, 0);
			playerRb.angularVelocity = 0;
			audioManager.GetComponent<AudioManagerScript>().WallHit.Play();
			activeBoostID = 0;
			Instantiate(shieldBoostPlayerEffectDestroy, new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
			Destroy(currentBoostObject);
		}
		else
		{
			//Checking is that wall or non damaging block
			if (other.gameObject.CompareTag("Map") || other.gameObject.CompareTag("Wall"))
			{
				if (shakeTimer <= 0)
				{
					audioManager.GetComponent<AudioManagerScript>().WallHit.Play();
					shakeTimer = 0.2f;
					Shake();
				}
			}
			else if (other.gameObject.CompareTag("Obstacle"))
			{
				if (!menuManager.GetComponent<MenuManagerScript>().isAdWatched)
				{
					audioManager.GetComponent<AudioManagerScript>().Death.Play();
					Shake();
					//Reseting stats
					menuManager.GetComponent<MenuManagerScript>().canInteract = false;
					gameManager.GetComponent<GameManagerScript>().isGameStarted = false;
					canStart = false;
					playerRb.simulated = false;
					playerRb.gravityScale = 0f;
					playerRb.velocity = new Vector3(0, 0, 0);
					playerRb.angularVelocity = 0f;
					pressed = false;
					activeBoostID = 0;
					//Handheld.Vibrate();
					//Opening ad panel
					menuManager.GetComponent<MenuManagerScript>().StartCoroutine("ToggleAdPanel", true);
				}
				else
				{
					audioManager.GetComponent<AudioManagerScript>().Death.Play();
					Shake();
					StartCoroutine("KillPlayer");
				}
			}
		}
	}
	#endregion
}