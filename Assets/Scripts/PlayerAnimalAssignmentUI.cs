using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimalAssignmentUI : MonoBehaviour
{
    [SerializeField] private GameObject playerAssignmentWindow;
    [SerializeField] private GameObject mainGameWindow;
    [SerializeField] private Image favouriteChild;
    [SerializeField] private Image favouriteChildMainWindow;


    void Start()
    {
        PlayerAnimalAssignment.Instance.OnAnimalAssigned += PlayerAnimalAssignment_OnAnimalAssigned;
    }

    private void PlayerAnimalAssignment_OnAnimalAssigned() {
        favouriteChild.sprite = favouriteChildMainWindow.sprite = PlayerAnimalAssignment.Instance.AnimalSO.Sprite;
        playerAssignmentWindow.SetActive(true);
        mainGameWindow.SetActive(false);
        FreeFlyCamera.Instance.Disable();
        StartCoroutine(ShowMainWindow_Routine());
    }

    private IEnumerator ShowMainWindow_Routine(){
        yield return new WaitForSeconds(5);
        FreeFlyCamera.Instance.Enable();
        playerAssignmentWindow.SetActive(false);
        mainGameWindow.SetActive(true);
    }
}
