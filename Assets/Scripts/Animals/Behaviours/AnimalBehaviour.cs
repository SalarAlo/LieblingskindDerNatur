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
    private UrgeResponder reproductionUrgeResponder;

    public float Speed { get; private set; }
    public int SenseRange { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        isFemale = Random.Range(0, 2) == 1;
        urgeHandler = GetComponent<UrgeHandler>();
        noneUrgeResponder = GetComponent<NoneUrgeResponder>();
        thirstUrgeResponder = GetComponent<ThirstUrgeResponder>();
        hungerUrgeResponder = GetComponent<HungerUrgeResponder>();
        reproductionUrgeResponder = GetComponent<ReproductionUrgeResponder>();
    }

    public AnimalSO GetAnimalSO()
    {
        return animal;
    }

    public override string GetIdentifier()
    {
        return animal.name;
    }

    public bool IsFemale()
    {
        return isFemale;
    }

    protected override void Update()
    {
        base.Update();

        switch (urgeHandler.GetHighestUrge()) {
            case Urge.Thirst:
                thirstUrgeResponder.RespondToUrge();
                break;
            case Urge.Hunger:
                hungerUrgeResponder.RespondToUrge();
                break;
            case Urge.Reproduction:
                reproductionUrgeResponder.RespondToUrge();
                break;
            default:
                noneUrgeResponder.RespondToUrge();
                break;
        }
    }

    public void SetGenes() {
        Speed = animal.Speed;
        SenseRange = animal.SenseRange;
    }

    public void SetGenes(float speed, int senseRange) {
        Speed = speed;
        SenseRange = senseRange;
    }
}