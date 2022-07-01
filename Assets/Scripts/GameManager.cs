using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool playerCanJoinGame;
    public enum MatchResult
    {
        Blue,
        Red,
        Draw
    }

    public MatchResult result;

    public PhotonView pv;
    public int maxRounds = 10;

    public int roundsToWin;
    public int halfTimeRound = 5;
    [Space]
    public int roundsPlayed = 0;
    public int blueWins = 0;
    public int redWins = 0;

    public bool bombPlanted;

    public enum GameState {
        MatchStarting,
        RoundStarting,
        RoundPlaying,
        RoundEnding,
        MatchEnding,
        NotPlaying
    }

    [Space]
    [SerializeField] private GameState state;

    
    public bool freezeTime = false;
    public float gameStartTime;
    public float roundTime;
    public float roundEndTime;
    public float roundSpawnTime;
    public float gameEndTime;
    private float currentTimer;
    string currentStatus = "Waiting for Host to start...";
    private bool bombDefused = true;
    public GameUIManager UI;

    public static Action OnRoundEnded;
    public static Action OnRoundStarted;
    public static Action OnPlayerCanJoin;

    public bool IsHost
    {
        get { return PhotonNetwork.IsMasterClient; }
    }

    private void Awake()
    {
        instance = this;
        if (IsHost)
        {
            playerCanJoinGame = true;
        }
        else
        {
            OnPlayerJoinedRoom();
        }
    }

    private void OnPlayerJoinedRoom()
    {
        SetCanJoin();
    }

    private void SetCanJoin()
    {
        pv.RPC("TellServerToSetCanJoin", RpcTarget.All);
    }

    [PunRPC]
    private void TellServerToSetCanJoin()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("SetCanJoinRPC", RpcTarget.All, playerCanJoinGame);
        }
    }

    [PunRPC]
    private void SetCanJoinRPC(bool toggle)
    {
        playerCanJoinGame = toggle;
        OnPlayerCanJoin?.Invoke();
    }

    private void Update()
    {
        if (IsHost)
        {
            if(state == GameState.NotPlaying && Input.GetKeyDown(KeyCode.R))
            {
                currentStatus = "Starting match...";
                currentTimer = gameStartTime;
                state = GameState.MatchStarting;
                freezeTime = false;
            }

            if (freezeTime == false)
            {
                currentTimer -= Time.deltaTime;
                currentTimer = Mathf.Clamp(currentTimer, 0, float.MaxValue);
            }

            if(currentTimer <= 0)
            {
                OnTimeRanOut();
            }

            UI.pv.RPC("UpdateUI", RpcTarget.AllViaServer, roundsPlayed, maxRounds, currentTimer, blueWins, redWins, currentStatus);
        }
    }

    public void OnPlayerDied()
    {
        
    }
    public void OnBombDefused()
    {
        bombDefused = true;
        EndRound();
    }

    public void SwitchSides()
    {

    }

    public void OnTimeRanOut()
    {
        Debug.Log("Timer ran out");

        switch (state)
        {
            case GameState.MatchStarting:
                StartMatch();
                break;
            case GameState.RoundStarting:
                StartRound();
                break;
            case GameState.RoundPlaying:
                EndRound();
                break;
            case GameState.RoundEnding:
                NextRound();
                break;
            case GameState.MatchEnding:
                EndMatch();
                break;
        }
    }

    private void NextRound()
    {
        //Teleport players to spawn
        var bluePlayers = TeamsManager.instance.blueTeam;
        for (int i = 0; i < bluePlayers.Count; i++)
        {
            var player = PhotonView.Find(bluePlayers[i]).GetComponent<Player>().spawnedPlayer.
                GetComponent<PlayerMovement>();

            player.FreezeMovement(true);
            player.Teleport(SpawnPoints.instance.blueTeamSpawns[i].position, SpawnPoints.instance.blueTeamSpawns[i].rotation);
            
        }

        var redPlayers = TeamsManager.instance.redTeam;
        for (int i = 0; i < redPlayers.Count; i++)
        {
            var player = PhotonView.Find(redPlayers[i]).GetComponent<Player>().spawnedPlayer.
                GetComponent<PlayerMovement>();

            player.FreezeMovement(true);
            player.Teleport(SpawnPoints.instance.redTeamSpawns[i].position, SpawnPoints.instance.redTeamSpawns[i].rotation);
            
        }

        playerCanJoinGame = true;
        SetCanJoin();
        OnPlayerCanJoin?.Invoke();
        currentStatus = "Round starting...";
        state = GameState.RoundStarting;
        currentTimer = roundSpawnTime;

        //Assign bomb to random red team player

        OnRoundEnded?.Invoke();
    }

    private void StartMatch()
    {
        roundsPlayed = 0;
        blueWins = 0;
        redWins = 0;
        NextRound();
    }

    private void EndMatch()
    {
        freezeTime = true;
        Debug.Log("End match");
        state = GameState.NotPlaying;
        currentStatus = "Game over!";
        
    }

    private void StartRound()
    {
        playerCanJoinGame = false;
        SetCanJoin();
        Debug.Log("Start round");
        state = GameState.RoundPlaying;
        currentTimer = roundTime;
        currentStatus = "Round playing...";

        OnRoundStarted?.Invoke();

        var bluePlayers = TeamsManager.instance.blueTeam;
        for (int i = 0; i < bluePlayers.Count; i++)
        {
            var player = PhotonView.Find(bluePlayers[i]).GetComponent<Player>().spawnedPlayer.
                GetComponent<PlayerMovement>();

            player.FreezeMovement(false);
        }

        var redPlayers = TeamsManager.instance.redTeam;
        for (int i = 0; i < redPlayers.Count; i++)
        {
            var player = PhotonView.Find(redPlayers[i]).GetComponent<Player>().spawnedPlayer.
                GetComponent<PlayerMovement>();

            player.FreezeMovement(false);
        }
    }

    private void EndRound()
    {
        bool endGame = false;
        roundsPlayed++;

        if(roundsPlayed == halfTimeRound)
        {
            SwitchSides();
        }

        if (bombDefused)
        {
            //Blue team auto win round
            blueWins++;
        }

        if(blueWins == roundsToWin || redWins == roundsToWin || roundsPlayed == maxRounds)
        {
            //End match
            endGame = true;
        }

        if (endGame)
        {
            if(blueWins > redWins)
            {
                //Blue win
                result = MatchResult.Blue;
            }
            else if(redWins > blueWins)
            {
                //Red win
                result = MatchResult.Red;
            }
            else
            {
                //Draw
                result = MatchResult.Draw;
            }

            state = GameState.RoundEnding;
            currentTimer = roundEndTime;
            currentStatus = "Ending game...";
        }
        else
        {
            state = GameState.RoundEnding;
            currentTimer = roundEndTime;
            currentStatus = "Ending round...";
        }
    }
}
