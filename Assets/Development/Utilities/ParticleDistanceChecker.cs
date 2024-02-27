using UnityEngine;
using System.Collections.Generic;

public class ParticleDistanceChecker : MonoBehaviour
{
    private ParticleSystem ParticleSystem;

    private IDamageable characterHealthController;
    public IDamageable CharacterHealthController { get { return (characterHealthController == null) ? characterHealthController = GetComponent<IDamageable>() : characterHealthController; } }

    private CharacterWeaponController characterWeaponController;
    public CharacterWeaponController CharacterWeaponController { get { return (characterWeaponController == null) ? characterWeaponController = CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterWeaponController>() : characterWeaponController; } }

    private BoxCollider boxCollider;
    public BoxCollider BoxCollider { get { return (boxCollider == null) ? boxCollider = GetComponent<BoxCollider>() : boxCollider; } }

    // Custom class to store particle ID and state

    public float BufferDistanceValue = 2f;
    private class ParticleState
    {
        public uint ParticleID { get; set; }
        public bool IsProcessed { get; set; }
    }

    private Dictionary<uint, ParticleState> particleStates = new Dictionary<uint, ParticleState>();

    bool IsParticleInsideCollider(Vector3 particlePosition)
    {
        // Check if the particle position is within the boundaries of the BoxCollider
        if(BoxCollider != null)
            return (BoxCollider.bounds.Contains(particlePosition) || Vector3.Distance(transform.position,particlePosition) < BufferDistanceValue);
        else
            return (Vector3.Distance(transform.position, particlePosition) < BufferDistanceValue);
    }

    void Update()
    {
        if (ParticleSystem == null)
        {
            ParticleSystem = CharacterWeaponController.InstantiatedShootingParticles;
        }
        else
        {
            ParticleSystem.CollisionModule collision = ParticleSystem.collision;
            if (collision.enabled == false || collision.type == ParticleSystemCollisionType.Planes || (collision.collidesWith & LayerMask.GetMask("Enemy")) == 0)
            {
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ParticleSystem.particleCount];
                int numParticlesAlive = ParticleSystem.GetParticles(particles);

                if (numParticlesAlive == 0) return;

                for (int i = 0; i < numParticlesAlive; i++)
                {
                    uint particleID = particles[i].randomSeed;

                    if (!particleStates.TryGetValue(particleID, out ParticleState particleState))
                    {
                        particleState = new ParticleState { ParticleID = particleID, IsProcessed = false };
                        particleStates.Add(particleID, particleState);
                    }
                    else
                    {
                        particleState = particleStates[particleID];
                    }

                    if (IsParticleInsideCollider(particles[i].position) && !particleState.IsProcessed)
                    {
                        CharacterHealthController.TakeDamage(CombatGameManager.Instance.PlayerCharacter.TemporaryDamage * CombatGameManager.Instance.PlayerCharacter.DamageMultiplier);

                        particleState.IsProcessed = true;
                    }
                    else if (!IsParticleInsideCollider(particles[i].position) && particleState.IsProcessed)
                    {
                        particleStates.Remove(particleID);
                    }
                }
            }
        }
    }
}
