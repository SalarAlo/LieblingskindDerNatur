using UnityEngine;
using UnityEngine.UI;

public class UrgeUI : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private float lerpSpeed = 5f;

    private float targetFill = 0f;

    public void SetFill(float fillAmt)
    {
        targetFill = Mathf.Clamp01(fillAmt); // Ensure it's between 0 and 1
    }

    private void Update() {
        fill.fillAmount = Mathf.Lerp(fill.fillAmount, targetFill, Time.deltaTime * lerpSpeed);
    }
}
