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
            if (itemType == ItemType.Equipment && itemSlot.itemSlotEquipment != null)
            {
                // Display equipment name, image, and description
                itemSlot.monstersSubScreenManager.currentEquipment = itemSlot.itemSlotEquipment;

                itemSlot.monstersSubScreenManager.currentItemNameText.text = ($"{itemSlot.itemSlotEquipment.modifierName}");
                itemSlot.monstersSubScreenManager.currentItemImage.sprite = itemSlot.itemSlotEquipment.baseSprite;

                //itemSlot.monstersSubScreenManager.currentItemDescriptionBackgroundImage.sprite
                itemSlot.monstersSubScreenManager.currentItemDescriptionText.text =
                    ($"Equipment - {itemSlot.itemSlotEquipment.modifierRarity.ToString()}" +
                    $"\nRank {itemSlot.itemSlotEquipment.equipmentRank} / {Modifier.equipmentMaxRank}" +
                    $"\n{itemSlot.itemSlotEquipment.modifierDescription}");

                if (itemSlot.itemSlotEquipment.modifierOwner != null)
                    itemSlot.monstersSubScreenManager.currentItemDescriptionText.text += ($"\nEquipped by: {itemSlot.itemSlotEquipment.modifierOwner.name}");
            }
        }
    }
}
