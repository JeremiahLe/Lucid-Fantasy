using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitializeTeamButtonScript : MonoBehaviour
{
    public GameObject GameManager;
    public TeambuilderManager TeambuilderManagerScript;
    public Button thisButton;
    public TextMeshProUGUI teamName;
    public Team teamReference;

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        teamName = GetComponentInChildren<TextMeshProUGUI>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
        TeambuilderManagerScript = GameManager.GetComponent<TeambuilderManager>();

        thisButton.onClick.AddListener(delegate { TeambuilderManagerScript.DisplayTeam(teamName.text, teamReference ); } );
    }
}
