using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    private int shipNum = 0;
    private Ship forPlayer = null;
    [SerializeField]
    private GameObject[] hideOnPlay;
    [SerializeField]
    private GameObject[] showOnPlay;
    [HideInInspector]
    public bool gameStarted = false;
    [SerializeField]
    private Image[] listContents;
    [SerializeField]
    private LayerMask clickable;
    private Component[] currentComps;

    // Start is called before the first frame update
    void Awake()
    {
        //Make the player's ship
        forPlayer = generatePlayerShip(shipNum);
    }

    public void startLevel()
    {
        FindObjectOfType<Player>().playerShip = forPlayer;
        generateLevel();
        foreach (GameObject g in hideOnPlay)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in showOnPlay)
        {
            g.SetActive(true);
        }
        gameStarted = true;
    }

    public void setSlotType(Slot.slotType slotType)
    {
        if (slotType == Slot.slotType.engine)
        {
            currentComps = engines;
        } else if (slotType == Slot.slotType.weapon) {
            currentComps = weapons;
        }
        else {
            currentComps = inners;
        }
        for (int i = 0; i < listContents.Length; i++)
        {
            if (i >= currentComps.Length)
            {
                listContents[i].transform.gameObject.SetActive(false);
            } else
            {
                listContents[i].transform.gameObject.SetActive(true);
                listContents[i].GetComponentInChildren<TextMeshProUGUI>().text = currentComps[i].name.ToUpper();
            }
        }
        
    }

    public void generateNextShip()
    {
        shipNum++;
        shipNum %= shipPrefabs.Length;
        Destroy(forPlayer.gameObject);
        forPlayer = generatePlayerShip(shipNum);
    }

    public void generatePrevShip()
    {
        shipNum--;
        if (shipNum < 0)
        {
            shipNum += shipPrefabs.Length;
        }
        Destroy(forPlayer.gameObject);
        forPlayer = generatePlayerShip(shipNum);
    }

    public void recalcShips()
    {
        ships = FindObjectsOfType<Ship>();
    }

    void FixedUpdate()
    {
        if (!gameStarted && Input.GetMouseButton(0))
        {
            Ray ray = hideOnPlay[1].GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 128f, clickable))
            {
                Slot slotHit = hit.collider.transform.GetComponent<Slot>();
                setSlotType(slotHit.type);
            }
        } else
        {
            missiles = FindObjectsOfType<Missile>();
        }
    }

    private void generateLevel()
    {
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
                if (Random.Range(0f, 1f) < rockChance)
                {
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

    Ship generatePlayerShip(int i)
    {
        GameObject g = Instantiate(shipPrefabs[i], Vector3.zero, Quaternion.Euler(0f, 180f, 0f));
        return g.GetComponent<Ship>().customGenerate(true, metals, windows, paints, commands, engines, thrusters, weapons);
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
