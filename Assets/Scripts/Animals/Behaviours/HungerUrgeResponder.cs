using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class HungerUrgeResponder : UrgeResponder
{
    private bool foundFood;
    private bool hasWaitedForPreviousWalkBeforeStarting;
    private bool hasStartedWaitingForPreviousWalk;
    private AnimalMovementComponent movementComponent;
    private SurrounderSensor surrounderSensor;
    private Vector2Int destination;
    [SerializeField] private List<Vector2Int> path;
    private WorldPlacable target;

    public override Urge GetUrge() {
        return Urge.Hunger;
    }

    protected override void Awake() {
        base.Awake();
        movementComponent = GetComponent<AnimalMovementComponent>();   
        surrounderSensor = GetComponent<SurrounderSensor>();
    }

    private void SetWaitDone() {
        hasWaitedForPreviousWalkBeforeStarting = true;
    }
    public override void RespondToUrge() {
        Debug.Log("[HungerUrgeResponder] RespondToUrge called.");

        if(!hasWaitedForPreviousWalkBeforeStarting && movementComponent.IsDoingMove()) {
            Debug.Log("[HungerUrgeResponder] Waiting for previous move to finish.");
            if(!hasStartedWaitingForPreviousWalk) {
                movementComponent.OnMoveDone += MovementComponent_OnMoveDone;
                hasStartedWaitingForPreviousWalk = true;
                Debug.Log("[HungerUrgeResponder] Subscribed to OnMoveDone event.");
            }
            return;
        }  

        if(!foundFood) {
            if(movementComponent.IsDoingMove()) {
                return;
            }

            if(CheckForFood(out List<Vector2Int> possibleDestinations, out List<Vector2Int> foodPos)) {
                var position = transform.position.GetVector2Int();

                for(int i = 0; i < possibleDestinations.Count; i++) {
                    var possibleDest = possibleDestinations[i];
                    path = Pathfinding.FindPath(position, possibleDest);
                    if(path != null) {
                        destination = foodPos[i];
                        break;
                    }
                }
                if(path==null) {
                    GoToRandomDestination();
                    return;
                }
                foundFood = true;
                target = WorldPlacable.GetWorldPlacableAt(destination);
            } else {
                GoToRandomDestination();
            }     
        } else {
            if(movementComponent.IsDoingMove()) {
                return;
            }

            if(target == null) {
                foundFood = false;
                return;
            }

            // Todo check for nearest neighbour and set that to target
            var position = transform.position.GetVector2Int();
            path = Pathfinding.FindPath(position, target.Position);

            if(path.Count == 0)  {
                FoodGeneration.Instance.RemoveFood(destination);
                FinishUrge();
                return;
            }
            Vector3 currentDestination = new(path[0].x, 0, path[0].y);
            movementComponent.MoveTo(currentDestination);
            path.RemoveAt(0);
        }
    }

    private void MovementComponent_OnMoveDone() {
        SetWaitDone();
        movementComponent.OnMoveDone -= MovementComponent_OnMoveDone;
    }

    private void GoToRandomDestination() {
        Vector2Int destination;
        var posF = transform.position;
        Vector2Int position = new(Mathf.RoundToInt(posF.x), Mathf.RoundToInt(posF.z));
        do {
            destination = position+GetRandomDir();
        } while(!Pathfinding.IsInBounds(destination) || UnwalkableAreaMap.blockedArea.Contains(destination));
        movementComponent.MoveTo(new(destination.x, 0, destination.y));
    }

    private Vector2Int GetRandomDir() {
        Vector2Int x;
        do {
            x = new(UnityEngine.Random.Range(-1, 2),  UnityEngine.Random.Range(-1, 2));
        } while (x == Vector2Int.zero);
        return x;
    }

    private bool CheckForFood(out List<Vector2Int> grassTileFoodPos, out List<Vector2Int> foodPos) {
        return surrounderSensor.TrySenseNearestGrassTilesNearFood(out grassTileFoodPos, out foodPos);
    }
    private List<Vector2Int> GetCardinalNeighbors() {
        Vector2Int pos = transform.position.GetVector2Int();
        return new()
        {
            new(pos.x + 1, pos.y),
            new(pos.x - 1, pos.y),
            new(pos.x, pos.y + 1),
            new(pos.x, pos.y - 1)
        };
    }

    protected override void FinishUrge() {
        base.FinishUrge();
        foundFood = false;
        hasWaitedForPreviousWalkBeforeStarting = false;
        hasStartedWaitingForPreviousWalk = false;
    }
}