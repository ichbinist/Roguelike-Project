using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovementEnemy : EnemyCharacterMovementController
{
    private float timer = 0f;

    protected override void ControlStarted()
    {
        destination = GetRandomTargetDestination();
    }

    protected override void ControlStopped()
    {
        destination = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (!EnemyCharacterSettings.IsControlAllowed) return;
        if (CombatGameManager.Instance.isPaused)
            return;
        if (destination != Vector3.zero)
        {
            Movement();
            Rotation();

            timer += Time.deltaTime;
            if(timer > 1.5f)
            {
                timer = 0f;
                destination = GetRandomTargetDestination();
            }
        }
        else
        {
            destination = GetRandomTargetDestination();
        }

        if (IsDestinationReached())
        {
            destination = Vector3.zero;
        }   
    }

}
