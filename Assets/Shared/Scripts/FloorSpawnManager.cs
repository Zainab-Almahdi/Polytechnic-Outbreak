using System.Collections.Generic;
using UnityEngine;

public class FloorSpawnManager : MonoBehaviour
{
    [Header("Which floor is this? (0 = floor 1, 1 = floor 2...)")]
    public int floorIndex = 0;

    [Header("Drag all floor managers here")]
    public List<FloorSpawnManager> allFloors;

    [Header("Drag the GameSpawnController here")]
    public GameSpawnController spawnController;

    private List<ZombieSpawner> _spawners = new List<ZombieSpawner>();

    void Awake()
    {
        GetComponentsInChildren(true, _spawners);
        Debug.Log($"Floor {floorIndex + 1} found {_spawners.Count} spawners.");
    }

    // Fires when player walks into the floor's box collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered Floor {floorIndex + 1}");
            spawnController.OnPlayerChangedFloor(floorIndex);
        }
    }

    public void ActivateFloor()
    {
        foreach (ZombieSpawner spawner in _spawners)
            spawner.Activate();

        Debug.Log($"Floor {floorIndex + 1} activated.");
    }

    public void DeactivateFloor()
    {
        foreach (ZombieSpawner spawner in _spawners)
            spawner.Deactivate();

        Debug.Log($"Floor {floorIndex + 1} deactivated.");
    }

    public List<ZombieSpawner> GetSpawners()
    {
        return _spawners;
    }
}