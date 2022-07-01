using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ragdoll : MonoBehaviour
{
    public GameObject ragdoll;
    public List<Rigidbody> rigidbodies = new List<Rigidbody>();
    private List<Vector3> initialRBpositions = new List<Vector3>();
    private List<Quaternion> initialRBRotations = new List<Quaternion>();

    private void Awake()
    {
        for (int i = 0; i < rigidbodies.Count; i++)
        {
            initialRBpositions.Add(rigidbodies[i].transform.localPosition);
            initialRBRotations.Add(rigidbodies[i].transform.localRotation);
        }

        //Disabled by default
        gameObject.SetActive(false);
    }

    public void ToggleRagdoll(bool toggle)
    {
        for (int i = 0; i < rigidbodies.Count; i++)
        {
            var rb = rigidbodies[i];

            if (toggle)
            {
                //Add some force
                rb.AddExplosionForce(500, rb.transform.localPosition, 785);
                ragdoll.transform.SetParent(null);
            }
            else
            {
                ragdoll.transform.SetParent(transform);
                //Reset LOCAL position, rotation
                //Reset velocity
                rb.velocity = Vector3.zero;
                rb.transform.localPosition = initialRBpositions[i];
                rb.transform.localRotation = initialRBRotations[i];
            }
        }
    }
}
