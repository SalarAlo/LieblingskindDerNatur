using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ReproductionUrgeResponder : UrgeResponder
{
    private bool foundMate;
    private bool hasWaitedForPreviousWalkBeforeStarting;
    private bool hasStartedWaitingForPreviousWalk;
    private AnimalMovementComponent movementComponent;
    private SurrounderSensor surrounderSensor;
    private Vector2Int destination;
    [SerializeField] private List<Vector2Int> path;
    private float mutationStrengthSpeed = .1f;
    private int mutationStrengthSenseRange = 1;
    private AnimalBehaviour targetMate;
    private AnimalBehaviour behaviour;
    private AnimalEffects animalEffects;

    protected override void Awake()
    {
        base.Awake();
        movementComponent = GetComponent<AnimalMovementComponent>();
        surrounderSensor = GetComponent<SurrounderSensor>();
        animalEffects = GetComponent<AnimalEffects>();
        behaviour = GetComponent<AnimalBehaviour>();
    }

    public override void RespondToUrge()
    {
        if (HandleWaitForPreviousWalk())
            return;

        if (!foundMate)
        {
            if (!TryFindPathToMate())
            {
                Debug.Log("[ReproductionUrgeResponder] No mate found, going to random destination.");
                GoToRandomDestination();
            }
        }
        else
        {
            Debug.Log("[ReproductionUrgeResponder] Mate found, moving toward mate.");
            MoveTowardMate();
        }
    }

    public override Urge GetUrge()
    {
        return Urge.Reproduction;
    }

    private bool HandleWaitForPreviousWalk()
    {
        if (!hasWaitedForPreviousWalkBeforeStarting && movementComponent.IsDoingMove())
        {
            if (!hasStartedWaitingForPreviousWalk)
            {
                movementComponent.OnMoveDone += MovementComponent_OnMoveDone;
                hasStartedWaitingForPreviousWalk = true;
            }
            return true;
        }
        return false;
    }

    private bool TryFindPathToMate()
    {
        if (movementComponent.IsDoingMove())
        {
            return true;
        }

        if (CheckForMate(out List<Vector2Int> possibleDestinations, out List<Vector2Int> foodPos))
        {
            Debug.Log("[ReproductionUrgeResponder] calculating mate");

            var position = transform.position.ToVector2Int();

            for (int i = 0; i < possibleDestinations.Count; i++)
            {
                path = Pathfinding.FindPath(position, possibleDestinations[i]);
                if (path != null)
                {
                    destination = foodPos[i];
                    targetMate = WorldPlacable.GetWorldPlacableAt(destination) as AnimalBehaviour;
                    if (targetMate != null)
                    {
                        targetMate.GetComponent<AnimalMovementComponent>().DisableMovement();
                        targetMate.GetComponent<AnimalEffects>().EnableLoveEffect();
                        animalEffects.EnableLoveEffect();
                        foundMate = true;
                        Debug.Log("[ReproductionUrgeResponder] Found mate at: " + destination);
                        return true;
                    }
                    else
                    {
                    }
                }
            }
        }
        return false;
    }

    private void MoveTowardMate()
    {
        if (movementComponent.IsDoingMove())
        {
            return;
        }

        if (targetMate == null)
        {
            Debug.LogWarning("[ReproductionUrgeResponder] targetMate is died, resetting foundMate.");
            foundMate = false;
            animalEffects.DisableLoveEffect();
            return;
        }

        var position = transform.position.ToVector2Int();
        if (!TryGetNearestGrassTileTo(targetMate.Position, out Vector2Int nearestGrassTile))
        {
            Debug.LogWarning("[ReproductionUrgeResponder] No grass tile near mate.");
            foundMate = false;
            targetMate = null;
            targetMate.GetComponent<AnimalEffects>().DisableLoveEffect();
            animalEffects.DisableLoveEffect();
            return;
        }
        path = Pathfinding.FindPath(position, nearestGrassTile);

        if (path == null)
        {
            Debug.LogWarning("[ReproductionUrgeResponder] No path to nearest grass tile.");
            foundMate = false;
            targetMate = null;
            targetMate.GetComponent<AnimalEffects>().DisableLoveEffect();
            animalEffects.DisableLoveEffect();
            return;
        }

        if (path.Count == 0)
        {
            Debug.Log("[ReproductionUrgeResponder] Arrived at mate, finishing urge.");
            FinishUrge();
            return;
        }

        Vector3 currentDestination = new(path[0].x, 0, path[0].y);
        movementComponent.MoveTo(currentDestination);
        path.RemoveAt(0);
    }

    private bool TryGetNearestGrassTileTo(Vector2Int position, out Vector2Int nearestGrass)
    {
        nearestGrass = default;
        Vector2Int ownPosition = transform.position.ToVector2Int();
        List<Vector2Int> grassTiles = GetCardinalNeighbors(position);
        grassTiles = grassTiles
            .Where(tile => Pathfinding.IsInBounds(tile) && !UnwalkableAreaMap.blockedArea.Contains(tile))
            .OrderBy(validTile => (validTile - ownPosition).sqrMagnitude)
            .ToList();
        if (grassTiles.Count == 0)
        {
            Debug.LogWarning("[ReproductionUrgeResponder] No valid grass tiles found near position: ");
            return false;
        }
        nearestGrass = grassTiles[0];
        return true;
    }

    private void MovementComponent_OnMoveDone()
    {
        hasWaitedForPreviousWalkBeforeStarting = true;
        movementComponent.OnMoveDone -= MovementComponent_OnMoveDone;
    }

    private void GoToRandomDestination()
    {
        Vector2Int destination;
        Vector2Int position = transform.position.ToVector2Int();

        int tries = 0;
        do
        {
            destination = position + GetRandomDir();
            tries++;
        } while (!Pathfinding.IsInBounds(destination) || UnwalkableAreaMap.blockedArea.Contains(destination));

        movementComponent.MoveTo(new(destination.x, 0, destination.y));
    }

    private Vector2Int GetRandomDir()
    {
        Vector2Int dir;
        do
        {
            dir = new(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2));
        } while (dir == Vector2Int.zero);
        return dir;
    }

    private bool CheckForMate(out List<Vector2Int> grassTileFoodPos, out List<Vector2Int> foodPos)
    {
        bool result = surrounderSensor.TrySenseNearestGrassTilesNearMate(out grassTileFoodPos, out foodPos);
        return result;
    }

    private List<Vector2Int> GetCardinalNeighbors(Vector2Int pos)
    {
        return new() {
            new(pos.x + 1, pos.y),
            new(pos.x - 1, pos.y),
            new(pos.x, pos.y + 1),
            new(pos.x, pos.y - 1)
        };
    }

    protected override void FinishUrge()
    {
        base.FinishUrge();
        Debug.Log("[ReproductionUrgeResponder] FinishUrge called. Spawning offspring.");
        if (targetMate.TryGetComponent<AnimalMovementComponent>(out var moveComp))
            moveComp.EnableMovement();
        targetMate.GetComponent<AnimalEffects>().DisableLoveEffect();
        animalEffects.DisableLoveEffect();

        var matingPossibilities = behaviour.GetAnimalSO().MatingPossibilities;
        int childCount = UnityEngine.Random.Range(matingPossibilities.x, matingPossibilities.y);
        for (int i = 0; i < childCount; i++)
        {
            float childBaseSpeed = (targetMate.Speed + behaviour.Speed) / 2;
            int childBaseSenseRange = Mathf.RoundToInt((targetMate.SenseRange + behaviour.SenseRange) / 2.0f);
            var child = Instantiate(behaviour.GetAnimalSO().Model, behaviour.Position.ToVector3(), Quaternion.identity);

            var childBehaviour = child.GetComponent<AnimalBehaviour>();
            var childSpeed = childBaseSpeed + UnityEngine.Random.Range(-mutationStrengthSpeed, mutationStrengthSpeed);
            var childSenseRange = childBaseSenseRange + UnityEngine.Random.Range(-mutationStrengthSenseRange, mutationStrengthSenseRange + 1);
            childBehaviour.SetGenes(childSpeed, childSenseRange);
        }

        targetMate = null;
        foundMate = false;
        hasWaitedForPreviousWalkBeforeStarting = false;
        hasStartedWaitingForPreviousWalk = false;
    }

    void OnDestroy() {
        if (targetMate == null) return;
        targetMate.GetComponent<AnimalEffects>().DisableLoveEffect();
        animalEffects.DisableLoveEffect();
    }
}
