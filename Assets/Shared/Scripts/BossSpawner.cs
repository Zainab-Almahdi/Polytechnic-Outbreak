using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : ZombieSpawner
{
    [SerializeField] private AudioClip spawnScreech;
    private bool hasSpawned = false;

    public override void SetDifficulty(float newInterval, int newMaxZombies, List<GameObject> newPool)
    {
        zombiePool = newPool;
    }

    protected override IEnumerator SpawnLoop()
    {
        if (!hasSpawned && zombiePool != null && zombiePool.Count > 0)
        {
            SpawnZombie();
            hasSpawned = true;
        }
        yield break;
    }

    protected override void SpawnZombie()
    {
        base.SpawnZombie();
        
        if (spawnScreech != null)
        {
            AudioSource source = GetComponent<AudioSource>();
            if (source == null) source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.PlayOneShot(spawnScreech);
        }
    }
}