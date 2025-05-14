using System;
using UnityEngine;

public abstract class Food : WorldPlacable
{
    [SerializeField] private GameObject prefab;
    public abstract string GetIdentifier();

    public GameObject GetPrefab() {
        return prefab;
    }
}
