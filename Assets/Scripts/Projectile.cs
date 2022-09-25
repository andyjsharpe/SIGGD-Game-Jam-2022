using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public bool player = false;
    public float recoilForce;
    [SerializeField]
    public int damage;
    public Ship owner;
    [SerializeField]
    private GameObject spawnOnDestroy;
    
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
        Destroy(gameObject, 60);
    }

    private void OnTriggerEnter(Collider other)
    {
        Ship ship;
        Destructable dest;
        if (ship = other.transform.GetComponent<Ship>())
        {
            if (ship == owner)
            {
                return;
            }
            ship.damage(damage, owner);
            Instantiate(spawnOnDestroy, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else if (dest = other.transform.GetComponent<Destructable>())
        {
            dest.damage(damage);
            Instantiate(spawnOnDestroy, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
