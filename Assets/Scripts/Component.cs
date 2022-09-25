using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Component : MonoBehaviour
{
    public enum componentType { engine, inner, weapon, command };
    public componentType type;
    public Ship ship;
    public string description;
    public int cost;

    private void Awake()
    {
        ship = GetComponentInParent<Ship>();
    }
}
