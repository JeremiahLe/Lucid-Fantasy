using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class MonstersSubScreenManager : MonoBehaviour
{
    public List<GameObject> listOfMonsterSlots;
    public List<GameObject> monsterSlotImages;

    public AdventureManager adventureManager;
    public TextMeshProUGUI monsterAmountText;

    public string niceTime;
    public bool updateTimerVisual;

    [Header("Inventory Menu")]
    public GameObject InventoryMenu;
    public TextMeshProUGUI inventoryHeaderText;

    public Image currentItemImage;
    public Image currentItemDescriptionBackgroundImage;

    public TextMeshProUGUI currentItemNameText;
    public TextMeshProUGUI currentItemDescriptionText;

    public Button UseItemButton;
    public Button TrashItemButton;
    public Button UpgradeEquipmentButton;

    public Sprite emptyVisualSprite;

    public List<GameObject> inventorySlots;

    public void OnEnable()
    {
        InventoryMenu.SetActive(false);
    }

    public void ShowAvailableMonsters()
    {
        // Hide other menus
        ResetMenus();

        // Update text
        monsterAmountText.text =
            ($"<b>Chimerics: ({adventureManager.ListOfCurrentMonsters.Count}/4)</b>" +
            $"\n[Right-click for more info.]");

        // Show allied monster images
        foreach(GameObject slot in monsterSlotImages)
        {
            slot.SetActive(true);
        }

        // Show allied monsters
        int i = 0;
        foreach (GameObject monsterSlot in listOfMonsterSlots)
        {
            if (adventureManager.ListOfCurrentMonsters.Count > i)
            {
                monsterSlot.SetActive(true);
                monsterSlot.GetComponent<CreateReward>().adventureManager = adventureManager;
                monsterSlot.GetComponent<CreateReward>().subscreenManager = adventureManager.subscreenManager;
                monsterSlot.GetComponent<CreateReward>().monsterStatScreenScript = adventureManager.subscreenManager.monsterStatScreenScript;
                monsterSlot.GetComponent<CreateReward>().monsterReward = adventureManager.ListOfCurrentMonsters[i];
                monsterSlot.GetComponent<CreateReward>().rewardImage.sprite = monsterSlot.GetComponent<CreateReward>().monsterReward.baseSprite;
                monsterSlot.GetComponentInChildren<TextMeshProUGUI>().text = ($"<b>{monsterSlot.GetComponent<CreateReward>().monsterReward.name}</b> Lvl.{monsterSlot.GetComponent<CreateReward>().monsterReward.level}" +
                    $"\nHP: {monsterSlot.GetComponent<CreateReward>().monsterReward.health}/{monsterSlot.GetComponent<CreateReward>().monsterReward.maxHealth}");
                    //$"\n{monsterSlot.GetComponent<CreateReward>().monsterReward.monsterElement.element.ToString()}/{monsterSlot.GetComponent<CreateReward>().monsterReward.monsterSubElement.element.ToString()}");
            }

            i++;
        }
    }

    public void ShowAvailableModifiers()
    {
        // Hide other menus
        ResetMenus();

        // Update text
        monsterAmountText.text =
            ($"<b>Current Modifiers:</b>");

        // Display modifier info
        foreach (Modifier modifier in adventureManager.ListOfCurrentModifiers)
        {
            monsterAmountText.text +=
               ($"\n<b>{modifier.modifierName}</b> - {modifier.modifierDescription}");
        }
    }

    public void ShowStatus()
    {
        // Hide other menus
        ResetMenus();

        // Shows adventure status
        updateTimerVisual = true;
    }

    public void ShowInventory()
    {
        // Hide other menus
        ResetMenus();

        // Show inventory menu
        InventoryMenu.SetActive(true);

        // Update text
        monsterAmountText.text =
            ($"");

        inventoryHeaderText.text =
            ($"Consumables");

        currentItemNameText.text = "";
        currentItemDescriptionText.text = "";

        // Hide item buttons and images
        UseItemButton.enabled = false;
        TrashItemButton.enabled = false;
        UpgradeEquipmentButton.enabled = false;

        currentItemImage.sprite = emptyVisualSprite;
        currentItemDescriptionBackgroundImage.sprite = emptyVisualSprite;

        InitializeConsumables();
    }

    public void InitializeConsumables()
    {
        // Update text
        inventoryHeaderText.text =
            ($"Consumables");

        // clean the inventory slots first
        foreach (GameObject slot in inventorySlots)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.itemSlotEquipment = null;
            itemSlot.itemSlotItem = null;
            itemSlot.itemSlotImage.sprite = emptyVisualSprite;
            slot.GetComponent<Interactable>().ResetInteractable();
            slot.GetComponent<Interactable>().typeOfInteractable = Interactable.TypeOfInteractable.Item;
        }

        // Initialize consumables
        int i = 0;
        foreach (GameObject slot in inventorySlots)
        {
            if (adventureManager.ListOfInventoryItems.Where(item => item.itemType == Item.ItemType.Consumable).ToList().Count > i)
            {
                ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
                itemSlot.itemSlotItem = adventureManager.ListOfInventoryItems.Where(item => item.itemType == Item.ItemType.Consumable).ToList()[i];
                itemSlot.itemSlotImage.sprite = itemSlot.itemSlotItem.baseSprite;
                slot.GetComponent<Interactable>().InitiateInteractable(itemSlot.itemSlotItem);
            }

            i++;
        }
    }

    public void InitializeEquipment()
    {
        // Update text
        inventoryHeaderText.text =
            ($"Equipment");

        // clean the inventory slots first
        foreach (GameObject slot in inventorySlots)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.itemSlotEquipment = null;
            itemSlot.itemSlotItem = null;
            itemSlot.itemSlotImage.sprite = slot.GetComponentInChildren<DragAndDropItem>().emptySprite;
            itemSlot.GetComponent<Interactable>().ResetInteractable();
            slot.GetComponent<Interactable>().typeOfInteractable = Interactable.TypeOfInteractable.Modifier;
        }

        // Initialize the equipment
        int i = 0;
        foreach (GameObject slot in inventorySlots)
        {
            if (adventureManager.ListOfCurrentEquipment.Count > i)
            {
                ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
                itemSlot.inventoryManager = GetComponent<InventoryManager>();
                itemSlot.itemSlotEquipment = adventureManager.ListOfCurrentEquipment[i];
                itemSlot.itemSlotImage.sprite = itemSlot.itemSlotEquipment.baseSprite;
                slot.GetComponent<Interactable>().InitiateInteractable(itemSlot.itemSlotEquipment);
            }

            i++;
        }
    }

    public void InitializeAcensionMaterials()
    {
        // Update text
        inventoryHeaderText.text =
            ($"Ascension Materials");

        // clean the inventory slots first
        foreach (GameObject slot in inventorySlots)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.itemSlotEquipment = null;
            itemSlot.itemSlotItem = null;
            itemSlot.itemSlotImage.sprite = emptyVisualSprite;
            slot.GetComponent<Interactable>().ResetInteractable();
            slot.GetComponent<Interactable>().typeOfInteractable = Interactable.TypeOfInteractable.Item;
        }

        // Initialize ascension materials
        int i = 0;
        foreach (GameObject slot in inventorySlots)
        {
            if (adventureManager.ListOfInventoryItems.Where(item => item.itemType == Item.ItemType.AscensionMaterial).ToList().Count > i)
            {
                ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
                itemSlot.itemSlotItem = adventureManager.ListOfInventoryItems.Where(item => item.itemType == Item.ItemType.AscensionMaterial).ToList()[i];
                itemSlot.itemSlotImage.sprite = itemSlot.itemSlotItem.baseSprite;
                slot.GetComponent<Interactable>().InitiateInteractable(itemSlot.itemSlotItem);
            }

            i++;
        }
    }

    public void ResetMenus()
    {
        // Hide adventure status
        updateTimerVisual = false;

        // Hide inventory menuy
        InventoryMenu.SetActive(false);

        // Hide allied monster slots
        foreach (GameObject slot in listOfMonsterSlots)
        {
            slot.SetActive(false);
        }

        // Hide allied monster images
        foreach (GameObject slot in monsterSlotImages)
        {
            slot.SetActive(false);
        }
    }

    public void Update()
    {
        if (updateTimerVisual)
        {
            // Get adventure time
            int minutes = Mathf.FloorToInt(adventureManager.adventureTimer / 60F);
            int seconds = Mathf.FloorToInt(adventureManager.adventureTimer - minutes * 60);
            niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

            // Update text
            monsterAmountText.text =
                ($"<b>Adventure Status:</b>" +
                $"\n\n\nTime: {niceTime}" +
                $"\nRun: {adventureManager.adventureNGNumber}" +
                $"\nGold Spent: {adventureManager.playerGoldSpent}" +
                $"\nRerolls: {adventureManager.timesRerolled}" +
                $"\nAlly Chimerics Defeated: {adventureManager.playerMonstersLost}" +
                $"\nEnemy Chimerics Defeated: {adventureManager.playerMonstersKilled}");
        }
    }
}
