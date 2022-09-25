using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : Component
{
    [SerializeField]
    private float maxThrust;
    [SerializeField]
    private ParticleSystem engineParticles;
    [SerializeField]
    private AudioSource engineAudio;
    private float countdown;
    //returns the thrust of this engine
    public void engineOn(Rigidbody rigid)
    {
        rigid.AddForceAtPosition(-1 * Time.fixedDeltaTime * rigid.transform.forward * maxThrust * ship.healthRatio(), transform.position, ForceMode.Force);
        engineParticles.Play();
        if (!engineAudio.isPlaying)
        {
            engineAudio.Play();
        }
        countdown = 0.25f;
    }
    private void FixedUpdate()
    {
        if (countdown > 0)
        {
            countdown -= Time.fixedDeltaTime;
            if (countdown <= 0)
            {
                countdown = 0;
                engineAudio.Stop();
            }
        }

    }
}
