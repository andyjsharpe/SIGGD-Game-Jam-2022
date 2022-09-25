using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public Ship playerShip;
    [SerializeField]
    private Transform slotCamera;
    [SerializeField]
    private Slider health;
    [SerializeField]
    private Generator generator;
    [SerializeField]
    private TextMeshProUGUI TextMeshProUGUI;
    private bool victory = false;
    private int origionalPirateCount;

    private void Start()
    {
        generator = FindObjectOfType<Generator>();
        origionalPirateCount = getPirateCount();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerShip == null)
        {
            return;
        }
        health.maxValue = playerShip.maxHealth;
        health.value = playerShip.health;
        transform.position = playerShip.transform.position;
        slotCamera.rotation = playerShip.transform.rotation;
        //set pirates count
        int pirates = getPirateCount();
        TextMeshProUGUI.text = pirates.ToString() + " PIRATES LEFT";
        if (pirates <= 0)
        {
            SceneManager.LoadScene(2);
        }
        if (playerShip.health > 0)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                playerShip.forward();
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                playerShip.backward();
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                playerShip.left();
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                playerShip.right();
            }
            if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
            {
                playerShip.shoot();
            }
        }
    }

    private int getPirateCount()
    {
        int i = 0;
        foreach (Ship s in generator.ships)
        {
            if (s == null || s.pirate)
            {
                i++;
            }
        }
        return i;
    }
}
