using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerUI : MonoBehaviour
{
    public PhotonView pv;
    public GameObject playerUI;
    public GameObject spectateUI;

    private void Awake()
    {
        playerUI.SetActive(pv.IsMine);
        spectateUI.SetActive(false);
    }
}
