using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Launcher : MonoBehaviourPunCallbacks
{
    private bool isConnecting;
    
    private string gameVersion = "1";

    [Header("Login Panel")]
    public GameObject LoginPanel;
    public InputField PlayerNameInput;

    [Header("Selection Panel")]
    public GameObject SelectionPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomPanel;

    public InputField RoomNameInputField;
    public InputField MaxPlayersInputField;

    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomPanel;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomPanel;

    public Button StartGameButton;
    public GameObject PlayerListEntryPrefab;
    
    private const string playerNamePrefKey = "PlayerName";
    
    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        string defaultName = string.Empty;

        if (PlayerPrefs.HasKey(playerNamePrefKey))
        {
            defaultName = PlayerPrefs.GetString(playerNamePrefKey);
            PlayerNameInput.text = defaultName;
        }
        
        PhotonNetwork.NickName = defaultName;
    }

    public void SetPlayerName(string newName)
    {
        if (string.IsNullOrEmpty(newName))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }

        PhotonNetwork.NickName = newName;
        
        PlayerPrefs.SetString(playerNamePrefKey, newName);
    }
    
    private void SetActivePanel(string activePanel)
    {
        LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
        SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
        CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
        JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
        InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
    }
    
    public override void OnConnectedToMaster()
    {
        SetActivePanel(SelectionPanel.name);
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(SelectionPanel.name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(SelectionPanel.name);
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room " + Random.Range(1000, 10000);

        var options = new RoomOptions {MaxPlayers = 8};
        PhotonNetwork.CreateRoom(roomName, options, null);
    }
}
