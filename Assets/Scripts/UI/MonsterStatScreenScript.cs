using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterStatScreenScript : MonoBehaviour
{
    public Image monsterImage;
    public TextMeshProUGUI monsterNameAndExp;
    public TextMeshProUGUI monsterBaseStats;
    public TextMeshProUGUI monsterAdvancedStats;

    public GameObject BasicStatsWindow;
    public GameObject AdvancedStatsWindow;
    public TextMeshProUGUI monsterSecondPageStats;

    public TextMeshProUGUI monsterElements;
    public TextMeshProUGUI monsterAbilityDescription;
    public TextMeshProUGUI monsterFlavourText;

    public TextMeshProUGUI monsterAttacks;
    public TextMeshProUGUI monsterEquipment;

    public GameObject StatsWindow;
    public GameObject EquipmentWindow;
    public GameObject AscensionWindow;

    public List<GameObject> monsterAttackHolders;

    public Monster currentMonster;
    public Interactable currentMonsterElementMatchups;

    public MonstersSubScreenManager monstersSubScreenManager;

    public Button BasicStatsButton;
    public Button AdvancedStatsButton;

    public void DisplayMonsterStatsScreen()
    {
        DisplayMonsterStatScreenStats(currentMonster);
    }

    public void UpdateMonsterSubScreen()
    {
        if (monstersSubScreenManager != null && monstersSubScreenManager.gameObject.activeInHierarchy == true)
            monstersSubScreenManager.ShowAvailableMonsters();
    }

    public void DisplayMonsterStatScreenStats(Monster monster)
    {
        // Reset to Stats Window
        StatsWindow.SetActive(true);
        EquipmentWindow.SetActive(false);
        AscensionWindow.SetActive(false);

        // Display monster image
        currentMonster = monster;
        monsterImage.sprite = monster.baseSprite;

        // Display monster name, level and current exp
        monsterNameAndExp.text =
            ($"<b>{monster.name}</b>" +
            $"\nLvl.{monster.level} | Exp: {monster.monsterCurrentExp}/{monster.monsterExpToNextLevel}");

        // Display monster elements
        if (monster.monsterSubElement.element != ElementClass.MonsterElement.None)
        {
            monsterElements.text =
                ($"<b>Elements</b>" +
                $"\n{monster.monsterElement.element.ToString()} / {monster.monsterSubElement.element.ToString()}");
        }
        else
        {
            monsterElements.text =
                ($"<b>Element</b>" +
                $"\n{monster.monsterElement.element.ToString()}");
        }

        // Display monster ability and description
        monsterAbilityDescription.text =
            ($"<b>Ability: {monster.monsterAbility.abilityName}</b>" +
            $"\n{monster.monsterAbility.abilityDescription}");

        // Display monster flavour text
        monsterFlavourText.text = ($"{monster.monsterFlavourText}");

        //// Display monster elemental weaknesses- make sure it doesn't override resistances
        currentMonsterElementMatchups.interactableDescription = ("<b>Weak Against:</b>\n");
        foreach(var element in monster.monsterElement.listOfWeaknesses)
        {
            if (!monster.monsterElement.listOfResistances.Contains(element) && !monster.monsterSubElement.listOfResistances.Contains(element))
            {
                currentMonsterElementMatchups.interactableDescription += ($"{element.ToString()}");
                if (monster.monsterElement.listOfWeaknesses.IndexOf(element) != monster.monsterElement.listOfWeaknesses.Count - 1)
                {
                    currentMonsterElementMatchups.interactableDescription += ($", ");
                }
            }
        }

        //// Display monster elemental resistances - make sure it doesn't override weaknesess
        currentMonsterElementMatchups.interactableDescription += ("\n\n<b>Resists:</b>\n");
        foreach (var element in monster.monsterElement.listOfResistances)
        {
            if (!monster.monsterElement.listOfWeaknesses.Contains(element) && !monster.monsterSubElement.listOfWeaknesses.Contains(element))
            {
                currentMonsterElementMatchups.interactableDescription += ($"{element.ToString()}");
                if (monster.monsterElement.listOfResistances.IndexOf(element) != monster.monsterElement.listOfResistances.Count - 1)
                {
                    currentMonsterElementMatchups.interactableDescription += ($", ");
                }
            }
        }

        ShowBasicStats();

        // Display monster attacks
        int i = 0;
        foreach(GameObject monsterAttackHolder in monsterAttackHolders)
        {
            DragCommandController dragController = monsterAttackHolder.GetComponent<DragCommandController>();
            dragController.monsterAttackReference = currentMonster.ListOfMonsterAttacks[i];

            dragController.GetComponent<Interactable>().interactableName = dragController.monsterAttackReference.monsterAttackName;
            dragController.GetComponent<Interactable>().interactableDescription = dragController.monsterAttackReference.monsterAttackDescription;

            dragController.GetComponentInChildren<TextMeshProUGUI>().text =
                ($"<b>{dragController.monsterAttackReference.monsterAttackName}</b>" +
                $"\nSP: {dragController.monsterAttackReference.monsterAttackSPCost}" +
                $"\nElement: {dragController.monsterAttackReference.monsterElementClass.element} | Type: {dragController.monsterAttackReference.monsterAttackDamageType}");

            if (dragController.monsterAttackReference.monsterAttackType == MonsterAttack.MonsterAttackType.Attack) {
                dragController.GetComponentInChildren<TextMeshProUGUI>().text +=
                    ($"\nBase Power: {dragController.monsterAttackReference.monsterAttackDamageScalar} | Accuracy: {dragController.monsterAttackReference.monsterAttackAccuracy}%");
            }
            else
            {
                dragController.GetComponentInChildren<TextMeshProUGUI>().text +=
                    ($"\nBuff/Debuff Type: {dragController.monsterAttackReference.monsterAttackTargetType} | Accuracy: {dragController.monsterAttackReference.monsterAttackAccuracy}%");
            }

            i++;
        }

        // Don't show enemy buttons
        if (monster.aiType == Monster.AIType.Enemy && GetComponent<InventoryManager>().adventureManager.lockEquipmentInCombat == true)
        {
            InventoryManager inventoryManager = GetComponent<InventoryManager>();
            inventoryManager.equipmentButton.interactable = false;
            inventoryManager.ascensionButton.interactable = false;
        }
        else
        {
            InventoryManager inventoryManager = GetComponent<InventoryManager>();
            inventoryManager.equipmentButton.interactable = true;
            inventoryManager.ascensionButton.interactable = true;
        }
    }

    public void ShowAdvancedStats()
    {
        AdvancedStatsWindow.SetActive(true);
        BasicStatsWindow.SetActive(false);

        BasicStatsButton.enabled = true;
        AdvancedStatsButton.enabled = false;

        monsterSecondPageStats.text =
            ($"x{currentMonster.critDamage}" +
            $"\n{currentMonster.lifeStealPercent}%" +
            $"\n{currentMonster.bonusDamagePercent}%" +
            $"\n{currentMonster.damageReductionPercent}%");

        monsterAdvancedStats.text =
            ($"{currentMonster.currentSP}/{currentMonster.maxSP}" +
            $"\n{currentMonster.spRegen}" +
            $"\n{currentMonster.initialSP}");
    }

    public void ShowBasicStats()
    {
        AdvancedStatsWindow.SetActive(false);

        BasicStatsWindow.SetActive(true);

        BasicStatsButton.enabled = false;
        AdvancedStatsButton.enabled = true;

        // Display monster basic stats
        monsterBaseStats.text =
            ($"{currentMonster.health}/{currentMonster.maxHealth}" +
            $"\n{currentMonster.physicalAttack}" +
            $"\n{currentMonster.magicAttack}" +
            $"\n{currentMonster.physicalDefense}" +
            $"\n{currentMonster.magicDefense}");

        // Display monster advanced stats
        monsterAdvancedStats.text =
            ($"{currentMonster.speed}" +
            $"\n{currentMonster.evasion}%" +
            $"\n{currentMonster.critChance}%" +
            $"\n{currentMonster.bonusAccuracy}%");
    }

    public void NewUIScreenSelected()
    {
        monstersSubScreenManager.adventureManager.NewUIScreenSelected();
    }
}
