using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugManager : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            Time.timeScale = Math.Max(1, Time.timeScale-1);
        } else if(Input.GetKeyDown(KeyCode.RightArrow)) {
            Time.timeScale += 1;
        }
    }
}
