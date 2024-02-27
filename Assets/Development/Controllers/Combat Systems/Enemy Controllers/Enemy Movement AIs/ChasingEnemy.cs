using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingEnemy : EnemyCharacterMovementController
{
    private float timer = 0f;
    private float dashTimer = 0f;
    private bool ableToDash = true;
    private float localDashCooldown;

    [FoldoutGroup("Local Settings", true)]
    public bool CanDash = true;

    private void Start()
    {
        localDashCooldown = Random.Range(EnemyCharacterSettings.DashCooldown.x, EnemyCharacterSettings.DashCooldown.y);
    }

    protected override void ControlStarted()
    {
        destination = GetPlayerTargetDestination();
    }

    protected override void ControlStopped()
    {
        destination = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (CombatGameManager.Instance.isPaused)
            return;

        if (!EnemyCharacterSettings.IsControlAllowed) return;

        if (destination != Vector3.zero)
        {
            Movement();
            Rotation();

            if (CanDash)
            {
                if (!ableToDash)
                {
                    dashTimer += Time.deltaTime;
                }

                if (!ableToDash && dashTimer > localDashCooldown)
                {
                    dashTimer = 0f;
                    ableToDash = true;
                }


                if (Vector3.Distance(transform.position, GetPlayerTargetDestination()) < EnemyCharacterSettings.DashDistanceToPlayer && ableToDash)
                    Dash();
            }

            timer += Time.deltaTime;
            if (timer > 0.25f)
            {
                timer = 0f;
                destination = GetPlayerTargetDestination();
            }
        }
        else
        {
            destination = GetPlayerTargetDestination();
        }

        if (IsDestinationReached())
        {
            destination = Vector3.zero;
        }
    }


    public void Dash()
    {
        ableToDash = false;
        Vector3 dashDirection = GetMovementDirection().normalized;
        Vector3 dashTargetPosition = _Rigidbody.position + dashDirection * EnemyCharacterSettings.DashRange;
        float dashTime = EnemyCharacterSettings.DashRange / EnemyCharacterSettings.DashSpeed;
        Vector3 dashVelocity = (dashTargetPosition - _Rigidbody.position) / dashTime;
        _Rigidbody.velocity = dashVelocity;
        EnemyCharacterSettings.IsControlAllowed = false;
        StartCoroutine(CompleteDash(dashTime));
    }

    private IEnumerator CompleteDash(float dashTime)
    {
        yield return new WaitForSeconds(dashTime);
        _Rigidbody.velocity = Vector3.zero;
        EnemyCharacterSettings.IsControlAllowed = true;
    }
}
