using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class Gun : MonoBehaviour
{
    public int baseDamage = 30;
    public bool automatic = false;
    public Transform fireOrigin;
    public float maxRange;
    public LayerMask hitMask;
    

    public float fireRate;
    private float timeSinceLastShot;

    public Action OnFire;
    public Action OnReload;

    private void Update()
    {

        bool fireInput = automatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
        timeSinceLastShot += Time.deltaTime;
        timeSinceLastShot = Mathf.Clamp(timeSinceLastShot, 0, 60 / fireRate);

        if (fireInput && timeSinceLastShot >= 60/fireRate)
        {
            //Tried to fire weapon
            Fire();
            timeSinceLastShot = 0;
        }
    }

    public void Fire() 
    {
        Debug.Log("Fired gun");
        Vector3 origin = fireOrigin.position;
        Vector3 direction = fireOrigin.forward;

        if(Physics.Raycast(origin, direction,out var hit, maxRange,hitMask))
        {
            Debug.Log($"Hit {hit.transform.name}");
            if(hit.transform.TryGetComponent<Health>(out var healthComponent))
            {
                Debug.Log($"Damage {hit.transform.name}");
                healthComponent.AttemptDealDamage(baseDamage);
            }
        }

        OnFire?.Invoke();
    }

    public void Reload()
    {
        OnReload?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (fireOrigin != null)
        {
            Debug.DrawRay(fireOrigin.position, fireOrigin.forward * maxRange, Color.red, 3f);
        }
    }
}
