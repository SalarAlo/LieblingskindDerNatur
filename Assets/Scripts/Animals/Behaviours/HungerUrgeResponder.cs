using System.Collections.Generic;
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
        if(!hasWaitedForPreviousWalkBeforeStarting && movementComponent.IsDoingMove()) {
            if(!hasStartedWaitingForPreviousWalk) {
                movementComponent.OnMoveDone += MovementComponent_OnMoveDone;
                hasStartedWaitingForPreviousWalk = true;
            }
            return;
        }  

        if(!foundFood) {
            if(movementComponent.IsDoingMove()) return;

            if(CheckForFood(out List<Vector2Int> possibleDestinations, out List<Vector2Int> foodPos)) {
                var posF = transform.position;
                Vector2Int position = new(Mathf.RoundToInt(posF.x), Mathf.RoundToInt(posF.z));
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
            } else {
                GoToRandomDestination();
            }     
        } else {
            if(movementComponent.IsDoingMove()) return;

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

    protected override void FinishUrge() {
        base.FinishUrge();
        foundFood = false;
        hasWaitedForPreviousWalkBeforeStarting = false;
        hasStartedWaitingForPreviousWalk = false;
    }
}