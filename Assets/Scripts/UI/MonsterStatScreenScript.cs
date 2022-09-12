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

    public TextMeshProUGUI monsterElements;
    public TextMeshProUGUI monsterAbilityDescription;
    public TextMeshProUGUI monsterFlavourText;

    public TextMeshProUGUI monsterAttacks;
    public TextMeshProUGUI monsterEquipment;

    public GameObject StatsWindow;
    public GameObject EquipmentWindow;
    public GameObject AscensionWindow;

    public void DisplayMonsterStatScreenStats(Monster monster)
    {
        // Reset to Stats Window
        StatsWindow.SetActive(true);
        EquipmentWindow.SetActive(false);
        AscensionWindow.SetActive(false);

        // Display monster image
        monsterImage.sprite = monster.baseSprite;

        // Display monster name, level and current exp
        monsterNameAndExp.text =
            ($"<b>{monster.name}</b>" +
            $"\nLvl.{monster.level} | Exp: {monster.monsterCurrentExp}/{monster.monsterExpToNextLevel}");

        // Display monster elements
        monsterElements.text =
            ($"<b>Elements</b>" +
            $"\n{monster.monsterElement.element.ToString()} / {monster.monsterSubElement.element.ToString()}");

        // Display monster ability and description
        monsterAbilityDescription.text =
            ($"<b>Ability: {monster.monsterAbility.abilityName}</b>" +
            $"\n{monster.monsterAbility.abilityDescription}");

        // Display monster flavour text
        monsterFlavourText.text = ($"{monster.monsterFlavourText}");

        //// Display monster elemental weaknesses- make sure it doesn't override resistances
        //monsterInfo.text += ("\n\nWeaknesses: ");
        //foreach(var element in monster.monsterElement.listOfWeaknesses)
        //{
        //    if (!monster.monsterElement.listOfResistances.Contains(element) && !monster.monsterSubElement.listOfResistances.Contains(element))
        //    {
        //        monsterInfo.text += ($"{element.ToString()}");
        //        if (monster.monsterElement.listOfWeaknesses.IndexOf(element) != monster.monsterElement.listOfWeaknesses.Count - 1)
        //        {
        //            monsterInfo.text += ($", ");
        //        }
        //    }
        //}

        //// Display monster elemental resistances - make sure it doesn't override weaknesess
        //monsterInfo.text += ("\n\nResistances: ");
        //foreach (var element in monster.monsterElement.listOfResistances)
        //{
        //    if (!monster.monsterElement.listOfWeaknesses.Contains(element) && !monster.monsterSubElement.listOfWeaknesses.Contains(element))
        //    {
        //        monsterInfo.text += ($"{element.ToString()}");
        //        if (monster.monsterElement.listOfResistances.IndexOf(element) != monster.monsterElement.listOfResistances.Count - 1)
        //        {
        //            monsterInfo.text += ($", ");
        //        }
        //    }
        //}

        // Display monster basic stats
        monsterBaseStats.text =
            ($"{monster.health}/{monster.maxHealth}" +
            $"\n{monster.physicalAttack}" +
            $"\n{monster.magicAttack}" +
            $"\n{monster.physicalDefense}" +
            $"\n{monster.magicDefense}");

        // Display monster advanced stats
        monsterAdvancedStats.text =
            ($"{monster.speed}" +
            $"\n{monster.evasion}" +
            $"\n{monster.critChance}" +
            $"\n{monster.bonusAccuracy}");

        //// Display monster attacks
        //monsterAttacks.text = ("- Attacks -");
        //foreach(MonsterAttack attack in monster.ListOfMonsterAttacks)
        //{
        //    monsterAttacks.text += 
        //        ($"\n{attack.monsterAttackName}" +
        //        $"\nPower: {attack.monsterAttackDamage} | Accuracy: {attack.monsterAttackAccuracy}" +
        //        $"\nType: {attack.monsterAttackDamageType} | Element: {attack.monsterAttackElement}" +
        //        $"\n{attack.monsterAttackDescription}\n");
        //}

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
}
