using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscreenManager : MonoBehaviour
{
    public GameObject RewardSlotOne;
    public GameObject RewardSlotTwo;
    public GameObject RewardSlotThree;

    public List<GameObject> listOfRewardSlots;

    public AdventureManager adventureManager;

    //
    public void Awake()
    {
        adventureManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AdventureManager>();
    }

    // load reward slots
    public void LoadRewardSlots(AdventureManager.RewardType rewardType)
    {
        switch (rewardType)
        {
            case AdventureManager.RewardType.Monster:
                foreach (GameObject rewardSlot in listOfRewardSlots)
                {
                    rewardSlot.SetActive(true);
                    Monster monster = GetRandomMonster();
                    rewardSlot.GetComponent<CreateReward>().subscreenManager = this;
                    rewardSlot.GetComponent<CreateReward>().adventureManager = adventureManager;
                    rewardSlot.GetComponent<CreateReward>().rewardType = AdventureManager.RewardType.Monster;
                    rewardSlot.GetComponent<CreateReward>().monsterReward = monster;
                    rewardSlot.GetComponent<CreateReward>().rewardImage.sprite = monster.baseSprite;
                    rewardSlot.GetComponent<CreateReward>().rewardName.text = ($"{monster.name} Lvl.{monster.level}" +
                        $"\n{monster.monsterElement}/{monster.monsterSubElement}" +
                        $"\n-{monster.ListOfMonsterAttacks[0].monsterAttackName}" +
                        $"\n-{monster.ListOfMonsterAttacks[1].monsterAttackName}" +
                        $"\n-{monster.ListOfMonsterAttacks[2].monsterAttackName}" +
                        $"\n-{monster.ListOfMonsterAttacks[3].monsterAttackName}");
                }
                break;

            case AdventureManager.RewardType.Modifier:
                foreach (GameObject rewardSlot in listOfRewardSlots)
                {
                    rewardSlot.SetActive(true);
                    Modifier modifier = GetRandomModifier();
                    rewardSlot.GetComponent<CreateReward>().subscreenManager = this;
                    rewardSlot.GetComponent<CreateReward>().adventureManager = adventureManager;
                    rewardSlot.GetComponent<CreateReward>().rewardType = AdventureManager.RewardType.Modifier;
                    rewardSlot.GetComponent<CreateReward>().modifierReward = modifier;
                    rewardSlot.GetComponent<CreateReward>().rewardImage.sprite = modifier.baseSprite;
                    rewardSlot.GetComponent<CreateReward>().rewardName.text = ($"{modifier.modifierName}" +
                        $"\n-{modifier.modifierDescription}");
                }
                break;

            default:
                break;
        }
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

        return randMonster;
    }

    //
    public Modifier GetRandomModifier()
    {
        Modifier randModifier = Instantiate(adventureManager.ListOfAvailableRewardModifiers[Random.Range(0, adventureManager.ListOfAvailableRewardModifiers.Count)]);

        return randModifier;
    }
}
