using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;

public abstract class UrgeResponder : MonoBehaviour
{
    public abstract Urge GetUrge();
    public abstract void RespondToUrge();
    public virtual void OnUrgeChanged() {}
}
