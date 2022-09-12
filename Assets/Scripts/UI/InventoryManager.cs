using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public Modifier currentDraggedEquipment;
    public Monster currentMonsterEquipment;

    public List<GameObject> inventorySlots;
    public List<GameObject> monsterEquipmentSlots;

    public AdventureManager adventureManager;

    public Button equipmentButton;
    public Button ascensionButton;

    [Header("Ascension Window")]
    public Image monsterBaseImage;
    public Image monsterAscensionOneImage;
    public Image monsterAscensionTwoImage;

    public TextMeshProUGUI monsterAscensionOneText;
    public TextMeshProUGUI monsterAscensionTwoText;

    public TextMeshProUGUI monsterBaseText;
    public TextMeshProUGUI monsterElementsText;

    public Button ascendOneButton;
    public Button ascendTwoButton;

    public Sprite NoAscensionSprite;

    [Header("Check Ascension Window")]
    public Image monsterNewAbility;
    public Image monsterNewCommand;

    public TextMeshProUGUI ascensionTraits;
    public Monster currentAscensionPath;

    public Image monsterBaseImageCheckAscension;
    public Image monsterAscensionCheckAscension;

    public TextMeshProUGUI monsterBaseCheckAscensionText;
    public TextMeshProUGUI monsterAscensionCheckAscensionText;

    public TextMeshProUGUI ascensionRequirements;

    private void Awake()
    {
        adventureManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AdventureManager>();
    }

    public void InitializeInventorySlots()
    {
        // Initialize the inventory slots
        int i = 0;

        // Clear the slot visuals first
        foreach (GameObject slot in inventorySlots)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.itemSlotEquipment = null;
            itemSlot.itemSlotImage.sprite = slot.GetComponentInChildren<DragAndDropItem>().emptySprite;
            itemSlot.GetComponent<Interactable>().ResetInteractable();
        }

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

        // Initialize the monster's current equipment
        i = 0;
        List<Modifier> tempList = currentMonsterEquipment.ListOfModifiers.Where(modifier => modifier.adventureEquipment == true).ToList();

        // Clear the slot visuals first
        foreach (GameObject slot in monsterEquipmentSlots)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.itemSlotEquipment = null;
            itemSlot.itemSlotImage.sprite = slot.GetComponentInChildren<DragAndDropItem>().emptySprite;
            itemSlot.RemoveEquipmentText();
        }

        // Assign the equipment
        foreach(GameObject slot in monsterEquipmentSlots)
        {
            if (tempList.Count > i)
            {
                ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
                itemSlot.inventoryManager = GetComponent<InventoryManager>();
                itemSlot.itemSlotEquipment = tempList[i];
                itemSlot.itemSlotImage.sprite = itemSlot.itemSlotEquipment.baseSprite;
                slot.GetComponent<Interactable>().InitiateInteractable(itemSlot.itemSlotEquipment);
                itemSlot.DisplayEquipmentText();
            }

            i++;
        }
    }

    public void InitializeAscensionWindow()
    {
        // Initialize base monster info
        monsterBaseImage.sprite = currentMonsterEquipment.baseSprite;
        monsterBaseText.text =
            ($"<b>{currentMonsterEquipment.name}</b>" +
            $"\nLv.{currentMonsterEquipment.level} | Exp: {currentMonsterEquipment.monsterCurrentExp}/{currentMonsterEquipment.monsterExpToNextLevel}" +
            $"\n\n<b>Ability: {currentMonsterEquipment.monsterAbility.abilityName}</b>" +
            $"\n{currentMonsterEquipment.monsterAbility.abilityDescription}");
        monsterElementsText.text = 
            ($"<b>Elements</b>" +
            $"\n{currentMonsterEquipment.monsterElement.element} / {currentMonsterEquipment.monsterSubElement.element}");

        // Disable ascension buttons before checking 
        ascendOneButton.interactable = false;
        ascendTwoButton.interactable = false;

        // Initialize monster ascension one info
        if (currentMonsterEquipment.firstEvolutionPath != null)
        {
            monsterAscensionOneImage.sprite = currentMonsterEquipment.firstEvolutionPath.baseSprite;
            monsterAscensionOneText.text =
                ($"{currentMonsterEquipment.firstEvolutionPath.ascensionType.ToString()} Ascension");
            ascendOneButton.interactable = true;

            // If level req is met, reveal monster sprite and name
            if (currentMonsterEquipment.level == currentMonsterEquipment.firstEvolutionLevelReq)
            {
                monsterAscensionOneText.text += ($"\n{currentMonsterEquipment.firstEvolutionPath.name}");
            }
            else
            {
                monsterAscensionOneText.text += ($"\n???");
            }
        }
        else
        {
            monsterAscensionOneImage.sprite = NoAscensionSprite;
            monsterAscensionOneText.text =
                ("No Ascension Available...");
        }

        // Initialize monster ascension two info
        if (currentMonsterEquipment.secondEvolutionPath != null)
        {
            monsterAscensionTwoImage.sprite = currentMonsterEquipment.secondEvolutionPath.baseSprite;
            monsterAscensionTwoText.text =
                ($"{currentMonsterEquipment.secondEvolutionPath.ascensionType.ToString()} Ascension");
            ascendTwoButton.interactable = true;

            // If level req is met, reveal monster sprite and name
            if (currentMonsterEquipment.level == currentMonsterEquipment.secondEvolutionLevelReq)
            {
                monsterAscensionTwoText.text += ($"\n{currentMonsterEquipment.secondEvolutionPath.name}");
            }
            else
            {
                monsterAscensionTwoText.text += ($"\n???");
            }
        }
        else
        {
            monsterAscensionTwoImage.sprite = NoAscensionSprite;
            monsterAscensionTwoText.text =
                ("No Ascension Available...");
        }

        // If in battle, disable Check Ascension buttons
        if (adventureManager.lockEquipmentInCombat)
        {
            ascendOneButton.interactable = false;
            ascendTwoButton.interactable = false;
        }
    }

    public void InitializeConfirmAscensionWindow(int ascensionNumber)
    {
        int levelReq = 0;
        Item ascensionMaterial = null;
        int goldReq = 0;

        // Assign the monster ascension path
        if (ascensionNumber == 1)
        {
            currentAscensionPath = currentMonsterEquipment.firstEvolutionPath;
            levelReq = currentMonsterEquipment.firstEvolutionLevelReq;
            ascensionMaterial = currentMonsterEquipment.ascensionOneMaterial;
            goldReq = currentAscensionPath.ascensionGoldRequirement;
        }
        else if (ascensionNumber == 2)
        {
            currentAscensionPath = currentMonsterEquipment.secondEvolutionPath;
            levelReq = currentMonsterEquipment.secondEvolutionLevelReq;
            ascensionMaterial = currentMonsterEquipment.ascensionTwoMaterial;
            goldReq = currentAscensionPath.ascensionGoldRequirement;
        }

        // Display Ascension Traits
        ascensionTraits.text =
            ($"<b>Ascension Traits</b>" +
            $"\nNew Ability: {currentAscensionPath.monsterAbility.abilityName}" +
            $"\nNew Command: {currentAscensionPath.monsterAscensionAttack.monsterAttackName}");

        // Monster Ability Text
        monsterNewAbility.GetComponent<Interactable>().interactableName = currentAscensionPath.monsterAbility.abilityName;
        monsterNewAbility.GetComponent<Interactable>().interactableDescription = ($"{currentAscensionPath.monsterAbility.abilityDescription}");

        // Monster Command Text
        monsterNewCommand.GetComponent<Interactable>().interactableName = ($"{currentAscensionPath.monsterAscensionAttack.monsterAttackName} ({currentAscensionPath.monsterAscensionAttack.monsterElementClass.element}, {currentAscensionPath.monsterAscensionAttack.monsterAttackDamageType})");
        monsterNewCommand.GetComponent<Interactable>().interactableDescription = ($"Base Power: {currentAscensionPath.monsterAscensionAttack.monsterAttackDamage} | Accuracy: {currentAscensionPath.monsterAscensionAttack.monsterAttackAccuracy}" +
            $"\n{currentAscensionPath.monsterAscensionAttack.monsterAttackDescription}");

        // Assign base and ascension sprites and names
        monsterBaseImageCheckAscension.sprite = currentMonsterEquipment.baseSprite;
        monsterBaseCheckAscensionText.text = ($"{currentMonsterEquipment.name}" +
            $"\n{currentMonsterEquipment.monsterElement.element} / {currentMonsterEquipment.monsterSubElement.element}");

        // Show name only if level req is met
        monsterAscensionCheckAscension.sprite = currentAscensionPath.baseSprite;
        if (currentMonsterEquipment.level == levelReq)
        {
            monsterAscensionCheckAscensionText.text = ($"{currentAscensionPath.name}");
        }
        else
        {
            monsterAscensionCheckAscensionText.text = ($"???");
        }

        monsterAscensionCheckAscensionText.text += ($"\n{currentAscensionPath.monsterElement.element} / {currentAscensionPath.monsterSubElement.element}");

        // Show Requirements
        ascensionRequirements.text = ($"<b>Requirements</b>" +
            $"\n{ascensionMaterial.itemName} x 1 ({adventureManager.ListOfInventoryItems.Where(item => item.itemName == ascensionMaterial.itemName).ToList().Count})" +
            $"\n{goldReq} Gold ({adventureManager.playerGold})");
    }
}
