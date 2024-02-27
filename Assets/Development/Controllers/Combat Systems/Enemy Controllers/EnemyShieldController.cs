using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShieldController : MonoBehaviour, IDamageable
{
    public float ShieldHealth = 5;
    public AudioClip ShieldDestroyedSound;
    public ParticleSystem ShieldDestroyParticle;
    public bool IsDamageable => true;

    private float timer = 0f;

    public CharacterType CharacterType => CharacterType.Enemy;

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void TakeDamage(float damage)
    {
        ShieldHealth = Mathf.Clamp(ShieldHealth - damage, 0, 100);

        if(ShieldHealth <= 0)
        {
            ShieldDestroyParticle.Play();
            AudioPoolManager.Instance.PlaySound(ShieldDestroyedSound, 0.25f);
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        IDamageable IDamageable = collision.gameObject.GetComponent<IDamageable>();
        if (IDamageable != null && IDamageable.CharacterType == CharacterType.Player && IDamageable.IsDamageable)
        {
            timer += Time.deltaTime;
            if(timer > 0.2f)
            {
                IDamageable.TakeDamage(0.1f);
            }
        }
    }

    public void TakeDamage(float damage, Vector3 direction)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage, Vector3 direction, bool pushBack)
    {
        TakeDamage(damage);
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticleSystem particleSystem = other.GetComponent<ParticleSystem>();
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        CharacterSettings damageSource = other.GetComponentInParent<CharacterSettings>();
        IDamageDealer damageDealer = other.gameObject.GetComponentInParent<IDamageDealer>();

        particleSystem.GetCollisionEvents(other, collisionEvents);

        foreach (var collisionEvent in collisionEvents)
        {
            if (IsDamageable)
            {
                float damageAmount = damageSource != null ? damageSource.TemporaryDamage * damageSource.DamageMultiplier : (damageDealer != null ? damageDealer.Damage : 0f);
                TakeDamage(damageAmount, other.transform.forward);
            }
        }
    }
}
