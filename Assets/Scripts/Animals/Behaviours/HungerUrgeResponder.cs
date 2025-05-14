using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HungerUrgeResponder : UrgeResponder {
    private bool foundFood;
    private bool hasWaitedForPreviousWalkBeforeStarting;
    private bool hasStartedWaitingForPreviousWalk;
    private AnimalMovementComponent movementComponent;
    private SurrounderSensor surrounderSensor;
    private Vector2Int destination;
    [SerializeField] private List<Vector2Int> path;
    private WorldPlacable target;

    public override Urge GetUrge() => Urge.Hunger;

    protected override void Awake() {
        base.Awake();
        movementComponent = GetComponent<AnimalMovementComponent>();
        surrounderSensor = GetComponent<SurrounderSensor>();
    }

    public override void RespondToUrge() {
        if (HandleWaitForPreviousWalk())
            return;

        if (!foundFood) {
            if (!TryFindPathToFood())
                GoToRandomDestination();
        } else {
            MoveTowardFood();
        }
    }

    private bool HandleWaitForPreviousWalk() {
        if (!hasWaitedForPreviousWalkBeforeStarting && movementComponent.IsDoingMove()) {
            if (!hasStartedWaitingForPreviousWalk) {
                movementComponent.OnMoveDone += MovementComponent_OnMoveDone;
                hasStartedWaitingForPreviousWalk = true;
            }
            return true;
        }
        return false;
    }

    private bool TryFindPathToFood() {
        if (movementComponent.IsDoingMove())
            return true;

        if (CheckForFood(out List<Vector2Int> possibleDestinations, out List<Vector2Int> foodPos)) {
            var position = transform.position.ToVector2Int();

            for (int i = 0; i < possibleDestinations.Count; i++) {
                path = Pathfinding.FindPath(position, possibleDestinations[i]);
                if (path != null) {
                    destination = foodPos[i];
                    target = WorldPlacable.GetWorldPlacableAt(destination);
                    foundFood = true;
                    return true;
                }
            }
        }
        return false;
    }

    private void MoveTowardFood() {
        if (movementComponent.IsDoingMove())
            return;

        if (target == null) {
            foundFood = false;
            return;
        }

        var position = transform.position.ToVector2Int();
        if(!TryGetNearestGrassTileTo(target.Position, out Vector2Int nearestGrassTile)) {
            foundFood = false;
            return;
        }
        path = Pathfinding.FindPath(position, nearestGrassTile);

        if (path == null || path.Count == 0) {
            if(target is AnimalBehaviour) {
                Destroy(target.gameObject);
            } else {
                FoodGeneration.Instance.RemoveFood(destination);
            }
            FinishUrge();
            return;
        }

        Vector3 currentDestination = new(path[0].x, 0, path[0].y);
        movementComponent.MoveTo(currentDestination);
        path.RemoveAt(0);
    }

    private bool TryGetNearestGrassTileTo(Vector2Int position, out Vector2Int nearestGrass) {
        nearestGrass = default;
        Vector2Int ownPosition = transform.position.ToVector2Int();
        List<Vector2Int> grassTiles = GetCardinalNeighbors(position);
        grassTiles = grassTiles
            .Where(tile => Pathfinding.IsInBounds(tile) && !UnwalkableAreaMap.blockedArea.Contains(tile))
            .OrderBy(validTile => (validTile - ownPosition).sqrMagnitude)
            .ToList();
        if(grassTiles.Count == 0) return false;
        nearestGrass = grassTiles[0];
        return true;
    }

    private void MovementComponent_OnMoveDone() {
        hasWaitedForPreviousWalkBeforeStarting = true;
        movementComponent.OnMoveDone -= MovementComponent_OnMoveDone;
    }

    private void GoToRandomDestination() {
        Vector2Int destination;
        Vector2Int position = transform.position.ToVector2Int();

        do {
            destination = position + GetRandomDir();
        } while (!Pathfinding.IsInBounds(destination) || UnwalkableAreaMap.blockedArea.Contains(destination));

        movementComponent.MoveTo(new(destination.x, 0, destination.y));
    }

    private Vector2Int GetRandomDir() {
        Vector2Int dir;
        do {
            dir = new(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2));
        } while (dir == Vector2Int.zero);
        return dir;
    }

    private bool CheckForFood(out List<Vector2Int> grassTileFoodPos, out List<Vector2Int> foodPos) {
        return surrounderSensor.TrySenseNearestGrassTilesNearFood(out grassTileFoodPos, out foodPos);
    }

    private List<Vector2Int> GetCardinalNeighbors(Vector2Int pos) {
        return new() {
            new(pos.x + 1, pos.y),
            new(pos.x - 1, pos.y),
            new(pos.x, pos.y + 1),
            new(pos.x, pos.y - 1)
        };
    }

    protected override void FinishUrge() {
        base.FinishUrge();
        target = null;
        foundFood = false;
        hasWaitedForPreviousWalkBeforeStarting = false;
        hasStartedWaitingForPreviousWalk = false;
    }
}
