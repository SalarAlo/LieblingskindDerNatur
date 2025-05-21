using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BloodSplashInOrder : MonoBehaviour
{
    [SerializeField] private GameObject bloodSplash;
    [SerializeField] private List<GameObject> thingsToDestroy;

    void Update() {
        if (!Input.GetKeyDown(KeyCode.V)) return;
        for (int i = 0; i < thingsToDestroy.Count; i++) {
            Instantiate(bloodSplash, thingsToDestroy[i].transform.position, Quaternion.identity);
            Destroy(thingsToDestroy[i]);
        }
    }
}
