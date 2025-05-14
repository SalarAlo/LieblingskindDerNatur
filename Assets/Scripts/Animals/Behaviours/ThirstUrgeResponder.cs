using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ThirstUrgeResponder : UrgeResponder
{
    private bool foundWater;
    private bool hasWaitedForPreviousWalkBeforeStarting;
    private bool hasStartedWaitingForPreviousWalk;
    private AnimalMovementComponent movementComponent;
    private SurrounderSensor surrounderSensor;
    [SerializeField] private List<Vector2Int> path;

    public override Urge GetUrge() {
        return Urge.Thirst;
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
            Debug.Log("Still Waiting before starting search");
            return;
        }  

        if(!foundWater) {
            if(movementComponent.IsDoingMove()) return;

            if(CheckForWater(out List<Vector2Int> possibleDestinations)) {
                var posF = transform.position;
                Vector2Int position = new(Mathf.RoundToInt(posF.x), Mathf.RoundToInt(posF.z));
                foreach(var destination in possibleDestinations) {
                    path = Pathfinding.FindPath(position, destination);
                    Debug.Log(path);
                    if(path != null)
                        break;
                }
                foundWater = true;
            } else {
                GoToRandomDestination();
            }
        } else {
            if(movementComponent.IsDoingMove()) return;

            if(path.Count == 0)  {
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

    private bool CheckForWater(out List<Vector2Int> waterPos) {
        return surrounderSensor.TrySenseNearestGrassNearWater(out waterPos);
    }

    protected override void FinishUrge() {
        base.FinishUrge();
        foundWater = false;
        hasWaitedForPreviousWalkBeforeStarting = false;
        hasStartedWaitingForPreviousWalk = false;
    }
}
