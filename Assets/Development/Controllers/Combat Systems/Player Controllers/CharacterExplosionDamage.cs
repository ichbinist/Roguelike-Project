using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class CharacterExplosionDamage : MonoBehaviour
{
    public float ExplosionRadius;
    public float ExplosionDamage;
    public AudioClip ExplosionClip;
    public ParticleSystem ExplosionParticleSystem;
    ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[10];

    private bool exploded;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }


    private void Update()
    {
        if (ExplosionParticleSystem.isPlaying && !exploded)
        {
            Explode();
            exploded = true;
        }
    }

    public void Explode()
    {
        int numParticlesAlive = ExplosionParticleSystem.GetParticles(m_Particles);
        
        List<IDamageable> damageables = new List<IDamageable>();

        damageables = FindObjectsOfType<MonoBehaviour>().OfType<IDamageable>().Where(x => Vector3.Distance(m_Particles[0].position, x.GetPosition()) < ExplosionRadius && x.IsDamageable && x.CharacterType != CharacterType.Player).ToList();

        foreach (IDamageable damageable in damageables)
        {
            if (damageable.IsDamageable)
                damageable.TakeDamage(ExplosionDamage, -(m_Particles[0].position - damageable.GetPosition()).normalized);
        }
        AudioPoolManager.Instance.PlaySound(ExplosionClip, 0.3f);
    }
}