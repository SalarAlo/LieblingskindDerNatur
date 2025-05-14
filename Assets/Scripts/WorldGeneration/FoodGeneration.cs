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
            var gameObj = Instantiate(randomSO.Model, spawnPos, Quaternion.identity);
            food.Add(new() {
                FoodSO = randomSO,
                Position = new((int)spawnPos.x, (int)spawnPos.z),
                Instance = gameObj
            });
        }
    }

    private Vector3 GetWalkableCoord() {
        Vector2Int valid = Pathfinding.GetRandomWaklkablePosition();
        return new(valid.x, 0, valid.y);
    }

    public HashSet<FoodInstance> GetFoodTiles() {
        return food;
    }

    public void RemoveFood(Vector2Int position){
        FoodInstance? toRemove = null;
        foreach (var f in food) {
            if (f.Position == position) {
                toRemove = f;
                break;
            }
        }
        if (toRemove.HasValue) {
            Debug.Log("Food not found");
            Destroy(toRemove.Value.Instance);
            food.Remove(toRemove.Value);
        }
    }
}

public struct FoodInstance : IEquatable<FoodInstance> {
    public Vector2Int Position;
    public FoodSO FoodSO;
    public GameObject Instance;

    public bool Equals(FoodInstance other) {
        return Position.Equals(other.Position) && Equals(FoodSO, other.FoodSO);
    }

    public override bool Equals(object obj) {
        return obj is FoodInstance other && Equals(other);
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = hash * 31 + Position.GetHashCode();
            hash = hash * 31 + (FoodSO != null ? FoodSO.GetHashCode() : 0);
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
