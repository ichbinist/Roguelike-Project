using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_Protection : BasePower
{
    public float Duration;
    public bool LockShooting;
    public ParticleSystem ShieldParticle;
    private ParticleSystem instantiatedShield;
    public override void UsePower(Vector3 position)
    {
        base.UsePower(position);
        instantiatedShield = Instantiate(ShieldParticle, CombatGameManager.Instance.PlayerCharacter.transform);
        instantiatedShield.transform.localPosition = Vector3.up * 1.25f;
        GameObject CoroutineStarter = new GameObject("Corotine Starter Protection Power");
        ProtectionCoroutineStarter starter = CoroutineStarter.AddComponent<ProtectionCoroutineStarter>();
        starter.Timer = Duration;
        starter.instantiatedShield = instantiatedShield.gameObject;
        starter.LockShooting = LockShooting;
        starter.StartRoutine();
    }
}


class ProtectionCoroutineStarter : MonoBehaviour
{
    public float Timer;
    public GameObject instantiatedShield;
    public bool LockShooting;
    public void StartRoutine()
    {
        StartCoroutine(UsePowerCoroutine());

    }
    public IEnumerator UsePowerCoroutine()
    {
        while (Timer > 0f)
        {
            CombatGameManager.Instance.PlayerCharacter.IsDamageable = false;
            if (LockShooting)
            {
                CombatGameManager.Instance.PlayerCharacter.IsShootingAllowed = false;
            }
            gameObject.layer = LayerMask.NameToLayer("Player_NoCollision");
            Timer -= Time.deltaTime;
            if (CombatGameManager.Instance.isDead)
            {
                gameObject.layer = LayerMask.NameToLayer("Player");
                Destroy(instantiatedShield);
                Destroy(gameObject);

                if (LockShooting)
                {
                    CombatGameManager.Instance.PlayerCharacter.IsShootingAllowed = true;
                }
            }
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        if (LockShooting)
        {
            CombatGameManager.Instance.PlayerCharacter.IsShootingAllowed = true;
        }
        CombatGameManager.Instance.PlayerCharacter.IsDamageable = true;
        Destroy(instantiatedShield);
        gameObject.layer = LayerMask.NameToLayer("Player");
        Destroy(gameObject);
    }
}