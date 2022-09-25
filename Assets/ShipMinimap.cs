using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMinimap : MonoBehaviour
{
    [SerializeField]
    private Material[] materials;
    private MeshRenderer rend;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    public void setColor(int i)
    {
        if (rend == null)
        {
            rend = GetComponent<MeshRenderer>();
        }
        rend.material = materials[i];
    }
}
