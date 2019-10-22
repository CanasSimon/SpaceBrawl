using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    private const string playerNamePrefKey = "PlayerName";

    private void Start()
    {
        var defaultName = string.Empty;
        var inputField = GetComponent<InputField>();

        if (inputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                inputField.text = defaultName;
            }
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
}
