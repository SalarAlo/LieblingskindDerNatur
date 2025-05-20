using UnityEngine;

public class GeneticAdjustments : MonoBehaviour
{
    public static GeneticAdjustments Instance;
    
    [SerializeField] private AnimalSO mouseSO;
    [SerializeField] private AnimalSO elephantSO;
    [SerializeField] private AnimalSO snakeSO;

    private int mouseSpeedAdjust;
    private int elephantSpeedAdjust;
    private int snakeSpeedAdjust;

    private int mouseSenseAdjust;
    private int elephantSenseAdjust;
    private int snakeSenseAdjust;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void IncreaseSpeed(AnimalSO animal)
    {
        if (animal == mouseSO) mouseSpeedAdjust++;
        else if (animal == elephantSO) elephantSpeedAdjust++;
        else if (animal == snakeSO) snakeSpeedAdjust++;
    }

    public void DecreaseSpeed(AnimalSO animal)
    {
        if (animal == mouseSO) mouseSpeedAdjust--;
        else if (animal == elephantSO) elephantSpeedAdjust--;
        else if (animal == snakeSO) snakeSpeedAdjust--;
    }

    public void IncreaseSense(AnimalSO animal)
    {
        if (animal == mouseSO) mouseSenseAdjust++;
        else if (animal == elephantSO) elephantSenseAdjust++;
        else if (animal == snakeSO) snakeSenseAdjust++;
    }

    public void DecreaseSense(AnimalSO animal)
    {
        if (animal == mouseSO) mouseSenseAdjust--;
        else if (animal == elephantSO) elephantSenseAdjust--;
        else if (animal == snakeSO) snakeSenseAdjust--;
    }

    public int GetSpeedAdjust(AnimalSO animal)
    {
        if (animal == mouseSO) return mouseSpeedAdjust;
        if (animal == elephantSO) return elephantSpeedAdjust;
        if (animal == snakeSO) return snakeSpeedAdjust;
        return 0;
    }

    public int GetSenseAdjust(AnimalSO animal)
    {
        if (animal == mouseSO) return mouseSenseAdjust;
        if (animal == elephantSO) return elephantSenseAdjust;
        if (animal == snakeSO) return snakeSenseAdjust;
        return 0;
    }
}
