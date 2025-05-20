using System;
using TMPro;
using UnityEngine;
using System.Diagnostics;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedTest;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            Time.timeScale = Math.Max(1, Time.timeScale-1);
            speedTest.text = "Simulations Geschwindgkeit: " + Time.timeScale;
        } else if(Input.GetKeyDown(KeyCode.RightArrow)) {
            Time.timeScale = Math.Min(Time.timeScale+1, 10);
            speedTest.text = "Simulationsgeschwindgkeit: " + Time.timeScale;
        }
    }
}


public class AppRelauncher : MonoBehaviour
{
    public static void Relaunch()
    {
#if UNITY_STANDALONE
        string executablePath = Process.GetCurrentProcess().MainModule.FileName;
        Process.Start(executablePath);
        Application.Quit();
#else
        Debug.LogWarning("Relaunch is only supported in standalone builds.");
#endif
    }
}