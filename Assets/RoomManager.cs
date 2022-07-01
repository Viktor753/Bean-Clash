using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public string menuScene;
    public bool inGame = false;
    private bool leftRoom = false;

    public void StartMatch()
    {
        if (PhotonNetwork.IsConnected)
        {
            inGame = true;
            PhotonNetwork.LoadLevel("Level1");
        }
    }

    public void ExitRoom()
    {
        if (leftRoom == false)
        {
            PhotonNetwork.LeaveRoom();
            leftRoom = true;
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(menuScene);
    }

    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = PhotonNetwork.CurrentRoom.Name;
    }
}
