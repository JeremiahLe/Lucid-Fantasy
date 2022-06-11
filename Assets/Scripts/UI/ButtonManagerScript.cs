using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ButtonManagerScript : MonoBehaviour
{
    // Button Objects
    public GameObject AttacksButton;
    public GameObject AutoBattleButton;

    public GameObject Attack1Button;
    public GameObject Attack2Button;
    public GameObject Attack3Button;
    public GameObject Attack4Button;

    public GameObject BackButton;

    //public GameObject ConfirmButton;
    public GameObject ConfirmQuitButton;
    public GameObject ContinueButton;
    public GameObject QuitButton;

    public List<GameObject> AttacksHUDButtons;
    public List<GameObject> InitialHUDButtons;

    public List<MonsterAttack> ListOfMonsterAttacks;

    public MonsterAttackManager monsterAttackManager;
    public CombatManagerScript combatManagerScript;
    public UIManager uiManager;

    public ColorBlock startColors;

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        AddButtonsToList();
        ManualHideButtons();
    }

    // This functions initializes the gameObject's components
    public void InitializeComponents()
    {
        monsterAttackManager = GetComponent<MonsterAttackManager>();
        combatManagerScript = GetComponent<CombatManagerScript>();
        uiManager = GetComponent<UIManager>();
    }

    // This function adds all the HUD buttons to an initial list to turn them off and on at will (may be slow)
    public void AddButtonsToList()
    {
        InitialHUDButtons.Add(AttacksButton);
        InitialHUDButtons.Add(AutoBattleButton);

        AttacksHUDButtons.Add(Attack1Button);
        AttacksHUDButtons.Add(Attack2Button);
        AttacksHUDButtons.Add(Attack3Button);
        AttacksHUDButtons.Add(Attack4Button);

        startColors = AttacksHUDButtons[0].GetComponent<Button>().colors;

        // Hide all attack buttons at start
        HideAllButtons("AttacksHUDButtons");
        HideAllButtons("All"); // round start implementation
    }

    // This function stores all buttons to be hidden manually
    public void ManualHideButtons()
    {
        //HideButton(ConfirmButton);
    }

    // This function manually hides a button in start
    public void HideButton(GameObject button)
    {
        button.SetActive(false);
    }

    // This function manually shows a button by name
    public void ShowButton(string buttonName)
    {
        switch (buttonName)
        {
            case "ConfirmButton":
                //ConfirmButton.SetActive(true);
                break;
            case "BackButton":
                BackButton.SetActive(true);
                break;
            default:
                Debug.Log("Incorrect or missing button name or reference", this);
                break;
        }
    }

    // This function shows or hides all buttons based on the name of the list of buttons passed in (had to do it this way because of button functions in the inspector)
    public void HideAllButtons(string ButtonsListName)
    {
        if (ButtonsListName == "AttacksHUDButtons")
        {
            foreach (GameObject button in AttacksHUDButtons)
            {
                if (button.activeInHierarchy == true) { button.SetActive(false); }
                else
                if (button.activeInHierarchy == false) { button.SetActive(true); }
            }
        }
        else if (ButtonsListName == "InitialHUDButtons")
        {
            foreach (GameObject button in InitialHUDButtons)
            {
                if (button.activeInHierarchy == true) { button.SetActive(false); }
                else
                if (button.activeInHierarchy == false) { button.SetActive(true); }
            }

            HideAllButtons("AttacksHUDButtons"); // Show attacks after
        }
        else if (ButtonsListName == "All")
        {
            foreach (GameObject button in InitialHUDButtons)
            {
                if (button.activeInHierarchy == true) { button.SetActive(false); }
            }

            BackButton.SetActive(false);
            //ConfirmButton.SetActive(false);
            HideButton(ConfirmQuitButton);
            HideButton(ContinueButton);
            QuitButton.SetActive(false);
        }
    }

    // This function resets the HUD to a default state for use with in-game buttons
    public void ResetHUD()
    {
        foreach (GameObject button in AttacksHUDButtons)
        {
            button.SetActive(false);
        }
        foreach (GameObject button in InitialHUDButtons)
        {
            button.SetActive(true);
        }

        // This should reset the message at the top of screen ( What will <monster name> do? ) && hide the confirm attack button plus attack description
        uiManager.ResetCombatMessage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name);
        monsterAttackManager.ResetHUD();

        //HideButton(ConfirmButton);
        HideButton(ConfirmQuitButton);
        HideButton(ContinueButton);
        QuitButton.SetActive(true); // GitHub comment
        BackButton.SetActive(false);

        combatManagerScript.CurrentMonsterAttack = null;
        combatManagerScript.targeting = false;
    }

    // This function is called when back is clicked to check Quit
    public void CheckQuit()
    {
        if (AttacksButton.activeInHierarchy)
        {
            AttacksButton.SetActive(false);
            AutoBattleButton.SetActive(false);

            ConfirmQuitButton.SetActive(true);
            ContinueButton.SetActive(true);
            QuitButton.SetActive(false);
        }
        else 
        {
            ResetHUD();
        }
    }

    // This function quits the game
    public void QuitButtonClicked()
    {
        SceneManager.LoadScene(0);
    }

    // This function assigns the current ally monsters attack moves to each of the four attack buttons
    public void AssignAttackMoves(Monster monster)
    {
        foreach (MonsterAttack attack in monster.ListOfMonsterAttacks)
        {
            //MonsterAttack attackInstance = Instantiate(attack);
            ListOfMonsterAttacks.Add(attack);
        }
    }

    // This function displays the names of the attack moves the current monster has // Git edit
    public void DisplayAttackMoves()
    {
        QuitButton.SetActive(false);
        BackButton.SetActive(true);

        for (int i = 0; i < ListOfMonsterAttacks.Count; i++)
        {
            if (ListOfMonsterAttacks[i] != null)
            {
                AttacksHUDButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = ListOfMonsterAttacks[i].monsterAttackName;

                if (ListOfMonsterAttacks[i].attackOnCooldown)
                {
                    var colors = AttacksHUDButtons[i].GetComponent<Button>().colors;
                    colors.normalColor = Color.gray;
                    AttacksHUDButtons[i].GetComponent<Button>().colors = colors;
                    AttacksHUDButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = ($"<s>{ListOfMonsterAttacks[i].monsterAttackName}</s>");
                }
                else
                {
                    AttacksHUDButtons[i].GetComponent<Button>().colors = startColors;
                }
            }
        }
    }
}
