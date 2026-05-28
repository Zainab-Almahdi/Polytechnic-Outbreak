using System.Collections.Generic;
using UnityEngine;
using Assets.UI.Scripts;

public class GameSpawnController : MonoBehaviour
{
    [Header("All floors - drag in order (floor1 first)")]
    public List<FloorSpawnManager> floors;

    [Header("Difficulty settings per floor - one entry per floor")]
    public List<FloorDifficultySettings> floorDifficulties;

    private FloorSpawnManager _currentFloor;
    private int _currentFloorIndex = -1;

    void Start()
    {
        if (floorDifficulties.Count != floors.Count)
        {
            Debug.LogError("Floor count and difficulty count don't match!");
            return;
        }
    }

    public void OnPlayerChangedFloor(int floorIndex)
    {
        if (_currentFloorIndex == floorIndex) return;

        // Update HUD
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.SetFloorLabel((floorIndex + 1).ToString());
        }

        if (_currentFloor != null) _currentFloor.DeactivateFloor();

        _currentFloorIndex = floorIndex;
        _currentFloor = floors[floorIndex];

        if (GameManager.Instance != null)
            GameManager.Instance.UpdateMaxFloor(floorIndex);

        if (_currentFloor == null) return;

        FloorDifficultySettings settings = floorDifficulties[floorIndex];
        if (settings == null) return;

        List<GameObject> pool = new List<GameObject>();
        if (settings.zombiePrefabs != null) pool.AddRange(settings.zombiePrefabs);

        // If after floor 2 (index 1), mix in previous levels, but skip for Floor 6 (index 5)
        if (floorIndex >= 2 && floorIndex < 5)
        {
            for (int i = 0; i < floorIndex; i++)
            {
                if (floorDifficulties[i].zombiePrefabs != null)
                    pool.AddRange(floorDifficulties[i].zombiePrefabs);
            }
        }

        List<ZombieSpawner> spawners = _currentFloor.GetSpawners();
        foreach (ZombieSpawner spawner in spawners)
        {
            if (spawner != null)
                spawner.SetDifficulty(settings.spawnInterval, settings.maxZombies, pool);
        }

        _currentFloor.ActivateFloor();
    }
}