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

    public bool TrySenseNearestWater(out List<Vector2Int> position) {
        Vector2Int ownPosition = new((int)transform.position.x, (int)transform.position.z);
        position = default;
        var sensingArea = GetSensingArea();
        var waterArea = WorldGeneration.Instance.WaterTiles;
        
        var visibleWaterTiles = sensingArea.Intersect(waterArea).ToList();
        if(visibleWaterTiles.Count == 0) return false;
        
        position = visibleWaterTiles
            .OrderBy(p => (p - ownPosition).sqrMagnitude)
            .ToList();
        return true;
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
