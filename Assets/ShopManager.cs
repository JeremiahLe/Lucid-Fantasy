using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct ItemType 
{
    public AdventureManager.RewardType rewardType;
    public int itemWeight;
}

public class ShopManager : MonoBehaviour
{
    public AdventureManager adventureManager;

    public List<GameObject> ListOfShopItems;

    public TextMeshProUGUI shopkeeperDialogue;

    [Header("Dialogue Quips")]
    public List<string> OnWelcomeQuips;
    public List<string> OnRerollQuips;
    public List<string> OnPurchaseQuips;

    [Header("Item Type Probabilities")]
    public List<ItemType> itemTypes;

    public enum ShopEvent { OnWelcome, OnReroll, OnPurchase }

    public void OnEnable()
    {
        UpdateShopKeeperDialogue(ShopEvent.OnWelcome);

        GenerateShopItems();
    }

    public void GenerateShopItems()
    {
        foreach (GameObject shopItem in ListOfShopItems)
        {
            shopItem.GetComponent<CreateReward>().shopManager = this;
            shopItem.GetComponent<CreateReward>().InitializeSelectable(adventureManager, CreateReward.TypeOfMonsterSelect.ShopItem);
        }
    }

    public void RegenerateShopItems()
    {
        foreach (GameObject shopItem in ListOfShopItems)
        {
            shopItem.GetComponent<CreateReward>().InitializeItem();
        }
    }

    public void RerollShopItems()
    {
        if (adventureManager.rerollAmount >= 1)
        {
            adventureManager.rerollAmount -= 1;
            adventureManager.timesRerolled += 1;

            adventureManager.monstersSubScreenManager.playerRerollAmount.text = ($"{adventureManager.rerollAmount}");

            adventureManager.ResetEquipmentList();
            adventureManager.ResetModifierList();
            adventureManager.ResetItemList();

            RegenerateShopItems();

            UpdateShopKeeperDialogue(ShopEvent.OnReroll);
        }
    }

    public void UpdateShopKeeperDialogue(ShopEvent shopEvent)
    {
        shopkeeperDialogue.text = GetRandomDialogueQuip(shopEvent);
    }

    private string GetRandomDialogueQuip(ShopEvent shopEvent)
    {
        switch (shopEvent)
        {
            case ShopEvent.OnWelcome:
                return OnWelcomeQuips[UnityEngine.Random.Range(0, OnWelcomeQuips.Count)];

            case ShopEvent.OnReroll:
                return OnRerollQuips[UnityEngine.Random.Range(0, OnWelcomeQuips.Count)];

            case ShopEvent.OnPurchase:
                return OnPurchaseQuips[UnityEngine.Random.Range(0, OnWelcomeQuips.Count)];

            default:
                return ("Yeah, I got nothing.");
        }
    }
}
