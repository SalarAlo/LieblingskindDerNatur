using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AnimalMovementComponent)), BurstCompile]
public class NoneUrgeResponder : UrgeResponder
{
    private AnimalMovementComponent movementComponent;
    private bool reachedDestination;
    private Vector3Int currentDestination;

    void Awake() {
        reachedDestination = true;
        movementComponent = GetComponent<AnimalMovementComponent>();
        movementComponent.OnMoveDone += MovementComponent_OnMoveDone;
    }

    private void MovementComponent_OnMoveDone(){
        reachedDestination = true;
    }

    public override Urge GetUrge(){
        return Urge.None;
    }

    [BurstCompile]
    public override void RespondToUrge(){
        if(!reachedDestination) return;

        currentDestination = GetRandomDestination();
        movementComponent.MoveTo(currentDestination);
        reachedDestination = false;
    }

    [BurstCompile]
    private Vector3Int GetRandomDestination() {
        var positionAsIntVector = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        bool isUnwalkable;
        bool isInBounds;
        do {
            currentDestination = positionAsIntVector + GetRandomDir();
            isUnwalkable = UnwalkableAreaMap.blockedArea.Contains(new(currentDestination.x, currentDestination.z));
            isInBounds = currentDestination.x > 0 && currentDestination.z > 0 && currentDestination.x < WorldGeneration.WORLD_SIZE && currentDestination.z < WorldGeneration.WORLD_SIZE && currentDestination.z < WorldGeneration.WORLD_SIZE;
        } while(isUnwalkable || !isInBounds);
        return currentDestination;
    }

    [BurstCompile]
    private Vector3Int GetRandomDir() {
        Vector3Int x;
        do {
            x = new(Random.Range(-1, 2), 0, Random.Range(-1, 2));
        } while (x == Vector3Int.zero);
        return x;
    }
}
