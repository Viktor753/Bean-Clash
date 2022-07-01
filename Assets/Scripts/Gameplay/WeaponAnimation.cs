using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponAnimation : MonoBehaviour
{
    public PhotonView pv;

    public Gun gun;
    public Animator animator;

    public string fireTrigger;
    public string reloadTrigger;


    private void OnEnable()
    {
        gun.OnFire += OnFire;
        gun.OnReload += OnReload;
    }

    private void OnDisable()
    {
        gun.OnFire -= OnFire;
        gun.OnReload -= OnReload;
    }

    
    private void OnFire()
    {
        animator.SetTrigger(fireTrigger);
    }

    private void OnReload()
    {
        animator.SetTrigger(reloadTrigger);
    }
}
