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

    // Called by FloorTriggerZone
    public void OnPlayerEntered()
    {
        // Tell the controller the player changed floors
        spawnController.OnPlayerChangedFloor(floorIndex);
    }

    public void ActivateFloor()
    {
        foreach (ZombieSpawner spawner in _spawners)
            spawner.Activate();
    }

    public void DeactivateFloor()
    {
        foreach (ZombieSpawner spawner in _spawners)
            spawner.Deactivate();
    }

    // Used by GameSpawnController to adjust difficulty
    public List<ZombieSpawner> GetSpawners()
    {
        return _spawners;
    }
}