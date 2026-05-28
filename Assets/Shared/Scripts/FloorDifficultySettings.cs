using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloorDifficultySettings
{
    [Header("Spawn interval for this floor (seconds)")]
    public float spawnInterval = 3f;

    [Header("Max zombies per spawner on this floor")]
    public int maxZombies = 3;

    [Header("Zombies that can spawn on this floor")]
    public List<GameObject> zombiePrefabs;
}