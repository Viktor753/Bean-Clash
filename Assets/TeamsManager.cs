using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using TMPro;
using System.Threading.Tasks;

public class TeamsManager : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    public static TeamsManager instance;

    //PhotonView IDs
    public List<int> blueTeam = new List<int>();
    public List<string> blueTeamNicks = new List<string>();
    public List<int> redTeam = new List<int>();
    public List<string> redTeamNicks = new List<string>();

    public PlayerDisplay[] blueTeamSelectDisplays;
    public PlayerDisplay[] redTeamSelectDisplays;

    public PlayerDisplay[] blueTeamScoreboardDisplays;
    public PlayerDisplay[] redTeamScoreboardDisplays;

    public GameObject scoreBoardUI;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        scoreBoardUI.SetActive(Input.GetKey(KeyCode.Tab));
    }

    public void UpdateUICalledByPlayer()
    {
        pv.RPC("SyncUI", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void SyncUI()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("SyncTeams", RpcTarget.AllViaServer ,blueTeam.ToArray(),blueTeamNicks.ToArray(), redTeam.ToArray(), redTeamNicks.ToArray());
        }
    }

    public void OnPlayerJoined(int photonViewID, string nickName, int teamToJoin)
    {
        if (PhotonNetwork.IsConnected)
        {
            FindObjectOfType<GameChat>().OnPlayerJoined(nickName, teamToJoin);
            pv.RPC("TellServerToUpdatePlayerJoined", RpcTarget.AllViaServer, photonViewID, nickName, teamToJoin);
        }
    }

    public void OnPlayerLeft(int photonViewID, string nickName, int team)
    {
        if (PhotonNetwork.IsConnected)
        {
            FindObjectOfType<GameChat>().OnPlayerLeft(nickName, team);
            pv.RPC("TellServerToUpdatePlayerLeft", RpcTarget.AllViaServer, photonViewID);
        }
    }

    [PunRPC]
    public void TellServerToUpdatePlayerJoined(int playerID, string nickName, int teamID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(teamID == 0)
            {
                blueTeam.Add(playerID);
                blueTeamNicks.Add(nickName);
            }
            else
            {
                redTeam.Add(playerID);
                redTeamNicks.Add(nickName);
            }
            pv.RPC("SyncTeams", RpcTarget.All, blueTeam.ToArray(),blueTeamNicks.ToArray(), redTeam.ToArray(), redTeamNicks.ToArray());
        }
    }

    [PunRPC]
    public void TellServerToUpdatePlayerLeft(int playerID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (blueTeam.Contains(playerID))
            {
                var index = blueTeam.IndexOf(playerID);
                blueTeamNicks.RemoveAt(index);
                blueTeam.Remove(playerID);
            }

            if (redTeam.Contains(playerID))
            {
                var index = redTeam.IndexOf(playerID);
                redTeamNicks.RemoveAt(index);
                redTeam.Remove(playerID);
            }

            pv.RPC("SyncTeams", RpcTarget.All, blueTeam.ToArray(),blueTeamNicks.ToArray(), redTeam.ToArray(), redTeamNicks.ToArray());
        }
    }

    

    [PunRPC]
    private void SyncTeams(int[] blueTeam, string[] blueTeamNicks, int[] redTeam, string[] redTeamNicks)
    {
        this.blueTeam = blueTeam.ToList();
        this.redTeam = redTeam.ToList();
        this.blueTeamNicks = blueTeamNicks.ToList();
        this.redTeamNicks = redTeamNicks.ToList();




        for (int i = 0; i < blueTeamSelectDisplays.Length; i++)
        {
            var playerDisplay = this.blueTeamSelectDisplays[i];
            if (i < this.blueTeam.Count)
            {
                playerDisplay.nameText.text = this.blueTeamNicks[i];
            }
            else
            {
                playerDisplay.nameText.text = string.Empty;
            }
        }


        for (int i = 0; i < redTeamSelectDisplays.Length; i++)
        {
            var playerDisplay = this.redTeamSelectDisplays[i];
            if (i < this.redTeam.Count)
            {
                playerDisplay.nameText.text = this.redTeamNicks[i];
            }
            else
            {
                playerDisplay.nameText.text = string.Empty;
            }
        }

        scoreBoardUI.SetActive(true);

        for (int i = 0; i < blueTeamScoreboardDisplays.Length; i++)
        {
            var playerDisplay = this.blueTeamScoreboardDisplays[i];
            if (i < this.blueTeam.Count)
            {
                playerDisplay.nameText.text = this.blueTeamNicks[i];
            }
            else
            {
                playerDisplay.nameText.text = string.Empty;
            }
        }

        for (int i = 0; i < redTeamScoreboardDisplays.Length; i++)
        {
            var playerDisplay = this.redTeamScoreboardDisplays[i];
            if (i < this.redTeam.Count)
            {
                playerDisplay.nameText.text = this.redTeamNicks[i];
            }
            else
            {
                playerDisplay.nameText.text = string.Empty;
            }
        }
        scoreBoardUI.SetActive(false);

    }
}
