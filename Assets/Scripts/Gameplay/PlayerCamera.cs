using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCamera : MonoBehaviour
{
    public PhotonView pv;
    public PlayerMovement movement;
    public Camera cam;

    public Transform player;

    public float sensitivity = 200f;
    private float xRotation = 0.0f;

    private void Start()
    {
        cam.enabled = pv.IsMine;
        if(pv.IsMine == false)
        {
            return;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (pv.IsMine == false)
        {
            return;
        }

        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        if (GameChat.chatOpen || PlayerInventory.buyMenuOpen)
        {
            mouseX = 0;
            mouseY = 0;
        }

        player.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
