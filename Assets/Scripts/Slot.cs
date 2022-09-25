using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public enum slotType {engine, inner, weapon, command};
    public slotType type;
    private Component component;

    public void markComponent(GameObject obj)
    {
        component = obj.GetComponent<Component>();
    }
}
