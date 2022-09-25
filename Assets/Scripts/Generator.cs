using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Generator : MonoBehaviour
{
    //Ship generation
    [SerializeField]
    private GameObject[] shipPrefabs;
    [SerializeField]
    private Material[] metals;
    [SerializeField]
    private Material[] windows;
    [SerializeField]
    private Material[] paints;
    [SerializeField]
    private Command[] commands;
    [SerializeField]
    private Engine[] engines;
    [SerializeField]
    private Weapon[] weapons;
    [SerializeField]
    private Inner[] inners;
    [SerializeField]
    private Thruster[] thrusters;

    //Station generation
    [SerializeField]
    private GameObject[] stations;

    //Object generation
    [SerializeField]
    private GameObject[] rocks;
    [SerializeField]
    private GameObject minimapPlane;

    //Generation numbers
    [SerializeField]
    private int areaLength;
    [SerializeField]
    private float rockChance;
    [SerializeField]
    private float shipChance;

    //values to avoid find calls
    public Ship[] ships;
    public Missile[] missiles;

    // Start is called before the first frame update
    void Awake()
    {
        //Make the player's ship
        Ship forPlayer = generatePlayerShip();
        FindObjectOfType<Player>().playerShip = forPlayer;

        //Generate ships
        for (int i = -areaLength / 2; i <= areaLength / 2; i += 50)
        {
            for (int j = -areaLength / 2; j <= areaLength / 2; j += 50)
            {
                if (Random.Range(0f, 1f) < shipChance)
                {
                    generateShip(Vector3.right * i + Vector3.forward * j);
                }
            }
        }

        ships = FindObjectsOfType<Ship>();

        //Generate rocks
        for (int i = -areaLength / 2; i <= areaLength / 2; i++)
        {
            for (int j = -areaLength / 2; j <= areaLength / 2; j++)
            {
                if (Random.Range(0f, 1f) < rockChance) {
                    generateRock(Vector3.right * i + Vector3.forward * j);
                }
            }
        }

        //Generate planes
        for (int i = -areaLength / 2; i <= areaLength / 2; i += 200)
        {
            for (int j = -areaLength / 2; j <= areaLength / 2; j += 200)
            {
                Instantiate(minimapPlane, Vector3.right * i + Vector3.forward * j, Quaternion.identity);
            }
        }
    }

    public void recalcShips()
    {
        ships = FindObjectsOfType<Ship>();
    }

    void FixedUpdate()
    {
        missiles = FindObjectsOfType<Missile>();
    }

    Ship generatePlayerShip()
    {
        GameObject g = Instantiate(shipPrefabs[Random.Range(0, shipPrefabs.Length)], Vector3.zero, Quaternion.Euler(0f, 180f, 0f));
        return g.GetComponent<Ship>().generate(true, 1, metals, windows, paints, commands, engines, weapons, inners, thrusters);
    }

    Ship generateShip(Vector3 position)
    {
        GameObject g = Instantiate(shipPrefabs[Random.Range(0, shipPrefabs.Length)], position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        return g.GetComponent<Ship>().generate(false, 0.75f, metals, windows, paints, commands, engines, weapons, inners, thrusters);
    }

    void generateRock(Vector3 position)
    {
        GameObject g = Instantiate(rocks[Random.Range(0, rocks.Length)], position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        g.transform.localScale = new Vector3(Random.Range(0.125f, 1f), Random.Range(0.125f, 1f), Random.Range(0.125f, 1f));
    }
}
