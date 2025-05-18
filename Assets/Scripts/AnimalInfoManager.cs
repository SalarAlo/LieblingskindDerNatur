using System;
using UnityEngine;

public class AnimalInfoManager : MonoBehaviour
{
    [SerializeField] private AnimalSO mouseSO;
    [SerializeField] private AnimalSO elephantSO;
    [SerializeField] private AnimalSO snakeSO;
    [SerializeField] private AnimalInfoUI mouseInfo;
    [SerializeField] private AnimalInfoUI elephantInfo;
    [SerializeField] private AnimalInfoUI snakeInfo;
    private int mouseAmount;
    private int snakeAmount;
    private int elephantAmount;

    void Awake()
    {
        WorldPlacable.OnAnyWorldPlacableRemoved += WorldPlacable_OnAnyWorldPlacableRemoved;
        WorldPlacable.OnAnyWorldPlacableSpawned += WorldPlacable_OnAnyWorldPlacableSpawned;
    }

    private void WorldPlacable_OnAnyWorldPlacableSpawned(WorldPlacable placable) {
        if (placable is not AnimalBehaviour animalBehaviour) return;
        string animalName = animalBehaviour.GetAnimalSO().Name;
        if (animalName == snakeSO.Name) {
            snakeAmount++;
            snakeInfo.SetTextAmount(snakeAmount);
        }
        else if (animalName == mouseSO.Name) {
            mouseAmount++;
            mouseInfo.SetTextAmount(mouseAmount);
        }
        else {
            elephantAmount++;
            elephantInfo.SetTextAmount(elephantAmount);
        }
    }

    private void WorldPlacable_OnAnyWorldPlacableRemoved(WorldPlacable placable)
    {
        if (placable is not AnimalBehaviour animalBehaviour) return;
        string animalName = animalBehaviour.GetAnimalSO().Name;
        if (animalName == snakeSO.Name) {
            snakeAmount--;
            snakeInfo.SetTextAmount(snakeAmount);
        }
        else if (animalName == mouseSO.Name) {
            mouseAmount--;
            mouseInfo.SetTextAmount(mouseAmount);
        }
        else {
            elephantAmount--;
            elephantInfo.SetTextAmount(elephantAmount);
        }
    }
}
