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

    public TextMeshProUGUI playerGoldAmount;

    [Header("Inventory Menu")]
    public Item currentItem;
    public Modifier currentEquipment;

    public GameObject InventoryMenu;
    public TextMeshProUGUI inventoryHeaderText;

    public Image currentItemImage;
    public Image currentItemDescriptionBackgroundImage;

    public TextMeshProUGUI currentItemNameText;
    public TextMeshProUGUI currentItemDescriptionText;

    public Button UseItemButton;
    public Button TrashItemButton;
    public Button UpgradeEquipmentButton;
    public Button EquipEquipmentButton;

    public Sprite emptyVisualSprite;

    public List<GameObject> inventorySlots;

    public MonsterSelectPanelManager monsterSelectPanelManager;

    public void OnEnable()
    {
        PlayNewUIScreenSFX();

        playerGoldAmount.text = ($"{adventureManager.playerGold}");

        InventoryMenu.SetActive(false);
    }

    public void ShowAvailableMonsters()
    {
        PlayNewUIScreenSFX();

        // Hide other menus
        ResetMenus();

        // Update text
        monsterAmountText.text =
            ($"<b>Chimerics: ({adventureManager.ListOfCurrentMonsters.Count}/{adventureManager.playerMonsterLimit})</b>");

        monsterSelectPanelManager.InitializeMonsterSelectCards(adventureManager, CreateReward.TypeOfMonsterSelect.View);

        //// Show allied monster images
        //foreach(GameObject slot in monsterSlotImages)
        //{
        //    slot.SetActive(true);
        //}

        //// Show allied monsters
        //int i = 0;
        //foreach (GameObject monsterSlot in listOfMonsterSlots)
        //{
        //    if (adventureManager.ListOfCurrentMonsters.Count > i)
        //    {
        //        monsterSlot.SetActive(true);
        //        monsterSlot.GetComponent<CreateReward>().adventureManager = adventureManager;
        //        monsterSlot.GetComponent<CreateReward>().subscreenManager = adventureManager.subscreenManager;
        //        monsterSlot.GetComponent<CreateReward>().monsterStatScreenScript = adventureManager.subscreenManager.monsterStatScreenScript;
        //        monsterSlot.GetComponent<CreateReward>().monsterStatScreenScript.monstersSubScreenManager = this;
        //        monsterSlot.GetComponent<CreateReward>().monsterReward = adventureManager.ListOfCurrentMonsters[i];
        //        monsterSlot.GetComponent<CreateReward>().rewardImage.sprite = monsterSlot.GetComponent<CreateReward>().monsterReward.baseSprite;
        //        monsterSlot.GetComponentInChildren<TextMeshProUGUI>().text = ($"<b>{monsterSlot.GetComponent<CreateReward>().monsterReward.name}</b> Lvl.{monsterSlot.GetComponent<CreateReward>().monsterReward.level}" +
        //            $"\nHP: {monsterSlot.GetComponent<CreateReward>().monsterReward.health}/{monsterSlot.GetComponent<CreateReward>().monsterReward.maxHealth}");
        //            //$"\n{monsterSlot.GetComponent<CreateReward>().monsterReward.monsterElement.element.ToString()}/{monsterSlot.GetComponent<CreateReward>().monsterReward.monsterSubElement.element.ToString()}");
        //    }

        //    i++;
        //}
    }

    public void QueueHealingConsumable()
    {
        if (currentItem == null)
            return;

        monsterAmountText.text =
            ($"<b>Select a Chimeric to heal...</b>" +
            $"\nCurrent Item: {currentItem.itemName} - {currentItem.itemDescription}");

        // Show Return To Items Button
    }

    public void ShowAvailableModifiers()
    {
        PlayNewUIScreenSFX();

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
        PlayNewUIScreenSFX();

        // Hide other menus
        ResetMenus();

        // Shows adventure status
        updateTimerVisual = true;
    }

    public void ShowInventory()
    {
        PlayNewUIScreenSFX();

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
        EquipEquipmentButton.gameObject.SetActive(false);

        UseItemButton.interactable = false;
        TrashItemButton.interactable = false;
        UpgradeEquipmentButton.interactable = false;

        currentItemImage.sprite = emptyVisualSprite;

        InitializeConsumables();
    }

    public void InitializeConsumables()
    {
        PlayNewUIScreenSFX();

        // Update text
        inventoryHeaderText.text =
            ($"Consumables");

        // Adjust buttons
        UseItemButton.gameObject.SetActive(true);
        EquipEquipmentButton.gameObject.SetActive(false);

        //UseItemButton.interactable = true;
        //TrashItemButton.interactable = true;
        //UpgradeEquipmentButton.interactable = false;
        //EquipEquipmentButton.interactable = false;

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

                itemSlot.adventureManager = adventureManager;
                itemSlot.monstersSubScreenManager = this;

                itemSlot.itemSlotItem = adventureManager.ListOfInventoryItems.Where(item => item.itemType == Item.ItemType.Consumable).ToList()[i];
                itemSlot.itemSlotImage.sprite = itemSlot.itemSlotItem.baseSprite;

                slot.GetComponent<Interactable>().InitiateInteractable(itemSlot.itemSlotItem);
                slot.GetComponentInChildren<CheckItemScript>().itemSlot = itemSlot;
                slot.GetComponentInChildren<CheckItemScript>().itemType = CheckItemScript.ItemType.Item;
            }

            i++;
        }
    }

    public void InitializeEquipment()
    {
        PlayNewUIScreenSFX();

        // Update text
        inventoryHeaderText.text =
            ($"Equipment");

        // Adjust buttons
        UseItemButton.gameObject.SetActive(false);
        EquipEquipmentButton.gameObject.SetActive(true);

        //UseItemButton.interactable = false;
        //TrashItemButton.interactable = true;
        //UpgradeEquipmentButton.interactable = true;
        //EquipEquipmentButton.interactable = true;

        // clean the inventory slots first
        foreach (GameObject slot in inventorySlots)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.itemSlotEquipment = null;
            itemSlot.itemSlotItem = null;
            itemSlot.itemSlotImage.sprite = emptyVisualSprite;
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

                itemSlot.adventureManager = adventureManager;
                itemSlot.monstersSubScreenManager = this;

                itemSlot.itemSlotEquipment = adventureManager.ListOfCurrentEquipment[i];
                itemSlot.itemSlotImage.sprite = itemSlot.itemSlotEquipment.baseSprite;

                slot.GetComponent<Interactable>().InitiateInteractable(itemSlot.itemSlotEquipment);
                slot.GetComponentInChildren<CheckItemScript>().itemSlot = itemSlot;
                slot.GetComponentInChildren<CheckItemScript>().itemType = CheckItemScript.ItemType.Equipment;
            }

            i++;
        }
    }

    public void InitializeAcensionMaterials()
    {
        PlayNewUIScreenSFX();

        // Update text
        inventoryHeaderText.text =
            ($"Ascension Materials");

        // Adjust buttons
        UseItemButton.gameObject.SetActive(true);
        EquipEquipmentButton.gameObject.SetActive(false);

        //UseItemButton.interactable = true;
        //TrashItemButton.interactable = true;
        //UpgradeEquipmentButton.interactable = false;
        //EquipEquipmentButton.interactable = false;

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

                itemSlot.adventureManager = adventureManager;
                itemSlot.monstersSubScreenManager = this;

                itemSlot.itemSlotItem = adventureManager.ListOfInventoryItems.Where(item => item.itemType == Item.ItemType.AscensionMaterial).ToList()[i];
                itemSlot.itemSlotImage.sprite = itemSlot.itemSlotItem.baseSprite;

                slot.GetComponent<Interactable>().InitiateInteractable(itemSlot.itemSlotItem);
                slot.GetComponentInChildren<CheckItemScript>().itemSlot = itemSlot;
                slot.GetComponentInChildren<CheckItemScript>().itemType = CheckItemScript.ItemType.Item;
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

        monsterSelectPanelManager.HideMonsterSelectCards();
    }

    public void PlayNewUIScreenSFX()
    {
        adventureManager.NewUIScreenSelected();
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
