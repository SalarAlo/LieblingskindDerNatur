using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour
{
    [SerializeField] private AnimalSO animal;
    [SerializeField] private Image displayImg;
    [SerializeField] private bool isFavChild;
    [SerializeField] private Button speedButton;
    [SerializeField] private Button senseButton;
    [SerializeField] private TextMeshProUGUI costSense;
    [SerializeField] private TextMeshProUGUI costSpeed;
    [SerializeField] private Transform parentOfUpgradeDisplaySpeed;
    [SerializeField] private Transform parentOfUpgradeDisplaySenseRange;
    [SerializeField] private GameObject upgradeDisplaySenses;
    private const int maxUpgrade = 5;
    private static readonly int[] costs = new int[] { 5, 10, 15, 25, 30 };
    private int levelSpeed = 0;
    private int levelSense = 0;

    private void Awake() {
        speedButton.onClick.AddListener(SpeedButton_OnClick);
        senseButton.onClick.AddListener(SenseButton_OnClick);
        costSense.text = costs[0] + " EVP";
        costSpeed.text = costs[0] + " EVP";
    }

    void Start() {
        PlayerAnimalAssignment.Instance.OnAnimalAssigned += PlayerAnimalAssignment_OnAnimalAssigned;
    }

    private void PlayerAnimalAssignment_OnAnimalAssigned()
    {
        animal = isFavChild ?
            PlayerAnimalAssignment.Instance.AnimalSO :
            PlayerAnimalAssignment.Instance.AnimalSO.Prey;
        displayImg.sprite = animal.Sprite;
    }

    private void SenseButton_OnClick() {
        if (AnimalInfoManager.Instance.EvolutionPoints < costs[levelSense]) return;
        if (parentOfUpgradeDisplaySenseRange.childCount >= maxUpgrade) return;

        AnimalInfoManager.Instance.DecreaseEvolutionPointsBy(costs[levelSense]);
        levelSense++;

        Instantiate(upgradeDisplaySenses, parentOfUpgradeDisplaySenseRange);

        if (isFavChild)
            GeneticAdjustments.Instance.IncreaseSense(animal);
        else
            GeneticAdjustments.Instance.DecreaseSense(animal);
        
        costSense.text = costs[levelSense] + " EVP";
        Debug.Log(costs[levelSense] + " EVP");
    }

    private void SpeedButton_OnClick() {
        if (AnimalInfoManager.Instance.EvolutionPoints < costs[levelSpeed]) return;
        if (parentOfUpgradeDisplaySpeed.childCount >= maxUpgrade) return;

        AnimalInfoManager.Instance.DecreaseEvolutionPointsBy(costs[levelSpeed]);
        levelSpeed++;

        Instantiate(upgradeDisplaySenses, parentOfUpgradeDisplaySpeed);

        if (isFavChild)
            GeneticAdjustments.Instance.IncreaseSpeed(animal);
        else
            GeneticAdjustments.Instance.DecreaseSpeed(animal);
        
        costSpeed.text = costs[levelSpeed] + " EVP";
    }
}
