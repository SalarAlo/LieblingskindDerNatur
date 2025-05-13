using System;
using UnityEngine;

public class ThirstUrgeHandler : UrgeResponder
{
    private bool foundWater;
    private bool hasWaitedForPreviousWalk;
    private AnimalMovementComponent movementComponent;
    private SurrounderSensor surrounderSensor;
    private Vector2Int destination;

    public override Urge GetUrge() {
        return Urge.Thirst;
    }

    void Awake() {
        movementComponent = GetComponent<AnimalMovementComponent>();   
        surrounderSensor = GetComponent<SurrounderSensor>();
    }

    private void SetWaitDone() {
        hasWaitedForPreviousWalk = true;
    }

    public override void RespondToUrge() {
        if(!hasWaitedForPreviousWalk && movementComponent.IsDoingMove()) {
            movementComponent.OnMoveDone += MovementComponent_OnMoveDone;
            return;
        }

        if(!foundWater) {
            if(!movementComponent.IsDoingMove()) {
                if(CheckForWater(out Vector2Int destination)) {
                    foundWater = true;
                } else {
                    movementComponent.MoveTo(transform.position+GetRandomDir());
                }
            }
        } else {
            
        }
    }

    private void MovementComponent_OnMoveDone() {
        hasWaitedForPreviousWalk = true;
    }

    private Vector3Int GetRandomDir() {
        Vector3Int x;
        do {
            x = new(UnityEngine.Random.Range(-1, 2), 0, UnityEngine.Random.Range(-1, 2));
        } while (x == Vector3Int.zero);
        return x;
    }

    private bool CheckForWater(out Vector2Int waterPos) {
        return surrounderSensor.TrySenseNearestWater(out waterPos);
    }

    public override void OnUrgeChanged() {
        foundWater = false;
        hasWaitedForPreviousWalk = false;
    }
}
