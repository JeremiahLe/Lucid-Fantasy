using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SubscreenManager : MonoBehaviour
{
    public GameObject RewardSlotOne;
    public GameObject RewardSlotTwo;
    public GameObject RewardSlotThree;

    public GameObject ReturnToMainMenuButton;
    public GameObject RerollButton;
    public GameObject FightButton;

    public AdventureManager.RewardType thisRewardType;

    public List<GameObject> listOfMonsterSlots;
    public List<GameObject> listOfRewardSlots;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI summaryText;
    public TextMeshProUGUI monsterListText;
    public TextMeshProUGUI rerollsLeftText;

    public GameObject BattleImage;
    public Sprite mysteryIcon;

    public AdventureManager adventureManager;

    public int randomBattleMonsterCount;
    public int randomBattleMonsterLimit;

    //
    public void Awake()
    {
        adventureManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AdventureManager>();
    }

    // load reward slots
    public void LoadRewardSlots(AdventureManager.RewardType rewardType)
    {
        rerollsLeftText.text = ($"Rerolls left: {adventureManager.rerollAmount}");
        RerollButton.SetActive(true);

        switch (rewardType)
        {
            case AdventureManager.RewardType.Monster:
                foreach (GameObject rewardSlot in listOfRewardSlots)
                {
                    rewardSlot.SetActive(true);
                    thisRewardType = rewardType;
                    Monster monster = GetRandomMonster();
                    rewardSlot.GetComponent<CreateReward>().subscreenManager = this;
                    rewardSlot.GetComponent<CreateReward>().adventureManager = adventureManager;
                    rewardSlot.GetComponent<CreateReward>().rewardType = AdventureManager.RewardType.Monster;
                    rewardSlot.GetComponent<CreateReward>().monsterReward = monster;
                    rewardSlot.GetComponent<CreateReward>().rewardImage.sprite = monster.baseSprite;
                    rewardSlot.GetComponent<CreateReward>().rewardName.text = ($"{monster.name} Lvl.{monster.level}" +
                        $"\n{monster.monsterElement}/{monster.monsterSubElement}" +
                        $"\n- {monster.ListOfMonsterAttacks[0].monsterAttackName}" +
                        $"\n- {monster.ListOfMonsterAttacks[1].monsterAttackName}" +
                        $"\n- {monster.ListOfMonsterAttacks[2].monsterAttackName}" +
                        $"\n- {monster.ListOfMonsterAttacks[3].monsterAttackName}");
                }
                break;

            case AdventureManager.RewardType.Modifier:
                foreach (GameObject rewardSlot in listOfRewardSlots)
                {
                    rewardSlot.SetActive(true);
                    thisRewardType = rewardType;
                    Modifier modifier = GetRandomModifier();
                    rewardSlot.GetComponent<CreateReward>().subscreenManager = this;
                    rewardSlot.GetComponent<CreateReward>().adventureManager = adventureManager;
                    rewardSlot.GetComponent<CreateReward>().rewardType = AdventureManager.RewardType.Modifier;
                    rewardSlot.GetComponent<CreateReward>().modifierReward = modifier;
                    rewardSlot.GetComponent<CreateReward>().rewardImage.sprite = modifier.baseSprite;
                    rewardSlot.GetComponent<CreateReward>().rewardName.text = ($"{modifier.modifierName}" +
                        $"\n- {modifier.modifierDescription}");
                }
                break;

            default:
                break;
        }
    }

    //
    public void HideRewardSlots()
    {
        rerollsLeftText.text = ("");
        RerollButton.SetActive(false);

        foreach (GameObject rewardSlot in listOfRewardSlots)
        {
            rewardSlot.SetActive(false);
        }
    }

    //
    public void LoadRandomBattle()
    {
        FightButton.SetActive(true);
        FightButton.GetComponent<CreateReward>().subscreenManager = this;
        FightButton.GetComponent<CreateReward>().adventureManager = adventureManager;

        randomBattleMonsterLimit = GetRandomBattleMonsterLimit();
        randomBattleMonsterCount = GetRandomBattleMonsterCount();

        adventureManager.randomBattleMonsterCount = randomBattleMonsterCount;
        adventureManager.randomBattleMonsterLimit = randomBattleMonsterLimit;

        BattleImage.SetActive(true);
        BattleImage.GetComponent<Image>().sprite = mysteryIcon;
        BattleImage.GetComponentInChildren<TextMeshProUGUI>().text = ($"Monsters in Battle: Random" +
            $"\nEnemies present: {randomBattleMonsterCount}" +
            $"\nAllies allowed: {randomBattleMonsterLimit}");

        bool bossAdded = false;
        // populate enemy list
        for (int j = 0; j < randomBattleMonsterCount; j++)
        {
            if (adventureManager.currentSelectedNode.GetComponent<CreateNode>().nodeType == CreateNode.NodeType.Boss && !bossAdded)
            {
                adventureManager.ListOfEnemyBattleMonsters.Add(GetBossMonster(adventureManager.adventureBoss, 10));
                BattleImage.GetComponent<Image>().sprite = adventureManager.adventureBoss.baseSprite;
                BattleImage.GetComponentInChildren<TextMeshProUGUI>().text = ($"Monsters in Battle: Boss + Random" +
                $"\nEnemies present: {randomBattleMonsterCount}" +
                $"\nAllies allowed: {randomBattleMonsterLimit}");
                bossAdded = true;
                continue;
            }

            adventureManager.ListOfEnemyBattleMonsters.Add(GetRandomMonster());
        }

        // Show allied monsters to choose from
        ShowAlliedMonstersAvailable();
    }

    // TODO - Also show dead monsters?
    public void ShowAlliedMonstersAvailable()
    {
        // Show allied monsters
        int i = 0;
        foreach (GameObject monsterSlot in listOfMonsterSlots)
        {
            if (adventureManager.ListOfCurrentMonsters.Count > i)
            {
                monsterSlot.SetActive(true);
                monsterSlot.GetComponent<CreateReward>().adventureManager = adventureManager;
                monsterSlot.GetComponent<CreateReward>().subscreenManager = this;
                monsterSlot.GetComponent<CreateReward>().monsterReward = adventureManager.ListOfCurrentMonsters[i];
                monsterSlot.GetComponent<CreateReward>().rewardImage.sprite = monsterSlot.GetComponent<CreateReward>().monsterReward.baseSprite;
                monsterSlot.GetComponentInChildren<TextMeshProUGUI>().text = ($"{monsterSlot.GetComponent<CreateReward>().monsterReward.name} Lvl.{monsterSlot.GetComponent<CreateReward>().monsterReward.level}" +
                    $"\nHP: {monsterSlot.GetComponent<CreateReward>().monsterReward.health}/{monsterSlot.GetComponent<CreateReward>().monsterReward.maxHealth}" +
                    $"\n{monsterSlot.GetComponent<CreateReward>().monsterReward.monsterElement}/{monsterSlot.GetComponent<CreateReward>().monsterReward.monsterSubElement}" +
                        $"\n- {monsterSlot.GetComponent<CreateReward>().monsterReward.ListOfMonsterAttacks[0].monsterAttackName}" +
                        $"\n- {monsterSlot.GetComponent<CreateReward>().monsterReward.ListOfMonsterAttacks[1].monsterAttackName}" +
                        $"\n- {monsterSlot.GetComponent<CreateReward>().monsterReward.ListOfMonsterAttacks[2].monsterAttackName}" +
                        $"\n- {monsterSlot.GetComponent<CreateReward>().monsterReward.ListOfMonsterAttacks[3].monsterAttackName}");
            }

            i++;
        }
    }

    //
    public int GetRandomBattleMonsterCount()
    {
        if (randomBattleMonsterLimit == 1)
        {
            return 1;
        }

        return Random.Range(2, 4);
    }

    //
    public int GetRandomBattleMonsterLimit()
    {
        if (adventureManager.ListOfCurrentMonsters.Count == 1)
        {
            return 1;
        }

        return 2;
    }

    //
    public Monster GetRandomMonster()
    {
        Monster randMonster = Instantiate(adventureManager.ListOfAvailableRewardMonsters[Random.Range(0, adventureManager.ListOfAvailableRewardMonsters.Count)]);

        // random moves
        for (int i = 0; i < 4; i++)
        {
            MonsterAttack randomAttack = randMonster.ListOfMonsterAttacksAvailable[Random.Range(0, randMonster.ListOfMonsterAttacksAvailable.Count)];
            randMonster.ListOfMonsterAttacks[i] = randomAttack;
            randMonster.ListOfMonsterAttacksAvailable.Remove(randomAttack);
        }

        // random stats 
        randMonster.level = Random.Range(5, 9);
        randMonster.health = Mathf.RoundToInt(randMonster.health + randMonster.level);
        randMonster.maxHealth = randMonster.health;

        randMonster.physicalAttack = Mathf.RoundToInt((randMonster.physicalAttack + randMonster.level - 5) * randMonster.physicalAttackScaler);

        randMonster.magicAttack = Mathf.RoundToInt((randMonster.magicAttack + randMonster.level - 5) * randMonster.magicAttackScaler);

        randMonster.physicalDefense = Mathf.RoundToInt((randMonster.physicalDefense + randMonster.level - 5) * randMonster.physicalDefenseScaler);

        randMonster.magicDefense = Mathf.RoundToInt((randMonster.magicDefense + randMonster.level - 5) * randMonster.magicDefenseScaler);

        randMonster.speed = Mathf.RoundToInt((randMonster.speed + randMonster.level - 5) * randMonster.speedScaler);

        return randMonster;
    }

    //

    public Monster GetBossMonster(Monster setMonster, int level)
    {
        Monster newMonster = Instantiate(setMonster);

        // random moves
        for (int i = 0; i < 4; i++)
        {
            MonsterAttack randomAttack = newMonster.ListOfMonsterAttacksAvailable[Random.Range(0, newMonster.ListOfMonsterAttacksAvailable.Count)];
            newMonster.ListOfMonsterAttacks[i] = randomAttack;
            newMonster.ListOfMonsterAttacksAvailable.Remove(randomAttack);
        }

        // bonus stats
        newMonster.level = level;
        newMonster.health += Mathf.RoundToInt(newMonster.health * .5f);
        newMonster.maxHealth = newMonster.health;

        newMonster.physicalAttack = Mathf.RoundToInt((newMonster.physicalAttack + newMonster.level - 5) * newMonster.physicalAttackScaler);

        newMonster.magicAttack = Mathf.RoundToInt((newMonster.magicAttack + newMonster.level - 5) * newMonster.magicAttackScaler);

        newMonster.physicalDefense = Mathf.RoundToInt((newMonster.physicalDefense + newMonster.level - 5) * newMonster.physicalDefenseScaler);

        newMonster.magicDefense = Mathf.RoundToInt((newMonster.magicDefense + newMonster.level - 5) * newMonster.magicDefenseScaler);

        newMonster.speed = Mathf.RoundToInt((newMonster.speed + newMonster.level - 5) * newMonster.speedScaler);
        newMonster.name += ($" (Boss)")
;
        return newMonster;
    }

    //
    public Modifier GetRandomModifier()
    {
        if (adventureManager.ListOfAvailableRewardModifiers.Count == 0)
        {
            return null; 
        }

        Modifier randModifier = adventureManager.ListOfAvailableRewardModifiers[Random.Range(0, adventureManager.ListOfAvailableRewardModifiers.Count)];
        adventureManager.ListOfAvailableRewardModifiers.Remove(randModifier);
        Modifier randModifierSO = Instantiate(randModifier);

        return randModifierSO;
    }


    //
    public void ShowFinalResultsMenu(bool Win)
    {
        // Get ADVENTURE TIME
        adventureManager.timeStarted = false;
        int minutes = Mathf.FloorToInt(adventureManager.adventureTimer / 60F);
        int seconds = Mathf.FloorToInt(adventureManager.adventureTimer - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        ReturnToMainMenuButton.SetActive(true);
        BattleImage.SetActive(true);
        BattleImage.GetComponent<Button>().enabled = false;

        Monster monster = adventureManager.GetMVPMonster();
        BattleImage.GetComponent<Image>().sprite = monster.baseSprite;
        BattleImage.GetComponentInChildren<TextMeshProUGUI>().text =
            ($"MVP: {monster.name}" +
            $"\nDamage Done: {monster.cachedDamageDone}" +
            $"\nKills: {monster.monsterKills}");

        summaryText.text = ($"Adventure Summary: " +
            $"\nTime: {niceTime}" +
            $"\nGold Spent: {adventureManager.playerGoldSpent}" +
            $"\nRerolls: {adventureManager.timesRerolled}" +
            $"\nAlly Monsters Defeated: {adventureManager.playerMonstersLost}" +
            $"\nEnemy Monsters Defeated: {adventureManager.playerMonstersKilled}" +
            $"\nModifiers: ");

        foreach(Modifier modifier in adventureManager.ListOfCurrentModifiers)
        {
            summaryText.text += ($"{modifier.modifierName}\n");
        }

        if (Win)
        {
            adventureManager.PlayNewBGM(adventureManager.winBGM, .60f);
            titleText.text = "Adventure Completed!";
        }
        else
        {
            titleText.text = "Adventure Failed...";
        }

        // Reset bools
    }


    //
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }

    //
    public void RerollRewards()
    {
        if (adventureManager.rerollAmount >= 1)
        {
            adventureManager.rerollAmount -= 1;
            adventureManager.timesRerolled += 1;

            if (thisRewardType == AdventureManager.RewardType.Modifier)
            {
                adventureManager.ResetModifierList();
            }

            LoadRewardSlots(thisRewardType);
        }
    }
}
