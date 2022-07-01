using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    public PhotonView pv;

    public CharacterController controller;
    public Transform cam;

    public float movementSpeed = 4.6f;
    public float jumpHeight = 3.0f;

    public float gravity = -9.82f;
    private Vector3 velocity;

    private bool isGrounded = true;
    public Transform groundCheckTransform;
    public float groundCheckRadius;
    public LayerMask groundMask;

    private bool freezeMovement;

    private bool teleporting = false;
   

    private void Update()
    {
        if(pv.IsMine == false)
        {
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundMask);

        if(velocity.y < 0 && isGrounded)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;


        Move();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && GameChat.chatOpen == false && freezeMovement == false)
        {
            Jump();
        }
    }


    private void Jump()
    {
        velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
    }

    private void Move()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (GameChat.chatOpen)
        {
            verticalInput = 0;
            horizontalInput = 0;
        }


        if (freezeMovement == false)
        {
            controller.Move((transform.forward * verticalInput + transform.right * horizontalInput).normalized * Time.deltaTime * movementSpeed);

            controller.Move(velocity * Time.deltaTime);
        }
        
    }


    public void Teleport(Vector3 position, Quaternion rotation)
    {
        pv.RPC("TellServerToTeleport", RpcTarget.All, position, rotation);
    }

    public void FreezeMovement(bool freeze)
    {
        pv.RPC("TellServerToFreezeMovement", RpcTarget.All, freeze);
    }

    [PunRPC]
    private void TellServerToFreezeMovement(bool freeze)
    {
        if (pv.IsMine)
        {
            freezeMovement = freeze;
        }
    }

    [PunRPC]
    private void TellServerToTeleport(Vector3 position, Quaternion rotation)
    {
        if (pv.IsMine)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
    }
}
