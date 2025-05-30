using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SurrounderSensor : MonoBehaviour
{
    private AnimalBehaviour behaviour;
    private AnimalSO animalSO;

    void Awake() {
        behaviour = GetComponent<AnimalBehaviour>();
        animalSO = behaviour.GetAnimalSO();
    }


    public bool TrySenseNearestGrassTilesNearMate(
        out List<Vector2Int> adjacentWalkableTilesNearMates,
        out List<Vector2Int> matePositions
    ) {
        Vector2Int ownPos = transform.position.ToVector2Int();
        var tempGrassTiles = new List<Vector2Int>();
        var tempMateTiles = new List<Vector2Int>();


        var sensingArea = GetSensingArea();

        var mateLocations = WorldPlacable.GetAnimals(animalSO).Where(a => a.IsFemale() != behaviour.IsFemale()).Select(a => a.Position).ToList();

        foreach (var position in mateLocations) {
            if (!sensingArea.Contains(position)) continue;

            foreach (var neighbor in GetCardinalNeighbors(position)) {
                if (!Pathfinding.IsInBounds(neighbor)) continue;
                if (UnwalkableAreaMap.blockedArea.Contains(neighbor)) continue;

                tempGrassTiles.Add(neighbor);
                tempMateTiles.Add(position);
            }
        }

        if (tempGrassTiles.Count == 0) {
            adjacentWalkableTilesNearMates = null;
            matePositions = null;
            return false;
        }

        var combined = new List<(Vector2Int Grass, Vector2Int Food)>();
        for (int i = 0; i < tempGrassTiles.Count; i++) {
            combined.Add((tempGrassTiles[i], tempMateTiles[i]));
        }

        combined.Sort((a, b) =>
            ((a.Grass - ownPos).sqrMagnitude).CompareTo((b.Grass - ownPos).sqrMagnitude));

        adjacentWalkableTilesNearMates = new();
        matePositions = new();
        foreach (var pair in combined) {
            adjacentWalkableTilesNearMates.Add(pair.Grass);
            matePositions.Add(pair.Food);
        }
        return true;
    }


    public bool TrySenseNearestGrassTilesNearFood(
        out List<Vector2Int> grassTiles,
        out List<Vector2Int> correspondingFoodTiles)
    {
        Vector2Int ownPos = transform.position.ToVector2Int();
        var tempGrassTiles = new List<Vector2Int>();
        var tempFoodTiles = new List<Vector2Int>();


        var sensingArea = GetSensingArea();

        var foodArea = FoodGeneration.Instance.GetFoodTiles();

        var eatableFoodArea = foodArea.Where(food => animalSO.EatableFood.Contains(food.FoodSO)).Select(x => x.Position).ToList();
        var preyLocation = WorldPlacable.GetAnimals(animalSO.Target).Select(a => a.Position).ToList();
        eatableFoodArea.AddRange(preyLocation);

        foreach (var position in eatableFoodArea)
        {
            if (!sensingArea.Contains(position)) continue;

            foreach (var neighbor in GetCardinalNeighbors(position))
            {
                if (!Pathfinding.IsInBounds(neighbor)) continue;
                if (UnwalkableAreaMap.blockedArea.Contains(neighbor)) continue;

                tempGrassTiles.Add(neighbor);
                tempFoodTiles.Add(position);
            }
        }

        if (tempGrassTiles.Count == 0)
        {
            grassTiles = null;
            correspondingFoodTiles = null;
            return false;
        }

        // Create list of pairs and sort
        var combined = new List<(Vector2Int Grass, Vector2Int Food)>();
        for (int i = 0; i < tempGrassTiles.Count; i++)
        {
            combined.Add((tempGrassTiles[i], tempFoodTiles[i]));
        }

        combined.Sort((a, b) =>
            ((a.Grass - ownPos).sqrMagnitude).CompareTo((b.Grass - ownPos).sqrMagnitude));

        // Output
        grassTiles = new();
        correspondingFoodTiles = new();
        foreach (var pair in combined)
        {
            grassTiles.Add(pair.Grass);
            correspondingFoodTiles.Add(pair.Food);
        }
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
