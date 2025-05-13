using Unity.IO.LowLevel.Unsafe;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;

[RequireComponent(typeof(UrgeHandler))]
public abstract class UrgeResponder : MonoBehaviour
{
    private UrgeHandler urgeHandler;

    protected virtual void Awake() {
        urgeHandler = GetComponent<UrgeHandler>();
    }

    protected virtual void FinishUrge() {
        urgeHandler.SatisfyUrge(GetUrge());
    }
    public abstract Urge GetUrge();
    public abstract void RespondToUrge();
}
