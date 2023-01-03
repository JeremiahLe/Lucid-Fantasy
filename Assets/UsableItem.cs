using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UsableItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CanvasGroup canvasGroup;

    public ItemSlot itemSlot;

    public ConsumableWindowScript consumableWindowScript;

    public void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        itemSlot = GetComponent<ItemSlot>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemSlot.itemSlotItem == null)
            return;

        canvasGroup.alpha = .6f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (itemSlot.itemSlotItem == null)
            return;

        canvasGroup.alpha = 1f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemSlot.itemSlotItem == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            consumableWindowScript.SetCurrentItem(itemSlot.itemSlotItem);

            consumableWindowScript.adventureManager.combatManagerScript.buttonManagerScript.ReturnFromItemButton.SetActive(true);

            consumableWindowScript.HideConsumableWindow();

            consumableWindowScript.adventureManager.combatManagerScript.QueueUsableItem(consumableWindowScript.GetCurrentItem());
        }
    }
}
