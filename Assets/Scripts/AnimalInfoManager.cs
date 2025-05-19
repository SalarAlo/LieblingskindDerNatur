using System;
using UnityEngine;

public class AnimalInfoManager : MonoBehaviour
{
    public static AnimalInfoManager Instance;
    public Action OnAnimalAmountChanged;
    [SerializeField] private AnimalSO mouseSO;
    [SerializeField] private AnimalSO elephantSO;
    [SerializeField] private AnimalSO snakeSO;
    [SerializeField] private AnimalInfoUI mouseInfo;
    [SerializeField] private AnimalInfoUI elephantInfo;
    [SerializeField] private AnimalInfoUI snakeInfo;
    private int mouseAmount;
    private int snakeAmount;
    private int elephantAmount;
    private bool evolutionIncActive;
    public int EvolutionPoints { get; private set;}

    void Awake() {
        EvolutionPoints = 10;
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        WorldPlacable.OnAnyWorldPlacableRemoved += WorldPlacable_OnAnyWorldPlacableRemoved;
        WorldPlacable.OnAnyWorldPlacableSpawned += WorldPlacable_OnAnyWorldPlacableSpawned;

        PlayerAnimalAssignment.Instance.OnAnimalAssigned += () => evolutionIncActive = true;
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
        if (evolutionIncActive && PlayerAnimalAssignment.Instance.AnimalSO.Name == animalName) {
            EvolutionPoints++;
        }
        OnAnimalAmountChanged?.Invoke();
    }

    private void WorldPlacable_OnAnyWorldPlacableRemoved(WorldPlacable placable) {
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
        OnAnimalAmountChanged?.Invoke();
    }
    public int GetAmountOfAnimal(AnimalSO animalSO) {
        string animalName = animalSO.Name;
        if (animalName == snakeSO.Name) {
            return snakeAmount;
        }
        else if (animalName == mouseSO.Name) {
            return mouseAmount;
        }
        else {
            return elephantAmount;
        }
    }
}
