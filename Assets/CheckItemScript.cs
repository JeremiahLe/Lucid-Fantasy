using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class CheckItemScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private CanvasGroup canvasGroup;
    public ItemSlot itemSlot;

    public enum ItemType { Item, Equipment }
    public ItemType itemType;

    public void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
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
        // Display current item information
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (itemType == ItemType.Equipment)
            {
                if (itemSlot.itemSlotEquipment == null)
                    return;

                // Display equipment name, image, and description
                itemSlot.monstersSubScreenManager.currentItem = null;
                itemSlot.monstersSubScreenManager.currentEquipment = itemSlot.itemSlotEquipment;

                itemSlot.monstersSubScreenManager.currentItemNameText.text = ($"{itemSlot.itemSlotEquipment.modifierName}");
                itemSlot.monstersSubScreenManager.currentItemImage.sprite = itemSlot.itemSlotEquipment.baseSprite;

                itemSlot.monstersSubScreenManager.currentItemDescriptionText.text =
                    ($"{itemType} - {itemSlot.itemSlotEquipment.modifierRarity}" +
                    $"\nRank {itemSlot.itemSlotEquipment.equipmentRank} / {Modifier.equipmentMaxRank}" +
                    $"\n{itemSlot.itemSlotEquipment.modifierDescription}");

                if (itemSlot.itemSlotEquipment.modifierOwner != null)
                    itemSlot.monstersSubScreenManager.currentItemDescriptionText.text += ($"\nEquipped by: {itemSlot.itemSlotEquipment.modifierOwner.name}");

                return;
            }

            if (itemType == ItemType.Item)
            {
                if (itemSlot.itemSlotItem == null)
                    return;

                // Display equipment name, image, and description
                itemSlot.monstersSubScreenManager.currentEquipment = null;
                itemSlot.monstersSubScreenManager.currentItem = itemSlot.itemSlotItem;

                itemSlot.monstersSubScreenManager.currentItemNameText.text = ($"{itemSlot.itemSlotItem.itemName}");
                itemSlot.monstersSubScreenManager.currentItemImage.sprite = itemSlot.itemSlotItem.baseSprite;

                itemSlot.monstersSubScreenManager.currentItemDescriptionText.text =
                    ($"{itemType} - {itemSlot.itemSlotItem.itemRarity}" +
                    $"\n{itemSlot.itemSlotItem.itemDescription}");

                itemSlot.monstersSubScreenManager.UseItemButton.interactable = true;

                return;
            }
        }
    }
}
