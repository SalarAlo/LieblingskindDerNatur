using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public const float WATER_LEVEL = 0.3f;
    public const int WORLD_SIZE = 60;
    public static WorldGeneration Instance;
    public HashSet<Vector2Int> WaterTiles { get; private set; }
    [SerializeField] private float scale;
    [SerializeField] private bool generateIsland;
    [SerializeField] private int seed;
    [SerializeField] private MeshFilter worldRenderer;
    private ObstacleGeneration obstacleGeneration;
    private AnimalGeneration animalGeneration;
    private FoodGeneration foodGeneration;
    private List<Tile> tiles;

    private void Awake()
    {
        tiles = new();
        WaterTiles = new();
        MakeSingleton();
        seed = UnityEngine.Random.Range(0, 1_000_000);

        obstacleGeneration = GetComponent<ObstacleGeneration>();
        animalGeneration = GetComponent<AnimalGeneration>();
        foodGeneration = GetComponent<FoodGeneration>();
    }

    private void MakeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Generate();
    }

    private void Generate() {
        GenerateWorld();
        GenerateMesh();
    }

    private void GenerateWorld()
    {
        for (int z = 0; z < WORLD_SIZE; z++)
        {
            for (int x = 0; x < WORLD_SIZE; x++)
            {
                float noise = Mathf.PerlinNoise(
                    (x + seed) * scale,
                    (z + seed) * scale
                );

                float height = noise;
                if (generateIsland)
                {
                    float nx = x / (float)WORLD_SIZE - 0.5f;
                    float nz = z / (float)WORLD_SIZE - 0.5f;

                    float distance = Mathf.Sqrt(nx * nx + nz * nz) * 2f;
                    distance = Mathf.Clamp01(distance); // Ensure distance stays in [0, 1]

                    height = Mathf.Lerp(noise, 0.0f, Mathf.Pow(distance, 3f)); // pondSharpness ∈ [1.5, 4]
                }


                Tile tile = new()
                {
                    Height = height,
                    Position = new Vector2Int(x, z)
                };
                tiles.Add(tile);

                if (height < WATER_LEVEL)
                {
                    UnwalkableAreaMap.blockedArea.Add(tile.Position);
                    WaterTiles.Add(tile.Position);
                }
            }
        }


        obstacleGeneration.SpawnObstacles();
        animalGeneration.SpawnAnimals();
        foodGeneration.SpawnFood();
    }

    private void GenerateMesh()
    {
        ChunkMeshBuilder chunkMeshBuilderGrass = new();
        ChunkMeshBuilder chunkMeshBuilderWater = new();
        foreach (Tile t in tiles)
        {
            var chunkMeshBuilder = t.Height < WATER_LEVEL ? chunkMeshBuilderWater : chunkMeshBuilderGrass;
            float3 offset = new(t.Position.x, 0, t.Position.y);
            chunkMeshBuilder.AddFace(Direction.Up, offset, t.Height);
            if (t.Position.x == 0)
                chunkMeshBuilder.AddFace(Direction.Left, offset, t.Height);
            if (t.Position.y == 0)
                chunkMeshBuilder.AddFace(Direction.Back, offset, t.Height);

            if (t.Position.x == WORLD_SIZE - 1)
                chunkMeshBuilder.AddFace(Direction.Right, offset, t.Height);
            if (t.Position.y == WORLD_SIZE - 1)
                chunkMeshBuilder.AddFace(Direction.Forward, offset, t.Height);
        }
        var grassMesh = chunkMeshBuilderGrass.GetMesh();
        var waterMesh = chunkMeshBuilderWater.GetMesh();
        SetChunkMesh(new() { TerrainMesh = grassMesh, WaterMesh = waterMesh });
    }


    private void SetChunkMesh(ChunkMesh chunkMesh) {
        var mesh = new Mesh();

        // Combine vertices
        var terrainVertices = chunkMesh.TerrainMesh.vertices;
        var waterVertices = chunkMesh.WaterMesh.vertices;
        var combinedVertices = terrainVertices.Concat(waterVertices).ToArray();

        // Combine UVs
        var terrainUVs = chunkMesh.TerrainMesh.uv;
        var waterUVs = chunkMesh.WaterMesh.uv;
        var combinedUVs = terrainUVs.Concat(waterUVs).ToArray();

        // Triangles
        var terrainTriangles = chunkMesh.TerrainMesh.GetTriangles(0);
        var waterTriangles = chunkMesh.WaterMesh.GetTriangles(0);

        int terrainVertexCount = terrainVertices.Length;
        var adjustedWaterTriangles = waterTriangles.Select(i => i + terrainVertexCount).ToArray();

        mesh.vertices = combinedVertices;
        mesh.uv = combinedUVs;
        mesh.subMeshCount = 2;
        mesh.SetTriangles(terrainTriangles, 0);
        mesh.SetTriangles(adjustedWaterTriangles, 1);

        mesh.RecalculateNormals();
        worldRenderer.mesh = mesh;
    }
}

public class Tile {
    public float Height;
    public Vector2Int Position;
}

public class ChunkMesh {
    public Mesh TerrainMesh;
    public Mesh WaterMesh;
}