using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class Player : MonoBehaviourPunCallbacks
{
    public static Player localInstance;
    public PhotonView pv;
    public enum Team
    {
        Blue,
        Red
    }

    public Team team;

    public GameObject blueSidePlayer;
    public GameObject redSidePlayer;
    [HideInInspector] public GameObject spawnedPlayer;

    public static Action OnPlayerSpawned;
    public static Action OnPlayerDeSpawned;

    public bool readyToEnterGame = false;

    private int teamID;
    private int spawnIndex;

    

    private void Awake()
    {
        if (pv.IsMine)
        {
            localInstance = this;
            GameManager.OnPlayerCanJoin += OnAvailbleToJoin;


            //PhotonNetwork.CurrentRoom.CustomProperties.
        }
    }


    public override void OnEnable()
    {
        base.OnEnable();
        if (pv.IsMine)
        {
            TeamSelectManager.OnPlayerSelectedTeam += SetReadyToJoin;
            TeamsManager.instance.UpdateUICalledByPlayer();
        }
    }



    public override void OnDisable()
    {
        base.OnEnable();
        if (pv.IsMine)
        {
            DeSpawnPlayerController();
            TeamSelectManager.OnPlayerSelectedTeam -= SetReadyToJoin;
        }
        else
        {
            TeamsManager.instance.OnPlayerLeft(pv.ViewID, pv.Owner.NickName, teamID);
        }
    }


    private void SetReadyToJoin(int teamID, int spawnIndex)
    {
        this.teamID = teamID;
        this.spawnIndex = spawnIndex;
        pv.RPC("SyncLocalTeamID", RpcTarget.All, this.teamID);
        readyToEnterGame = true;
        if(readyToEnterGame == true && GameManager.playerCanJoinGame)
        {
            SpawnPlayerController(teamID, spawnIndex);
        }
        else
        {
            FindObjectOfType<TeamSelectManager>().joinNextRoundText.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    private void SyncLocalTeamID(int teamID)
    {
        this.teamID = teamID;
        if(this.teamID == 0)
        {
            team = Team.Blue;
        }
        else
        {
            team = Team.Red;
        }
    }

    private void OnAvailbleToJoin()
    {
        if(readyToEnterGame && spawnedPlayer == null)
        {
            //Spawn player
            SpawnPlayerController(teamID, spawnIndex);

            //spawned in during round start. Freeze movement
            spawnedPlayer.GetComponent<PlayerMovement>().FreezeMovement(true);
        }
    }
   

    [ContextMenu("Spawn")]
    public void SpawnPlayerController(int teamID, int spawnIndex)
    {
        team = teamID == 0 ? Team.Blue : Team.Red;
        var playerToSpawn = teamID == 0 ? blueSidePlayer : redSidePlayer;

        var spawnPoints = FindObjectOfType<SpawnPoints>();
        Transform spawn = teamID == 0 ? spawnPoints.blueTeamSpawns[spawnIndex] : spawnPoints.redTeamSpawns[spawnIndex];

        spawnedPlayer = PhotonNetwork.Instantiate(playerToSpawn.name, spawn.position, spawn.rotation);
        
        TeamsManager.instance.OnPlayerJoined(pv.ViewID, PhotonNetwork.NickName, PlayerPrefs.GetInt("PlayerTeam"));


        OnPlayerSpawned?.Invoke();
        pv.RPC("SetSpawnedPlayer", RpcTarget.Others, spawnedPlayer.GetComponent<PhotonView>().ViewID);

        FindObjectOfType<TeamSelectManager>().joinNextRoundText.gameObject.SetActive(false);
    }

    [ContextMenu("DeSpawn")]
    public void DeSpawnPlayerController()
    {
        Debug.Log("Despawn player...");
        OnPlayerDeSpawned?.Invoke();

        PhotonNetwork.Destroy(spawnedPlayer);
        spawnedPlayer = null;
    }

    [PunRPC]
    private void SetSpawnedPlayer(int spawnedPlayerID)
    {
        spawnedPlayer = PhotonView.Find(spawnedPlayerID).gameObject;
    }
}
