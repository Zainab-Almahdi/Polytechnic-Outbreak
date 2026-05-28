using UnityEngine;

public class ZombieDropper : MonoBehaviour
{
    // Drag your powerup prefabs here in Inspector
    public GameObject[] powerupPrefabs;

    // Drop chance between 0 and 1 (0.02 = 2%, 0.10 = 10%)
    public float dropChance = 0.05f;

    // TODO: Call this function from zombie death script
    public void OnZombieDeath()
    {
        float roll = Random.Range(0f, 1f);

        if (roll <= dropChance && powerupPrefabs.Length > 0)
        {
            // Pick a random powerup
            int index = Random.Range(0, powerupPrefabs.Length);
            GameObject prefab = powerupPrefabs[index];

            // Spawn it at zombie position with a slight height offset (floating)
            Vector3 spawnPos = transform.position + Vector3.up * 1.0f;
            Instantiate(prefab, spawnPos, Quaternion.identity);

            Debug.Log("Powerup dropped: " + prefab.name);
        }
    }
}