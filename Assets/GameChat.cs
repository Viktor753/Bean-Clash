using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class GameChat : MonoBehaviour
{
    public Transform logs;
    public PhotonView pv;
    public TextMeshProUGUI textPrefab;
    public List<TextMeshProUGUI> spawnedTextMessages = new List<TextMeshProUGUI>();

    public TMP_InputField chatInputField;


    public float messageLifeSpan = 5;

    public Color localColor;
    public Color blueColor;
    public Color redColor;

    public static bool chatOpen = false;



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chatInputField.gameObject.activeSelf)
            {
                SendMessage();
                CloseChat();
            }
            else
            {
                OpenChat();
            }
        }
    }


    #region ClientSide

    public void OnPlayerJoined(string senderName, int teamID)
    {
        pv.RPC("SendMessageToClients", RpcTarget.All, senderName, "joined",
                teamID == 0 ? GetVectorColor(blueColor) : GetVectorColor(redColor));
    }

    public void OnPlayerLeft(string senderName, int teamID)
    {
        pv.RPC("SendMessageToClients", RpcTarget.All, senderName, "left",
                teamID == 0 ? GetVectorColor(blueColor) : GetVectorColor(redColor));
    }

    public void SendMessage()
    {
        if (chatInputField.text.Length > 0)
        {
            string nick = PhotonNetwork.NickName;
            SpawnText(nick,chatInputField.text,GetVectorColor(localColor));
            var message = chatInputField.text;

            chatInputField.text = string.Empty;
            
            pv.RPC("SendMessageToClients", RpcTarget.Others, nick, message,
                Player.localInstance.team == Player.Team.Blue ? GetVectorColor(blueColor) : GetVectorColor(redColor));
        }
    }

    private Vector3 GetVectorColor(Color color)
    {
        return new Vector3(color.r, color.g, color.b);
    }

    public void OpenChat()
    {
        chatInputField.gameObject.SetActive(true);
        chatInputField.ActivateInputField();

        chatOpen = true;
    }

    public void CloseChat()
    {
        chatInputField.gameObject.SetActive(false);
        chatOpen = false;
    }

    private void SpawnText(string senderName, string message, Vector3 color)
    {
        var newText = Instantiate(textPrefab, logs);
        newText.color = new Color(color.x, color.y, color.z);
        newText.text = senderName + ": " + message;
        spawnedTextMessages.Add(newText);

        StartCoroutine(DeleteText(newText, messageLifeSpan));
    }

    private IEnumerator DeleteText(TextMeshProUGUI textToDelete, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        spawnedTextMessages.Remove(textToDelete);
        Destroy(textToDelete.gameObject);
    }

    #endregion

    #region Networked

    [PunRPC]
    private void SendMessageToClients(string senderName, string message, Vector3 col)
    {
        SpawnText(senderName, message, col);
    }

    #endregion

}
