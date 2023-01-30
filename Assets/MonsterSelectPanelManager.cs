using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSelectPanelManager : MonoBehaviour
{
    AdventureManager adventureManager;

    [SerializeField]
    private GameObject monsterSelectCardTemplate;

    public List<GameObject> monsterSelectCards;

    //private void Awake()
    //{
    //    if (adventureManager == null)
    //        adventureManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AdventureManager>();
    //}

    public void InitializeMonsterSelectCards(AdventureManager _adventureManager, CreateReward.TypeOfMonsterSelect _typeOfMonsterSelect)
    {
        if (adventureManager == null)
            adventureManager = _adventureManager;

        ClearMonsterSelectCards();

        gameObject.SetActive(true);

        for (int i = 0; i < adventureManager.ListOfCurrentMonsters.Count; i++)
        {
            var currentMonsterCard = adventureManager.ListOfCurrentMonsters[i].monsterSelectCard;

            var monsterSelectCard = Instantiate(currentMonsterCard, monsterSelectCardTemplate.transform.parent);

            monsterSelectCards.Add(monsterSelectCard);

            monsterSelectCard.SetActive(true);

            CreateReward rewardComponent = monsterSelectCard.GetComponent<CreateReward>();

            rewardComponent.InitializeSelectable(adventureManager, _typeOfMonsterSelect);

            rewardComponent.InitializeMonsterSelectCardData(adventureManager.ListOfCurrentMonsters[i]);
        }
    }

    public void ClearMonsterSelectCards()
    {
        for (int i = 0; i < monsterSelectCards.ToArray().Length; i++)
        {
            //if (i == 0)
            //continue;

            Destroy(monsterSelectCards[i]);

            monsterSelectCards.Remove(monsterSelectCards[i]);
        }
    }

    public void HideMonsterSelectCards()
    {
        gameObject.SetActive(false);
    }
}
