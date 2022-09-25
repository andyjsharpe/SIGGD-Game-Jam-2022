using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Ship playerShip;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerShip.health <= 0)
        {
            //Player has died
        }
        transform.position = playerShip.transform.position;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
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
