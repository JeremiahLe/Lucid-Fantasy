using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MonstersSubScreenManager : MonoBehaviour
{
    public List<GameObject> listOfMonsterSlots;
    public AdventureManager adventureManager;
    public TextMeshProUGUI monsterAmountText;

    public void ShowAvailableMonsters()
    {
        monsterAmountText.text =
            ($"Monsters: ({adventureManager.ListOfCurrentMonsters.Count}/4)" +
            $"\n[Right-click for more info.]");

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
                monsterSlot.GetComponentInChildren<TextMeshProUGUI>().text = ($"{monsterSlot.GetComponent<CreateReward>().monsterReward.name} Lvl.{monsterSlot.GetComponent<CreateReward>().monsterReward.level}" +
                    $"\nHP: {monsterSlot.GetComponent<CreateReward>().monsterReward.health}/{monsterSlot.GetComponent<CreateReward>().monsterReward.maxHealth}" +
                    $"\n{monsterSlot.GetComponent<CreateReward>().monsterReward.monsterElement.element.ToString()}/{monsterSlot.GetComponent<CreateReward>().monsterReward.monsterSubElement.element.ToString()}" +
                        $"\n- {monsterSlot.GetComponent<CreateReward>().monsterReward.ListOfMonsterAttacks[0].monsterAttackName}" +
                        $"\n- {monsterSlot.GetComponent<CreateReward>().monsterReward.ListOfMonsterAttacks[1].monsterAttackName}" +
                        $"\n- {monsterSlot.GetComponent<CreateReward>().monsterReward.ListOfMonsterAttacks[2].monsterAttackName}" +
                        $"\n- {monsterSlot.GetComponent<CreateReward>().monsterReward.ListOfMonsterAttacks[3].monsterAttackName}");
            }

            i++;
        }
    }

    public void ShowAvailableModifiers()
    {
        // Hide monster slots
        foreach (GameObject monsterSlot in listOfMonsterSlots)
        {
            monsterSlot.SetActive(false);
        }

        monsterAmountText.text =
            ($"Current Modifiers:");

        foreach (Modifier modifier in adventureManager.ListOfCurrentModifiers)
        {
            monsterAmountText.text +=
               ($"\n<b>{modifier.modifierName}</b> - {modifier.modifierDescription}");
        }
    }
}
