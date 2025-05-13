using System.Collections.Generic;
using UnityEngine;

public static class UnwalkableAreaMap 
{
    public static HashSet<Vector2Int> blockedArea;

    static UnwalkableAreaMap() {
        blockedArea = new();
    }
}
