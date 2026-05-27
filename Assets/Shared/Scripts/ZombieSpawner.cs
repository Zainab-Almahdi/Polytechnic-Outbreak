using System.Collections;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Drag your Zombie prefab here")]
    public GameObject zombiePrefab;

    [Header("How often a zombie spawns (seconds)")]
    public float spawnInterval = 3f;

    [Header("Max zombies alive from this spawner")]
    public int maxZombies = 3;

    private static int _aliveCount = 0;
    private Coroutine _spawnRoutine;

    public void Activate()
    {
        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);

        _spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void Deactivate()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }

        _aliveCount = 0;
    }

    // Called by GameSpawnController to adjust difficulty
    public void SetDifficulty(float newInterval, int newMaxZombies)
    {
        spawnInterval = newInterval;
        maxZombies = newMaxZombies;

        Debug.Log($"{gameObject.name} difficulty set — interval: {newInterval}s, max: {newMaxZombies}");

        // Restart coroutine to apply new interval immediately
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = StartCoroutine(SpawnLoop());
        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (_aliveCount < maxZombies)
                SpawnZombie();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnZombie()
    {
        if (zombiePrefab == null)
        {
            Debug.LogError($"{gameObject.name} has no Zombie Prefab assigned!");
            return;
        }

        Vector3 randomOffset = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        );

        Vector3 spawnPosition = transform.position + randomOffset;
        GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
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

    IEnumerator WaitForDeath(GameObject zombie)
    {
        // Keep checking until the zombie is destroyed
        while (zombie != null)
        {
            yield return null;
        }

        // Zombie was destroyed — reduce count
        _aliveCount--;
        Debug.Log($"{gameObject.name} zombie died. Alive: {_aliveCount}");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}