using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullGrenadeController : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    public ParticleSystem _ParticleSystem {  get { return (_particleSystem == null) ? _particleSystem = GetComponent<ParticleSystem>() : _particleSystem; } }

    private SphereCollider sphereCollider;
    public SphereCollider SphereCollider { get { return (sphereCollider == null) ? sphereCollider = GetComponent<SphereCollider>() : sphereCollider; } }

    public SphereCollider SecondaryCollider;

    private List<Rigidbody> rigidbodies = new List<Rigidbody>();

    public bool IsEffectingPlayer;
    public float PullingForce = 750f;

    private void OnTriggerEnter(Collider other)
    {
        if (_ParticleSystem.isPlaying)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();
            if (IsEffectingPlayer)
            {
                if(damageable != null)
                {
                    if(rigidbodies.Contains(rigidbody) == false)
                    {
                        rigidbodies.Add(rigidbody);
                    }
                }
                else
                {
                }
            }
            else
            {
                if (damageable != null)
                {
                    if (rigidbodies.Contains(rigidbody) == false && damageable.CharacterType != CharacterType.Player)
                    {
                        rigidbodies.Add(rigidbody);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_ParticleSystem.isPlaying)
        {
            CharacterSettings character = other.GetComponent<CharacterSettings>();
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();

            if (rigidbodies.Contains(rigidbody) == true)
            {
                rigidbody.velocity = Vector3.zero;
                
                if(character != null)
                    character.IsControlAllowed = true;
                
                rigidbodies.Remove(rigidbody);
            }
        }
    }

    private void Update()
    {
        AddForce();
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1];
        int particleCount = _ParticleSystem.GetParticles(particles);

        if (particleCount > 0)
        {
            // Access the first particle in the array
            ParticleSystem.Particle firstParticle = particles[0];
            SphereCollider.center = firstParticle.position;
            SecondaryCollider.center = firstParticle.position;
            SecondaryCollider.enabled = true;
        }
        else
        {
            SecondaryCollider.enabled = false;
        }

        if (CombatGameManager.Instance.isDead)
        {
            Destroy(gameObject);
        }
    }

    private void ResetObject()
    {
        for (int i = 0; i < rigidbodies.Count; i++)
        {
            if (rigidbodies[i] != null)
            {
                rigidbodies[i].velocity = Vector3.zero;
                CharacterSettings characterSettings = rigidbodies[i].GetComponent<CharacterSettings>();
                if(characterSettings != null)
                    characterSettings.IsControlAllowed = true;
            }
            else
            {
                continue;
            }
        }
    }

    private void AddForce()
    {
        for (int i = 0; i < rigidbodies.Count; i++)
        {
            if (rigidbodies[i] != null)
            {
                Vector3 direction = (transform.TransformPoint(SphereCollider.center) - rigidbodies[i].position).normalized;
                rigidbodies[i].velocity = direction * PullingForce;
                CharacterSettings characterSettings = rigidbodies[i].GetComponent<CharacterSettings>();
                if (characterSettings != null)
                    characterSettings.IsControlAllowed = false;
            }
            else
            {
                continue;
            }
        }
    }

    private void OnDestroy()
    {
        ResetObject();
    }
}