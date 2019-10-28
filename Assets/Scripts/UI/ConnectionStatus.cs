using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionStatus : MonoBehaviour
{
    private string connectionStatusMessage;
    private Text text;
    
    void Start()
    {
        text = GetComponent<Text>();
        connectionStatusMessage = text.text;
    }

    void Update()
    {
        text.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
    }
}
