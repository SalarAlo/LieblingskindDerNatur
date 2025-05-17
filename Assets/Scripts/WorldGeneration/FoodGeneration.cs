using System.Collections.Generic;
using UnityEngine;

public class FoodGeneration : MonoBehaviour
{
    public static FoodGeneration Instance;

    private HashSet<FoodInstance> food;

    [SerializeField] private List<Food> foodObjects;
    [SerializeField] private int initialSpawnAmount = 10;
    [SerializeField] private int minimumSpawnAmount = 2;
    [SerializeField] private int dayInterval = 10;
    [SerializeField] private float spawnReductionPerDay = 1f;

    private int daysPassed = 0;
    private float dayTime;

    void Awake()
    {
        food = new();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        dayTime += Time.deltaTime;
        if (dayTime > dayInterval)
        {
            SpawnFood();
            dayTime = 0;
        }
    }

    public void SpawnFood()
    {
        int spawnAmount = Mathf.Max(minimumSpawnAmount, Mathf.RoundToInt(initialSpawnAmount - daysPassed * spawnReductionPerDay));
        daysPassed++;

        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 spawnPos = GetWalkableCoord();
            var randomFood = foodObjects[Random.Range(0, foodObjects.Count)];
            var gameObj = Instantiate(randomFood.GetPrefab(), spawnPos, Quaternion.identity);
            var generatedFood = new FoodInstance()
            {
                FoodSO = randomFood,
                Position = new((int)spawnPos.x, (int)spawnPos.z),
                Instance = gameObj
            };
            food.Add(generatedFood);
            UnwalkableAreaMap.blockedArea.Add(generatedFood.Position);
        }
    }

    private Vector3 GetWalkableCoord()
    {
        Vector2Int valid = Pathfinding.GetRandomWaklkablePosition();
        return new Vector3(valid.x, 0, valid.y);
    }

    public HashSet<FoodInstance> GetFoodTiles()
    {
        return food;
    }

    public void RemoveFood(Vector2Int position)
    {
        FoodInstance? toRemove = null;
        foreach (var f in food)
        {
            if (f.Position == position)
            {
                toRemove = f;
                break;
            }
        }

        if (toRemove.HasValue)
        {
            Destroy(toRemove.Value.Instance);
            food.Remove(toRemove.Value);
            UnwalkableAreaMap.blockedArea.Remove(toRemove.Value.Position);
        }
    }
}

public struct FoodInstance
{
    public Vector2Int Position;
    public Food FoodSO;
    public GameObject Instance;
}
