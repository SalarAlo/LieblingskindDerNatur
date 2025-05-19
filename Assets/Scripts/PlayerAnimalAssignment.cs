using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimalAssignment : MonoBehaviour
{
    public static PlayerAnimalAssignment Instance;
    [SerializeField] private List<AnimalSO> possibleAnimals;
    public AnimalSO AnimalSO { get; private set; }
    public Action OnAnimalAssigned;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private IEnumerator Start() {
        AnimalSO = possibleAnimals[UnityEngine.Random.Range(0, possibleAnimals.Count)];
        yield return null;
        OnAnimalAssigned?.Invoke();
    }
}
