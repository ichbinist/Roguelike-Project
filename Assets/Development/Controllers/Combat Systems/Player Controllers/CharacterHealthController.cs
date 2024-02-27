using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterHealthController : MonoBehaviour, IDamageable
{
    private Rigidbody characterRigidbody;
    [HideInInspector] public MaterialPropertyBlock materialPropertyBlock;

    private float executeTotalTimer = 0f;

    private CharacterWeaponController characterWeaponController;
    public CharacterWeaponController CharacterWeaponController { get { return (characterWeaponController == null) ? characterWeaponController = GetComponent<CharacterWeaponController>() : characterWeaponController; } }

    [Header("Enemy Character References")]
    [SerializeField, FoldoutGroup("Enemy Character References")] private UnityEvent onCharacterExecuted = new UnityEvent();
    [SerializeField, FoldoutGroup("Enemy Character References")] private BoxCollider characterBoxCollider;
    [SerializeField, FoldoutGroup("Enemy Character References")] private InteractionUIController executeInteractionUI;
    [SerializeField, FoldoutGroup("Enemy Character References")] private Cinemachine.CinemachineVirtualCamera executionCamera;

    [Header("Particles")]
    [SerializeField, FoldoutGroup("Particles")] private ParticleSystem deathParticle;
    [SerializeField, FoldoutGroup("Particles")] private ParticleSystem hitParticle;
    [SerializeField, FoldoutGroup("Particles")] private ParticleSystem stunParticle;

    [Header("Sound Settings")]
    [SerializeField, FoldoutGroup("Sound Settings")] private float hitSoundVolume;

    [HideInInspector] public UnityEvent onDamageTaken = new UnityEvent();
    [HideInInspector] private UnityEvent<CharacterSettings> onCharacterDeath = new UnityEvent<CharacterSettings>();

    private CharacterSettings characterSettings;

    public bool IsDamageable => characterSettings.IsDamageable;
    public CharacterType CharacterType => characterSettings.CharacterType;

    private void Awake()
    {
        InitializeCharacterSettings();
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        characterRigidbody = GetComponent<Rigidbody>();

        if (characterSettings.isEnemy)
        {
            materialPropertyBlock = new MaterialPropertyBlock();
        }
    }

    private void InitializeCharacterSettings()
    {
        characterSettings = GetComponent<CharacterSettings>();
        characterSettings.CurrentHealth = characterSettings.MaximumHealth;
    }

    private void OnParticleCollision(GameObject other)
    {
        HandleParticleCollision(other);
    }

    private void HandleParticleCollision(GameObject other)
    {
        ParticleSystem particleSystem = other.GetComponent<ParticleSystem>();
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        CharacterSettings damageSource = other.GetComponentInParent<CharacterSettings>();
        IDamageDealer damageDealer = other.gameObject.GetComponentInParent<IDamageDealer>();

        particleSystem.GetCollisionEvents(other, collisionEvents);

        foreach (var collisionEvent in collisionEvents)
        {
            if (characterSettings.IsDamageable)
            {
                float damageAmount = damageSource != null ? damageSource.TemporaryDamage * damageSource.DamageMultiplier : (damageDealer != null ? damageDealer.Damage : 0f);
                TakeDamage(damageAmount, other.transform.forward);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (characterSettings != null && characterSettings.CharacterType == CharacterType.Player)
        {
            HandlePlayerCollision(collision);
        }
    }

    private void HandlePlayerCollision(Collision collision)
    {
        if (characterSettings.IsDamageable)
        {
            EnemyCharacterSettings enemyCharacterSettings = collision.gameObject.GetComponent<EnemyCharacterSettings>();
            if (enemyCharacterSettings != null)
            {
                TakeDamage(enemyCharacterSettings.CollisionDamage, (transform.position - enemyCharacterSettings.transform.position).normalized);
            }
        }
    }

    public virtual void TakeDamage(float damage, Vector3 hitDirection)
    {
        characterSettings.CurrentHealth -= damage;

        if (characterSettings.HitSound)
        {
            AudioPoolManager.Instance.PlaySound(characterSettings.HitSound, hitSoundVolume);
        }

        if (characterSettings.CharacterType == CharacterType.Player)
        {
            DamageTakenEffect(hitDirection);
        }

        onDamageTaken.Invoke();

        if (characterSettings.CurrentHealth <= 0)
        {
            characterSettings.CurrentHealth = 0;
            Death();
        }
        else
        {
            PlayHitParticle(hitDirection);
        }
    }

    private void DamageTakenEffect(Vector3 hitDirection)
    {
        if(characterSettings.IsDamageable)
            StartCoroutine(DamageTakenCoroutine(hitDirection));
    }

    private IEnumerator DamageTakenCoroutine(Vector3 hitDirection)
    {
        Vector3 dashDirection = hitDirection.normalized;
        Vector3 dashTargetPosition = characterRigidbody.position + dashDirection * characterSettings.DamageTakenPushbackForce;
        float dashTime = characterSettings.DamageTakenDelay / 2f;
        Vector3 dashVelocity = (dashTargetPosition - characterRigidbody.position) / dashTime;

        characterSettings.IsControlAllowed = false;
        characterSettings.IsDamageable = false;

        yield return new WaitForSeconds(characterSettings.DamageTakenDelay);

        characterSettings.IsControlAllowed = true;
        characterRigidbody.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.25f);

        characterSettings.IsDamageable = true;
    }

    private void PlayHitParticle(Vector3 direction)
    {
        hitParticle.transform.rotation = Quaternion.LookRotation(direction);
        hitParticle.Play();
    }

    public void Death()
    {
        onCharacterDeath.Invoke(characterSettings);

        if (characterSettings.CharacterType == CharacterType.Enemy)
        {
            if (characterSettings.IsExecuteable)
            {
                if (ExecuteStart())
                {
                    executeInteractionUI.ShowInteractionUI();
                }
                else
                {
                    DestroyCharacter();
                }
            }
            else
            {
                DestroyCharacter();
            }
        }

        if (characterSettings.CharacterType == CharacterType.Player)
        {
            DeathPanelController.Instance.PauseGame();
        }
    }

    private void DestroyCharacter()
    {
        (characterSettings as EnemyCharacterSettings)?.RoomSettings.OnEnemyKilled.Invoke(characterSettings);
        InstantiateDeathParticle();
        Destroy(gameObject);
    }

    private void InstantiateDeathParticle()
    {
        Instantiate(deathParticle, transform.position, Quaternion.LookRotation(-transform.forward));
    }

    private void Update()
    {
        if (CombatGameManager.Instance.isPaused) return;

        if (characterSettings.CharacterType == CharacterType.Player)
        {
            return;
        }

        if (characterSettings.IsExecuteState)
        {
            DisableCharacter();

            executeTotalTimer += Time.deltaTime;

            if (executeTotalTimer > characterSettings.ExecuteStateDuration)
            {
                characterSettings.IsExecuteable = false;
                Death();
            }
            else
            {
                //UpdateExecuteInteractionUI();
            }

            ApplyExecutionParticle();
        }
    }

    private void DisableCharacter()
    {
        characterSettings.IsDamageable = false;
        characterSettings.IsControlAllowed = false;
        characterSettings.IsShootingAllowed = false;
        characterSettings.MovementSpeed = 0f;
        characterRigidbody.velocity = Vector3.zero;
        characterRigidbody.angularVelocity = Vector3.zero;
        //characterBoxCollider.enabled = false;
    }

    private void UpdateExecuteInteractionUI()
    {
        if (!CombatGameManager.Instance.PlayerCharacter.IsControlAllowed)
        {
            //executeTotalTimer = 0f;
        }
        else
        {
            UpdateInteractionUIBasedOnDistance();
        }
    }

    private void UpdateInteractionUIBasedOnDistance()
    {
        if (Vector3.Distance(CombatGameManager.Instance.PlayerCharacter.transform.position, transform.position) < characterSettings.ExecuteRange)
        {
            executeInteractionUI.ShowInteractionUI();

            if (Input.GetKey(KeyCode.F))
            {
                executeTotalTimer = Mathf.Clamp01(executeTotalTimer + Time.deltaTime);

                if (executeTotalTimer >= 0.99f)
                {
                    StartCoroutine(FinishExecution());
                }

                executeInteractionUI.UpdateInteractionUI(executeTotalTimer);
            }
            else
            {
                executeTotalTimer = Mathf.Clamp01(executeTotalTimer - Time.deltaTime);
                executeInteractionUI.UpdateInteractionUI(executeTotalTimer);
            }
        }
        else
        {
            executeTotalTimer = Mathf.Clamp01(executeTotalTimer - Time.deltaTime);
            executeInteractionUI.UpdateInteractionUI(executeTotalTimer);
            executeInteractionUI.HideInteractionUI();
        }
    }

    private void ApplyExecutionParticle()
    {
        if (!stunParticle.isPlaying)
        {
            stunParticle.Play();
        }
    }

    protected virtual bool ExecuteStart()
    {
        bool isExecuteStateActive = (Random.Range(0f, 100f) < characterSettings.ExecuteStateProbability);
        characterSettings.IsExecuteState = isExecuteStateActive;
        DisableCharacter();
        return isExecuteStateActive;
    }

    protected IEnumerator FinishExecution()
    {
        executionCamera.Priority = 10;

        CombatGameManager.Instance.PlayerCharacter.IsDamageable = false;
        CombatGameManager.Instance.PlayerCharacter.IsControlAllowed = false;
        CombatGameManager.Instance.PlayerCharacter.IsShootingAllowed = false;

        DisableCharacter();
        yield return new WaitForSeconds(0.2f);

        PerformExecutionJump();

        yield return new WaitForSeconds(characterSettings.ExecuteActionDuration + 0.05f);

        HideCharacterModel();
        //InstantiateDeathParticle();
        onCharacterExecuted.Invoke();

        yield return new WaitForSeconds(0.1f);

        ResetCharacterStateAfterExecution();
        Death();
    }

    private void PerformExecutionJump()
    {
        CombatGameManager.Instance.PlayerCharacter.transform.DOJump(transform.position, 5f, 1, characterSettings.ExecuteActionDuration).SetEase(Ease.Linear);
    }

    private void HideCharacterModel()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void ResetCharacterStateAfterExecution()
    {
        characterRigidbody.velocity = Vector3.zero;
        characterRigidbody.angularVelocity = Vector3.zero;
        CombatGameManager.Instance.PlayerCharacter.CurrentHealth = Mathf.Clamp(CombatGameManager.Instance.PlayerCharacter.CurrentHealth + characterSettings.ExecuteHealthRecoveryRate, 0f, CombatGameManager.Instance.PlayerCharacter.MaximumHealth);
        CombatGameManager.Instance.PlayerCharacter.IsControlAllowed = true;
        CombatGameManager.Instance.PlayerCharacter.IsShootingAllowed = true;
        CombatGameManager.Instance.PlayerCharacter.IsDamageable = true;
        characterSettings.IsExecuteable = false;
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
        characterSettings.CurrentHealth -= damage;

        if (characterSettings.HitSound && pushBack)
        {
            AudioPoolManager.Instance.PlaySound(characterSettings.HitSound, hitSoundVolume);
        }

        if (characterSettings.CharacterType == CharacterType.Player && pushBack)
        {
            DamageTakenEffect(direction);
        }

        onDamageTaken.Invoke();

        if (characterSettings.CurrentHealth <= 0)
        {
            characterSettings.CurrentHealth = 0;
            Death();
        }
        else
        {
            PlayHitParticle(direction);
        }
    }
}
