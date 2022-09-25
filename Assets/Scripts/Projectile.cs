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
    
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Ship ship;
        Destructable dest;
        if (ship = collision.transform.GetComponent<Ship>())
        {
            ship.damage(damage);
            Destroy(gameObject);
        }
        else if (dest = collision.transform.GetComponent<Destructable>())
        {
            dest.damage(damage);
            Destroy(gameObject);
        }
    }
}
