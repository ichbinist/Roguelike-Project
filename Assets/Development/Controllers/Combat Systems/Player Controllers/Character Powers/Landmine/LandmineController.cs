using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LandmineController : MonoBehaviour
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
    public Renderer EmissiveRenderer;
    [HideInInspector]
    public bool IsAllowedToExplode = false;

    MaterialPropertyBlock materialPropertyBlock;
    [FoldoutGroup("Explosion Settings")]
    public float EmissiveIntensity;
    private float emissiveIntensityTimer;
    private Color color;
    private Color color2;
    private float lerpTime = 0f;
    private bool isReverse = false;

    private void Awake()
    {
        materialPropertyBlock = new MaterialPropertyBlock();
        color = EmissiveRenderer.material.GetColor("_EmissionColor");
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        RoomManager.Instance.OnRoomEntered.AddListener(PlayerEnteredRoom);
    }

    private void OnDisable()
    {
        RoomManager.Instance.OnRoomEntered.RemoveListener(PlayerEnteredRoom);
    }

    private void PlayerEnteredRoom(RoomSettings room)
    {
        if (ExplosionParticle != null)
        {
            ParticleSystem explosion = Instantiate(ExplosionParticle, transform.position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * ExplosionRadius;
        }

        Destroy(gameObject);
    }


    /*void OnParticleCollision(GameObject other)
    {
        ParticleSystem part = other.GetComponent<ParticleSystem>();
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

        part.GetCollisionEvents(other, collisionEvents);

        int i = 0;

        while (i < collisionEvents.Count)
        {
            if (!isExploding)
            {
                StartExplosion();
            }
            i++;
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        CharacterSettings characterSettings = other.GetComponent<CharacterSettings>();
        if(characterSettings != null && characterSettings.CharacterType != CharacterType.Player)
        {
            if (!isExploding && IsAllowedToExplode)
            {
                StartExplosion();
            }
        }
    }


    private void Update()
    {
        if (isExploding)
        {
            lerpTime += Time.deltaTime * 8f;

            if (!isReverse)
            {
                color2 = Color.Lerp(Color.blue * 7f, color, lerpTime);
                if (lerpTime >= 1f)
                {
                    lerpTime = 0f;
                    isReverse = true;
                }
            }
            else
            {
                color2 = Color.Lerp(color, Color.blue * 7f, lerpTime);
                if (lerpTime >= 1f)
                {
                    lerpTime = 0f;
                    isReverse = false;
                }
            }

            materialPropertyBlock.SetColor("_EmissionColor", color2);
            EmissiveRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }



    private void StartExplosion()
    {
        StartCoroutine(ExplosionCoroutine());
        ExplosionWarningParticle.transform.localScale = Vector3.one * ExplosionRadius * 2;
        //ExplosionWarningParticle.Play();
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

        damageables = FindObjectsOfType<MonoBehaviour>().OfType<IDamageable>().Where(x => Vector3.Distance(transform.position, x.GetPosition()) < ExplosionRadius && x.IsDamageable).ToList();

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
}
