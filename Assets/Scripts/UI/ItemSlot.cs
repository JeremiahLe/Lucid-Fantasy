using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public Image itemSlotImage;
    public Modifier itemSlotEquipment;
    public InventoryManager inventoryManager;
    public TextMeshProUGUI itemSlotEquipmentText;

    public void DisplayEquipmentText()
    {
        itemSlotEquipmentText.text = 
            ($"{itemSlotEquipment.modifierName}" +
            $"\n{itemSlotEquipment.modifierDescription}");
    }

    public void RemoveEquipmentText()
    {
        itemSlotEquipmentText.text =
            ("");
    }
}
