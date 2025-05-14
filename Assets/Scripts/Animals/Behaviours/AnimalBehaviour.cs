using System.Net.Sockets;
using UnityEngine;

public class AnimalBehaviour : Food
{
    [SerializeField] private bool isFemale;
    [SerializeField] private AnimalSO animal;
    private UrgeHandler urgeHandler;
    private NoneUrgeResponder noneUrgeResponder;
    private ThirstUrgeResponder thirstUrgeResponder;
    private HungerUrgeResponder hungerUrgeResponder;

    protected override void Awake() {
        base.Awake();
        isFemale = Random.Range(0, 2) == 1;
        urgeHandler = GetComponent<UrgeHandler>();
        noneUrgeResponder = GetComponent<NoneUrgeResponder>();
        thirstUrgeResponder = GetComponent<ThirstUrgeResponder>();
        hungerUrgeResponder = GetComponent<HungerUrgeResponder>();
    }

    public AnimalSO GetAnimalSO(){
        return animal;
    }

    public override string GetIdentifier() {
        return animal.name;
    }

    public bool IsFemale() {
        return isFemale;
    }

    protected override void Update() {
        base.Update();

        switch (urgeHandler.GetHighestUrge()) {
            case Urge.Thirst:
                thirstUrgeResponder.RespondToUrge();
                break;
            case Urge.Hunger:
                hungerUrgeResponder.RespondToUrge();
                break;
            default:
                noneUrgeResponder.RespondToUrge();
                break;
        }
    }
}