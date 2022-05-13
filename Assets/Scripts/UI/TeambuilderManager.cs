using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeambuilderManager : MonoBehaviour
{
    public int maxTeams = 20;
    public Team currentSelectedTeam;

    public GameObject TeamListLog, TeamButtonObject;
    public GameObject TeamViewerObject;

    public TextMeshProUGUI TeamMaxAlertText;
    public TMP_InputField teamNameInput;

    [SerializeField]
    List<Team> teamList = new List<Team>();

    public void Awake()
    {
    }

    // This function sends a message to the combat log
    public void CreateTeam()
    {
        // Check if max teams reached
        if (teamList.Count >= maxTeams)
        {
            TeamMaxAlertText.text = ($"Maximum team capacity reached! Please delete a team to make space.");
            TeamMaxAlertText.GetComponent<HoverMeScript>().Invoke("ResetText", 3.0f);
            return;
        }

        // Create new team and add to list
        Team newTeam = new Team();
        teamList.Add(newTeam);

        // Create the team button object, grab its script, and assign its name and team reference
        GameObject newTeamObject = Instantiate(TeamButtonObject, TeamListLog.transform);
        newTeam.teamNameObject = newTeamObject.GetComponentInChildren<TextMeshProUGUI>();
        newTeam.teamButtonIcon = newTeamObject;
        newTeam.teamButtonScript = newTeamObject.GetComponent<InitializeTeamButtonScript>();
        newTeam.teamButtonScript.teamReference = newTeam;

        newTeam.teamName = ($"MyTeam{teamList.IndexOf(newTeam) + 1}");
        newTeam.teamNameObject.text = newTeam.teamName;

        //HUDanimationManager.UpdateScrollBar();
    }

    // This function displays the current selected team
    public void DisplayTeam(string teamName, Team currentTeam)
    {
        // Display chosen team and name
        TeamViewerObject.SetActive(true);
        TextMeshProUGUI TeamNameObject = TeamViewerObject.GetComponentInChildren<TextMeshProUGUI>();
        TeamNameObject.text = teamName;

        // Set current team
        currentSelectedTeam = currentTeam;
    }

    // This function saves the currently edited team
    public void SaveTeam()
    {
        // Check if complete team?

        // Check if blank
        if (teamNameInput.text == "")
        {
            currentSelectedTeam.teamName = ($"MyTeam{teamList.IndexOf(currentSelectedTeam) + 1}");
            currentSelectedTeam.teamNameObject.text = ($"MyTeam{teamList.IndexOf(currentSelectedTeam) + 1}");
            return;
        }

        // Update team name 
        currentSelectedTeam.teamName = teamNameInput.text;
        currentSelectedTeam.teamNameObject.text = teamNameInput.text;
    }

    // This function deletes the current edited team
    public void DeleteTeam()
    {
        Destroy(currentSelectedTeam.teamButtonIcon);
        teamList.Remove(currentSelectedTeam);
        TeamViewerObject.SetActive(false);
    }
}

[System.Serializable]
public class Team
{
    public string teamName;
    public string teamDate;
    public TextMeshProUGUI teamNameObject;
    public GameObject teamButtonIcon;
    public InitializeTeamButtonScript teamButtonScript;
    public List<Monster> teamListOfMonsters;
}