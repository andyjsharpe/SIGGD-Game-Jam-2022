using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : Component
{
    [SerializeField]
    private float maxThrust;
    [SerializeField]
    private ParticleSystem engineParticles;
    //returns the thrust of this engine
    public void engineOn(Rigidbody rigid)
    {
        rigid.AddForceAtPosition(-1 * Time.fixedDeltaTime * rigid.transform.forward * maxThrust * ship.health / ship.maxHealth, transform.position, ForceMode.Force);
        engineParticles.Play();
    }
}
