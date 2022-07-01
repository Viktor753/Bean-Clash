using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class Health : MonoBehaviour
{
    public static bool isDead = false;
    public CharacterController controller;
    public Ragdoll ragdoll;
    public PlayerMovement movement;
    public PlayerCamera playerCamera;
    public GameObject playerModel;


    public GameObject playerUI;
    public GameObject spectateUI;

    [SerializeField] private PhotonView pv;

    [SerializeField] private int maxHealth = 100;
    
    private int localHealthValue;
    private int serverHealthValue;

    public Action<int> OnHealthChanged;
    public Action OnPlayerDied;
    public Action OnPlayerSpawn;

    public int HealthValue
    {
        get { return serverHealthValue; }
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            OnSpawn();
        }
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                localHealthValue -= 5;
                SetHealth(localHealthValue);
            }
        }
    }

    public void AttemptDealDamage(int damage)
    {
        //TODO: Calculate armor
        int newHealth = serverHealthValue - damage;

        if (serverHealthValue - damage <= 0)
        {
            newHealth = 0;
        }

        //This gets called on everyone
        pv.RPC("DealDamage", RpcTarget.AllViaServer, newHealth);
    }

    [PunRPC]
    private void DealDamage(int newHealth)
    {
        Debug.Log("DEAL DMG TO PLAYER!");
        //Only call on this owner client to set health locally first then to server
        if (pv.IsMine)
        {
            Debug.Log("Dealing damage from gun only on hit player locally new health : " + newHealth);
            SetHealth(newHealth);
        }
    }

    private void SetHealth(int newHealthValue)
    {
        if (isDead)
        {
            return;
        }

        localHealthValue = newHealthValue;
        localHealthValue = Mathf.Clamp(localHealthValue, 0, maxHealth);

        OnHealthChanged?.Invoke(localHealthValue);

        if(localHealthValue <= 0 && isDead == false)
        {
            OnDeath();
        }

        pv.RPC("SetHealthServer", RpcTarget.AllViaServer, localHealthValue);
    }

    [PunRPC]
    private void SetHealthServer(int value)
    {
        serverHealthValue = value;
    }

    private void OnDeath()
    {
        OnPlayerDied?.Invoke();

        isDead = true;
        movement.enabled = false;
        //playerCamera.enabled = false;
        playerUI.SetActive(false);
        spectateUI.SetActive(true);
        pv.RPC("OnDeathServer", RpcTarget.All);
    }

    private void OnSpawn()
    {
        OnPlayerSpawn?.Invoke();

        isDead = false;
        SetHealth(maxHealth);
        movement.enabled = true;
        //playerCamera.enabled = true;
        playerUI.SetActive(true);
        spectateUI.SetActive(false);

        pv.RPC("OnSpawnServer", RpcTarget.All);
    }

    [PunRPC]
    private void OnDeathServer()
    {
        ragdoll.gameObject.SetActive(true);
        ragdoll.ToggleRagdoll(true);
        playerModel.SetActive(false);

        controller.enabled = false;
    }

    [PunRPC]
    private void OnSpawnServer()
    {
        playerModel.SetActive(true);
        ragdoll.ToggleRagdoll(false);
        ragdoll.gameObject.SetActive(false);
        controller.enabled = true;
    }
}
