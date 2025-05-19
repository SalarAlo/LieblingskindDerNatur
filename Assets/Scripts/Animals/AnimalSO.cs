using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/new Animal")]
public class AnimalSO : ScriptableObject
{
    public List<Food> EatableFood;
    public Sprite Sprite;
    public AnimalSO Prey;
    public GameObject Model;
    public string Name;
    public float Speed;
    public int SenseRange;
    public Vector2Int MatingPossibilities;
    public float UrgencySensitivity;
}
