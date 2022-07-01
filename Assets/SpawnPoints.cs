using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public Transform[] blueTeamSpawns;
    public Transform[] redTeamSpawns;

    public static SpawnPoints instance;

    private void Awake()
    {
        instance = this;
    }
}
