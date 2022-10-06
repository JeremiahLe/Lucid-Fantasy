using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("Equipment Window")]
    public Modifier currentDraggedEquipment;
    public Monster currentMonsterEquipment;

    public List<GameObject> inventorySlots;
    public List<GameObject> monsterEquipmentSlots;

    public AdventureManager adventureManager;
    public MonsterStatScreenScript monsterStatScreenScript;

    public Button equipmentButton;
    public Button ascensionButton;

    public Image currentMonsterEquipmentImage;
    public TextMeshProUGUI currentMonsterEquipmentName;
    public TextMeshProUGUI currentMonsterEquipmentStats;
    public TextMeshProUGUI monsterSelectCounter;

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
    public TextMeshProUGUI monsterAscensionStatGrowths;

    public Sprite requirementMetSprite;
    public Sprite requirementUnmetSprite;

    public Image levelReqImage;
    public Image matReqImage;
    public Image goldReqImage;

    public Button confirmAscensionButton;

    [Header("Ascension Window")]
    public Image ascendingMonsterSprite;
    public GameObject continueToAfterAscensionButton;

    [Header("After Ascension Window")]
    public Image ascendedMonster;
    public List<GameObject> monsterAscendedAttackHolders;
    public TextMeshProUGUI ascendedMonsterText;
    public Monster preAscensionReference;
    public GameObject newCommandHolder;

    private void Awake()
    {
        adventureManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AdventureManager>();
        monsterStatScreenScript = GetComponent<MonsterStatScreenScript>();
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
            itemSlot.adventureManager = adventureManager;
            itemSlot.GetComponent<Interactable>().ResetInteractable();
            itemSlot.itemSlotEquipmentStatus = slot.GetComponentInChildren<TextMeshProUGUI>();
            itemSlot.itemSlotEquipmentStatus.text = ("");
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

                if (itemSlot.itemSlotEquipment.modifierOwner != null)
                    itemSlot.itemSlotEquipmentStatus.text = ("E");
                else
                    itemSlot.itemSlotEquipmentStatus.text = ("");
            }

            i++;
        }

        // Initialize the monster's current equipment
        i = 0;
        List<Modifier> tempList = currentMonsterEquipment.ListOfModifiers.Where(modifier => modifier.adventureEquipment == true).ToList();

        //Debug.Log($"tempList count: {tempList.Count}" +
        //    $"\ncurrent monster: {currentMonsterEquipment.name}");

        // Clear the slot visuals first
        foreach (GameObject slot in monsterEquipmentSlots)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.itemSlotEquipment = null;
            slot.GetComponent<Interactable>().ResetInteractable();
            itemSlot.itemSlotImage.sprite = slot.GetComponentInChildren<DragAndDropItem>().emptySprite;
            itemSlot.adventureManager = adventureManager;
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

        // Display current monster and name
        currentMonsterEquipmentImage.sprite = currentMonsterEquipment.baseSprite;
        currentMonsterEquipmentName.text = ($"{currentMonsterEquipment.name} Lv.{currentMonsterEquipment.level}");

        if (!currentMonsterEquipment.monsterIsOwned)
        {
            monsterSelectCounter.text = ($"1 / 1");
            return;
        }

        monsterSelectCounter.text = ($"{adventureManager.ListOfCurrentMonsters.IndexOf(currentMonsterEquipment) + 1} / {adventureManager.ListOfCurrentMonsters.Count}");
    }

    // Override function to play equip animation on recently equipped itemSlot
    public void InitializeInventorySlots(Modifier newItemSlotEquipment)
    {
        // Initialize the inventory slots
        int i = 0;

        // Clear the slot visuals first
        foreach (GameObject slot in inventorySlots)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.itemSlotEquipment = null;
            itemSlot.itemSlotImage.sprite = slot.GetComponentInChildren<DragAndDropItem>().emptySprite;
            itemSlot.adventureManager = adventureManager;
            itemSlot.GetComponent<Interactable>().ResetInteractable();
            itemSlot.itemSlotEquipmentStatus = slot.GetComponentInChildren<TextMeshProUGUI>();
            itemSlot.itemSlotEquipmentStatus.text = ("");
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

                if (itemSlot.itemSlotEquipment.modifierOwner != null)
                    itemSlot.itemSlotEquipmentStatus.text = ("E");
                else
                    itemSlot.itemSlotEquipmentStatus.text = ("");
            }

            i++;
        }

        // Initialize the monster's current equipment
        i = 0;
        List<Modifier> tempList = currentMonsterEquipment.ListOfModifiers.Where(modifier => modifier.adventureEquipment == true).ToList();

        //Debug.Log($"tempList count: {tempList.Count}" +
        //    $"\ncurrent monster: {currentMonsterEquipment.name}");

        // Clear the slot visuals first
        foreach (GameObject slot in monsterEquipmentSlots)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.itemSlotEquipment = null;
            slot.GetComponent<Interactable>().ResetInteractable();
            itemSlot.itemSlotImage.sprite = slot.GetComponentInChildren<DragAndDropItem>().emptySprite;
            itemSlot.adventureManager = adventureManager;
            itemSlot.RemoveEquipmentText();
        }

        // Assign the equipment
        foreach (GameObject slot in monsterEquipmentSlots)
        {
            if (tempList.Count > i)
            {
                ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
                itemSlot.inventoryManager = GetComponent<InventoryManager>();
                itemSlot.itemSlotEquipment = tempList[i];
                itemSlot.itemSlotImage.sprite = itemSlot.itemSlotEquipment.baseSprite;
                slot.GetComponent<Interactable>().InitiateInteractable(itemSlot.itemSlotEquipment);
                itemSlot.DisplayEquipmentText();
                if (itemSlot.itemSlotEquipment == newItemSlotEquipment)
                    itemSlot.TriggerEquipAnimation();
            }

            i++;
        }

        // Display current monster and name
        currentMonsterEquipmentImage.sprite = currentMonsterEquipment.baseSprite;
        currentMonsterEquipmentName.text = ($"{currentMonsterEquipment.name} Lv.{currentMonsterEquipment.level}");

        if (!currentMonsterEquipment.monsterIsOwned)
        {
            monsterSelectCounter.text = ($"1 / 1");
            return;
        }

        monsterSelectCounter.text = ($"{adventureManager.ListOfCurrentMonsters.IndexOf(currentMonsterEquipment) + 1} / {adventureManager.ListOfCurrentMonsters.Count}");
    }

    // Increment monster in list
    public void NextMonsterEquipment()
    {
        if (!currentMonsterEquipment.monsterIsOwned)
            return;

        if (adventureManager.ListOfCurrentMonsters.Count != 1 && adventureManager.ListOfCurrentMonsters.IndexOf(currentMonsterEquipment) + 1 < adventureManager.ListOfCurrentMonsters.Count)
        {
            currentMonsterEquipment = adventureManager.ListOfCurrentMonsters[adventureManager.ListOfCurrentMonsters.IndexOf(currentMonsterEquipment) + 1];
            monsterStatScreenScript.currentMonster = currentMonsterEquipment;
            InitializeInventorySlots();
        }
        else 
        if (adventureManager.ListOfCurrentMonsters.Count != 1 && adventureManager.ListOfCurrentMonsters.IndexOf(currentMonsterEquipment) + 1 == adventureManager.ListOfCurrentMonsters.Count)
        {
            currentMonsterEquipment = adventureManager.ListOfCurrentMonsters[0];
            monsterStatScreenScript.currentMonster = currentMonsterEquipment;
            InitializeInventorySlots();
        }
    }

    // Decrement monster in list
    public void PreviousMonsterEquipment()
    {
        if (!currentMonsterEquipment.monsterIsOwned)
            return;

        if (adventureManager.ListOfCurrentMonsters.Count != 1 && adventureManager.ListOfCurrentMonsters.IndexOf(currentMonsterEquipment) - 1 > -1)
        {
            currentMonsterEquipment = adventureManager.ListOfCurrentMonsters[adventureManager.ListOfCurrentMonsters.IndexOf(currentMonsterEquipment) - 1];
            monsterStatScreenScript.currentMonster = currentMonsterEquipment;
            InitializeInventorySlots();
        }
        else
        if (adventureManager.ListOfCurrentMonsters.Count != 1 && adventureManager.ListOfCurrentMonsters.IndexOf(currentMonsterEquipment) - 1 == -1)
        {
            currentMonsterEquipment = adventureManager.ListOfCurrentMonsters[adventureManager.ListOfCurrentMonsters.Count - 1];
            monsterStatScreenScript.currentMonster = currentMonsterEquipment;
            InitializeInventorySlots();
        }
    }

    // Show initial ascension window
    public void InitializeAscensionWindow()
    {
        // Initialize base monster info
        monsterBaseImage.sprite = currentMonsterEquipment.baseSprite;
        monsterBaseText.text =
            ($"<b>{currentMonsterEquipment.name}</b>" +
            $"\nLv.{currentMonsterEquipment.level} | Exp: {currentMonsterEquipment.monsterCurrentExp}/{currentMonsterEquipment.monsterExpToNextLevel}" +
            $"\n\n<b>Ability: {currentMonsterEquipment.monsterAbility.abilityName}</b>" +
            $"\n{currentMonsterEquipment.monsterAbility.abilityDescription}");

        // Display monster elements
        if (currentMonsterEquipment.monsterSubElement.element != ElementClass.MonsterElement.None)
        {
            monsterElementsText.text =
                ($"<b>Elements</b>" +
                $"\n{currentMonsterEquipment.monsterElement.element.ToString()} / {currentMonsterEquipment.monsterSubElement.element.ToString()}");
        }
        else
        {
            monsterElementsText.text =
                ($"<b>Element</b>" +
                $"\n{currentMonsterEquipment.monsterElement.element.ToString()}");
        }

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

    // Show ascension path window
    public void InitializeConfirmAscensionWindow(int ascensionNumber)
    {
        int levelReq = 0;
        Item ascensionMaterial = null;
        int goldReq = 0;
        int reqsMet = 0;

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
            ($"New Ability: {currentAscensionPath.monsterAbility.abilityName}" +
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
        monsterBaseCheckAscensionText.text = ($"{currentMonsterEquipment.name}");

        // Display monster elements
        if (currentMonsterEquipment.monsterSubElement.element != ElementClass.MonsterElement.None)
        {
            monsterBaseCheckAscensionText.text += ($"\n{currentMonsterEquipment.monsterElement.element.ToString()} / {currentMonsterEquipment.monsterSubElement.element.ToString()}");
        }
        else
        {
            monsterBaseCheckAscensionText.text += ($"\n{currentMonsterEquipment.monsterElement.element.ToString()}");
        }

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

        // Display ascension monster elements
        if (currentAscensionPath.monsterSubElement.element != ElementClass.MonsterElement.None)
        {
            monsterAscensionCheckAscensionText.text += ($"\n{currentAscensionPath.monsterElement.element.ToString()} / {currentAscensionPath.monsterSubElement.element.ToString()}");
        }
        else
        {
            monsterAscensionCheckAscensionText.text += ($"\n{currentAscensionPath.monsterElement.element.ToString()}");
        }

        // Show Requirements
        ascensionRequirements.text =
            ($"Lv.{levelReq} ({currentMonsterEquipment.level})"+
            $"\n{ascensionMaterial.itemName} x 1 ({adventureManager.ListOfInventoryItems.Where(item => item.itemName == ascensionMaterial.itemName).ToList().Count})" +
            $"\n{goldReq} Gold ({adventureManager.playerGold})");

        // Check requirements
        levelReqImage.sprite = levelReq == currentMonsterEquipment.level ? requirementMetSprite : requirementUnmetSprite;
        matReqImage.sprite = adventureManager.ListOfInventoryItems.Where(item => item.itemName == ascensionMaterial.itemName).ToList().Count >= 1 ? requirementMetSprite : requirementUnmetSprite;
        goldReqImage.sprite = goldReq <= adventureManager.playerGold ? requirementMetSprite : requirementUnmetSprite;

        // Enable ascension if reqs met
        if (levelReq == currentMonsterEquipment.level)
            reqsMet++;

        if (adventureManager.ListOfInventoryItems.Where(item => item.itemName == ascensionMaterial.itemName).ToList().Count >= 1)
            reqsMet++;

        if (goldReq <= adventureManager.playerGold)
            reqsMet++;

        if (reqsMet == 3)
        {
            confirmAscensionButton.interactable = true;
        }
        else
        {
            confirmAscensionButton.interactable = false;
        }

        // Show stat growths
        // HP
        // PhysAtk
        // MagAtk
        // PhysDef
        // MagDef
        // Speed
        // Evasion
        // CritChance
        // BonusAccuracy

        monsterAscensionStatGrowths.text =
            ($"{currentMonsterEquipment.maxHealth} -> {currentMonsterEquipment.maxHealth + currentAscensionPath.healthGrowth} (<b>+{currentAscensionPath.healthGrowth}</b>)" +

            $"\n{currentMonsterEquipment.physicalAttack} -> {currentMonsterEquipment.physicalAttack + currentAscensionPath.physicalAttackGrowth} (<b>{ReturnSign(currentAscensionPath.physicalAttackGrowth)}{currentAscensionPath.physicalAttackGrowth}</b>)" +
            $"\n{currentMonsterEquipment.magicAttack} -> {currentMonsterEquipment.magicAttack + currentAscensionPath.magicAttackGrowth} (<b>{ReturnSign(currentAscensionPath.magicAttackGrowth)}{currentAscensionPath.magicAttackGrowth}</b>)" +

            $"\n{currentMonsterEquipment.physicalDefense} -> {currentMonsterEquipment.physicalDefense + currentAscensionPath.physicalDefenseGrowth} (<b>{ReturnSign(currentAscensionPath.physicalDefenseGrowth)}{currentAscensionPath.physicalDefenseGrowth}</b>)" +
            $"\n{currentMonsterEquipment.magicDefense} -> {currentMonsterEquipment.magicDefense + currentAscensionPath.magicDefenseGrowth} (<b>{ReturnSign(currentAscensionPath.magicDefenseGrowth)}{currentAscensionPath.magicDefenseGrowth}</b>)" +

            $"\n{currentMonsterEquipment.speed} -> {currentMonsterEquipment.speed + currentAscensionPath.speedGrowth} (<b>{ReturnSign(currentAscensionPath.speedGrowth)}{currentAscensionPath.speedGrowth}</b>)" +

            $"\n{currentMonsterEquipment.evasion} -> {currentMonsterEquipment.evasion + currentAscensionPath.evasionGrowth} (<b>{ReturnSign(currentAscensionPath.evasionGrowth)}{currentAscensionPath.evasionGrowth}</b>)" +
            $"\n{currentMonsterEquipment.critChance} -> {currentMonsterEquipment.critChance + currentAscensionPath.critChanceGrowth} (<b>{ReturnSign(currentAscensionPath.critChanceGrowth)}{currentAscensionPath.critChanceGrowth}</b>)" +
            $"\n{currentMonsterEquipment.bonusAccuracy} -> {currentMonsterEquipment.bonusAccuracy + currentAscensionPath.bonusAccuracyGrowth} (<b>{ReturnSign(currentAscensionPath.bonusAccuracyGrowth)}{currentAscensionPath.bonusAccuracyGrowth}</b>)");
    }

    // Play monster ascension animation
    public void BeginMonsterAscension()
    {
        ascendingMonsterSprite.sprite = currentMonsterEquipment.baseSprite;
        ascendingMonsterSprite.GetComponent<Animator>().SetBool("Ascend", true);
    }

    // Change monster ascension sprite
    public void ChangeMonsterSprite()
    {
        ascendingMonsterSprite.sprite = currentAscensionPath.baseSprite;
    }

    // Ascend monster
    public void AscendMonster()
    {
        // Change monster in list to ascension monster
        Monster newMonster = Instantiate(currentAscensionPath);
        newMonster.monsterIsOwned = true;

        // Copy moves of previous monster
        for (int i = 0; i < 4; i++)
        {
            newMonster.ListOfMonsterAttacks[i] = currentMonsterEquipment.ListOfMonsterAttacks[i];
        }

        // Copy equipment of previous monster
        int equipmentCount = currentMonsterEquipment.ListOfModifiers.Where(equipment => equipment.adventureEquipment == true).ToList().Count;

        for (int i = 0; i < equipmentCount; i++)
        {
            newMonster.ListOfModifiers.Add(currentMonsterEquipment.ListOfModifiers.Where(equipment => equipment.adventureEquipment == true).ToList()[i]);
            currentMonsterEquipment.ListOfModifiers.Where(equipment => equipment.adventureEquipment == true).ToList()[i].modifierOwner = newMonster;
        }

        // Update current monster reference
        adventureManager.ListOfCurrentMonsters[adventureManager.ListOfCurrentMonsters.IndexOf(currentMonsterEquipment)] = newMonster;
        adventureManager.ListOfAllMonsters[adventureManager.ListOfAllMonsters.IndexOf(currentMonsterEquipment)] = newMonster;

        // Store reference
        preAscensionReference = currentMonsterEquipment;

        monsterStatScreenScript.currentMonster = newMonster;
        currentMonsterEquipment = newMonster;

        // Show continue button
        continueToAfterAscensionButton.SetActive(true);
    }

    // Show Ascended monster screen
    public void ShowAscendedMonsterWindow()
    {
        // Display image and text
        ascendedMonster.sprite = currentMonsterEquipment.baseSprite;
        ascendedMonsterText.text = 
            ($"{preAscensionReference.name} ascended into {currentMonsterEquipment.name}!" +
            $"\n\nReplace an existing command with {currentMonsterEquipment.monsterAscensionAttack.monsterAttackName}?");

        // Display new command
        DragCommandController newAttackHolder = newCommandHolder.GetComponent<DragCommandController>();
        newAttackHolder.monsterAttackReference = currentMonsterEquipment.monsterAscensionAttack;

        newAttackHolder.GetComponent<Interactable>().interactableName = newAttackHolder.monsterAttackReference.monsterAttackName;
        newAttackHolder.GetComponent<Interactable>().interactableDescription = newAttackHolder.monsterAttackReference.monsterAttackDescription;

        newAttackHolder.GetComponentInChildren<TextMeshProUGUI>().text =
            ($"{newAttackHolder.monsterAttackReference.monsterAttackName}" +
            $"\nElement: {newAttackHolder.monsterAttackReference.monsterElementClass.element.ToString()} | Type: {newAttackHolder.monsterAttackReference.monsterAttackDamageType}");

        if (newAttackHolder.monsterAttackReference.monsterAttackType == MonsterAttack.MonsterAttackType.Attack)
        {
            newAttackHolder.GetComponentInChildren<TextMeshProUGUI>().text +=
                ($"\nBase Power: {newAttackHolder.monsterAttackReference.monsterAttackDamage} | Accuracy: {newAttackHolder.monsterAttackReference.monsterAttackAccuracy}%");
        }
        else
        {
            newAttackHolder.GetComponentInChildren<TextMeshProUGUI>().text +=
                ($"\nBuff/Debuff Type: {newAttackHolder.monsterAttackReference.monsterAttackTargetType} | Accuracy: {newAttackHolder.monsterAttackReference.monsterAttackAccuracy}%");
        }

        // Display monster attacks
        int i = 0;
        foreach (GameObject monsterAttackHolder in monsterAscendedAttackHolders)
        {
            // Don't override the new command
            if (i == 4)
                continue;

            // Grab reference to command holder and assign it's monster attack reference
            DragCommandController dragController = monsterAttackHolder.GetComponent<DragCommandController>();
            dragController.monsterAttackReference = currentMonsterEquipment.ListOfMonsterAttacks[i];

            // Initialize the command holder's interactable component
            dragController.GetComponent<Interactable>().interactableName = dragController.monsterAttackReference.monsterAttackName;
            dragController.GetComponent<Interactable>().interactableDescription = dragController.monsterAttackReference.monsterAttackDescription;

            // Display current command's name, element, and damage type
            dragController.GetComponentInChildren<TextMeshProUGUI>().text =
                ($"{dragController.monsterAttackReference.monsterAttackName}" +
                $"\nElement: {dragController.monsterAttackReference.monsterElementClass.element.ToString()} | Type: {dragController.monsterAttackReference.monsterAttackDamageType}");

            // If current command is an attack display its power, otherwise it must be a buff / debuff
            if (dragController.monsterAttackReference.monsterAttackType == MonsterAttack.MonsterAttackType.Attack)
            {
                dragController.GetComponentInChildren<TextMeshProUGUI>().text +=
                    ($"\nBase Power: {dragController.monsterAttackReference.monsterAttackDamage} | Accuracy: {dragController.monsterAttackReference.monsterAttackAccuracy}%");
            }
            else
            {
                dragController.GetComponentInChildren<TextMeshProUGUI>().text +=
                    ($"\nBuff/Debuff Type: {dragController.monsterAttackReference.monsterAttackTargetType} | Accuracy: {dragController.monsterAttackReference.monsterAttackAccuracy}%");
            }

            i++;
        }
    }

    // This function returns a negative or positive sign for text applications
    public string ReturnSign(float stat)
    {
        // if currentStat is smaller than baseStat, must be stat debuffed
        if (stat < 0)
        {
            return "";
        }

        // regular buff
        return "+";
    }
}
