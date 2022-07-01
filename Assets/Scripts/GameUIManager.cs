using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class GameUIManager : MonoBehaviour
{
    public PhotonView pv;
    public TextMeshProUGUI totalRoundsPlayedText;
    public TextMeshProUGUI roundTimerText;
    public TextMeshProUGUI blueScoreText;
    public TextMeshProUGUI redScoreText;

    public TextMeshProUGUI statusText;

    
    [PunRPC]
    public void UpdateUI(int roundsPlayed,int totalRounds, float roundTimer, int blueScore, int redScore, string statusMessage)
    {
        totalRoundsPlayedText.text = $"{roundsPlayed}/{totalRounds}";
        roundTimerText.text = roundTimer.ToString("F1");
        blueScoreText.text = blueScore.ToString();
        redScoreText.text = redScore.ToString();
        statusText.text = statusMessage;
    }
}
