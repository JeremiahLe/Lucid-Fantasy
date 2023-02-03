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

    public Animator shopKeeperAnimator;

    public Interactable interactable;

    [Header("Dialogue Quips")]
    public List<string> OnWelcomeQuips;
    public List<string> OnRerollQuips;
    public List<string> OnPurchaseQuips;
    public List<string> OnAttemptedPurchaseQuips;

    [Header("Item Type Probabilities")]
    public List<ItemType> itemTypes;

    public enum ShopEvent { OnWelcome, OnReroll, OnPurchase, OnAttemptedPurchase }

    public void OnEnable()
    {
        UpdateShopKeeperDialogue(ShopEvent.OnWelcome);

        GenerateShopItems();

        shopKeeperAnimator.SetTrigger("Quip");
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

            shopKeeperAnimator.SetTrigger("Quip");
        }
    }

    public void ValidatePurchase(CreateReward item)
    {
        if (item.shopItemState == CreateReward.ShopItemState.Purchased)
        {
            // Already purchased!!
            shopkeeperDialogue.text = "You've already purchased that.";

            shopKeeperAnimator.SetTrigger("Quip");

            return;
        }

        if (item.shopItemCost > adventureManager.playerGold)
        {
            // Cannot purchase! Not enough gold!!!
            UpdateShopKeeperDialogue(ShopEvent.OnAttemptedPurchase);

            shopKeeperAnimator.SetTrigger("Quip");

            return;
        }

        if (item.rewardType == AdventureManager.RewardType.Monster && adventureManager.ListOfCurrentMonsters.Count == adventureManager.playerMonsterLimit)
        {

            shopkeeperDialogue.text = "You already have the max amount of monsters.";

            shopKeeperAnimator.SetTrigger("Quip");

            return;
        }

        PurchaseItem(item);
    }

    public void PurchaseItem(CreateReward item)
    {
        adventureManager.playerGold -= item.shopItemCost;
        adventureManager.monstersSubScreenManager.playerGoldAmount.text = ($"{adventureManager.playerGold}");

        UpdateShopKeeperDialogue(ShopEvent.OnPurchase);

        shopKeeperAnimator.SetTrigger("Quip");

        AddItemToInventory(item);

        item.ItemPurchased();
    }

    public void AddItemToInventory(CreateReward item)
    {
        switch (item.rewardType)
        {
            case AdventureManager.RewardType.Monster:
                item.monsterReward.monsterIsOwned = true;
                adventureManager.ListOfCurrentMonsters.Add(item.monsterReward);
                adventureManager.ListOfAllMonsters.Add(item.monsterReward);
                break;

            case AdventureManager.RewardType.Modifier:
                adventureManager.ListOfCurrentModifiers.Add(item.modifierReward);
                adventureManager.ApplyPassiveModifiers(item.modifierReward);
                break;

            case AdventureManager.RewardType.Equipment:
                adventureManager.ListOfCurrentEquipment.Add(item.modifierReward);
                break;

            case AdventureManager.RewardType.Item:
                adventureManager.ListOfInventoryItems.Add(item.itemReward);
                break;
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
                return OnRerollQuips[UnityEngine.Random.Range(0, OnRerollQuips.Count)];

            case ShopEvent.OnPurchase:
                return OnPurchaseQuips[UnityEngine.Random.Range(0, OnPurchaseQuips.Count)];

            case ShopEvent.OnAttemptedPurchase:
                return OnAttemptedPurchaseQuips[UnityEngine.Random.Range(0, OnAttemptedPurchaseQuips.Count)];

            default:
                return ("Yeah, I got nothing.");
        }
    }

    public void LeaveShop()
    {
        gameObject.SetActive(false);

        adventureManager.ActivateNextNode();
    }
}
