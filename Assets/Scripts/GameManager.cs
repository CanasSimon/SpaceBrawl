using System;
using System.Collections.Generic;
using Cinemachine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
	public static GameManager Instance;

	private CinemachineVirtualCamera mainCamera;
	
	[SerializeField] private GameObject playerPrefab;

	#region UI Parameters
	[Header("UI")]
	[SerializeField] private GameObject pausePanel;
	[SerializeField] private GameObject winPanel;
	[SerializeField] private Text winText;
	
	[SerializeField] private Text pingText;
	#endregion
	
	private bool CanPause;

	#region Unity Methods
	//Instantiates the local player and makes the camera follow them
	private void Start()
	{
		if (Instance == null) Instance = this;
		
		if (playerPrefab != null)
		{
			if (PlayerController.LocalPlayerInstance == null)
			{
				PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);
			}
		}

		var mainCam = Camera.main;
		if (mainCam != null)
		{
			mainCamera = mainCam.GetComponentInChildren<CinemachineVirtualCamera>();
			mainCamera.m_Follow = PlayerController.LocalPlayerInstance.transform;
		}
		else Debug.Log("Error: No Camera in scene");
	}

	private void Update()
	{
		pingText.text = PhotonNetwork.GetPing() + " ms";
			
		if (Input.GetButtonDown("Pause") && CanPause) pausePanel.SetActive(!pausePanel.activeSelf);
	}

	#endregion

	#region PUN Methods
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if (PhotonNetwork.IsMasterClient) LoadArena();
	}
	
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if (PhotonNetwork.PlayerList.Length < 2)
		{
			PhotonNetwork.LeaveRoom();
		}
	}
	
	public override void OnLeftRoom()
	{
		SceneManager.LoadScene("Lobby");
	}
	#endregion

	private static void LoadArena()
	{
		PhotonNetwork.LoadLevel("Arena");
	}
	
	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

	public void CheckPlayerStatus()
	{
		var alivePlayer = new List<Player>();
		foreach (var p in PhotonNetwork.PlayerList)
		{
			if (!p.CustomProperties.TryGetValue("Health", out var health)) continue;
			Debug.Log(p.NickName);
			Debug.Log((int) health);
			
			if ((int) health > 0)
			{
				alivePlayer.Add(p);
			}
		}
		
		if (alivePlayer.Count == 1)
		{
			Debug.Log(photonView);
			photonView.RPC("DisplayWinScreen", RpcTarget.All, alivePlayer[0]);
		}
	}

	[PunRPC]
	public void DisplayWinScreen(Player winner)
	{
		CanPause = false;
		winText.text = winner.NickName + " won!";
		winPanel.SetActive(true);
	}
}