using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class SceneButtonManager : MonoBehaviour
{
    public LauncherScript launcherScript;
    public TextMeshProUGUI connectingText;
    public Animator SceneTransitions;

    [Header("Menus")]
    public GameObject MainMenu;
    public GameObject CreateRoomMenu;
    public GameObject QuickplaySettingsMenu;
    public GameObject MultiplayerMenu;
    public GameObject TeamBuilderMenu;
    public GameObject SingleplayerMenu;

    [Header("Scene Buttons")]
    public Button QuickplayButton;
    public Button MultiplayerButton;
    public Button BackButton;

    public Button CreateMatchButton;
    public Button FindMatchButton;

    public Button QuitButton;

    // Start is called before the first frame update
    void Start()
    {
        launcherScript = GetComponent<LauncherScript>();
    }

    // This function loads a hardcoded scene by index on button press
    public void GoToScene(string sceneName)
    {
        // TODO - Random Team + Enemies, Set difficulty, pick monsters for team on Quickplay 
        SceneManager.LoadScene(sceneName);
    }

    // This function loads a hardcoded scene by index on button press
    public void GoToSceneCoroutine(string sceneName)
    {
        StartCoroutine(SceneTransition(sceneName));
    }

    // This function is called when the Multiplayer button is clicked
    public void MultiplayerClicked()
    {
        launcherScript.ConnectToMasterServer();
        connectingText.text = "Connecting to Server...";

        MainMenu.SetActive(false);
    }

    // This function is called when create match button is clicked - TODO - Create room function
    public void CreateMatchClicked()
    {
        CreateRoomMenu.SetActive(true);

        MainMenu.SetActive(false);
        MultiplayerMenu.SetActive(false);
    }

    // This function is called when create match button is clicked - TODO - Create room function
    public void QuickplayClicked()
    {
        MainMenu.SetActive(false);
        QuickplaySettingsMenu.SetActive(true);
        SingleplayerMenu.SetActive(false);
    }

    // This function is called when team builder button is clicked
    public void TeamBuilderClicked()
    {
        MainMenu.SetActive(false);
        TeamBuilderMenu.SetActive(true);
    }

    // This function is called when Singleplayer is clicked
    public void SingleplayerClicked()
    {
        MainMenu.SetActive(false);
        SingleplayerMenu.SetActive(true);
    }

    // This function is called when Adventure is clicked
    public void AdventureClicked()
    {
        StartCoroutine(SceneTransition("AdventureBeginScene"));
    }

    // This function is called when return to main menu is clicked
    public void ReturnToMainMenuClicked()
    {
        StartCoroutine(SceneTransition("StartScreen"));
    }

    IEnumerator SceneTransition(string sceneName)
    {
        SceneTransitions.SetTrigger("Start");

        yield return new WaitForSeconds(.5f);

        GoToScene(sceneName);
    }

    // This function is called after the client successfully connects to the server
    public void Connected()
    {
        connectingText.text = "";

        MultiplayerMenu.SetActive(true);
    }

    // This function is called when return to Multiplayer button is clicked
    public void ReturnToMultiplayerMenu()
    {
        MultiplayerMenu.SetActive(true);
        CreateRoomMenu.SetActive(false);
        TeamBuilderMenu.SetActive(false);
    }

    // This function is called when the Back button is clicked
    public void BackButtonClicked()
    {
        PhotonNetwork.Disconnect();

        MainMenu.SetActive(true);

        MultiplayerMenu.SetActive(false);
    }

    // This override function is called when the Back button is clicked
    public void BackButtonClickedNotOnline(string Override)
    {
        MainMenu.SetActive(true);

        QuickplaySettingsMenu.SetActive(false);
        SingleplayerMenu.SetActive(false);
        TeamBuilderMenu.SetActive(false);
    }

    // This function quits the game when called
    public void QuitButtonClicked()
    {
        Application.Quit();
    }
}
