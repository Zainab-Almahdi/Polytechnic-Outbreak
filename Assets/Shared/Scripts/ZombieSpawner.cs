using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Pool of Zombie prefabs")]
    public List<GameObject> zombiePool;

    [Header("How often a zombie spawns (seconds)")]
    public float spawnInterval = 3f;

    [Header("Max zombies alive from this spawner")]
    public int maxZombies = 3;

    protected int _aliveCount = 0;
    protected Coroutine _spawnRoutine;

    public virtual void Activate()
    {
        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);

        _spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public virtual void Deactivate()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }

        _aliveCount = 0;
    }

    // Called by GameSpawnController to adjust difficulty
    public virtual void SetDifficulty(float newInterval, int newMaxZombies, List<GameObject> newPool)
    {
        spawnInterval = newInterval;
        maxZombies = newMaxZombies;
        zombiePool = newPool;

        // Restart coroutine to apply new interval immediately
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = StartCoroutine(SpawnLoop());
        }
    }

    protected virtual IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (_aliveCount < maxZombies)
                SpawnZombie();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    protected virtual void SpawnZombie()
    {
        if (zombiePool == null || zombiePool.Count == 0)
        {
            Debug.LogError($"{gameObject.name} has no Zombie Prefabs in pool!");
            return;
        }

        GameObject prefab = zombiePool[Random.Range(0, zombiePool.Count)];

        Vector3 randomOffset = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        );

        Vector3 spawnPosition = transform.position + randomOffset;
        GameObject zombie = Instantiate(prefab, spawnPosition, Quaternion.identity);
        _aliveCount++;

        // Hook into teammate's ZombieHealth
        ZombieHealth health = zombie.GetComponent<ZombieHealth>();
        if (health != null)
        {
            // Start watching for when the zombie is destroyed
            StartCoroutine(WaitForDeath(zombie));
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} spawned a zombie with no ZombieHealth!");
        }

        Debug.Log($"{gameObject.name} spawned a zombie. Alive: {_aliveCount}");
    }

    protected IEnumerator WaitForDeath(GameObject zombie)
    {
        // Keep checking until the zombie is destroyed
        while (zombie != null)
        {
            yield return null;
        }

        // Zombie was destroyed reduce count
        _aliveCount--;
        Debug.Log($"{gameObject.name} zombie died. Alive: {_aliveCount}");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}