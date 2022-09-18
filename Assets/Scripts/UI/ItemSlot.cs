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
    public AdventureManager adventureManager;
    public MonstersSubScreenManager monstersSubScreenManager;

    public TextMeshProUGUI itemSlotEquipmentText;
    public Item itemSlotItem;
    public TextMeshProUGUI itemSlotEquipmentStatus;

    public Animator animator;

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerEquipAnimation()
    {
        animator.SetTrigger("equipped");
    }

    public void EndEquipAnimation()
    {
        animator.SetTrigger("equipped");
    }

    public void DisplayEquipmentText()
    {
        inventoryManager.currentMonsterEquipmentStats.text +=
            ($"{itemSlotEquipment.modifierDescription}\n");
    }

    public void RemoveEquipmentText()
    {
        inventoryManager.currentMonsterEquipmentStats.text = "";
    }
}
