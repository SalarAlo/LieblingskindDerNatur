using UnityEngine;

public class AnimalBehaviour : Eatable
{
    [SerializeField] private bool isFemale;
    [SerializeField] private AnimalSO animal;
    private UrgeHandler urgeHandler;
    private NoneUrgeResponder noneUrgeResponder;
    private ThirstUrgeResponder thirstUrgeResponder;
    private HungerUrgeResponder hungerUrgeResponder;

    void Awake() {
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

    private void Update() {
        switch (urgeHandler.GetHighestUrge()) {
            case Urge.None:
                noneUrgeResponder.RespondToUrge();
                break;
            case Urge.Thirst:
                thirstUrgeResponder.RespondToUrge();
                break;
            case Urge.Hunger:
                hungerUrgeResponder.RespondToUrge();
                break;
        }
    }
}