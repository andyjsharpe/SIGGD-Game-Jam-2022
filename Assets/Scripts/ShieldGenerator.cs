using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShieldGenerator : Inner
{
    [SerializeField]
    private float maxShieldHitpoints;
    private float shieldHitpoints;
    [SerializeField]
    private float regenPerSecond;
    [SerializeField]
    private GameObject shield;
    [SerializeField]
    private SphereCollider sCollider;

    private void Awake()
    {
        shieldHitpoints = maxShieldHitpoints;
    }

    private void FixedUpdate()
    {
        if (ship.health > 0)
        {
            shieldHitpoints += regenPerSecond * Time.deltaTime;
            if (shieldHitpoints > maxShieldHitpoints)
            {
                shieldHitpoints = maxShieldHitpoints;
            }
            if (shieldHitpoints > 0)
            {
                shield.SetActive(true);
                sCollider.enabled = true;
            }
        }
    }

    public void damage(int damage)
    {
        if (shieldHitpoints > 0)
        {
            shieldHitpoints -= damage;
            if (shieldHitpoints <= 0)
            {
                shield.SetActive(false);
                sCollider.enabled = false;
            }
        }
    }
}
