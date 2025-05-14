using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;

public class FoodGeneration : MonoBehaviour
{
    public static FoodGeneration Instance;
    private HashSet<FoodInstance> food;
    [SerializeField] private List<Food> foodObjects;
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
            var randomFood = foodObjects[UnityEngine.Random.Range(0, foodObjects.Count)];
            var gameObj = Instantiate(randomFood.GetPrefab(), spawnPos, Quaternion.identity);
            var generatedFood = new FoodInstance() {
                FoodSO = randomFood,
                Position = new((int)spawnPos.x, (int)spawnPos.z),
                Instance = gameObj
            };
            food.Add(generatedFood);
            UnwalkableAreaMap.blockedArea.Add(generatedFood.Position);
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
            Destroy(toRemove.Value.Instance);
            food.Remove(toRemove.Value);
            UnwalkableAreaMap.blockedArea.Remove(toRemove.Value.Position);
        }
    }
}

public struct FoodInstance {
    public Vector2Int Position;
    public Food FoodSO;
    public GameObject Instance;
}
