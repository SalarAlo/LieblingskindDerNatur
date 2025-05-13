using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public const float WATER_LEVEL = 0.34f;
    public const int WORLD_SIZE = 60;
    public static WorldGeneration Instance;
    public HashSet<Vector2Int> WaterTiles {get; private set;}
    [SerializeField] private float scale;
    [SerializeField] private MeshFilter worldRenderer;
    private ObstacleGeneration obstacleGeneration;
    private AnimalGeneration animalGeneration;
    private FoodGeneration foodGeneration;
    private List<Tile> tiles;

    private void Awake() {
        tiles = new();
        WaterTiles = new();
        MakeSingleton();

        obstacleGeneration = GetComponent<ObstacleGeneration>();
        animalGeneration = GetComponent<AnimalGeneration>();
        foodGeneration = GetComponent<FoodGeneration>();
    }

    private void MakeSingleton(){
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        GenerateWorld();
        GenerateMesh();
    }

    private void GenerateWorld() {
        for(int z = 0; z < WORLD_SIZE; z++) {
            for(int x = 0; x < WORLD_SIZE; x++) {
                float height = Mathf.PerlinNoise(x * scale, z * scale);
                Tile tile = new() {
                    Height = height,
                    Position = new Vector2Int(x, z)
                };
                tiles.Add(tile);

                if(height < WATER_LEVEL) {
                    UnwalkableAreaMap.blockedArea.Add(tile.Position);
                    WaterTiles.Add(tile.Position);
                }
            }
        }

        obstacleGeneration.SpawnObstacles();
        animalGeneration.SpawnAnimals();
        foodGeneration.SpawnFood();
    }

    private void GenerateMesh(){
        ChunkMeshBuilder chunkMeshBuilderGrass = new();
        ChunkMeshBuilder chunkMeshBuilderWater = new();
        foreach (Tile t in tiles) {
            var chunkMeshBuilder = t.Height < WATER_LEVEL ? chunkMeshBuilderWater : chunkMeshBuilderGrass;
            float3 offset = new(t.Position.x, 0, t.Position.y);
            chunkMeshBuilder.AddFace(Direction.Up, offset, t.Height);
            if(t.Position.x == 0) 
                chunkMeshBuilder.AddFace(Direction.Left, offset, t.Height);
            if(t.Position.y == 0)
                chunkMeshBuilder.AddFace(Direction.Back, offset, t.Height);

            if(t.Position.x == WORLD_SIZE-1) 
                chunkMeshBuilder.AddFace(Direction.Right, offset, t.Height);
            if(t.Position.y == WORLD_SIZE-1)
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