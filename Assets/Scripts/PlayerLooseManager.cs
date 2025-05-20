using System;
using System.Collections;
using UnityEngine;

public class PlayerLooseManager : MonoBehaviour
{
    [SerializeField] private AnimalSO mouse;
    [SerializeField] private AnimalSO snake;
    [SerializeField] private AnimalSO elephant;

    [SerializeField] private GameObject mainGameWindow;
    [SerializeField] private GameObject looseWindow;
    [SerializeField] private GameObject winWindow;
    private bool lost;

    private IEnumerator Start() {
        yield return null;
        yield return null;
        yield return null;
        AnimalInfoManager.Instance.OnAnimalAmountChanged += CheckForLoss;
        AnimalInfoManager.Instance.OnAnimalAmountChanged += CheckForWin;
    }

    private void CheckForWin() {
        AnimalSO favChild = PlayerAnimalAssignment.Instance.AnimalSO;
        AnimalSO other1 = null, other2 = null;

        if (favChild == mouse) {
            other1 = snake;
            other2 = elephant;
        } else if (favChild == snake) {
            other1 = mouse;
            other2 = elephant;
        } else if (favChild == elephant) {
            other1 = mouse;
            other2 = snake;
        }

        if (AnimalInfoManager.Instance.GetAmountOfAnimal(other1) == 0 && AnimalInfoManager.Instance.GetAmountOfAnimal(other2) == 0) {
            mainGameWindow.SetActive(false);
            winWindow.SetActive(true);
            FreeFlyCamera.Instance.Disable();
        }
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
    