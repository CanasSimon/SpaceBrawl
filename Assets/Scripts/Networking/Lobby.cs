using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Lobby : MonoBehaviourPunCallbacks
{
	private bool isConnecting;

	private string gameVersion = "1";

	[Header("Login Panel")] 
	[SerializeField] private GameObject loginPanel;
	[SerializeField] private InputField playerNameInput;

	[Header("Selection Panel")] 
	[SerializeField] private GameObject selectionPanel;
	[SerializeField] private InputField newRoomNameInput;
	[SerializeField] private InputField roomNameInput;

	[Header("Join Random Room Panel")] 
	[SerializeField] private GameObject joinRandomRoomPanel;

	[Header("Inside Room Panel")] 
	[SerializeField] private Text roomNameText;
	[SerializeField] private GameObject insideRoomPanel;
	[SerializeField] private Button gameStartButton;
	[SerializeField] private GameObject playerInRoomPrefab;

	private Dictionary<int, GameObject> playerListEntries;

	private const string playerNamePrefKey = "PlayerName";

	//Gets the "PlayerName" PrefKey and puts it in the name InputField
	public void Awake()
	{
		if (PhotonNetwork.IsConnected)
		{
			SetActivePanel(selectionPanel.name);
			return;
		}
		
		PhotonNetwork.AutomaticallySyncScene = true;

		string defaultName = string.Empty;

		if (PlayerPrefs.HasKey(playerNamePrefKey))
		{
			defaultName = PlayerPrefs.GetString(playerNamePrefKey);
			playerNameInput.text = defaultName;
		}

		PhotonNetwork.NickName = defaultName;
	}

	#region PUN methods
	public override void OnConnectedToMaster()
	{
		SetActivePanel(selectionPanel.name);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		SetActivePanel(selectionPanel.name);
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		SetActivePanel(selectionPanel.name);
	}

	//Creates a room if no room is found
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		string roomName = "Room " + Random.Range(0, 10000);

		var options = new RoomOptions {MaxPlayers = 4, BroadcastPropsChangeToAll = true};
		PhotonNetwork.CreateRoom(roomName, options);
	}

	//Instantiates playerInRoomPrefab for each player present in the room and stores them in a Dictionary with their player number
	public override void OnJoinedRoom()
	{
		SetActivePanel(insideRoomPanel.name);
		roomNameText.text = PhotonNetwork.CurrentRoom.Name;

		if (playerListEntries == null)
		{
			playerListEntries = new Dictionary<int, GameObject>();
		}

		foreach (var p in PhotonNetwork.PlayerList)
		{
			var entry = Instantiate(playerInRoomPrefab, insideRoomPanel.transform, true);
			entry.transform.localScale = Vector3.one;
			entry.GetComponent<PlayerInRoom>().Initialize(p.ActorNumber, p.NickName);

			playerListEntries.Add(p.ActorNumber, entry);
		}

		gameStartButton.interactable = playerListEntries.Count >= 2 && PhotonNetwork.IsMasterClient;

		var props = new Hashtable
		{
			{AsteroidsGame.PLAYER_LOADED_LEVEL, false}
		};
		PhotonNetwork.LocalPlayer.SetCustomProperties(props);
	}

	//Destroys the playerInRoomPrefab clones and goes back to the selection screen
	public override void OnLeftRoom()
	{
		SetActivePanel(selectionPanel.name);

		foreach (var entry in playerListEntries.Values)
		{
			Destroy(entry.gameObject);
		}

		playerListEntries.Clear();
		playerListEntries = null;
	}

	//Instantiates a new playerInRoomPrefab and adds the Player to the Dictionary
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		var entry = Instantiate(playerInRoomPrefab, insideRoomPanel.transform, true);
		entry.transform.localScale = Vector3.one;
		entry.GetComponent<PlayerInRoom>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

		playerListEntries.Add(newPlayer.ActorNumber, entry);

		gameStartButton.interactable = playerListEntries.Count >= 2 && PhotonNetwork.IsMasterClient;
	}

	//Destroys the playerInRoomPrefab clone and removes the Player from the Dictionary
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
		playerListEntries.Remove(otherPlayer.ActorNumber);

		gameStartButton.interactable = playerListEntries.Count >= 2 && PhotonNetwork.IsMasterClient;
	}

	#endregion

	#region UI Methods
	//Sets the name inputted on the login screen as the Photon Nickname
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

	//Toggles on/off the panels to have the correct one active
	private void SetActivePanel(string activePanel)
	{
		loginPanel.SetActive(activePanel.Equals(loginPanel.name));
		selectionPanel.SetActive(activePanel.Equals(selectionPanel.name));
		joinRandomRoomPanel.SetActive(activePanel.Equals(joinRandomRoomPanel.name));
		insideRoomPanel.SetActive(activePanel.Equals(insideRoomPanel.name));
	}

	//Logs in the player when they click the Login button if the name field isn't empty 
	public void OnLoginButtonClicked()
	{
		string playerName = playerNameInput.text;

		if (!playerName.Equals(""))
		{
			PhotonNetwork.LocalPlayer.NickName = playerName;
			PhotonNetwork.ConnectUsingSettings();
		}
		else
		{
			Debug.LogError("Player Name is invalid.");
		}
	}

	public void OnJoinRoomButtonClicked()
	{
		if (roomNameInput.text == string.Empty)
		{
			SetActivePanel(joinRandomRoomPanel.name);

			PhotonNetwork.JoinRandomRoom();
		}
		else
		{
			PhotonNetwork.JoinRoom(roomNameInput.text);
		}
	}

	//Creates a new room with the specified name in roomNameInput
	public void OnCreateRoomButtonClicked()
	{
		var options = new RoomOptions {MaxPlayers = 4, BroadcastPropsChangeToAll = true};

		string roomName;
		if (newRoomNameInput.text != string.Empty) roomName = newRoomNameInput.text;
		else roomName = "Room " + Random.Range(0, 10000);
		
		PhotonNetwork.CreateRoom(roomName, options);
	}

	public void OnLeaveGameButtonClicked()
	{
		PhotonNetwork.LeaveRoom();
	}

	//Loads the arena, only the MasterClient can launch the game
	public void OnStartGameButtonClicked()
	{
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;

		SceneManager.LoadScene("Arena");
	}

	#endregion
}