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
    public Monster currentMonster;

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
        List<Modifier> tempList = currentMonster.ListOfModifiers.Where(modifier => modifier.modifierType == Modifier.ModifierType.equipmentModifier).ToList();

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
        currentMonsterEquipmentImage.sprite = currentMonster.baseSprite;
        currentMonsterEquipmentName.text = ($"{currentMonster.name} Lv.{currentMonster.level}");

        if (!currentMonster.monsterIsOwned)
        {
            monsterSelectCounter.text = ($"1 / 1");
            return;
        }

        monsterSelectCounter.text = ($"{adventureManager.ListOfCurrentMonsters.IndexOf(currentMonster) + 1} / {adventureManager.ListOfCurrentMonsters.Count}");

        ResetThenCalculateEquipment();
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
        List<Modifier> tempList = currentMonster.ListOfModifiers.Where(modifier => modifier.modifierType == Modifier.ModifierType.equipmentModifier).ToList();

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
        currentMonsterEquipmentImage.sprite = currentMonster.baseSprite;
        currentMonsterEquipmentName.text = ($"{currentMonster.name} Lv.{currentMonster.level}");

        if (!currentMonster.monsterIsOwned)
        {
            monsterSelectCounter.text = ($"1 / 1");
            return;
        }

        monsterSelectCounter.text = ($"{adventureManager.ListOfCurrentMonsters.IndexOf(currentMonster) + 1} / {adventureManager.ListOfCurrentMonsters.Count}");

        ResetThenCalculateEquipment();
    }

    public void ResetThenCalculateEquipment()
    {
        ResetAllMonsterEquipment();

        CalculateAllMonsterEquipment();
    }

    public void ResetAllMonsterEquipment()
    {
        // Reset flat multipliers
        foreach (Modifier equipment in currentMonster.ListOfModifiers.Where(equipment => equipment.modifierType == Modifier.ModifierType.equipmentModifier).ToList())
        {
            float removedStatValue = equipment.equipmentCachedAmount * -1f;

            AdjustModifiedStatToNewValue(equipment.statModified, currentMonster, removedStatValue);
        }
    }

    public void CalculateAllMonsterEquipment()
    {
        // Add flat multipliers
        foreach (Modifier equipment in currentMonster.ListOfModifiers.Where(equipment => equipment.modifierType == Modifier.ModifierType.equipmentModifier && equipment.modifierAmountFlatBuff == true).ToList())
        {
            //float statModified = ReturnModifiedStat(equipment.statModified, currentMonster);

            equipment.equipmentCachedAmount = equipment.modifierAmount;

            float additionalStatValue = equipment.equipmentCachedAmount;

            AdjustModifiedStatToNewValue(equipment.statModified, currentMonster, additionalStatValue);
        }

        // Add percentage multipliers
        foreach (Modifier equipment in currentMonster.ListOfModifiers.Where(equipment => equipment.modifierType == Modifier.ModifierType.equipmentModifier && equipment.modifierAmountFlatBuff == false).ToList())
        {
            float statModified = ReturnModifiedStat(equipment.statModified, currentMonster);

            float percentageModifierAmount = equipment.modifierAmount / 100f;

            float additionalStatValue = Mathf.RoundToInt(statModified * percentageModifierAmount);

            if (additionalStatValue <= 0)
                additionalStatValue = 1;

            equipment.equipmentCachedAmount = additionalStatValue;

            AdjustModifiedStatToNewValue(equipment.statModified, currentMonster, additionalStatValue);
        }
    }

    // Increment monster in list
    public void NextMonsterEquipment()
    {
        if (!currentMonster.monsterIsOwned)
            return;

        if (adventureManager.ListOfCurrentMonsters.Count != 1 && adventureManager.ListOfCurrentMonsters.IndexOf(currentMonster) + 1 < adventureManager.ListOfCurrentMonsters.Count)
        {
            currentMonster = adventureManager.ListOfCurrentMonsters[adventureManager.ListOfCurrentMonsters.IndexOf(currentMonster) + 1];
            monsterStatScreenScript.currentMonster = currentMonster;
            InitializeInventorySlots();
        }
        else 
        if (adventureManager.ListOfCurrentMonsters.Count != 1 && adventureManager.ListOfCurrentMonsters.IndexOf(currentMonster) + 1 == adventureManager.ListOfCurrentMonsters.Count)
        {
            currentMonster = adventureManager.ListOfCurrentMonsters[0];
            monsterStatScreenScript.currentMonster = currentMonster;
            InitializeInventorySlots();
        }
    }

    // Decrement monster in list
    public void PreviousMonsterEquipment()
    {
        if (!currentMonster.monsterIsOwned)
            return;

        if (adventureManager.ListOfCurrentMonsters.Count != 1 && adventureManager.ListOfCurrentMonsters.IndexOf(currentMonster) - 1 > -1)
        {
            currentMonster = adventureManager.ListOfCurrentMonsters[adventureManager.ListOfCurrentMonsters.IndexOf(currentMonster) - 1];
            monsterStatScreenScript.currentMonster = currentMonster;
            InitializeInventorySlots();
        }
        else
        if (adventureManager.ListOfCurrentMonsters.Count != 1 && adventureManager.ListOfCurrentMonsters.IndexOf(currentMonster) - 1 == -1)
        {
            currentMonster = adventureManager.ListOfCurrentMonsters[adventureManager.ListOfCurrentMonsters.Count - 1];
            monsterStatScreenScript.currentMonster = currentMonster;
            InitializeInventorySlots();
        }
    }

    // Show initial ascension window
    public void InitializeAscensionWindow()
    {
        // Initialize base monster info
        monsterBaseImage.sprite = currentMonster.baseSprite;
        monsterBaseText.text =
            ($"<b>{currentMonster.name}</b>" +
            $"\nLv.{currentMonster.level} | Exp: {currentMonster.monsterCurrentExp}/{currentMonster.monsterExpToNextLevel}");

        // Display monster elements
        if (currentMonster.monsterSubElement.element != ElementClass.MonsterElement.None)
        {
            monsterElementsText.text =
                ($"<b>Elements</b>" +
                $"\n{currentMonster.monsterElement.element} / {currentMonster.monsterSubElement.element}");
        }
        else
        {
            monsterElementsText.text =
                ($"<b>Element</b>" +
                $"\n{currentMonster.monsterElement.element}");
        }

        // Disable ascension buttons before checking 
        ascendOneButton.interactable = false;
        ascendTwoButton.interactable = false;

        // Initialize monster ascension one info
        if (currentMonster.firstEvolutionPath != null)
        {
            monsterAscensionOneImage.sprite = currentMonster.firstEvolutionPath.baseSprite;
            monsterAscensionOneText.text =
                ($"{currentMonster.firstEvolutionPath.ascensionType.ToString()} Ascension");
            ascendOneButton.interactable = true;

            // If level req is met, reveal monster sprite and name
            if (currentMonster.level == currentMonster.firstEvolutionLevelReq)
            {
                monsterAscensionOneText.text += ($"\n{currentMonster.firstEvolutionPath.name}");
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
        if (currentMonster.secondEvolutionPath != null)
        {
            monsterAscensionTwoImage.sprite = currentMonster.secondEvolutionPath.baseSprite;
            monsterAscensionTwoText.text =
                ($"{currentMonster.secondEvolutionPath.ascensionType.ToString()} Ascension");
            ascendTwoButton.interactable = true;

            // If level req is met, reveal monster sprite and name
            if (currentMonster.level == currentMonster.secondEvolutionLevelReq)
            {
                monsterAscensionTwoText.text += ($"\n{currentMonster.secondEvolutionPath.name}");
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

        adventureManager.NewUIScreenSelected();

        // Assign the monster ascension path
        if (ascensionNumber == 1)
        {
            currentAscensionPath = currentMonster.firstEvolutionPath;
            levelReq = currentMonster.firstEvolutionLevelReq;
            ascensionMaterial = currentMonster.ascensionOneMaterial;
            goldReq = currentAscensionPath.ascensionGoldRequirement;
        }
        else if (ascensionNumber == 2)
        {
            currentAscensionPath = currentMonster.secondEvolutionPath;
            levelReq = currentMonster.secondEvolutionLevelReq;
            ascensionMaterial = currentMonster.ascensionTwoMaterial;
            goldReq = currentAscensionPath.ascensionGoldRequirement;
        }

        // Display Ascension Traits
        ascensionTraits.text =
            ($"New Ability: {currentAscensionPath.monsterAbility.abilityName}" +
            $"\nNew Attack: {currentAscensionPath.monsterAscensionAttack.monsterAttackName}");

        // Monster Ability Text
        monsterNewAbility.GetComponent<Interactable>().interactableName = currentAscensionPath.monsterAbility.abilityName;
        monsterNewAbility.GetComponent<Interactable>().interactableDescription = ($"{currentAscensionPath.monsterAbility.abilityDescription}");

        // Monster Command Text
        monsterNewCommand.GetComponent<Interactable>().interactableName = ($"{currentAscensionPath.monsterAscensionAttack.monsterAttackName} ({currentAscensionPath.monsterAscensionAttack.monsterElementClass.element}, {currentAscensionPath.monsterAscensionAttack.monsterAttackDamageType})");
        monsterNewCommand.GetComponent<Interactable>().interactableDescription = ($"Base Power: {currentAscensionPath.monsterAscensionAttack.baseDamage} | Accuracy: {currentAscensionPath.monsterAscensionAttack.monsterAttackAccuracy}" +
            $"\n{currentAscensionPath.monsterAscensionAttack.monsterAttackDescription}");

        // Assign base and ascension sprites and names
        monsterBaseImageCheckAscension.sprite = currentMonster.baseSprite;
        monsterBaseCheckAscensionText.text = ($"{currentMonster.name}");

        // Display monster elements
        if (currentMonster.monsterSubElement.element != ElementClass.MonsterElement.None)
        {
            monsterBaseCheckAscensionText.text += ($"\n{currentMonster.monsterElement.element} / {currentMonster.monsterSubElement.element}");
        }
        else
        {
            monsterBaseCheckAscensionText.text += ($"\n{currentMonster.monsterElement.element}");
        }

        // Show name only if level req is met
        monsterAscensionCheckAscension.sprite = currentAscensionPath.baseSprite;
        if (currentMonster.level == levelReq)
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
            monsterAscensionCheckAscensionText.text += ($"\n{currentAscensionPath.monsterElement.element} / {currentAscensionPath.monsterSubElement.element}");
        }
        else
        {
            monsterAscensionCheckAscensionText.text += ($"\n{currentAscensionPath.monsterElement.element}");
        }

        // Show Requirements
        ascensionRequirements.text =
            ($"Lv.{levelReq} ({currentMonster.level})"+
            $"\n{ascensionMaterial.itemName} x 1 ({adventureManager.ListOfInventoryItems.Where(item => item.itemName == ascensionMaterial.itemName).ToList().Count})" +
            $"\n{goldReq} Gold ({adventureManager.playerGold})");

        // Check requirements
        levelReqImage.sprite = levelReq == currentMonster.level ? requirementMetSprite : requirementUnmetSprite;
        matReqImage.sprite = adventureManager.ListOfInventoryItems.Where(item => item.itemName == ascensionMaterial.itemName).ToList().Count >= 1 ? requirementMetSprite : requirementUnmetSprite;
        goldReqImage.sprite = goldReq <= adventureManager.playerGold ? requirementMetSprite : requirementUnmetSprite;

        // Enable ascension if reqs met
        if (levelReq == currentMonster.level)
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
            ($"{currentMonster.maxHealth} -> {currentMonster.maxHealth + currentAscensionPath.healthGrowth} (<b>+{currentAscensionPath.healthGrowth}</b>)" +

            $"\n{currentMonster.physicalAttack} -> {currentMonster.physicalAttack + currentAscensionPath.physicalAttackGrowth} (<b>{ReturnSign(currentAscensionPath.physicalAttackGrowth)}{currentAscensionPath.physicalAttackGrowth}</b>)" +
            $"\n{currentMonster.magicAttack} -> {currentMonster.magicAttack + currentAscensionPath.magicAttackGrowth} (<b>{ReturnSign(currentAscensionPath.magicAttackGrowth)}{currentAscensionPath.magicAttackGrowth}</b>)" +

            $"\n{currentMonster.physicalDefense} -> {currentMonster.physicalDefense + currentAscensionPath.physicalDefenseGrowth} (<b>{ReturnSign(currentAscensionPath.physicalDefenseGrowth)}{currentAscensionPath.physicalDefenseGrowth}</b>)" +
            $"\n{currentMonster.magicDefense} -> {currentMonster.magicDefense + currentAscensionPath.magicDefenseGrowth} (<b>{ReturnSign(currentAscensionPath.magicDefenseGrowth)}{currentAscensionPath.magicDefenseGrowth}</b>)" +

            $"\n{currentMonster.speed} -> {currentMonster.speed + currentAscensionPath.speedGrowth} (<b>{ReturnSign(currentAscensionPath.speedGrowth)}{currentAscensionPath.speedGrowth}</b>)" +

            $"\n{currentMonster.evasion} -> {currentMonster.evasion + currentAscensionPath.evasionGrowth} (<b>{ReturnSign(currentAscensionPath.evasionGrowth)}{currentAscensionPath.evasionGrowth}</b>)" +
            $"\n{currentMonster.critChance} -> {currentMonster.critChance + currentAscensionPath.critChanceGrowth} (<b>{ReturnSign(currentAscensionPath.critChanceGrowth)}{currentAscensionPath.critChanceGrowth}</b>)" +
            $"\n{currentMonster.bonusAccuracy} -> {currentMonster.bonusAccuracy + currentAscensionPath.bonusAccuracyGrowth} (<b>{ReturnSign(currentAscensionPath.bonusAccuracyGrowth)}{currentAscensionPath.bonusAccuracyGrowth}</b>)");
    }

    // Play monster ascension animation
    public void BeginMonsterAscension()
    {
        ascendingMonsterSprite.sprite = currentMonster.baseSprite;
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
            newMonster.ListOfMonsterAttacks[i] = currentMonster.ListOfMonsterAttacks[i];
        }

        // Copy equipment of previous monster
        int equipmentCount = currentMonster.ListOfModifiers.Where(equipment => equipment.modifierType == Modifier.ModifierType.equipmentModifier).ToList().Count;

        for (int i = 0; i < equipmentCount; i++)
        {
            newMonster.ListOfModifiers.Add(currentMonster.ListOfModifiers.Where(equipment => equipment.modifierType == Modifier.ModifierType.equipmentModifier).ToList()[i]);
            currentMonster.ListOfModifiers.Where(equipment => equipment.modifierType == Modifier.ModifierType.equipmentModifier).ToList()[i].modifierOwner = newMonster;
        }

        // Update current monster reference
        adventureManager.ListOfCurrentMonsters[adventureManager.ListOfCurrentMonsters.IndexOf(currentMonster)] = newMonster;
        adventureManager.ListOfAllMonsters[adventureManager.ListOfAllMonsters.IndexOf(currentMonster)] = newMonster;

        // Store reference
        preAscensionReference = currentMonster;

        monsterStatScreenScript.currentMonster = newMonster;
        currentMonster = newMonster;

        // Show continue button
        continueToAfterAscensionButton.SetActive(true);
    }

    // Show Ascended monster screen
    public void ShowAscendedMonsterWindow()
    {
        // Display image and text
        ascendedMonster.sprite = currentMonster.baseSprite;
        ascendedMonsterText.text = 
            ($"{preAscensionReference.name} ascended into {currentMonster.name}!" +
            $"\n\nReplace an existing attack with {currentMonster.monsterAscensionAttack.monsterAttackName}?");

        // Display new command
        DragCommandController newAttackHolder = newCommandHolder.GetComponent<DragCommandController>();
        newAttackHolder.monsterAttackReference = currentMonster.monsterAscensionAttack;

        newAttackHolder.GetComponent<Interactable>().interactableName = newAttackHolder.monsterAttackReference.monsterAttackName;
        newAttackHolder.GetComponent<Interactable>().interactableDescription = newAttackHolder.monsterAttackReference.monsterAttackDescription;

        newAttackHolder.GetComponentInChildren<TextMeshProUGUI>().text =
            ($"{newAttackHolder.monsterAttackReference.monsterAttackName}" +
            $"\nElement: {newAttackHolder.monsterAttackReference.monsterElementClass.element.ToString()} | Type: {newAttackHolder.monsterAttackReference.monsterAttackDamageType}");

        if (newAttackHolder.monsterAttackReference.monsterAttackType == MonsterAttack.MonsterAttackType.Attack)
        {
            newAttackHolder.GetComponentInChildren<TextMeshProUGUI>().text +=
                ($"\nBase Power: {newAttackHolder.monsterAttackReference.baseDamage} | Accuracy: {newAttackHolder.monsterAttackReference.monsterAttackAccuracy}%");
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
            dragController.monsterAttackReference = currentMonster.ListOfMonsterAttacks[i];

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
                    ($"\nBase Power: {dragController.monsterAttackReference.baseDamage} | Accuracy: {dragController.monsterAttackReference.monsterAttackAccuracy}%");
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

    public float ReturnModifiedStat(AttackEffect.StatToChange modifiedStat, Monster currentMonster)
    {
        switch (modifiedStat)
        {
            case (AttackEffect.StatToChange.PhysicalAttack):
                return currentMonster.physicalAttack;

            case (AttackEffect.StatToChange.MagicAttack):
                return currentMonster.magicAttack;

            case (AttackEffect.StatToChange.PhysicalDefense):
                return currentMonster.physicalDefense;

            case (AttackEffect.StatToChange.MagicDefense):
                return currentMonster.magicDefense;

            case (AttackEffect.StatToChange.Speed):
                return currentMonster.speed;

            case (AttackEffect.StatToChange.Evasion):
                return currentMonster.evasion;

            case (AttackEffect.StatToChange.Accuracy):
                return currentMonster.bonusAccuracy;

            case (AttackEffect.StatToChange.CritChance):
                return currentMonster.critChance;

            case (AttackEffect.StatToChange.CritDamage):
                return currentMonster.critDamage;

            case (AttackEffect.StatToChange.MaxHealth):
                return currentMonster.maxHealth;

            case (AttackEffect.StatToChange.MaxSP):
                return currentMonster.maxSP;

            case (AttackEffect.StatToChange.SPRegen):
                return currentMonster.spRegen;

            default:
                Debug.Log("Missing stat or monster reference?", this);
                return 0;
        }
    }

    public void AdjustModifiedStatToNewValue(AttackEffect.StatToChange modifiedStat, Monster currentMonster, float additionalStatValue)
    {
        switch (modifiedStat)
        {
            case (AttackEffect.StatToChange.PhysicalAttack):
                currentMonster.physicalAttack += additionalStatValue;
                break;

            case (AttackEffect.StatToChange.MagicAttack):
                currentMonster.magicAttack += additionalStatValue;
                break;

            case (AttackEffect.StatToChange.PhysicalDefense):
                currentMonster.physicalDefense += additionalStatValue;
                break;

            case (AttackEffect.StatToChange.MagicDefense):
                currentMonster.magicDefense += additionalStatValue;
                break;

            case (AttackEffect.StatToChange.Speed):
                currentMonster.speed += additionalStatValue;
                break;

            case (AttackEffect.StatToChange.Evasion):
                currentMonster.evasion += additionalStatValue;
                break;

            case (AttackEffect.StatToChange.Accuracy):
                currentMonster.bonusAccuracy += additionalStatValue;
                break;

            case (AttackEffect.StatToChange.CritChance):
                currentMonster.critChance += additionalStatValue;
                break;

            case (AttackEffect.StatToChange.CritDamage):
                currentMonster.critDamage += additionalStatValue;
                break;

            case (AttackEffect.StatToChange.MaxHealth):
                currentMonster.maxHealth += additionalStatValue;
                break;

            case (AttackEffect.StatToChange.MaxSP):
                currentMonster.maxSP += additionalStatValue;
                break;

            case (AttackEffect.StatToChange.SPRegen):
                currentMonster.spRegen += additionalStatValue;
                break;

            default:
                Debug.Log("Missing stat or monster reference?", this);
                break;
        }
    }
}
