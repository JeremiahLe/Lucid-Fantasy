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
        monsterImage.sprite = monster.baseSprite;

        monsterInfo.text = 
            ($"{monster.name} Lvl.{monster.level}" +
            $" | Exp: {monster.monsterCurrentExp}/{monster.monsterExpToNextLevel}" +
            $"\nElements: {monster.monsterElement}/{monster.monsterSubElement}" +
            $"\nWeak To: None" +
            $"\nResists: None" +
            $"\n\nAbility: AbilityName" +
            $"\nAbilityDescription");

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

        monsterAttacks.text = ("- Attacks -");

        foreach(MonsterAttack attack in monster.ListOfMonsterAttacks)
        {
            monsterAttacks.text += 
                ($"\n{attack.monsterAttackName}" +
                $"\nPower: {attack.monsterAttackDamage} | Accuracy: {attack.monsterAttackAccuracy}" +
                $"\nType: {attack.monsterAttackDamageType} | Element: {attack.monsterAttackElement}" +
                $"\n{attack.monsterAttackDescription}\n");
        }

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
