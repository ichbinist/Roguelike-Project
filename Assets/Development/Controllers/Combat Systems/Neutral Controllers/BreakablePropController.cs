using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePropController : MonoBehaviour, IDamageable
{
    public bool IsPropDamageable = true;
    public float Health = 1f;
    public bool IsDamageable => IsPropDamageable;

    public CharacterType CharacterType => CharacterType.Neutral;

    public ParticleSystem HitParticle, DeathParticle;

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem part = other.GetComponent<ParticleSystem>();
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        CharacterSettings characterSettings = other.GetComponentInParent<CharacterSettings>();
        part.GetCollisionEvents(other, collisionEvents);
        IDamageDealer DamageDealer = other.gameObject.GetComponentInParent<IDamageDealer>();

        int i = 0;

        while (i < collisionEvents.Count)
        {
            if (IsDamageable)
                if (characterSettings)
                    TakeDamage(characterSettings.TemporaryDamage * characterSettings.DamageMultiplier, other.transform.forward);
                else if(DamageDealer != null)
                    TakeDamage(DamageDealer.Damage, other.transform.forward);
            i++;
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(damage, Vector3.zero);
    }

    public void TakeDamage(float damage, Vector3 hitDirection)
    {
        Health -= damage;


        if (Health <= 0)
        {
            Health = 0;
            Death();
        }
        else
        {
            HitParticle.transform.rotation = Quaternion.LookRotation(hitDirection);
            HitParticle.Play();
        }
    }

    public virtual void Death()
    {
        Instantiate(DeathParticle, transform.position, Quaternion.LookRotation(-transform.forward));
        Destroy(gameObject);
    }

    public void TakeDamage(float damage, Vector3 direction, bool pushBack)
    {
        TakeDamage(damage, Vector3.zero);
    }
}
