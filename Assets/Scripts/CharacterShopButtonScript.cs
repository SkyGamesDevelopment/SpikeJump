using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterShopButtonScript : MonoBehaviour
{

	#region variables
	public int characterID;
	public bool isSetUp;
	[HideInInspector] public bool isOwned, isSpecial;
	[SerializeField] private Sprite beforeNormalSprite, beforeSpecialSprite, afterSprite;
	[SerializeField] private GameObject after, before;
	[SerializeField] private GameObject coinImage;
	[SerializeField] private Sprite specialCoin, nonSpecialCoin;
	[SerializeField] private GameObject characterSprite;
	[SerializeField] private GameObject costText;
	[HideInInspector] public GameObject menuManager;
	#endregion

	private void Start()
	{
		menuManager = GameObject.Find("MenuManager");
	}
	public void SetID(int x)
	{
		characterID = x;
	}
	public void SetSprite(Sprite x)
	{
		this.characterSprite.GetComponent<Image>().sprite = x;
	}
	public void SetCost(int x)
	{
		this.costText.GetComponent<TextMeshProUGUI>().text = x.ToString();
	}
	// x == true is after (character owned)
	public void SetState(bool x, bool y)
	{
		isSpecial = y;
		if (y)
			coinImage.GetComponent<Image>().sprite = specialCoin;
		else
			coinImage.GetComponent<Image>().sprite = nonSpecialCoin;
		if (x)
		{
			isOwned = true;
			before.gameObject.SetActive(false);
			after.gameObject.SetActive(true);
			this.gameObject.GetComponent<Image>().sprite = afterSprite;
		}
		else
		{
			isOwned = false;
			before.gameObject.SetActive(true);
			after.gameObject.SetActive(false);
			if (y)
				this.gameObject.GetComponent<Image>().sprite = beforeSpecialSprite;
			else
				this.gameObject.GetComponent<Image>().sprite = beforeNormalSprite;
		}
	}
	//On button click
	public void OnClick()
	{
		menuManager.GetComponent<MenuManagerScript>().CharactersButtonClicked(characterID, isOwned, isSpecial);
	}
}
