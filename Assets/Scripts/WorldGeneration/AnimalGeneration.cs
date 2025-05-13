using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimalGeneration : MonoBehaviour
{
    [SerializeField] private List<AnimalSpawn> animalsToSpawn;

    public void SpawnAnimals() {
        foreach(AnimalSpawn animalSpawn in animalsToSpawn) {
            for(int i = 0; i < animalSpawn.SpawnAmount; i++) {
                Vector3Int spawnPos;
                do {
                    spawnPos = new(Random.Range(0, WorldGeneration.WORLD_SIZE), 0, Random.Range(0, WorldGeneration.WORLD_SIZE));
                } while(UnwalkableAreaMap.blockedArea.Contains(new Vector2Int(spawnPos.x, spawnPos.z)));
                Instantiate(animalSpawn.animal.Model, spawnPos, Quaternion.identity);
            }
        }
    }
}

[System.Serializable]
public class AnimalSpawn {
    public int SpawnAmount;
    public AnimalSO animal;
}