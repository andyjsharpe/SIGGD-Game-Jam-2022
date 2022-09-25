using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public int maxHealth = 10;
    [HideInInspector]
    public int health = 10;
    [SerializeField]
    private GameObject createOnDestroy;

    private void Start()
    {
        health = maxHealth;
    }

    public void damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Instantiate(createOnDestroy, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
