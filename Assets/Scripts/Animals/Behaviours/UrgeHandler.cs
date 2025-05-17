using System;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

[BurstCompile]
public class UrgeHandler : MonoBehaviour
{
    [SerializeField] private float urgeIncreaseTime = 1f / 100f;
    private const float hungerIncrease = .3f;
    private const float thirstIncrease = 1f;
    private const float reproductionIncrease = 0.4f;
    private float urgeSensitivity;  
    private float reproductionUrge;
    private float hungerUrge;
    private float thirstUrge;
    private float timeUntilReproductionAble = 50;
    private bool canReproduce;

    void Awake() {
        urgeSensitivity = GetComponent<AnimalBehaviour>().GetAnimalSO().UrgencySensitivity;
        hungerUrge = thirstUrge = reproductionUrge = UnityEngine.Random.Range(0f, 0.1f);
    }

    void Update()
    {
        hungerUrge += Time.deltaTime * urgeIncreaseTime * hungerIncrease;
        thirstUrge += Time.deltaTime * urgeIncreaseTime * thirstIncrease;

        if (!canReproduce)
        {
            timeUntilReproductionAble -= Time.deltaTime;
            if (timeUntilReproductionAble <= 0) canReproduce = true;
        }
        else
        {
            reproductionUrge += Time.deltaTime * urgeIncreaseTime * reproductionIncrease;
        }

        CheckDeath();
    }

    private void CheckDeath() {
        float max = Math.Max(Math.Max(hungerUrge, thirstUrge), reproductionUrge);
        if (max >= 1)
            Destroy(gameObject);
    }

    public Urge GetHighestUrge() {
        if(urgeSensitivity > thirstUrge && urgeSensitivity > hungerUrge && urgeSensitivity > reproductionUrge) return Urge.None;

        if (hungerUrge >= thirstUrge && hungerUrge >= reproductionUrge)
            return Urge.Hunger;
        else if (thirstUrge >= hungerUrge && thirstUrge >= reproductionUrge)
            return Urge.Thirst;
        else
            return Urge.Reproduction;
    }

    public float GetUrge(Urge u) {
        return u switch {
            Urge.Hunger => hungerUrge,
            Urge.Reproduction => reproductionUrge,
            Urge.Thirst => thirstUrge,
            _ => 0,
        };
    }

    public void SatisfyUrge(Urge urge){
        if(urge == Urge.Hunger) hungerUrge = 0;
        else if (urge == Urge.Thirst) thirstUrge = 0;
        else if (urge == Urge.Reproduction) reproductionUrge = 0;
    }
}

public enum Urge {
    Hunger,
    Thirst,
    Reproduction,
    None
}