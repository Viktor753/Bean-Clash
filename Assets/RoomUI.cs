using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class RoomUI : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI lobbyName;
    public Image[] blueTeam;
    public Image[] redTeam;

    public GameObject UIblocker;
    public GameObject StartButton;

    public override void OnEnable()
    {
        base.OnEnable();

        if (PhotonNetwork.IsConnected)
        {
            UIblocker.SetActive(PhotonNetwork.IsMasterClient == false);
            StartButton.SetActive(PhotonNetwork.IsMasterClient);
            lobbyName.text = PhotonNetwork.CurrentRoom.Name;
            OnJoinedRoom();
        }
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (PhotonNetwork.IsConnected)
        {
            UIblocker.SetActive(PhotonNetwork.IsMasterClient == false);
            StartButton.SetActive(PhotonNetwork.IsMasterClient);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("joined room");
        //UpdatePlayerUIList();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("left room");
        //UpdatePlayerUIList();
    }

    public void UpdatePlayerUIList()
    {
        Debug.Log("Hello!");
        //Fetch players in room and update UI
    }
}
