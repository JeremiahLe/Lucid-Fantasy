using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class DragAndDropItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{ 
    private CanvasGroup canvasGroup;
    public ItemSlot itemSlot;
    public Sprite emptySprite;

    public enum SlotType { Equipped, Unequipped }
    public SlotType slotType;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        itemSlot = GetComponentInParent<ItemSlot>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        canvasGroup.alpha = .6f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemSlot.inventoryManager.adventureManager.lockEquipmentInCombat)
        {
            itemSlot.GetComponent<Interactable>().ShowInteractable("Cannot adjust equipment in battle!");
            return;
        }

        // Add to monster's current equipment
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (slotType == SlotType.Unequipped && itemSlot.itemSlotEquipment != null)
            {
                // first check if inventory is already full
                if (itemSlot.inventoryManager.currentMonsterEquipment.ListOfModifiers.Count >= 4)
                {
                    itemSlot.GetComponent<Interactable>().ShowInteractable("Chimeric has max amount of equipment!");
                    return;
                }

                // Add equipment to monster inventory and remove from global inventory
                itemSlot.inventoryManager.currentMonsterEquipment.ListOfModifiers.Add(itemSlot.itemSlotEquipment);
                itemSlot.inventoryManager.adventureManager.ListOfCurrentEquipment.Remove(itemSlot.itemSlotEquipment);

                // Clear the inventory sprite, modifier, and interactable
                itemSlot.itemSlotEquipment = null;
                itemSlot.itemSlotImage.sprite = emptySprite;
                itemSlot.GetComponent<Interactable>().ResetInteractable();

                // Reset to new inventory
                itemSlot.inventoryManager.InitializeInventorySlots();
            }
        }

        // Remove from monster's current equipment
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (slotType == SlotType.Equipped && itemSlot.itemSlotEquipment != null)
            {
                // first check if inventory is already full
                if (itemSlot.inventoryManager.adventureManager.ListOfCurrentEquipment.Count >= 16)
                {
                    itemSlot.GetComponent<Interactable>().ShowInteractable("Inventory is full!");
                    return;
                }

                // Add equipment to global inventory and remove from monster inventory
                itemSlot.inventoryManager.adventureManager.ListOfCurrentEquipment.Add(itemSlot.itemSlotEquipment);
                itemSlot.inventoryManager.currentMonsterEquipment.ListOfModifiers.Remove(itemSlot.itemSlotEquipment);

                // Clear the inventory sprite, modifier, and interactable
                itemSlot.itemSlotEquipment = null;
                itemSlot.itemSlotImage.sprite = emptySprite;
                itemSlot.GetComponent<Interactable>().ResetInteractable();
                itemSlot.RemoveEquipmentText();

                // Reset to new inventory
                itemSlot.inventoryManager.InitializeInventorySlots();
            }
        }
    }
}