using UnityEngine;
using UnityEngine.UI;

public class EndButtons : MonoBehaviour
{
    [SerializeField] private Button relaunchButton;
    [SerializeField] private Button quitButton;

    void Awake() {
        quitButton.onClick.AddListener(Application.Quit);
        relaunchButton.onClick.AddListener(AppRelauncher.Relaunch);
    }
}
