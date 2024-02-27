using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;

public class CharacterMovementController : MonoBehaviour
{
    private CharacterSettings characterSettings;
    public CharacterSettings CharacterSettings { get { return (characterSettings == null) ? characterSettings = GetComponent<CharacterSettings>() : characterSettings; } }

    private Rigidbody _rigidbody;
    public Rigidbody _Rigidbody { get { return (_rigidbody == null) ? _rigidbody = GetComponent<Rigidbody>() : _rigidbody; } }

    private float footstepTimer;

    public ParticleSystem DashParticle;
    ParticleSystem.EmissionModule EmissionModule;
    public float LocalDashCooldown { get; private set; }
    private bool isDashOnCooldown = false;

    private void Start()
    {
        EmissionModule = DashParticle.emission;
    }
    private void FixedUpdate()
    {
        if (CombatGameManager.Instance.isPaused)
            return;

        if (CharacterSettings.MapState == MapState.Overview)
        {
            _Rigidbody.velocity = Vector3.zero;
            return;
        }

        if (CharacterSettings.IsControlAllowed)
        {
            Movement();
        }
    }

    private void Update()
    {
        if (CombatGameManager.Instance.isPaused)
            return;

        if (CharacterSettings.MapState == MapState.Overview)
        {
            _Rigidbody.velocity = Vector3.zero;
            return;
        }

        if (CharacterSettings.IsControlAllowed && !isDashOnCooldown)
        {
            Dash();
        }
        else if (isDashOnCooldown)
        {
            LocalDashCooldown -= Time.deltaTime;
            if(LocalDashCooldown <= 0)
            {
                isDashOnCooldown = false;
            }
        }
        if(GetMovementDirection().magnitude > 0.1f)
        {
            footstepTimer += Time.deltaTime;

            if(footstepTimer > CharacterSettings.FootstepDelayAmount)
            {
                footstepTimer = 0;
                AudioPoolManager.Instance.PlaySound(GetFootstep(), 1f);
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private AudioClip GetFootstep()
    {
        return CharacterSettings.FootstepSounds[Random.Range(0, CharacterSettings.FootstepSounds.Count)];
    }

    public Vector3 GetMovementDirection()
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        float horizontalInput = 0;
        float verticalInput = 0;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        if (Input.GetKey(KeyCode.W))
        {
            horizontalInput += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            horizontalInput -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            verticalInput += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            verticalInput -= 1;
        }
        Vector3 moveDirection = forward * horizontalInput + right * verticalInput;
        return moveDirection;
    }

    public void Movement()
    {
        _Rigidbody.velocity = GetMovementDirection() * CharacterSettings.MovementSpeed;
    }

    public void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EmissionModule.rateOverTime = 50;
            if (CharacterSettings.DashSound)
                AudioPoolManager.Instance.PlaySound(CharacterSettings.DashSound);
            isDashOnCooldown = true;
            LocalDashCooldown = CharacterSettings.PlayerDashCooldown;
            Vector3 dashDirection = GetMovementDirection().normalized;
            Vector3 dashTargetPosition = _Rigidbody.position + dashDirection * CharacterSettings.DashRange;
            float dashTime = CharacterSettings.DashRange / CharacterSettings.DashSpeed;
            Vector3 dashVelocity = (dashTargetPosition - _Rigidbody.position) / dashTime;
            gameObject.layer = LayerMask.NameToLayer("Player_NoCollision");
            _Rigidbody.velocity = dashVelocity;
            CharacterSettings.IsControlAllowed = false;
            CharacterSettings.IsDamageable = false;
            StartCoroutine(CompleteDash(dashTime));
        }
    }
    private void CheckCollisionWithBreakables()
    {
        List<BreakablePropController> breakableList = new List<BreakablePropController>();
        breakableList.AddRange(FindObjectsOfType<BreakablePropController>().Where(x=> Vector3.Distance(x.transform.position,transform.position) < 5f));

        foreach (BreakablePropController breakable in breakableList)
        {
            if(breakable != null) //  && Vector3.Distance(transform.position,breakable.transform.position) < 2f
            {
                breakable.TakeDamage(999f);
            }
        }

        List<CharacterSettings> enemies = new List<CharacterSettings>();
        enemies.AddRange(FindObjectsOfType<CharacterSettings>().Where(x => Vector3.Distance(x.transform.position, transform.position) < 5f && x.isEnemy && x.IsExecuteable && x.IsExecuteState == true));
        
        foreach (CharacterSettings enemy in enemies)
        {
            if (enemy != null) //  && Vector3.Distance(transform.position,breakable.transform.position) < 2f
            {
                enemy.IsExecuteable = false;
                enemy.GetComponent<CharacterHealthController>().Death();
            }
        }
    }

    private IEnumerator CompleteDash(float dashTime)
    {
        float dashTimelocal = dashTime;

        while (dashTimelocal > 0)
        {
            dashTimelocal -= Time.deltaTime;
            CheckCollisionWithBreakables();
            yield return null;
        }

        _Rigidbody.velocity = Vector3.zero;
        gameObject.layer = LayerMask.NameToLayer("Player");
        CharacterSettings.IsControlAllowed = true;
        CharacterSettings.IsDamageable = true;
        yield return new WaitForSeconds(0.1f);
        EmissionModule.rateOverTime = 0;
    }
}