using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WorldPlacable : MonoBehaviour
{
    public Vector2Int Position {get; private set;}
    private static readonly List<WorldPlacable> worldPlacables = new();

    protected virtual void Awake() {
        worldPlacables.Add(this);
    }

    protected virtual void Update() {
        Position = transform.position.ToVector2Int();
    }

    public static WorldPlacable GetWorldPlacableAt(Vector2Int position) {
        return worldPlacables.FirstOrDefault(i => position == i.Position);
    }

    void OnDestroy() {
        worldPlacables.Remove(this);
    }

    public static List<AnimalBehaviour> GetAnimals(AnimalSO so) {
        List<AnimalBehaviour> animals = new();
        foreach(WorldPlacable worldPlacable in worldPlacables) {
            if(worldPlacable is AnimalBehaviour animalBehaviour && animalBehaviour.GetAnimalSO() == so) 
                animals.Add(animalBehaviour);
        }
        return animals;
    }
}
