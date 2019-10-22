using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    private bool isConnecting;
    
    [SerializeField] private byte maxPlayersPerRoom = 2;
    
    private string gameVersion = "1";

    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject progressLabel;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    public void Connect()
    {
        isConnecting = true;
        
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    #region Connection issue handling

    public override void OnConnectedToMaster()
    {
        Debug.Log("Successfully connected");

        if (isConnecting) PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        Debug.Log("Failed to connect");
    }
    
    #endregion

    #region Room Connection Handling

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayersPerRoom});
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");

        if (PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            PhotonNetwork.LoadLevel("Room for 2");
        }
    }

    #endregion
}
