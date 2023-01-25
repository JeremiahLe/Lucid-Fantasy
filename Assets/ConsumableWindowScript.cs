using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

public class ConsumableWindowScript : MonoBehaviour
{
    public Item currentItem;

    public List<GameObject> consumableSlots;

    public AdventureManager adventureManager;

    public Sprite emptySprite;

    public void Awake()
    {
        adventureManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AdventureManager>();
    }

    public void SetCurrentItem(Item item)
    {
        currentItem = item;
    }

    public void ResetCurrentItem()
    {
        currentItem = null;
    }

    public Item GetCurrentItem()
    {
        return currentItem;
    }

    public void AssignItemSlots()
    {
        // Initialize the inventory slots
        int i = 0;

        // Clear the slot visuals first
        foreach (GameObject slot in consumableSlots)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();

            itemSlot.itemSlotItem = null;
                
            itemSlot.itemSlotImage.sprite = emptySprite;

            itemSlot.adventureManager = adventureManager;

            itemSlot.GetComponent<Interactable>().ResetInteractable();

            UsableItem usableItem = slot.GetComponent<UsableItem>();

            usableItem.itemSlot.itemSlotItem = null;
        }

        foreach (GameObject slot in consumableSlots)
        {
            if (adventureManager.ListOfInventoryItems.Where(item => item.itemType == Item.ItemType.Consumable).ToList().Count > i)
            {
                ItemSlot itemSlot = slot.GetComponent<ItemSlot>();

                itemSlot.itemSlotItem = adventureManager.ListOfInventoryItems.Where(item => item.itemType == Item.ItemType.Consumable).ToList()[i];

                itemSlot.itemSlotImage.sprite = itemSlot.itemSlotItem.baseSprite;

                slot.GetComponent<Interactable>().InitiateInteractable(itemSlot.itemSlotItem);

                UsableItem usableItem = slot.GetComponent<UsableItem>();

                usableItem.itemSlot = itemSlot;

                usableItem.consumableWindowScript = this;
            }

            i++;
        }
    }

    public void HideConsumableWindow()
    {
        gameObject.SetActive(false);
    }

    public void ReturnToConsumableWindow()
    {
        gameObject.SetActive(true);

        AssignItemSlots();

        SetCurrentItem(null);

        adventureManager.combatManagerScript.monsterAttackManager.HideAttackDescription();

        adventureManager.combatManagerScript.itemPending = false;

        adventureManager.combatManagerScript.targeting = false;
    }
}
