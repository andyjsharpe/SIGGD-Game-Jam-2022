using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Slot;

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
    [SerializeField]
    private TextMeshProUGUI compsGroup;
    private Component[] currentComps;
    [SerializeField]
    private TextMeshProUGUI shipName;
    private Slot currentSlot;
    [SerializeField]
    private TextMeshProUGUI partName;
    [SerializeField]
    private TextMeshProUGUI partDescription;
    [SerializeField]
    private TextMeshProUGUI CostText;
    private int points;
    private bool spawnedPirate = false;

    // Start is called before the first frame update
    void Awake()
    {
        int maxPoints = PlayerPrefs.GetInt("Points");
        if (maxPoints == 0)
        {
            PlayerPrefs.SetInt("Points", 4);
        }

        //Make the player's ship
        forPlayer = generatePlayerShip(shipNum);
        string n = forPlayer.name;
        shipName.text = n.Substring(0, n.Length - 7);
        foreach (Image i in listContents)
        {
            i.transform.gameObject.SetActive(false);
        }

        int pointsUsed = 0;
        foreach (Component comp in forPlayer.transform.GetComponentsInChildren<Component>())
        {
            pointsUsed += comp.cost;
        }
        points = PlayerPrefs.GetInt("Points") - pointsUsed;
        CostText.text = "POINTS LEFT: " + points.ToString();
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
        forPlayer.beforeStartSetup();
        gameStarted = true;
    }

    public void chooseSlot(int i)
    {
        Component oldComp = currentSlot.GetComponentInChildren<Component>();
        foreach (Transform child in currentSlot.transform)
        {
            Destroy(child.gameObject);
        }
        if (oldComp != null)
        {
            points += oldComp.cost;
        }
        if (points - currentComps[i].cost < 0 || currentComps[i].gameObject.name == "Empty")
        {
            CostText.text = "POINTS LEFT: " + points.ToString();
            return;
        }
        points -= currentComps[i].cost;
        CostText.text = "POINTS LEFT: " + points.ToString();
        GameObject g = Instantiate(currentComps[i].gameObject, currentSlot.transform.position, forPlayer.transform.rotation * Quaternion.Euler(-90, 180, 0), currentSlot.transform);
        currentSlot.markComponent(g, true);
        Component newC = g.GetComponent<Component>();
        newC.ship = forPlayer;
        if (newC is Weapon)
        {
            g.GetComponent<Weapon>().isPlayer = true;
        }
        partName.text = currentComps[i].gameObject.name.ToUpper();
        partDescription.text = currentComps[i].description.ToUpper();
    }

    public void setSlotType(Slot.slotType slotType)
    {
        if (slotType == Slot.slotType.engine)
        {
            currentComps = engines;
            compsGroup.text = "ENGINES";
        } else if (slotType == Slot.slotType.weapon) {
            currentComps = weapons;
            compsGroup.text = "WEAPONS";
        }
        else {
            currentComps = inners;
            compsGroup.text = "MISC.";
        }
        for (int i = 0; i < listContents.Length; i++)
        {
            if (i >= currentComps.Length)
            {
                listContents[i].transform.gameObject.SetActive(false);
            } else
            {
                listContents[i].transform.gameObject.SetActive(true);
                listContents[i].GetComponentInChildren<TextMeshProUGUI>().text = (currentComps[i].name + " (cost " + currentComps[i].cost.ToString() + ")").ToUpper();
            }
        }
        
    }

    public void generateNextShip()
    {
        shipNum++;
        shipNum %= shipPrefabs.Length;
        Destroy(forPlayer.gameObject);
        forPlayer = generatePlayerShip(shipNum);
        string n = forPlayer.name;
        shipName.text = n.Substring(0, n.Length - 7);
        compsGroup.text = "CHOOSE A SLOT";
        foreach (Image i in listContents)
        {
            i.transform.gameObject.SetActive(false);
        }
        partName.text = "";
        partDescription.text = "";
        int pointsUsed = 0;
        foreach (Component comp in forPlayer.transform.GetComponentsInChildren<Component>())
        {
            pointsUsed += comp.cost;
        }
        points = PlayerPrefs.GetInt("Points") - pointsUsed;
        CostText.text = "POINTS LEFT: " + points.ToString();
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
        string n = forPlayer.name;
        shipName.text = n.Substring(0, n.Length - 7);
        compsGroup.text = "CHOOSE A SLOT";
        foreach (Image i in listContents)
        {
            i.transform.gameObject.SetActive(false);
        }
        int pointsUsed = 0;
        foreach (Component comp in forPlayer.transform.GetComponentsInChildren<Component>())
        {
            pointsUsed += comp.cost;
        }
        points = PlayerPrefs.GetInt("Points") - pointsUsed;
        CostText.text = "POINTS LEFT: " + points.ToString();
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
                currentSlot = slotHit;
            }
        } else
        {
            missiles = FindObjectsOfType<Missile>();
        }
    }

    private void generateLevel()
    {
        int len = areaLength * PlayerPrefs.GetInt("Points") / 4;
        //Generate ships
        for (int i = -len / 2; i <= len / 2; i += 50)
        {
            for (int j = -len / 2; j <= len / 2; j += 50)
            {
                if (Random.Range(0f, 1f) < shipChance)
                {
                    generateShip(Vector3.right * i + Vector3.forward * j);
                }
            }
        }

        ships = FindObjectsOfType<Ship>();

        //Generate rocks
        for (int i = -len / 2; i <= len / 2; i++)
        {
            for (int j = -len / 2; j <= len / 2; j++)
            {
                if (Random.Range(0f, 1f) < rockChance)
                {
                    generateRock(Vector3.right * i + Vector3.forward * j);
                }
            }
        }

        //Generate planes
        for (int i = -len / 2; i <= len / 2; i += 200)
        {
            for (int j = -len / 2; j <= len / 2; j += 200)
            {
                Instantiate(minimapPlane, Vector3.right * i + Vector3.forward * j, Quaternion.identity);
            }
        }
    }

    Ship generatePlayerShip(int i)
    {
        GameObject g = Instantiate(shipPrefabs[i], Vector3.zero, Quaternion.Euler(0f, 180f, 0f));
        Weapon[] w = new Weapon[weapons.Length - 1];
        for (int c = 1; c < weapons.Length; c++)
        {
            w[c - 1] = weapons[c];
        }
        return g.GetComponent<Ship>().customGenerate(metals, windows, paints, commands, engines[0], thrusters[0], w);
    }

    Ship generateShip(Vector3 position)
    {
        GameObject g = Instantiate(shipPrefabs[Random.Range(0, shipPrefabs.Length)], position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        Weapon[] w = new Weapon[weapons.Length - 1];
        for (int c = 1; c < weapons.Length; c++)
        {
            w[c - 1] = weapons[c];
        }
        Inner[] i = new Inner[inners.Length - 1];
        for (int c = 1; c < inners.Length; c++)
        {
            i[c - 1] = inners[c];
        }
        bool pir = Random.Range(0, 2) == 0;
        if (!spawnedPirate)
        {
            pir = true;
            spawnedPirate = true;
        }
        return g.GetComponent<Ship>().generate(false, pir, 0.5f, metals, windows, paints, commands, engines, w, i, thrusters);
    }

    void generateRock(Vector3 position)
    {
        GameObject g = Instantiate(rocks[Random.Range(0, rocks.Length)], position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        g.transform.localScale = new Vector3(Random.Range(0.125f, 1f), Random.Range(0.125f, 1f), Random.Range(0.125f, 1f));
    }
}
