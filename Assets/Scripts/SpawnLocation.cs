using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    public static List<SpawnLocation> spawnLocations = new List<SpawnLocation>();

    public ushort team;

    void Awake() {
        spawnLocations.Add(this);
    }
}
