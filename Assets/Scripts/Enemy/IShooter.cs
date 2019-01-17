using System;
using UnityEngine;

public interface IShooter
{
    void ResetProjectile(Projectile projectile);
    ParticleSystem Trails { get; }
}

