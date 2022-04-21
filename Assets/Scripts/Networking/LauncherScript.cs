using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LauncherScript : MonoBehaviourPunCallbacks
{
    SceneButtonManager sceneButtonManager;

    public void Awake()
    {
        sceneButtonManager = GetComponent<SceneButtonManager>();    
    }

    public void ConnectToMasterServer()
    {
        Debug.Log("Connecting to Master!", this);
        PhotonNetwork.ConnectUsingSettings(); // connects to master server with user settings
    }

    public override void OnConnectedToMaster()
    {   
        Debug.Log("Connected to Master!", this);
        PhotonNetwork.JoinLobby(); // Lobbys are where you find and create rooms
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!", this);
        sceneButtonManager.Connected();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected!", this);
    }
}
