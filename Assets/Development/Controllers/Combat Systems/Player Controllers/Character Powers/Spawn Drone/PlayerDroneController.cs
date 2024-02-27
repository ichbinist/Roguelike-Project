using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerDroneController : MonoBehaviour, IDamageable, IDamageDealer
{
    private Transform graphics;
    public Transform Graphics { get { return (graphics == null) ? graphics = transform.GetChild(0) : graphics; } }

    private Rigidbody _rigidbody;
    public Rigidbody _Rigidbody { get { return (_rigidbody == null) ? _rigidbody = GetComponent<Rigidbody>() : _rigidbody; } }

    private float shootTimer;
    private bool isShooting;
    private CharacterSettings enemyCharacterSettings;
    public Transform Head;
    public float TurretActiveTime;
    [HideInInspector]
    public Action OnBulletFired;
    public float Health;
    public float ShootingSpeed = 1f;
    public ParticleSystem ShootingParticles;
    public ParticleSystem HitParticle, DeathParticle;
    public bool IsTurretDamageable;
    public float DroneMovementSpeed;
    public float Damage { get; set; }
    public bool IsDamageable => IsTurretDamageable;

    public CharacterType CharacterType => CharacterType.Player;

    private void Start()
    {
        if (IsDamageable == false)
        {
            GetComponent<Collider>().enabled = false;
        }
        Graphics.transform.DOMoveY(1f,0.4f).SetLoops(-1,LoopType.Yoyo);
    }


    void OnParticleCollision(GameObject other)
    {
        if (IsDamageable)
        {
            ParticleSystem part = other.GetComponent<ParticleSystem>();
            List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
            CharacterSettings characterSettings = other.GetComponentInParent<CharacterSettings>();
            part.GetCollisionEvents(other, collisionEvents);

            int i = 0;

            while (i < collisionEvents.Count)
            {
                TakeDamage(characterSettings.TemporaryDamage * characterSettings.DamageMultiplier, other.transform.forward);
                i++;
            }
        }
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

    private void Update()
    {
        if (CombatGameManager.Instance.isPaused)
            return;

        if(Vector3.Distance(transform.position, CombatGameManager.Instance.PlayerCharacter.transform.position) > 5f)
        {
            Movement();
        }
        else
        {
            _Rigidbody.velocity = Vector3.zero;
        }

        if (CombatGameManager.Instance.isDead)
        {
            Destroy(gameObject);
        }

        if (CombatGameManager.Instance.CurrentRoom.IsThereCombat())
        {
            Rotation(GetEnemyDirection());
            HandleShooting(() => ShootBullet(GetEnemyDirection()));
        }
    }

    protected virtual Vector3 GetMovementDirection()
    {
        return (CombatGameManager.Instance.PlayerCharacter.transform.position - transform.position).normalized;
    }

    protected void Movement()
    {
        _Rigidbody.velocity = GetMovementDirection() * DroneMovementSpeed;
    }

    protected virtual Vector3 GetEnemyPosition()
    {
        if (enemyCharacterSettings == null)
        {
            enemyCharacterSettings = CombatGameManager.Instance.CurrentRoom.Enemies[UnityEngine.Random.Range(0, CombatGameManager.Instance.CurrentRoom.Enemies.Count)];
        }
        return enemyCharacterSettings.transform.position;
    }

    protected virtual Quaternion GetEnemyDirection()
    {
        Vector3 direction = GetEnemyPosition() - transform.position;
        direction.y = 0f;
        return Quaternion.LookRotation(direction);
    }

    protected void HandleShooting(Action e)
    {
        if (!isShooting)
        {
            isShooting = true;
            shootTimer = 0f;
        }

        shootTimer += Time.deltaTime;

        if (shootTimer >= 1f / ShootingSpeed)
        {
            e.Invoke(); // Mostly, it will be, ShootBullet(GetPlayerDirection())
            shootTimer = 0f;
        }
    }
    protected void Rotation(Quaternion direction)
    {
        Head.rotation = Quaternion.Lerp(Head.rotation, direction, Time.deltaTime * 5f); // Mostly, direction will be GetPlayerDirection()
    }

    protected void ShootBullet(Quaternion direction)
    {
        if (ShootingParticles != null)
        {
            ShootingParticles.transform.rotation = direction;
            ShootingParticles.Play();
            OnBulletFired?.Invoke();
        }
    }

    public void StartSelfDestruction()
    {
        StartCoroutine(SelfDestructCoroutine());
    }

    private IEnumerator SelfDestructCoroutine()
    {
        yield return new WaitForSeconds(TurretActiveTime);
        Death();
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(damage, Vector3.zero);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void TakeDamage(float damage, Vector3 direction, bool pushBack)
    {
        TakeDamage(damage, Vector3.zero);
    }
}
