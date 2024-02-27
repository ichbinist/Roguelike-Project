using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingDamagePattern : EnemyCharacterDamageController
{
    private float delayTimer;
    public Vector2 DamageDelay;
    private float localDelayValue;
    private void Start()
    {
        localDelayValue = Random.Range(DamageDelay.x, DamageDelay.y);
    }

    protected override void ControlStarted()
    {
    }

    protected override void ControlStopped()
    {
    }

    private void Update()
    {
        if (CombatGameManager.Instance.isPaused)
            return;
        if (!EnemyCharacterSettings.IsControlAllowed) 
            return;

        if(delayTimer > localDelayValue)
        {
            HandleShooting(() => ShootBullet(GetPlayerDirection()));
        }
        else
        {
            delayTimer += Time.deltaTime;
        }
    }
}