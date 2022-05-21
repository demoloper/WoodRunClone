using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
	public int itemId, wearId;
	public int price;

	public Button buyButton, equipButton, unequipButton;
	public Text priceText;
	public bool HasItem()
	{
		//0:Satýn alýnmamýþ
		//1:Satýn alýnmýþ giyinmemiþ
		//2:Hem satýn alýnmýþ hemde giyilmiþ
		bool hasItem = PlayerPrefs.GetInt("item" + itemId.ToString()) != 0;
		return hasItem;
	}
	public bool IsEquipped()
	{
		bool equippedItem = PlayerPrefs.GetInt("item" + itemId.ToString()) == 2;
		return equippedItem;
	}
	public void InitializeItem()
	{
		priceText.text = price.ToString();
		if (HasItem())
		{
			buyButton.gameObject.SetActive(false);
			if (IsEquipped())
			{
				EquipItem();
			}
			else
			{
				equipButton.gameObject.SetActive(true);
			}
		}
		else
		{
			buyButton.gameObject.SetActive(true);
		}


	}
	public void BuyItem()
	{
		if (!HasItem())
		{
			int money = PlayerPrefs.GetInt("money");
			if (money >= price)
			{
				PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.buyAudipClip, 0.1f);
				LevelController.Current.GiveMoneyToPlayer(-price);
				PlayerPrefs.SetInt("item" + itemId.ToString(), 1);
				buyButton.gameObject.SetActive(false);
				equipButton.gameObject.SetActive(true);


			}
		}
	}
	public void EquipItem()
	{


	}
	public void UnequipItem()
	{
		Item equippedItem = MarketController.Current.equippedItems[wearId];
		if (equippedItem != null)
		{
			MarketItem marketItem = MarketController.Current.items[equippedItem.item];
			PlayerPrefs.SetInt("item" + marketItem.itemId, 1);
			marketItem.equipButton.gameObject.SetActive(true);
			marketItem.unequipButton.gameObject.SetActive(false);
			Destroy(equippedItem.gameObject);
		}
	}



}
