using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public enum slotType {engine, inner, weapon, command};
    public slotType type;
    private Component component;
    private Generator generator;
    private bool isPlayer = false;

    public void markComponent(GameObject obj, bool p)
    {
        component = obj.GetComponent<Component>();
        isPlayer = p;
    }
}
