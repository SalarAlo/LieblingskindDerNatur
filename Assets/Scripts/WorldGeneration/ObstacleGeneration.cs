using System.Collections.Generic;
using UnityEngine;

public class ObstacleGeneration : MonoBehaviour
{
    [SerializeField] private List<GameObject> obstacles;
    [SerializeField] private List<GameObject> obstaclesTrees;
    [SerializeField, Range(0, 1)] private float tileChanceSelected;
    [SerializeField, Range(0, 1)] private float tileChanceTree;
    [SerializeField] private int steps;
    [SerializeField] private int iterations;

    private List<GameObject> spawnedObstacles = new List<GameObject>();

    public void SpawnObstacles()
    {
        MakeRegularObstacles();
        MakeTrees();
    }

    private void MakeTrees()
    {
        for (int z = 0; z < WorldGeneration.WORLD_SIZE; z++)
        {
            for (int x = 0; x < WorldGeneration.WORLD_SIZE; x++)
            {
                if (UnwalkableAreaMap.blockedArea.Contains(new(x, z))) continue;
                if (tileChanceSelected < Random.Range(0f, 1f)) continue;
                var tree = Instantiate(obstaclesTrees[Random.Range(0, obstaclesTrees.Count)], new Vector3(x, 0, z), Quaternion.identity);
                spawnedObstacles.Add(tree);
                UnwalkableAreaMap.blockedArea.Add(new(x, z));
            }
        }
    }

    private void MakeRegularObstacles()
    {
        for (int z = 0; z < WorldGeneration.WORLD_SIZE; z++)
        {
            for (int x = 0; x < WorldGeneration.WORLD_SIZE; x++)
            {
                if (UnwalkableAreaMap.blockedArea.Contains(new(x, z))) continue;
                if (tileChanceSelected < Random.Range(0f, 1f)) continue;
                MakeWalk(x, z);
            }
        }
    }

    private void MakeWalk(int x, int z)
    {
        for (int i = 0; i < iterations; i++)
        {
            Vector2Int cur = new(x, z);
            for (int j = 0; j < steps; j++)
            {
                Vector2Int randomDir = new(Random.Range(-1, 2), Random.Range(-1, 2));
                cur += randomDir;
                if (UnwalkableAreaMap.blockedArea.Contains(cur)) continue;
                if (cur.x < 0 || cur.y < 0 || cur.y >= WorldGeneration.WORLD_SIZE || cur.x >= WorldGeneration.WORLD_SIZE) continue;
                var obs = Instantiate(obstacles[Random.Range(0, obstacles.Count)], new Vector3(cur.x, 0, cur.y), Quaternion.identity);
                spawnedObstacles.Add(obs);
                UnwalkableAreaMap.blockedArea.Add(cur);
            }
        }
    }

    public void DestroyObstacles()
    {
        foreach (var obj in spawnedObstacles) {
            Destroy(obj);

        }
        spawnedObstacles.Clear();
    }
}
