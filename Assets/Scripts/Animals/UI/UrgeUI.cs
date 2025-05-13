using UnityEngine;
using UnityEngine.UI;

public class UrgeUI : MonoBehaviour
{
    [SerializeField] private Image fill;

    public void SetFill(float fillAmt) => fill.fillAmount = fillAmt;
}
