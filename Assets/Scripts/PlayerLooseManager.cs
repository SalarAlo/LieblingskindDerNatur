using System;
using System.Collections;
using UnityEngine;

public class PlayerLooseManager : MonoBehaviour
{
    [SerializeField] private GameObject mainGameWindow;
    [SerializeField] private GameObject looseWindow;
    private bool lost;

    private IEnumerator Start() {
        yield return null;
        yield return null;
        yield return null;
        AnimalInfoManager.Instance.OnAnimalAmountChanged += CheckForLoss;
    }

    private void CheckForLoss() {
        if (lost) return;
        int favChildAmt = AnimalInfoManager.Instance.GetAmountOfAnimal(PlayerAnimalAssignment.Instance.AnimalSO);
        if (favChildAmt != 0) return;
        FreeFlyCamera.Instance.Disable();
        mainGameWindow.SetActive(false);
        looseWindow.SetActive(true);
        lost = true;
    }
}
    