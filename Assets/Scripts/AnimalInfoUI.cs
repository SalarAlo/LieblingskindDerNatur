using TMPro;
using UnityEngine;

public class AnimalInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetTextAmount(int amount) {
        text.text = amount.ToString();
    }
}
