using UnityEngine;

public class AnimalBehaviour : Eatable
{
    [SerializeField] private bool isFemale;
    [SerializeField] private AnimalSO animal;
    private UrgeHandler urgeHandler;
    private NoneUrgeResponder noneUrgeResponder;

    void Awake() {
        isFemale = Random.Range(0, 2) == 1;
        urgeHandler = GetComponent<UrgeHandler>();
        noneUrgeResponder = GetComponent<NoneUrgeResponder>();
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
        if(urgeHandler.GetHighestUrge() == Urge.None)
            noneUrgeResponder.RespondToUrge();
    }
}