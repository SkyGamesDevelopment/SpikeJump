using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour, IStoreListener
{
	//This lane allows you to get functions and variables from this class
	private static IAPManager Instance { set; get; }
	//Controllers is objects that keeps initialization informations
	private static IStoreController m_StoreController;
	private static IExtensionProvider m_StoreExtensionProvider;

	#region Products ids
	public static string product1 = "standard1";
	public static string product2 = "standard2";
	public static string product3 = "standard3";
	public static string product4 = "standard4";
	public static string product5 = "standard5";
	public static string product6 = "standard6";
	public static string product7 = "special1";
	#endregion

	//This makes a Instance object
	void Awake()
	{
		Instance = this;
	}
	//That function checking that are we need initializing
	void Start()
	{
		if (m_StoreController == null)
		{
			InitializePurchasing();
		}
	}
	//That function initializing purchasing (loading and connecting)
	public void InitializePurchasing()
	{
		//If we are already initialized skiping process
		if (IsInitialized() || m_StoreController != null)
		{
			return;
		}
		//Making a new builder that can contain all product info
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		//Adding products to the builder variable
		builder.AddProduct(product1, ProductType.Consumable);
		builder.AddProduct(product2, ProductType.Consumable);
		builder.AddProduct(product3, ProductType.Consumable);
		builder.AddProduct(product4, ProductType.Consumable);
		builder.AddProduct(product5, ProductType.Consumable);
		builder.AddProduct(product6, ProductType.Consumable);
		builder.AddProduct(product7, ProductType.Consumable);
		//Initializing a new builder
		UnityPurchasing.Initialize(this, builder);
	}
	//The bool variable that returns is initializated
	private bool IsInitialized()
	{
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}
	//Simple buy function
	public void BuyProduct(int id)
	{
		//Seraching for product id to process
		switch (id)
		{
			case 1:
				BuyProductID(product1);
				break;
			case 2:
				BuyProductID(product2);
				break;
			case 3:
				BuyProductID(product3);
				break;
			case 4:
				BuyProductID(product4);
				break;
			case 5:
				BuyProductID(product5);
				break;
			case 6:
				BuyProductID(product6);
				break;
			case 7:
				BuyProductID(product7);
				break;
		}
	}
	//This function checking everything when trying to buy something and return errors
	private void BuyProductID(string productId)
	{
		//Checking that is initializated
		if (IsInitialized())
		{
			//Making a new product of an given productID
			Product product = m_StoreController.products.WithID(productId);
			//There you are able to purchase
			if (product != null && product.availableToPurchase)
			{
				Debug.Log(string.Format("Buying product: '{0}'", product.definition.id));
				m_StoreController.InitiatePurchase(product);
			}
			else
			{
				//You are initializated but product can be null or you cant purchase it
				this.GetComponent<MenuManagerScript>().IAPCallbackError();
			}
		}
		else
		{
			//Retrying initialization
			InitializePurchasing();
			//You are not initializated
			this.GetComponent<MenuManagerScript>().IAPInitializeError();
		}
	}
	//It runs when initialization is completed
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		//Attributing a new controller and extension
		Debug.Log("OnInitialized: PASS");
		m_StoreController = controller;
		m_StoreExtensionProvider = extensions;
	}
	//It runs when initialization failed
	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
	}
	//Final purchasing function
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
	{
		GameObject go = GameObject.Find("GameManager");
		GameObject go2 = GameObject.Find("MenuManager");
		//There you can pase all of the products you can purchase like below. Copy and paste all if state and swap "product1" with another one
		if (String.Equals(args.purchasedProduct.definition.id, product1, StringComparison.Ordinal))
		{
			go.GetComponent<GameManagerScript>().AddPurchasedMoney(100, false);
			StartCoroutine(go2.GetComponent<MenuManagerScript>().ToggleInfoPanel("Successfully got 100 bucks! Thanks a lot for your support! Now we can go for ice creams!", true));
		}
		else if (String.Equals(args.purchasedProduct.definition.id, product2, StringComparison.Ordinal))
		{
			go.GetComponent<GameManagerScript>().AddPurchasedMoney(300, false);
			StartCoroutine(go2.GetComponent<MenuManagerScript>().ToggleInfoPanel("Successfully got 300 bucks! Thanks a lot for your support! Now we can go for small pizza!", true));
		}
		else if (String.Equals(args.purchasedProduct.definition.id, product3, StringComparison.Ordinal))
		{
			go.GetComponent<GameManagerScript>().AddPurchasedMoney(700, false);
			StartCoroutine(go2.GetComponent<MenuManagerScript>().ToggleInfoPanel("Successfully got 700 bucks! Thanks a lot for your support! Now we can go for nice burgers!", true));
		}
		else if (String.Equals(args.purchasedProduct.definition.id, product4, StringComparison.Ordinal))
		{
			go.GetComponent<GameManagerScript>().AddPurchasedMoney(1950, false);
			StartCoroutine(go2.GetComponent<MenuManagerScript>().ToggleInfoPanel("Successfully got 1950 bucks! Thanks a lot for your support! Now we can make a small party!", true));
		}
		else if (String.Equals(args.purchasedProduct.definition.id, product5, StringComparison.Ordinal))
		{
			go.GetComponent<GameManagerScript>().AddPurchasedMoney(4600, false);
			StartCoroutine(go2.GetComponent<MenuManagerScript>().ToggleInfoPanel("Successfully got 4600 bucks! Thanks a lot for your support! Now we can get drunk all the way!", true));
		}
		else if (String.Equals(args.purchasedProduct.definition.id, product6, StringComparison.Ordinal))
		{
			go.GetComponent<GameManagerScript>().AddPurchasedMoney(10000, false);
			StartCoroutine(go2.GetComponent<MenuManagerScript>().ToggleInfoPanel("Successfully got 10000 bucks! Thanks a lot for your support! Now we can survive this week!", true));
		}
		else if (String.Equals(args.purchasedProduct.definition.id, product7, StringComparison.Ordinal))
		{
			go.GetComponent<GameManagerScript>().AddPurchasedMoney(250, true);
			StartCoroutine(go2.GetComponent<MenuManagerScript>().ToggleInfoPanel("Successfully got 250 crystals! Thaks a lot for your support! We hope you enjoy new characters!", true));
		}
		//This runs up when no one of above products can be found or run
		else
		{
			this.GetComponent<MenuManagerScript>().IAPCallbackError();
		}
		return PurchaseProcessingResult.Complete;
	}
	//It runs when purchase failed
	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
		this.GetComponent<MenuManagerScript>().IAPCallbackError();
	}
	//Returns product price
	public string ProductPrice(int id)
	{
		try
		{
			switch (id)
			{
				case 1:
					return m_StoreController.products.WithID(product1).metadata.localizedPrice.ToString();
					break;
				case 2:
					return m_StoreController.products.WithID(product2).metadata.localizedPrice.ToString();
					break;
				case 3:
					return m_StoreController.products.WithID(product3).metadata.localizedPrice.ToString();
					break;
				case 4:
					return m_StoreController.products.WithID(product4).metadata.localizedPrice.ToString();
					break;
				case 5:
					return m_StoreController.products.WithID(product5).metadata.localizedPrice.ToString();
					break;
				case 6:
					return m_StoreController.products.WithID(product6).metadata.localizedPrice.ToString();
					break;
				case 7:
					return m_StoreController.products.WithID(product7).metadata.localizedPrice.ToString();
					break;
			}
			return "Error";
		}
		catch (Exception e)
		{
			return null;
		}
	}
}