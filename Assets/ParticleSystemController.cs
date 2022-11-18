using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    public ParticleSystem particleSystem;

    private ParticleSystem.Particle[] _particles;
   
    void Start()
    {
        _particles = new ParticleSystem.Particle[1000];
        particleSystem.Emit(1000);

        var count = particleSystem.GetParticles(_particles);
        for (int i = 0; i < count; ++i)
        {
            ref var particle = ref _particles[i];
            particle.position = Random.insideUnitSphere * 10f;
            particle.startSize = 1f;
        }
        particleSystem.SetParticles(_particles);
    }


    void Update()
    {
        var count = particleSystem.GetParticles(_particles);
        for (int i = 0; i < count; ++i)
        {
            ref var particle = ref _particles[i];
            particle.position += Vector3.forward * Time.deltaTime;
            particle.remainingLifetime = 100f;
        }
        particleSystem.SetParticles(_particles, count);
    }
}
