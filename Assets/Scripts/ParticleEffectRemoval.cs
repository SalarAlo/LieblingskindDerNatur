using UnityEngine;

public class ParticleEffectRemoval : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake() {
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        var lifetime = main.startLifetime.constantMax;
        Destroy(gameObject, lifetime);
    }
}
