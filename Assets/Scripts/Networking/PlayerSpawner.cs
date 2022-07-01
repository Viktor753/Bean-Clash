using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public static Action OnLocalPlayerObjectSpawned;

    private GameObject playerSpawned;

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            playerSpawned = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
            playerSpawned.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            OnLocalPlayerObjectSpawned?.Invoke();
        }
    }
}
