using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Networking
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        private bool isConnecting;
    
        private string gameVersion = "1";

        [Header("Login Panel")]
        public GameObject LoginPanel;
        public InputField PlayerNameInput;

        [Header("Selection Panel")]
        public GameObject SelectionPanel;

        [Header("Join Random Room Panel")]
        public GameObject JoinRandomRoomPanel;

        [Header("Inside Room Panel")]
        public GameObject InsideRoomPanel;

        public GameObject PlayerInRoomPrefab;
    
        private Dictionary<int, GameObject> playerListEntries;
    
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

        #region PUN methods
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
            PhotonNetwork.CreateRoom(roomName, options);
        }
    
        public override void OnJoinedRoom()
        {
            SetActivePanel(InsideRoomPanel.name);

            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            foreach (var p in PhotonNetwork.PlayerList)
            {
                var entry = Instantiate(PlayerInRoomPrefab, InsideRoomPanel.transform, true);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<PlayerInRoom>().Initialize(p.ActorNumber, p.NickName);

                playerListEntries.Add(p.ActorNumber, entry);
            }

            var props = new Hashtable
            {
                {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnLeftRoom()
        {
            SetActivePanel(SelectionPanel.name);

            foreach (GameObject entry in playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            playerListEntries.Clear();
            playerListEntries = null;
        }
    
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            var entry = Instantiate(PlayerInRoomPrefab, InsideRoomPanel.transform, true);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerInRoom>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

            playerListEntries.Add(newPlayer.ActorNumber, entry);
        }
    
    
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);
        }

        #endregion

        #region UI Methods
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
            JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
        }
    
        public void OnLoginButtonClicked()
        {
            string playerName = PlayerNameInput.text;

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
    
        public void OnJoinRandomRoomButtonClicked()
        {
            SetActivePanel(JoinRandomRoomPanel.name);

            PhotonNetwork.JoinRandomRoom();
        }
        
        public void OnCreateRoomButtonClicked()
        {
            string roomName = "Room " + Random.Range(1000, 10000);
            var options = new RoomOptions {MaxPlayers = 4};

            PhotonNetwork.CreateRoom(roomName, options);
        }
        
        public void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnStartGameButtonClicked()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel("Arena");
        }
        #endregion
    }
}
