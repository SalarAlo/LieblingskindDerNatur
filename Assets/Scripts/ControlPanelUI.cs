using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject mainGameWindow;
    [SerializeField] private GameObject settingsWindow;
    private bool isActive;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !isActive)
        {
            isActive = true;
            mainGameWindow.SetActive(false);
            settingsWindow.SetActive(true);
            FreeFlyCamera.Instance.Disable();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isActive)
        {
            StartCoroutine(EnableCamera_Routine());
        }
    }

    private IEnumerator EnableCamera_Routine()
    {
        yield return null;
        isActive = false;
        FreeFlyCamera.Instance.Enable();
        mainGameWindow.SetActive(true);
        settingsWindow.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
