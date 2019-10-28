using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInRoom : MonoBehaviour
{
    [Header("UI References")] public Text PlayerNameText;

    private int ownerId;

    #region UNITY

    public void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == ownerId)
        {
            PhotonNetwork.LocalPlayer.SetScore(0);
        }
    }

    #endregion

    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        PlayerNameText.text = playerName;
    }
}