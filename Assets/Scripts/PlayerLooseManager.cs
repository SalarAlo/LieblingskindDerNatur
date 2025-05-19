using System;
using System.Collections;
using UnityEngine;

public class PlayerLooseManager : MonoBehaviour
{
    [SerializeField] private GameObject mainGameWindow;
    [SerializeField] private GameObject looseWindow;

    private IEnumerator Start() {
        yield return null;
        yield return null;
        yield return null;
        AnimalInfoManager.Instance.OnAnimalAmountChanged += CheckForLoss;
    }

    private void CheckForLoss() {
        int favChildAmt = AnimalInfoManager.Instance.GetAmountOfAnimal(PlayerAnimalAssignment.Instance.AnimalSO);
        if (favChildAmt != 0) return;
        FreeFlyCamera.Instance.Disable();
        mainGameWindow.SetActive(false);
        looseWindow.SetActive(true);
    }
}
    