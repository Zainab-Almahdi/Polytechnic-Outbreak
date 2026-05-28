using System.Collections.Generic;
using UnityEngine;

public class GameSpawnController : MonoBehaviour
{
    [Header("All floors — drag in order (floor1 first)")]
    public List<FloorSpawnManager> floors;

    [Header("Difficulty settings per floor — one entry per floor")]
    public List<FloorDifficultySettings> floorDifficulties;

    private FloorSpawnManager _currentFloor;
    private int _currentFloorIndex = -1;

    void Start()
    {
        Debug.Log($"GameSpawnController started. Floors: {floors.Count} | Difficulties: {floorDifficulties.Count}");

        if (floorDifficulties.Count != floors.Count)
        {
            Debug.LogError("Floor count and difficulty count don't match!");
            return;
        }

        // Print all floor settings on start
        for (int i = 0; i < floors.Count; i++)
        {
            if (floors[i] == null)
            {
                Debug.LogError($"Floor {i + 1} is NULL — did you forget to drag it in?");
                continue;
            }

            Debug.Log($"Floor {i + 1} registered — " +
                      $"Interval: {floorDifficulties[i].spawnInterval}s | " +
                      $"Max Zombies: {floorDifficulties[i].maxZombies}");
        }
    }

    public void OnPlayerChangedFloor(int floorIndex)
    {
        Debug.Log($"OnPlayerChangedFloor called — floorIndex: {floorIndex}");

        if (_currentFloorIndex == floorIndex)
        {
            Debug.Log($"Player is already on Floor {floorIndex + 1}, ignoring.");
            return;
        }

        // Deactivate previous floor
        if (_currentFloor != null)
        {
            Debug.Log($"Deactivating Floor {_currentFloorIndex + 1}");
            _currentFloor.DeactivateFloor();
        }

        _currentFloorIndex = floorIndex;
        _currentFloor = floors[floorIndex];

        if (_currentFloor == null)
        {
            Debug.LogError($"Floor {floorIndex + 1} is NULL in the list!");
            return;
        }

        // Apply settings
        FloorDifficultySettings settings = floorDifficulties[floorIndex];

        if (settings == null)
        {
            Debug.LogError($"Difficulty settings for Floor {floorIndex + 1} are NULL!");
            return;
        }

        Debug.Log($"Applying difficulty to Floor {floorIndex + 1} — " +
                  $"Interval: {settings.spawnInterval}s | Max Zombies: {settings.maxZombies}");

        List<ZombieSpawner> spawners = _currentFloor.GetSpawners();
        Debug.Log($"Floor {floorIndex + 1} has {spawners.Count} spawners");

        foreach (ZombieSpawner spawner in spawners)
        {
            if (spawner == null)
            {
                Debug.LogError("A spawner in the list is NULL!");
                continue;
            }

            spawner.SetDifficulty(settings.spawnInterval, settings.maxZombies);
            Debug.Log($"SetDifficulty called on {spawner.gameObject.name}");
        }

        // Activate floor
        Debug.Log($"Activating Floor {floorIndex + 1}");
        _currentFloor.ActivateFloor();
    }
}