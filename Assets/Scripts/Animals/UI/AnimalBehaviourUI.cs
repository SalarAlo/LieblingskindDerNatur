using System.Collections.Generic;
using UnityEngine;

public class AnimalBehaviourUI : MonoBehaviour
{
    [SerializeField] private UrgeUI hungerUrge;
    [SerializeField] private UrgeUI thirstUrge;
    [SerializeField] private UrgeUI reproductionUrge;

    private UrgeHandler handler;

    void Awake() {
        handler = GetComponent<UrgeHandler>();
    }

    void Update() {
        hungerUrge.SetFill(1-handler.GetUrge(Urge.Hunger));
        thirstUrge.SetFill(1-handler.GetUrge(Urge.Thirst));
        reproductionUrge.SetFill(1-handler.GetUrge(Urge.Reproduction));
    }
}
