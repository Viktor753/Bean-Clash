using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Item : MonoBehaviour
{
    public enum itemType
    {
        Primary,
        Secondary,
        Knife,
        Utility,
        Special
    }
    public itemType type;
    public bool canDrop = false;
    public bool dropOnDeath = false;
    public PhotonView photonView;
    public Rigidbody rb;
    public bool pickedUp = false;
    private int ownerID;

    public bool visible = false;

    private void Start()
    {
        photonView.RPC("OnPlayerJoinedLate", RpcTarget.All);
    }

    private void OnEnable()
    {
        Player.localInstance.spawnedPlayer.GetComponent<Health>().OnPlayerDied += OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        if (ownerID == Player.localInstance.spawnedPlayer.GetComponent<PhotonView>().ViewID)
        {
            if (dropOnDeath && canDrop)
            {
                DropFromPlayer();
            }
        }
    }


    [PunRPC]
    private void OnPlayerJoinedLate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (pickedUp)
            {
                photonView.RPC("SyncItemState", RpcTarget.All, pickedUp, ownerID);
            }
        }
    }

    [PunRPC]
    private void SyncItemState(bool pickedUp, int ownerID)
    {
        this.pickedUp = pickedUp;
        this.ownerID = ownerID;
        photonView.TransferOwnership(ownerID);
        
        UpdateItemPosition();
    }

    public void GiveToPlayer(int playerID)
    {
        if (pickedUp == false)
        {
            photonView.RPC("GiveToPlayerRPC", RpcTarget.All, playerID);
        }
    }

    public void DropFromPlayer()
    {
        if (pickedUp)
        {
            photonView.RPC("DropFromPlayerRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void GiveToPlayerRPC(int playerPhotonID)
    {
        photonView.TransferOwnership(playerPhotonID);
        ownerID = playerPhotonID;
        pickedUp = true;
        UpdateItemPosition();
    }

    [PunRPC]
    private void DropFromPlayerRPC()
    {
        photonView.TransferOwnership(PhotonNetwork.MasterClient);
        pickedUp = false;
        UpdateItemPosition();
    }


    private void UpdateItemPosition()
    {
        rb.isKinematic = pickedUp;
        rb.useGravity = !pickedUp;

        if (pickedUp)
        {
            var playerHand = PhotonView.Find(ownerID).GetComponent<PlayerInventory>().playerHand;
            transform.SetParent(playerHand.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.SetParent(null);
        }
    }

    public void ToggleItemVisible(bool toggle)
    {
        transform.GetChild(0).gameObject.SetActive(toggle);
        return;
        photonView.RPC("ToggleItemVisibleRPC", RpcTarget.All, toggle);
    }

    [PunRPC]
    private void ToggleItemVisibleRPC(bool toggle)
    {
        visible = toggle;
    }
}
