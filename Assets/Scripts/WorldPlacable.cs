using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldPlacable : MonoBehaviour
{
    public Vector2Int Position {get; private set;}
    private static readonly List<WorldPlacable> worldPlacables = new();


    private void Awake() {
        worldPlacables.Add(this);
    }

    protected virtual void Update() {
        Position = transform.position.GetVector2Int();
    }

    public static WorldPlacable GetWorldPlacableAt(Vector2Int position) {
        return worldPlacables.FirstOrDefault(i => position == i.Position);
    }

    void OnDestroy() {
        worldPlacables.Remove(this);
    }
}
