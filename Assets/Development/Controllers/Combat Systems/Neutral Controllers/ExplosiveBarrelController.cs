using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ExplosiveBarrelController : MonoBehaviour, IDamageable
{
    private bool isExploding = false;
    [FoldoutGroup("Explosion Settings")]
    public float ExplosionDelay = 0.5f;
    [FoldoutGroup("Explosion Settings")]
    public float ExplosionRadius = 5f;
    [FoldoutGroup("Explosion Settings")]
    public float ExplosionDamage = 2f;
    [FoldoutGroup("Explosion Settings")]
    public ParticleSystem ExplosionParticle;
    [FoldoutGroup("Explosion Settings")]
    public ParticleSystem ExplosionWarningParticle;
    [FoldoutGroup("Explosion Settings")]
    public Renderer BarrelRenderer;
    private RoomSettings room;

    MaterialPropertyBlock materialPropertyBlock;
    private Color color;
    private Color color2;

    private float lerpTime = 0f;
    private bool isReverse = false;

    public bool IsDamageable => true;

    public CharacterType CharacterType => CharacterType.Neutral;

    private void Awake()
    {
        materialPropertyBlock = new MaterialPropertyBlock();
        color = BarrelRenderer.material.GetColor("_EmissionColor");

        room = GetComponentInParent<RoomSettings>();
    }

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem part = other.GetComponent<ParticleSystem>();
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

        part.GetCollisionEvents(other, collisionEvents);

        int i = 0;

        while (i < collisionEvents.Count)
        {
            if(!isExploding)
            {
                StartExplosion();
            }
            i++;
        }
    }


    private void Update()
    {
        if (isExploding)
        {
            lerpTime += Time.deltaTime * 8f;

            if (!isReverse)
            {
                color2 = Color.Lerp(Color.white * 2f, color, lerpTime);
                if (lerpTime >= 1f)
                {
                    lerpTime = 0f;
                    isReverse = true;
                }
            }
            else
            {
                color2 = Color.Lerp(color, Color.white * 2f, lerpTime);
                if (lerpTime >= 1f)
                {
                    lerpTime = 0f;
                    isReverse = false;
                }
            }

            materialPropertyBlock.SetColor("_EmissionColor", color2);
            BarrelRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }



    private void StartExplosion()
    {

        StartCoroutine(ExplosionCoroutine());
        ExplosionWarningParticle.transform.localScale = Vector3.one * ExplosionRadius;
        ExplosionWarningParticle.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }

    private IEnumerator ExplosionCoroutine()
    {
        isExploding = true;
        yield return new WaitForSeconds(ExplosionDelay);

        List<IDamageable> damageables = new List<IDamageable>();

        damageables = FindObjectsOfType<MonoBehaviour>().OfType<IDamageable>().Where(x=> Vector3.Distance(transform.position, x.GetPosition()) < ExplosionRadius && x.IsDamageable).ToList();

        foreach (IDamageable damageable in damageables)
        {
            if (damageable.IsDamageable)
                damageable.TakeDamage(ExplosionDamage, -(transform.position - damageable.GetPosition()).normalized);
        }

        if (ExplosionParticle != null)
        {
            ParticleSystem explosion = Instantiate(ExplosionParticle, transform.position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * ExplosionRadius;
        }

        Destroy(gameObject);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void TakeDamage(float damage)
    {
        if (!isExploding)
        {
            StartExplosion();
        }
    }

    public void TakeDamage(float damage, Vector3 direction)
    {
        if (!isExploding)
        {
            StartExplosion();
        }
    }

    public void TakeDamage(float damage, Vector3 direction, bool pushBack)
    {
        if (!isExploding)
        {
            StartExplosion();
        }
    }
}
