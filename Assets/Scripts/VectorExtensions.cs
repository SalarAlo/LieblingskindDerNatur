using System;
using UnityEngine;

public static class VectorExtensions 
{
    public static Vector2Int ToVector2Int(this Vector3 vector) {
        return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.z));
    }
    public static Vector3 ToVector3(this Vector2Int vector) {
        return new(vector.x, 0, vector.y);
    }
}
