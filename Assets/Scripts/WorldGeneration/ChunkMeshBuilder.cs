using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ChunkMeshBuilder
{
    public List<DirectionalQuadData> faces;
    public static Dictionary<Direction, DirectionalQuadData> FaceDataByDirection;

    public class DirectionalQuadData {
        public float3 Normal;
        public float3 Offset;
        public float Height;

        public DirectionalQuadData(DirectionalQuadData directionalQuadData) {
            Normal = directionalQuadData.Normal;
            Offset = directionalQuadData.Offset;
        }
        public DirectionalQuadData() {
        }
    }

    static ChunkMeshBuilder() {
        FaceDataByDirection = new() {
            { 
                Direction.Forward, 
                new() {
                    Normal = math.forward()
                }
            },
            {
                Direction.Back, 
                new() {
                    Normal = math.back()
                }
            },
            {
                Direction.Right, 
                new() {
                    Normal = math.right()
                }
            },
            {
                Direction.Left, 
                new() {
                    Normal = math.left()
                }
            },
            {
                Direction.Up, 
                new() {
                    Normal = math.up()
                }
            },
            {
                Direction.Down, 
                new() {
                    Normal = math.down()
                }
            }
        };
    }

    public ChunkMeshBuilder() {
        faces = new();
    }

    public void AddFace(Direction dir, float3 offset, float height) {
        var baseData = FaceDataByDirection[dir];
        var quadData = new DirectionalQuadData() {
            Offset = offset,
            Normal = baseData.Normal,
            Height = height
        };
        faces.Add(quadData);
    }

    public Mesh GetMesh() {
        List<Vector3> vertices = new();
        List<Vector2> uvs = new();
        List<Vector3> normals = new();
        List<int> triangles = new();
        int vertexIndex = 0;

        foreach (var face in faces) {
            float3[] baseVertices = GetQuadVertices(face.Normal);
            Vector2[] faceUVs;

            faceUVs = GetQuadUVs(face.Height);

            for (int i = 0; i < 4; i++) {
                vertices.Add(new Vector3(baseVertices[i].x, baseVertices[i].y, baseVertices[i].z) + 
                             new Vector3(face.Offset.x, face.Offset.y, face.Offset.z));
                uvs.Add(faceUVs[i]);
                normals.Add(new Vector3(face.Normal.x, face.Normal.y, face.Normal.z));
            }

            triangles.Add(vertexIndex + 0);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 0);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);

            vertexIndex += 4;
        }

        Mesh mesh = new();
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateBounds();

        return mesh;
    }

    private static float3[] GetQuadVertices(float3 normal)
    {
        if (normal.Equals(math.forward()))
        {
            return new float3[]
            {
                new float3(-0.5f, -0.5f, 0.5f),
                new float3(0.5f, -0.5f, 0.5f),
                new float3(0.5f, 0.5f, 0.5f),
                new float3(-0.5f, 0.5f, 0.5f)
            };
        }
        else if (normal.Equals(math.back()))
        {
            return new float3[]
            {
                new(0.5f, -0.5f, -0.5f),
                new(-0.5f, -0.5f, -0.5f),
                new(-0.5f, 0.5f, -0.5f),
                new(0.5f, 0.5f, -0.5f)
            };
        }
        else if (normal.Equals(math.right()))
        {
            return new float3[]
            {
                new float3(0.5f, -0.5f, 0.5f),
                new float3(0.5f, -0.5f, -0.5f),
                new float3(0.5f, 0.5f, -0.5f),
                new float3(0.5f, 0.5f, 0.5f)
            };
        }
        else if (normal.Equals(math.left()))
        {
            return new float3[]
            {
                new float3(-0.5f, -0.5f, -0.5f),
                new float3(-0.5f, -0.5f, 0.5f),
                new float3(-0.5f, 0.5f, 0.5f),
                new float3(-0.5f, 0.5f, -0.5f)
            };
        }
        else if (normal.Equals(math.up()))
        {
            return new float3[]
            {
                new float3(-0.5f, 0.5f, 0.5f),
                new float3(0.5f, 0.5f, 0.5f),
                new float3(0.5f, 0.5f, -0.5f),
                new float3(-0.5f, 0.5f, -0.5f)
            };
        }
        else if (normal.Equals(math.down()))
        {
            return new float3[]
            {
                new(-0.5f, -0.5f, -0.5f),
                new(0.5f, -0.5f, -0.5f),
                new(0.5f, -0.5f, 0.5f),
                new(-0.5f, -0.5f, 0.5f)
            };
        }
        else
        {
            return new float3[0];
        }
    }

    private static Vector2[] GetQuadUVs(float height, int steps = 50) {
        float normalized;
        if (height < WorldGeneration.WATER_LEVEL)
        {
            normalized = 1 - height / WorldGeneration.WATER_LEVEL;
        }
        else
        {
            float inv = 1f - WorldGeneration.WATER_LEVEL;
            normalized = (height - WorldGeneration.WATER_LEVEL) / inv;
        }

        normalized = Mathf.Clamp01(normalized);
        int column = Mathf.Clamp(Mathf.RoundToInt(normalized * (steps - 1)), 0, steps - 1);
        float uStep = 1f / steps;
        float uStart = column * uStep;
        float uEnd = uStart + uStep;

        return new Vector2[]
        {
            new(uStart, 0f), // Bottom Left
            new(uEnd,   0f), // Bottom Right
            new(uEnd,   1f), // Top Right
            new(uStart, 1f)  // Top Left
        };
    }

}

