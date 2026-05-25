using UnityEngine;

[System.Serializable]
public class FloorDifficultySettings
{
    [Header("Spawn interval for this floor (seconds)")]
    public float spawnInterval = 3f;

    [Header("Max zombies per spawner on this floor")]
    public int maxZombies = 3;
}