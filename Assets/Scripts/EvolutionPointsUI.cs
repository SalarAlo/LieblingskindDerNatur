using System;
using TMPro;
using UnityEngine;

public class EvolutionPointsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI evolutionPointsUI;

    void Start() {
        AnimalInfoManager.Instance.OnAnimalAmountChanged += AnimalInfoManager_OnAnimalAmountChanged;
    }

    private void AnimalInfoManager_OnAnimalAmountChanged(){
        evolutionPointsUI.text = AnimalInfoManager.Instance.EvolutionPoints.ToString();
    }
}
