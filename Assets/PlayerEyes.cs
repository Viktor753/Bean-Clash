using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerEyes : MonoBehaviour
{
    public PhotonView pv;
    public MeshRenderer meshRenderer;
    public Transform cam;

    private void Start()
    {
        if (pv.IsMine)
        {
            meshRenderer.enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (pv.IsMine)
        {
            transform.forward = cam.forward;
        }
    }
}
