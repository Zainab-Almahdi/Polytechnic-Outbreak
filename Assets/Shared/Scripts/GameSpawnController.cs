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
        if (floorDifficulties.Count != floors.Count)
            Debug.LogError("Floor count and difficulty count don't match!");
    }

    public void OnPlayerChangedFloor(int floorIndex)
    {
        if (_currentFloorIndex == floorIndex) return;

        // Deactivate previous floor
        if (_currentFloor != null)
            _currentFloor.DeactivateFloor();

        _currentFloorIndex = floorIndex;
        _currentFloor = floors[floorIndex];

        // Apply this floor's settings
        FloorDifficultySettings settings = floorDifficulties[floorIndex];

        foreach (ZombieSpawner spawner in _currentFloor.GetSpawners())
            spawner.SetDifficulty(settings.spawnInterval, settings.maxZombies);

        // Activate the floor
        _currentFloor.ActivateFloor();

        Debug.Log($"Player moved to Floor {floorIndex + 1} — Interval: {settings.spawnInterval}s | Max Zombies: {settings.maxZombies}");
    }
}