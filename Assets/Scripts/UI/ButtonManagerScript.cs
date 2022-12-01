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
    public GameObject PassButton;
    public GameObject ChangeButton;
    public GameObject ItemsButton;

    public GameObject FrontRowButton;
    public GameObject CenterRowButton;
    public GameObject BackRowButton;

    public GameObject RowButtonsParent;
    public GameObject ReturnToAttacksButton;

    public GameObject Attack1Button;
    public GameObject Attack2Button;
    public GameObject Attack3Button;
    public GameObject Attack4Button;

    public GameObject BackButton;
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
        InitialHUDButtons.Add(PassButton);
        InitialHUDButtons.Add(ChangeButton);
        InitialHUDButtons.Add(ItemsButton);

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
        HideButton(ReturnToAttacksButton);
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
            case "ReturnToAttacksButton":
                ReturnToAttacksButton.SetActive(true);
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
            HideButton(ConfirmQuitButton);
            HideButton(ContinueButton);
            QuitButton.SetActive(false);
            ReturnToAttacksButton.SetActive(false);
            HideButton(RowButtonsParent);
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
        QuitButton.SetActive(true);
        BackButton.SetActive(false);
        HideButton(RowButtonsParent);

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
            PassButton.SetActive(false);
            ChangeButton.SetActive(false);
            ItemsButton.SetActive(false);

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

    // This function passes the current monster's turn
    public void PassButtonClicked()
    {
        HideAllButtons("All");
        Monster monster = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} passed!");
        uiManager.EditCombatMessage($"{monster.aiType} {monster.name} passed!");
        combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterActionAvailable = false;

        Invoke("CallNextMonsterTurn", 1.0f);
    }

    // This function brings up the change row buttons menu
    public void ChangeButtonClicked()
    {
        HideAllButtons("All");

        Monster monster = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;
        CreateMonster monsterComponent = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>();
        uiManager.EditCombatMessage($"Change {monster.aiType} {monster.name} to what stance? Current stance: {monsterComponent.monsterStance.ToString()}");

        RowButtonsParent.SetActive(true);

        // Reset row button colors
        FrontRowButton.GetComponent<Button>().colors = startColors;
        CenterRowButton.GetComponent<Button>().colors = startColors;
        BackRowButton.GetComponent<Button>().colors = startColors;

        var colors = FrontRowButton.GetComponent<Button>().colors;

        // Show available rows
        switch (monsterComponent.monsterStance)
        {
            case (CreateMonster.MonsterStance.Aggressive):
                colors = FrontRowButton.GetComponent<Button>().colors;
                colors.normalColor = Color.gray;
                FrontRowButton.GetComponent<Button>().colors = colors;

                // Disable and Enable buttons
                FrontRowButton.GetComponent<Button>().interactable = false;
                CenterRowButton.GetComponent<Button>().interactable = true;
                BackRowButton.GetComponent<Button>().interactable = true;
                break;

            case (CreateMonster.MonsterStance.Neutral):
                colors = CenterRowButton.GetComponent<Button>().colors;
                colors.normalColor = Color.gray;          
                CenterRowButton.GetComponent<Button>().colors = colors;

                // Disable and Enable buttons
                FrontRowButton.GetComponent<Button>().interactable = true;
                CenterRowButton.GetComponent<Button>().interactable = false;
                BackRowButton.GetComponent<Button>().interactable = true;
                break;

            case (CreateMonster.MonsterStance.Defensive):
                colors = BackRowButton.GetComponent<Button>().colors;
                colors.normalColor = Color.gray;
                BackRowButton.GetComponent<Button>().colors = colors;

                // Disable and Enable buttons
                FrontRowButton.GetComponent<Button>().interactable = true;
                CenterRowButton.GetComponent<Button>().interactable = true;
                BackRowButton.GetComponent<Button>().interactable = false;
                break;

            default:
                break;
        }
    }

    // This function is called by row
    public void RowButtonClicked(string newRow)
    {
        // Show new message, confirm row?
        Monster monster = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;
        CreateMonster monsterComponent = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>();
        RowButtonsParent.SetActive(false);

        // Preview new monster row
        switch (newRow)
        {
            case ("FrontRow"):
                monsterComponent.SetPositionAndOrientation(monsterComponent.transform, monsterComponent.combatOrientation, CreateMonster.MonsterStance.Aggressive, monsterComponent.monsterStance);
                break;

            case ("CenterRow"):
                monsterComponent.SetPositionAndOrientation(monsterComponent.transform, monsterComponent.combatOrientation, CreateMonster.MonsterStance.Neutral, monsterComponent.monsterStance);
                break;

            case ("BackRow"):
                monsterComponent.SetPositionAndOrientation(monsterComponent.transform, monsterComponent.combatOrientation, CreateMonster.MonsterStance.Defensive, monsterComponent.monsterStance);
                break;

            default:
                break;
        }
    }

    // This helper function calls the combatManagerScript's nextMonsterTurn function
    public void CallNextMonsterTurn()
    {
        combatManagerScript.NextMonsterTurn();
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
                AttacksHUDButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = ($"{ListOfMonsterAttacks[i].monsterAttackName} - {ListOfMonsterAttacks[i].monsterAttackSPCost} SP");

                if (ListOfMonsterAttacks[i].monsterAttackSPCost > combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.currentSP)
                {
                    var colors = AttacksHUDButtons[i].GetComponent<Button>().colors;
                    colors.normalColor = Color.gray;
                    AttacksHUDButtons[i].GetComponent<Button>().colors = colors;
                    AttacksHUDButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = ($"<s>{ListOfMonsterAttacks[i].monsterAttackName} - {ListOfMonsterAttacks[i].monsterAttackSPCost} SP</s>");
                }
                else
                {
                    AttacksHUDButtons[i].GetComponent<Button>().colors = startColors;
                }
            }
        }
    }

    // This function returns to the attack moves
    public void ReturnToAttackMoves()
    {
        // Hide attack description container
        uiManager.ResetCombatMessage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name);
        monsterAttackManager.ResetHUD();
        monsterAttackManager.ClearCurrentButtonAttack();
        monsterAttackManager.ListOfCurrentlyTargetedMonsters.Clear();

        // Redisplay attack moves and hide the back button
        HideAllButtons("AttacksHUDButtons");
        DisplayAttackMoves();
        HideButton(ReturnToAttacksButton);
    }
}
