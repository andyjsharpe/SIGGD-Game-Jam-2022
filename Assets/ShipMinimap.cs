using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMinimap : MonoBehaviour
{
    [SerializeField]
    private Material[] materials;
    [SerializeField]
    private Material[] doneMaterials;
    private MeshRenderer rend;
    public MeshRenderer commandRend;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        commandRend = transform.parent.GetComponentInChildren<Command>().transform.GetComponent<MeshRenderer>();
    }

    public void setColor(int i)
    {
        if (rend == null)
        {
            rend = GetComponent<MeshRenderer>();
        }
        rend.material = materials[i];
        commandRend.material = doneMaterials[i];
    }
}
