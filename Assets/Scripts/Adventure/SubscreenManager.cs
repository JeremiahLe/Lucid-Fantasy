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
    public GameObject ConfirmEquipmentButton;
    public GameObject FightButton;
    public GameObject EnterNewGameButton;
    public GameObject RestartGameButton;
    public GameObject SkipButton;

    public AdventureManager.RewardType thisRewardType;

    public List<GameObject> listOfMonsterSlots;
    public List<GameObject> listOfMonsterSlotsEquipment;
    public List<GameObject> listOfRewardSlots;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI summaryText;
    public TextMeshProUGUI monsterListText;
    public TextMeshProUGUI rerollsLeftText;

    public GameObject BattleImage;
    public Sprite mysteryIcon;

    public AdventureManager adventureManager;
    public GameObject monsterStatsWindow;
    public MonsterStatScreenScript monsterStatScreenScript;
    public MonstersSubScreenManager monstersSubScreenManager;

    public int randomBattleMonsterCount;
    public int randomBattleMonsterLimit;
    bool bossAdded = false;

    public void Awake()
    {
        adventureManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AdventureManager>();
        monstersSubScreenManager.adventureManager = adventureManager;
    }

    // This function grabs random rewards and displays them on screen
    public void LoadRewardSlots(AdventureManager.RewardType rewardType)
    {
        rerollsLeftText.text = ($"Rerolls left: {adventureManager.rerollAmount}");
        RerollButton.SetActive(true);
        SkipButton.SetActive(true);

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
                    rewardSlot.GetComponent<CreateReward>().monsterStatScreenScript = monsterStatScreenScript;
                    rewardSlot.GetComponent<CreateReward>().rewardType = AdventureManager.RewardType.Monster;
                    rewardSlot.GetComponent<CreateReward>().monsterReward = monster;
                    rewardSlot.GetComponent<CreateReward>().rewardImage.sprite = monster.baseSprite;
                    rewardSlot.GetComponent<CreateReward>().rewardName.text = ($"<b>{monster.name} Lvl.{monster.level}</b>" +
                        $"\n{monster.monsterElement.element.ToString()}/{monster.monsterSubElement.element.ToString()}" +
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
                    //rewardSlot.GetComponent<CreateReward>().rewardName.text = ($"{modifier.modifierName}" +
                        //$"\n- {modifier.modifierDescription}");
                    rewardSlot.GetComponent<CreateReward>().SetRarityColor();
                }
                break;

            case AdventureManager.RewardType.Equipment:
                ShowAlliedMonstersAvailableEquipment();
                ConfirmEquipmentButton.SetActive(true);
                foreach (GameObject rewardSlot in listOfRewardSlots)
                {
                    rewardSlot.SetActive(true);
                    thisRewardType = rewardType;
                    Modifier modifier = GetRandomEquipment();
                    rewardSlot.GetComponent<CreateReward>().subscreenManager = this;
                    rewardSlot.GetComponent<CreateReward>().adventureManager = adventureManager;
                    rewardSlot.GetComponent<CreateReward>().rewardType = AdventureManager.RewardType.Equipment;
                    rewardSlot.GetComponent<CreateReward>().modifierReward = modifier;
                    rewardSlot.GetComponent<CreateReward>().rewardImage.sprite = modifier.baseSprite;
                    //rewardSlot.GetComponent<CreateReward>().rewardName.text = ($"{modifier.modifierName}" +
                        //$"\n- {modifier.modifierDescription}");
                    rewardSlot.GetComponent<CreateReward>().SetRarityColor();
                }
                break;

            default:
                break;
        }
    }

    // This function hides all displayed rewards
    public void HideRewardSlots()
    {
        rerollsLeftText.text = ("");
        RerollButton.SetActive(false);
        SkipButton.SetActive(false);

        foreach (GameObject rewardSlot in listOfRewardSlots)
        {
            rewardSlot.SetActive(false);
        }
    }

    // This function grabs enemy random monsters for a battle
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
            $"\nAllies allowed: {randomBattleMonsterLimit}" +
                $"\nEnemy Modifiers: ");

                foreach (Modifier modifier in adventureManager.ListOfEnemyModifiers)
                {
                    BattleImage.GetComponentInChildren<TextMeshProUGUI>().text +=
                        ($"{modifier.modifierName}\n");
                }

        // populate enemy list && check if boss battle
        for (int j = 0; j < randomBattleMonsterCount; j++)
        {
            if (adventureManager.currentSelectedNode.GetComponent<CreateNode>().nodeType == CreateNode.NodeType.Boss && !bossAdded)
            {
                adventureManager.ListOfEnemyBattleMonsters.Add(GetBossMonster(adventureManager.adventureBoss, 5 + (5 * adventureManager.adventureNGNumber))); // difficulty scaled
                BattleImage.GetComponent<Image>().sprite = adventureManager.adventureBoss.baseSprite;
                BattleImage.GetComponentInChildren<TextMeshProUGUI>().text = ($"Monsters in Battle: Boss + Random" +
                $"\nEnemies present: {randomBattleMonsterCount}" +
                $"\nAllies allowed: {randomBattleMonsterLimit}" +
                $"\nEnemy Modifiers: ");

                foreach(Modifier modifier in adventureManager.ListOfEnemyModifiers)
                {
                    BattleImage.GetComponentInChildren<TextMeshProUGUI>().text +=
                        ($"{modifier.modifierName}\n");
                }

                bossAdded = true;
                continue;
            }

            adventureManager.ListOfEnemyBattleMonsters.Add(GetRandomMonster(adventureManager.adventureNGNumber));
        }

        // Show allied monsters to choose from
        ShowAlliedMonstersAvailable();
    }

    // This function shows the player's currently available monsters for battle
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
                monsterSlot.GetComponent<CreateReward>().monsterStatScreenScript = monsterStatScreenScript;
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

    // This function shows the player's currently available monsters to recieve an equipment
    public void ShowAlliedMonstersAvailableEquipment()
    {
        // Show allied monsters
        int i = 0;
        foreach (GameObject monsterSlot in listOfMonsterSlotsEquipment)
        {
            if (adventureManager.ListOfCurrentMonsters.Count > i)
            {
                monsterSlot.SetActive(true);
                monsterSlot.GetComponent<CreateReward>().adventureManager = adventureManager;
                monsterSlot.GetComponent<CreateReward>().subscreenManager = this;
                monsterSlot.GetComponent<CreateReward>().monsterStatScreenScript = monsterStatScreenScript;
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

    // This function hides the player's currently available monsters to recieve an equipment
    public void DisableAlliedMonstersAvailableEquipment()
    {
        foreach (GameObject monsterSlot in listOfMonsterSlotsEquipment)
        {
            monsterSlot.GetComponent<CreateReward>().rewardImage.sprite = monsterSlot.GetComponent<CreateReward>().monsterReward.baseSprite;
        }
    }

    // This function hides the player's selected equipment
    public void DisableEquipmentSlots()
    {
        foreach (GameObject equipmentSlot in listOfRewardSlots)
        {
            equipmentSlot.GetComponent<CreateReward>().rewardImage.sprite = equipmentSlot.GetComponent<CreateReward>().monsterReward.baseSprite;
        }
    }

    // Override for equipment
    public void ShowAlliedMonstersAvailableEquipment(bool Hide)
    {
        // Hide allied monsters
        int i = 0;
        foreach (GameObject monsterSlot in listOfMonsterSlotsEquipment)
        {
            if (adventureManager.ListOfCurrentMonsters.Count > i)
            {
                monsterSlot.SetActive(false);
            }

            i++;
        }

        ConfirmEquipmentButton.SetActive(false);
    }

    // This function determines how many enemy monsters will populate a battle
    public int GetRandomBattleMonsterCount()
    {
        if (randomBattleMonsterLimit == 1)
        {
            return 1;
        }

        // Always two enemies for the boss fight
        if (adventureManager.adventureNGNumber == 1)
        {
            return 2;
        }

        return Random.Range(2, 5);
    }

    // This function determines hpw many monsters the player can bring into battle
    public int GetRandomBattleMonsterLimit()
    {
        if (adventureManager.ListOfCurrentMonsters.Count == 1)
        {
            return 1;
        }

        return Random.Range(2, 5);
    }

    // This function returns a randomly created monster
    public Monster GetRandomMonster()
    {
        Monster randMonster = Instantiate(adventureManager.ListOfAvailableRewardMonsters[Random.Range(0, adventureManager.ListOfAvailableRewardMonsters.Count)]);

        // random moves
        for (int i = 0; i < 4; i++)
        {
            MonsterAttack randomAttack = randMonster.ListOfMonsterAttacksAvailable[Random.Range(0, randMonster.ListOfMonsterAttacksAvailable.Count)];
            randMonster.ListOfMonsterAttacks[i] = Instantiate(randomAttack);
            randMonster.ListOfMonsterAttacksAvailable.Remove(randomAttack);
        }

        // random stats 
        randMonster.level = GetMonsterRandomLevelRange();
        randMonster.health = Mathf.RoundToInt((randMonster.health + randMonster.level) * randMonster.healthScaler);
        randMonster.maxHealth = randMonster.health;

        randMonster.physicalAttack = Mathf.RoundToInt((randMonster.physicalAttack + randMonster.level - 5) * randMonster.physicalAttackScaler);

        randMonster.magicAttack = Mathf.RoundToInt((randMonster.magicAttack + randMonster.level - 5) * randMonster.magicAttackScaler);

        randMonster.physicalDefense = Mathf.RoundToInt((randMonster.physicalDefense + randMonster.level - 5) * randMonster.physicalDefenseScaler);

        randMonster.magicDefense = Mathf.RoundToInt((randMonster.magicDefense + randMonster.level - 5) * randMonster.magicDefenseScaler);

        randMonster.speed = Mathf.RoundToInt((randMonster.speed + randMonster.level - 5) * randMonster.speedScaler);

        return randMonster;
    }

    // This function returns a randomly created monster with scaled difficulty
    public Monster GetRandomMonster(int enemyLevelScaler)
    {
        // Scale level by NG+
        int scaledLevel = enemyLevelScaler - 2;

        Monster randMonster = Instantiate(adventureManager.ListOfAvailableRewardMonsters[Random.Range(0, adventureManager.ListOfAvailableRewardMonsters.Count)]);

        // random moves
        for (int i = 0; i < 4; i++)
        {
            MonsterAttack randomAttack = randMonster.ListOfMonsterAttacksAvailable[Random.Range(0, randMonster.ListOfMonsterAttacksAvailable.Count)];
            randMonster.ListOfMonsterAttacks[i] = Instantiate(randomAttack);
            randMonster.ListOfMonsterAttacksAvailable.Remove(randomAttack);
        }

        // random stats 
        randMonster.level = GetMonsterRandomLevelRange() + scaledLevel;
        randMonster.health = Mathf.RoundToInt((randMonster.health + randMonster.level) * (randMonster.healthScaler + 0.25f * adventureManager.adventureNGNumber));
        randMonster.maxHealth = randMonster.health;

        randMonster.physicalAttack = Mathf.RoundToInt((randMonster.physicalAttack + randMonster.level - 5) * randMonster.physicalAttackScaler);

        randMonster.magicAttack = Mathf.RoundToInt((randMonster.magicAttack + randMonster.level - 5) * randMonster.magicAttackScaler);

        randMonster.physicalDefense = Mathf.RoundToInt((randMonster.physicalDefense + randMonster.level - 5) * randMonster.physicalDefenseScaler);

        randMonster.magicDefense = Mathf.RoundToInt((randMonster.magicDefense + randMonster.level - 5) * randMonster.magicDefenseScaler);

        randMonster.speed = Mathf.RoundToInt((randMonster.speed + randMonster.level - 5) * randMonster.speedScaler);

        return randMonster;
    }

    // This function returns a randomly created boss monster
    public Monster GetBossMonster(Monster setMonster, int level)
    {
        Monster newMonster = Instantiate(setMonster);

        // random moves
        for (int i = 0; i < 4; i++)
        {
            MonsterAttack randomAttack = newMonster.ListOfMonsterAttacksAvailable[Random.Range(0, newMonster.ListOfMonsterAttacksAvailable.Count)];
            newMonster.ListOfMonsterAttacks[i] = Instantiate(randomAttack);
            newMonster.ListOfMonsterAttacksAvailable.Remove(randomAttack);
        }

        // bonus stats
        newMonster.level = level;
        newMonster.health += Mathf.RoundToInt((newMonster.health + newMonster.level) * 1.85f);
        newMonster.maxHealth = newMonster.health;

        newMonster.physicalAttack = Mathf.RoundToInt((newMonster.physicalAttack + newMonster.level - 5) * newMonster.physicalAttackScaler);

        newMonster.magicAttack = Mathf.RoundToInt((newMonster.magicAttack + newMonster.level - 5) * newMonster.magicAttackScaler);

        newMonster.physicalDefense = Mathf.RoundToInt((newMonster.physicalDefense + newMonster.level - 5) * newMonster.physicalDefenseScaler);

        newMonster.magicDefense = Mathf.RoundToInt((newMonster.magicDefense + newMonster.level - 5) * newMonster.magicDefenseScaler);

        newMonster.speed = Mathf.RoundToInt((newMonster.speed + newMonster.level - 5) * newMonster.speedScaler);
        newMonster.name += ($" <Boss>")
;
        return newMonster;
    }

    // This function returns a random modifier
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

    // This function returns a random equipment
    public Modifier GetRandomEquipment()
    {
        if (adventureManager.ListOfAvailableRewardEquipment.Count == 0)
        {
            return null;
        }

        Modifier randModifier = adventureManager.ListOfAvailableRewardEquipment[Random.Range(0, adventureManager.ListOfAvailableRewardEquipment.Count)];
        adventureManager.ListOfAvailableRewardEquipment.Remove(randModifier);

        Modifier randModifierSO = Instantiate(randModifier);

        // Fix weird reroll bugged amounts?
        randModifierSO.adventureEquipment = true;
        randModifierSO.modifierAmountFlatBuff = randModifier.modifierAmountFlatBuff;
        randModifierSO.modifierAmount = randModifier.modifierAmount;

        return randModifierSO;
    }

    // This function returns a random level range for randomly generated monsters based on what NG+ the player's adventure is on
    public int GetMonsterRandomLevelRange()
    {
        int adventureNGNum = adventureManager.adventureNGNumber;

        switch (adventureManager.adventureNGNumber)
        {
            case (1):
                return Random.Range(5, 9);

            case (2):
                return Random.Range(7, 11);

            case (3):
                return Random.Range(9, 14);

            default:
                return Random.Range(5 + (2 * adventureManager.adventureNGNumber) - Random.Range(1, 5), 5 + (2 * adventureManager.adventureNGNumber));
        }
    }

    // This function displays the final results screen
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
            $"\nRun: {adventureManager.adventureNGNumber}" +
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
            adventureManager.PlayNewBGM(adventureManager.winBGM, .20f);
            EnterNewGameButton.SetActive(true);
            titleText.text = "Adventure Completed!";
        }
        else
        {
            titleText.text = "Adventure Failed...";
            
            if (adventureManager.adventureNGNumber <= 1)
            {
                RestartGameButton.SetActive(true);
            }
        }

        // Reset bools
    }

    // This functions returns to the main menu screen
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }

    // This helper function calls the actual function on the AdventureManager script
    public void CallNewGameFunction()
    {
        adventureManager.InitiateNewGame();
    }

    // This helper function calls the actual function on the AdventureManager script
    public void CallRestartGameFunction()
    {
        adventureManager.RestartGame();
    }

    // This functions rerolls the currently displayed rewards
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

            if (thisRewardType == AdventureManager.RewardType.Equipment)
            {
                adventureManager.ResetEquipmentList();
                adventureManager.currentSelectedEquipment = null;
                adventureManager.currentSelectedMonsterForEquipment = null;
            }

            LoadRewardSlots(thisRewardType);
        } 
    }

    // This function skips the reward screen
    public void SkipRewards()
    {
        if (adventureManager.ListOfAllMonsters.Count == 0)
        {
            titleText.text = ("Please select a starting monster!");
            return;
        }

        HideRewardSlots();
        ShowAlliedMonstersAvailableEquipment(false);

        adventureManager.SubscreenMenu.SetActive(false);
        adventureManager.ActivateNextNode();
    }
}
