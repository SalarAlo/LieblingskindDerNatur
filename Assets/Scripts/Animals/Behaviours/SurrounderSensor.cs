using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SurrounderSensor : MonoBehaviour
{
    private AnimalBehaviour behaviour;

    void Awake() {
        behaviour = GetComponent<AnimalBehaviour>();
    }

    public bool TrySenseFood(out Vector2Int position) {
        Vector2Int ownPosition = new((int)transform.position.x, (int)transform.position.z);
        position = default;
        return true;
    }

    public bool TrySenseNearestGrassNearWater(out List<Vector2Int> grassTile) {
        Vector2Int ownPosition = new((int)transform.position.x, (int)transform.position.z);
        grassTile = default;

        var sensingArea = GetSensingArea();
        var waterArea = WorldGeneration.Instance.WaterTiles;
        var visibleWaterTiles = sensingArea.Intersect(waterArea).ToList();

        if (visibleWaterTiles.Count == 0)
            return false;

        HashSet<Vector2Int> candidateGrassTiles = new();

        foreach (var water in visibleWaterTiles) {
            foreach (var neighbor in GetCardinalNeighbors(water)) {
                if (!Pathfinding.IsInBounds(neighbor)) continue;
                if (waterArea.Contains(neighbor)) continue; // skip water
                if (UnwalkableAreaMap.blockedArea.Contains(neighbor)) continue; // skip blocked
                candidateGrassTiles.Add(neighbor);
            }
        }

        if (candidateGrassTiles.Count == 0)
            return false;

        grassTile = candidateGrassTiles
            .OrderBy(pos => (pos - ownPosition).sqrMagnitude)
            .ToList();

        return true;
    }
    private List<Vector2Int> GetCardinalNeighbors(Vector2Int pos) {
        return new()
        {
            new(pos.x + 1, pos.y),
            new(pos.x - 1, pos.y),
            new(pos.x, pos.y + 1),
            new(pos.x, pos.y - 1)
        };
    }


    public HashSet<Vector2Int> GetSensingArea() {
        Vector2Int ownPosition = new((int)transform.position.x, (int)transform.position.z);
        int senseRange = behaviour.GetAnimalSO().SenseRange;

        HashSet<Vector2Int> sensingArea = new();

        for (int dx = -senseRange; dx <= senseRange; dx++) {
            for (int dz = -senseRange; dz <= senseRange; dz++) {
                if (dx * dx + dz * dz <= senseRange * senseRange) {
                    Vector2Int sensedPos = ownPosition + new Vector2Int(dx, dz);
                    if(Pathfinding.IsInBounds(sensedPos))
                        sensingArea.Add(sensedPos);
                }
            }
        }

        return sensingArea;
    }
}
