using UnityEngine;

public class AnimalEffects : MonoBehaviour
{
    [SerializeField] private ParticleSystem bloodSplatPrefab;
    [SerializeField] private ParticleSystem love;

    void OnDestroy() {
        Instantiate(bloodSplatPrefab, love.transform.position, Quaternion.identity);
    }

    public void EnableLoveEffect() {
        love.Play();
    }

    public void DisableLoveEffect() {
        love.Stop();
    }
}
