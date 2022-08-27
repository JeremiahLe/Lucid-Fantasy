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

    public TextMeshProUGUI monsterAbilityDescription;
    public TextMeshProUGUI monsterFlavourText;

    public TextMeshProUGUI monsterAttacks;
    public TextMeshProUGUI monsterEquipment;

    public Slider monsterExpBar;

    public void DisplayMonsterStatScreenStats(Monster monster)
    {
        // Display monster image
        monsterImage.sprite = monster.baseSprite;

        // Display monster name, level and current exp
        monsterNameAndExp.text =
            ($"{monster.name} Lvl.{monster.level}" +
            $"\nExp: {monster.monsterCurrentExp}/{monster.monsterExpToNextLevel}");

        // Display monster ability and description
        monsterAbilityDescription.text =
            ($"Ability: {monster.monsterAbility.abilityName}" +
            $"\n{monster.monsterAbility.abilityTriggerTime}: {monster.monsterAbility.abilityDescription}");

        // Display monster flavour text
        monsterFlavourText.text = ($"{monster.monsterFlavourText}");

        // Display monster Exp Bar fill
        monsterExpBar.value = monster.monsterCurrentExp;
        monsterExpBar.maxValue = monster.monsterExpToNextLevel;

        monsterExpBar.value = monster.monsterCurrentExp;
        monsterExpBar.maxValue = monster.monsterExpToNextLevel;

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
    }
}
