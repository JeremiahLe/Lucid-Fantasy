using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CreateAdventure : MonoBehaviour
{
    [Header("Adventure Init")]
    public Adventure adventure;
    [DisplayWithoutEdit] public Adventure adventureReference;

    public string adventureName;

    public Adventure.AdventureDifficulty adventureDifficulty;

    public string adventureDescription;

    public Monster adventureBoss;

    public Image adventureIcon;

    [SerializeField] UnityEvent selectedAdventure;

    public GameObject descriptionContainer;
    public TextMeshProUGUI adventureNameText;
    public TextMeshProUGUI adventureDescriptionText;
    public Button thisButton;

    // Start is called before the first frame update
    void Awake()
    {
        InitiateAdventure();
    }

    // This function creates an adventure reference and inits adventure vars
    public void InitiateAdventure()
    {
        // Create SO reference and grab components
        adventureReference = Instantiate(adventure);
        thisButton = GetComponent<Button>();

        adventureName = adventure.adventureName;
        adventureDifficulty = adventure.adventureDifficulty;
        adventureDescription = adventure.adventureDescription;
        adventureBoss = Instantiate(adventure.adventureBoss);

        // Display in-game
        adventureNameText.text = adventureName;
        adventureIcon.sprite = adventure.adventureIcon;
    }

    // This function calls the button description when hovering
    public void DisplayAdventureDetails()
    {
        descriptionContainer.SetActive(true);
        adventureDescriptionText.text = ($"Difficulty: {adventureDifficulty.ToString()}\n" +
            $"{adventureDescription}");
    }

    // This function calls the button description when hovering
    public void DisplayAdventureDetails(string overrideString)
    {
        descriptionContainer.SetActive(false);
    }

    // This function invokes select Adventure
    public void InvokeConfirmAdventure()
    {
        selectedAdventure.Invoke();
    }
}
