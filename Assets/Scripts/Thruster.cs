using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : Inner
{
    [SerializeField]
    private float maxThrust;
    [SerializeField]
    private float maxThrustSpin;
    [SerializeField]
    private GameObject thrustHolder;
    [SerializeField]
    private ParticleSystem thrustParticles;

    public void thrusterBack(Rigidbody rigid)
    {
        float maxVelMod = Mathf.Min(2, rigid.velocity.magnitude);
        rigid.AddForceAtPosition(-1 * Time.fixedDeltaTime * rigid.velocity.normalized * maxVelMod * maxThrust * ship.healthRatio(), transform.position, ForceMode.Force);
        thrustHolder.transform.rotation = Quaternion.LookRotation(-1 * rigid.velocity, Vector3.up);
        thrustParticles.Play();
    }

    public void thrusterRotate(Rigidbody rigid, bool right)
    {
        //gives vector needed to turn ship
        Vector3 forceDir = Vector3.Cross(rigid.position - (transform.position + Vector3.up * 1.5f), Vector3.up).normalized;
        if (forceDir == Vector3.zero)
        {
            return;
        }
        if (!right)
        {
            forceDir *= -1;
        }
        rigid.AddForceAtPosition(Time.fixedDeltaTime * forceDir * maxThrustSpin * ship.healthRatio(), transform.position, ForceMode.Force);
        thrustHolder.transform.rotation = Quaternion.LookRotation(forceDir, Vector3.up);
        thrustParticles.Play();
    }
}
