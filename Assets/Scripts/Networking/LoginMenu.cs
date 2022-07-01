using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LoginMenu : MonoBehaviourPunCallbacks
{
    public TMP_InputField playerNameInput;
    public TMP_InputField roomInput;
    public GameObject loginPanel;
    public static string GameVersion;
    public bool isConnecting = false;
    public byte maxPlayersPerRoom = 10;
    public string sceneToLoad;

    public TextMeshProUGUI statusText;

    public enum JoinType
    {
        Create,
        Join,
        Random
    }

    private JoinType joinType;

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void LoginButton()
    {
        Connect();
    }

    public void CreateRoomButton()
    {
        joinType = JoinType.Create;
        Connect();
    }

    public void JoinRoomButton()
    {
        if (roomInput.text.Length > 0)
        {
            joinType = JoinType.Join;
            Connect();
        }
        else
        {
            statusText.text = "Unable to join a room with no name";
        }
    }

    public void JoinRandomRoomButton()
    {
        joinType = JoinType.Random;
        Connect();
    }

    private void Connect()
    {
        Debug.Log("Connect!");
        isConnecting = true;

        if (PhotonNetwork.IsConnected == false)
        {
            Debug.Log("Connected to photon!");
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = GameVersion;
        }

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = playerNameInput.text.Length > 0 ? playerNameInput.text : "Player" + Random.Range(0, 5000);

        loginPanel.SetActive(false);
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;
        loginPanel.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {

            statusText.text = "Joining room : '" + roomInput.text + "'...";
            // Load the Room Level. 
            PhotonNetwork.LoadLevel(sceneToLoad);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master...");

        if (isConnecting)
        {
            Debug.Log("Currently connecting...");

            switch (joinType)
            {
                case JoinType.Create:
                    PhotonNetwork.CreateRoom(RoomToJoin(), new RoomOptions { MaxPlayers = maxPlayersPerRoom }, TypedLobby.Default);
                    break;
                case JoinType.Join:
                    PhotonNetwork.JoinRoom(RoomToJoin());
                    break;
                case JoinType.Random:
                    PhotonNetwork.JoinRandomOrCreateRoom();
                    break;
            }
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = "Failed to create room...";
        loginPanel.SetActive(true);
        isConnecting = false;

        PhotonNetwork.Disconnect();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        statusText.text = "Failed to join random room...";
        loginPanel.SetActive(true);
        isConnecting = false;

        PhotonNetwork.Disconnect();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = "Failed to join room : '"+roomInput.text+"'";
        loginPanel.SetActive(true);
        isConnecting = false;

        PhotonNetwork.Disconnect();
    }


    private string RoomToJoin()
    {
        return roomInput.text.Length > 0 ? roomInput.text : null;
    }
}
