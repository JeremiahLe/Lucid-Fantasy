using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManagerScript : MonoBehaviour
{
    public GameObject AttacksButton;
    public GameObject AutoBattleButton;

    public GameObject Attack1Button;
    public GameObject Attack2Button;
    public GameObject Attack3Button;
    public GameObject Attack4Button;

    public GameObject BackButton;

    public List<GameObject> AttacksHUDButtons;
    public List<GameObject> InitialHUDButtons;

    // Start is called before the first frame update
    void Start()
    {
        AddButtonsToList();
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

        AttacksHUDButtons.Add(BackButton);

        // Hide all attack buttons at start
        HideAllButtons("AttacksHUDButtons");
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
    }
}
