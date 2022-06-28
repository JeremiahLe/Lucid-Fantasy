using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterStatScreenScript : MonoBehaviour
{
    public Image monsterImage;
    public TextMeshProUGUI monsterInfo;
    public TextMeshProUGUI monsterStats;
    public TextMeshProUGUI monsterAttacks;
    public TextMeshProUGUI monsterEquipment;

    public void DisplayMonsterStatScreenStats(Monster monster)
    {
        // Display monster image
        monsterImage.sprite = monster.baseSprite;

        // Display monster level, exp, and ability
        monsterInfo.text =
            ($"{monster.name} Lvl.{monster.level}" +
            $" | Exp: {monster.monsterCurrentExp}/{monster.monsterExpToNextLevel}" +
            $"\nElements: {monster.monsterElement.element.ToString()}/{monster.monsterSubElement.element.ToString()}" +
            $"\n\nAbility: AbilityName" +
            $"\nAbilityDescription");

        // Display monster elemental weaknesses- make sure it doesn't override resistances
        monsterInfo.text += ("\n\nWeaknesses: ");
        foreach(var element in monster.monsterElement.listOfWeaknesses)
        {
            if (!monster.monsterElement.listOfResistances.Contains(element) && !monster.monsterSubElement.listOfResistances.Contains(element))
            {
                monsterInfo.text += ($"{element.ToString()}");
            }
       
            if (monster.monsterElement.listOfWeaknesses.IndexOf(element) != monster.monsterElement.listOfWeaknesses.Count - 1)
            {
                monsterInfo.text += ($", ");
            }
        }

        // Display monster elemental resistances - make sure it doesn't override weaknesess
        monsterInfo.text += ("\n\nResistances: ");
        foreach (var element in monster.monsterElement.listOfResistances)
        {
            if (!monster.monsterElement.listOfWeaknesses.Contains(element) && !monster.monsterSubElement.listOfWeaknesses.Contains(element))
            {
                monsterInfo.text += ($"{element.ToString()}");
            }

            if (monster.monsterElement.listOfResistances.IndexOf(element) != monster.monsterElement.listOfResistances.Count - 1)
            {
                monsterInfo.text += ($", ");
            }
        }

        // Display monster combat stats
        monsterStats.text =
            ($"- Stats -" +
            $"\nHp: {monster.health}/{monster.maxHealth}" +
            $"\nPhysical Attack: {monster.physicalAttack}" +
            $"\nMagic Attack: {monster.magicAttack}" +
            $"\nPhysical Defense: {monster.physicalDefense}" +
            $"\nMagic Defense: {monster.magicDefense}" +
            $"\nSpeed: {monster.speed}" +
            $"\nEvasion: {monster.evasion}" +
            $"\nCritChance: {monster.critChance}");

        // Display monster attacks
        monsterAttacks.text = ("- Attacks -");
        foreach(MonsterAttack attack in monster.ListOfMonsterAttacks)
        {
            monsterAttacks.text += 
                ($"\n{attack.monsterAttackName}" +
                $"\nPower: {attack.monsterAttackDamage} | Accuracy: {attack.monsterAttackAccuracy}" +
                $"\nType: {attack.monsterAttackDamageType} | Element: {attack.monsterAttackElement}" +
                $"\n{attack.monsterAttackDescription}\n");
        }

        // Display monster equipment
        monsterEquipment.text = ("Equipment:");
        foreach (Modifier equipment in monster.ListOfModifiers)
        {
            if (equipment.adventureEquipment)
            {
                monsterEquipment.text +=
                    ($"\n{equipment.modifierName} ({equipment.modifierDescription})");
            }
        }

        if (monster.ListOfModifiers.Count == 0)
        {
            monsterEquipment.text += ("\nNone");
        }
    }
}
