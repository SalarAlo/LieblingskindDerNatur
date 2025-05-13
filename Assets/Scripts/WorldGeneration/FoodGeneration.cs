using System;
using System.Collections.Generic;
using UnityEngine;

public class FoodGeneration : MonoBehaviour
{
    public static FoodGeneration Instance;
    private HashSet<FoodInstance> food;
    [SerializeField] private List<FoodSO> foodObjects;
    [SerializeField] private int spawnAmountPerDay;

    void Awake() {
        food = new();
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void SpawnFood() {
        for(int i = 0; i < spawnAmountPerDay; i++) {
            Vector3 spawnPos = GetWalkableCoord();
            var randomSO = foodObjects[UnityEngine.Random.Range(0, foodObjects.Count)];
            Instantiate(randomSO.Model, spawnPos, Quaternion.identity);
            food.Add(new() {
                foodSO = randomSO,
                position = new((int)spawnPos.x, (int)spawnPos.y)
            });
        }
    }

    private Vector3 GetWalkableCoord() {
        Vector2Int valid = Pathfinding.GetRandomWaklkablePosition();
        return new(valid.x, 0, valid.y);
    }
}

public struct FoodInstance : IEquatable<FoodInstance> {
    public Vector2Int position;
    public FoodSO foodSO;

    public bool Equals(FoodInstance other) {
        return position.Equals(other.position) && Equals(foodSO, other.foodSO);
    }

    public override bool Equals(object obj) {
        return obj is FoodInstance other && Equals(other);
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = hash * 31 + position.GetHashCode();
            hash = hash * 31 + (foodSO != null ? foodSO.GetHashCode() : 0);
            return hash;
        }
    }

    public static bool operator ==(FoodInstance left, FoodInstance right) {
        return left.Equals(right);
    }

    public static bool operator !=(FoodInstance left, FoodInstance right) {
        return !(left == right);
    }
}
