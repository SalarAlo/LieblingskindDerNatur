using Unity.Entities;
using UnityEngine;

public class CameraRestriction : MonoBehaviour
{
    private int padding = 5;

    void Update() {
        transform.position = new(Mathf.Clamp(transform.position.x, -padding, WorldGeneration.WORLD_SIZE+padding), Mathf.Clamp(transform.position.y, 1, 30), Mathf.Clamp(transform.position.z, -padding, WorldGeneration.WORLD_SIZE+padding));
    }
}
