using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PooledParticle : PooledPrefab<PooledParticle>
{
    ParticleSystem particle;
    protected override void OnCreate()
    {
        base.OnCreate();
        particle = GetComponent<ParticleSystem>();
    }
    protected override void OnGet()
    {
        base.OnGet();
        particle.Play();
    }
    private void Update()
    {
        if (!particle.isPlaying) Release();
    }
}