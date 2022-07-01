using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using TMPro;

public class TeamSelectManager : MonoBehaviour
{
    public PhotonView pv;
    public GameObject gameUI;
    public GameObject teamSelectUI;

    public static Action<int, int> OnPlayerSelectedTeam;

    public int blueSpawnID;
    public int redSpawnID;


    public TextMeshProUGUI joinNextRoundText;

    private void Start()
    {
        gameUI.SetActive(false);
        teamSelectUI.SetActive(true);

        Debug.Log("Sync sever spawn IDs with local IDs");
        pv.RPC("TellServerToSyncSpawnIDs", RpcTarget.All);
    }

    public void SelectTeam(int team)
    {
        gameUI.SetActive(true);
        teamSelectUI.SetActive(false);
        PlayerPrefs.SetInt("PlayerTeam", team);

        int spawnIDToIncrement = team == 0 ? blueSpawnID : redSpawnID;
        OnPlayerSelectedTeam?.Invoke(team, spawnIDToIncrement);
        pv.RPC("IncrementSpawnID", RpcTarget.All, team);
    }

    public void OnPlayerLeft(int teamID)
    {
        if (PhotonNetwork.IsConnected)
        {
            pv.RPC("DecrementSpawnID", RpcTarget.All, teamID);
        }
    }


    [PunRPC]
    private void DecrementSpawnID(int teamID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(teamID == 0)
            {
                blueSpawnID--;
            }
            else
            {
                redSpawnID--;
            }
            pv.RPC("SyncSpawnIDToClients", RpcTarget.All, blueSpawnID, redSpawnID);
        }
    }

    [PunRPC]
    private void IncrementSpawnID(int teamID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(teamID == 0)
            {
                blueSpawnID++;
            }
            else
            {
                redSpawnID++;
            }

            pv.RPC("SyncSpawnIDToClients", RpcTarget.All, blueSpawnID, redSpawnID);
        }
    }

    [PunRPC]
    private void TellServerToSyncSpawnIDs()
    {
        Debug.Log("Server is syncing spawn IDs...");
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("SyncSpawnIDToClients", RpcTarget.All, blueSpawnID, redSpawnID);
        }
    }

    [PunRPC]
    private void SyncSpawnIDToClients(int blueSpawnID, int redSpawnID)
    {
        this.blueSpawnID = blueSpawnID;
        this.redSpawnID = redSpawnID;
        Debug.Log("Synced spawn IDs!");
        Debug.Log("Blue ID : " + blueSpawnID);
        Debug.Log("Red ID : " + redSpawnID);
    }
}
