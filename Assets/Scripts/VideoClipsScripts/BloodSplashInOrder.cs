using System.Collections.Generic;
using UnityEngine;

public class BloodSplashInOrder : MonoBehaviour
{
    [SerializeField] private GameObject bloodSplash;
    [SerializeField] private List<GameObject> thingsToDestroy;

    void Update() {
        if (!Input.GetKeyDown(KeyCode.V)) return;
        Instantiate(bloodSplash, thingsToDestroy[0].transform.position, Quaternion.identity);
        Destroy(thingsToDestroy[0]);
        thingsToDestroy.RemoveAt(0);
    }
}
