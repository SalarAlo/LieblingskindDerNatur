using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding
{
    public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        var openSet = new PriorityQueue<Vector2Int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        var gScore = new Dictionary<Vector2Int, float>();
        var fScore = new Dictionary<Vector2Int, float>();

        openSet.Enqueue(start, 0);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current == goal) {
                var path = ReconstructPath(cameFrom, current);
                path.RemoveAt(0);
                return path;
            }

            foreach (var neighbor in GetNeighbors(current))
            {
                if (!IsInBounds(neighbor)) continue;
                if (UnwalkableAreaMap.blockedArea.Contains(neighbor)) continue;

                // ✔ Diagonale Bewegung erkennen (teurer)
                float moveCost = (current.x != neighbor.x && current.y != neighbor.y) ? 1.4142f : 1f;
                float tentativeG = gScore[current] + moveCost;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }

        return null; // Kein Pfad gefunden
    }

    // ✔ Heuristik für diagonale Bewegung (Octile Distance)
    private static float Heuristic(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return (dx + dy) + (1.4142f - 2) * Mathf.Min(dx, dy);
    }

    // ✔ 8-Richtungsnachbarn (inkl. Diagonale)
    private static List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        return new()
        {
            new(pos.x + 1, pos.y),
            new(pos.x - 1, pos.y),
            new(pos.x, pos.y + 1),
            new(pos.x, pos.y - 1),
            new(pos.x + 1, pos.y + 1),
            new(pos.x - 1, pos.y + 1),
            new(pos.x + 1, pos.y - 1),
            new(pos.x - 1, pos.y - 1)
        };
    }

    private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        var path = new List<Vector2Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }

    public static bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < WorldGeneration.WORLD_SIZE &&
               pos.y >= 0 && pos.y < WorldGeneration.WORLD_SIZE;
    }

    public static Vector2Int GetRandomWaklkablePosition()
    {
        Vector2Int valid;
        do
        {
            valid = new(
                UnityEngine.Random.Range(0, WorldGeneration.WORLD_SIZE),
                UnityEngine.Random.Range(0, WorldGeneration.WORLD_SIZE)
            );
        } while (UnwalkableAreaMap.blockedArea.Contains(valid));
        return valid;
    }
}

public class PriorityQueue<T>
{
    private readonly List<(T item, float priority)> elements = new();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;
        float bestPriority = elements[0].priority;

        for (int i = 1; i < elements.Count; i++)
        {
            if (elements[i].priority < bestPriority)
            {
                bestPriority = elements[i].priority;
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].item;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }

    public bool Contains(T item)
    {
        return elements.Any(e => EqualityComparer<T>.Default.Equals(e.item, item));
    }
}
