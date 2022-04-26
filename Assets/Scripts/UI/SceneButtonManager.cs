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

    [Header("Menus")]
    public GameObject MainMenu;
    public GameObject CreateRoomMenu;
    public GameObject QuickplaySettingsMenu;

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

    // This function is called when the Multiplayer button is clicked
    public void MultiplayerClicked()
    {
        launcherScript.ConnectToMasterServer();
        connectingText.text = "Connecting to Server...";

        QuickplayButton.gameObject.SetActive(false);
        MultiplayerButton.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
    }

    // This function is called when create match button is clicked - TODO - Create room function
    public void CreateMatchClicked()
    {
        MainMenu.SetActive(false);
        CreateRoomMenu.SetActive(true);
    }

    // This function is called when create match button is clicked - TODO - Create room function
    public void QuickplayClicked()
    {
        MainMenu.SetActive(false);
        QuickplaySettingsMenu.SetActive(true);
    }

    // This function is called after the client successfully connects to the server
    public void Connected()
    {
        connectingText.text = "";

        CreateMatchButton.gameObject.SetActive(true);
        FindMatchButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }

    // This function is called when return to Multiplayer button is clicked
    public void ReturnToMultiplayerMenu()
    {
        MainMenu.SetActive(true);
        CreateRoomMenu.SetActive(false);
    }

    // This function is called when the Back button is clicked
    public void BackButtonClicked()
    {
        PhotonNetwork.Disconnect();

        CreateMatchButton.gameObject.SetActive(false);
        FindMatchButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);

        QuickplayButton.gameObject.SetActive(true);
        MultiplayerButton.gameObject.SetActive(true);
        QuitButton.gameObject.SetActive(true);
    }

    // This override function is called when the Back button is clicked
    public void BackButtonClickedNotOnline(string Override)
    {
        QuickplaySettingsMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    // This function quits the game when called
    public void QuitButtonClicked()
    {
        Application.Quit();
    }
}
