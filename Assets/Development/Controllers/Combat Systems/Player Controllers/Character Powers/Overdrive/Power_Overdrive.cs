using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_Overdrive : BasePower
{
    public float Duration;
    public float DamageMultiplier;
    public ParticleSystem OverdriveParticle;

    private ParticleSystem instantiatedOverdrive;
    public override void UsePower(Vector3 position)
    {
        base.UsePower(position);
        instantiatedOverdrive = Instantiate(OverdriveParticle, CombatGameManager.Instance.PlayerCharacter.transform);
        instantiatedOverdrive.transform.localPosition = Vector3.up * 1.25f;
        GameObject CoroutineStarter = new GameObject("Corotine Starter Overdrive Power");
        OverdriveCoroutineStarter starter = CoroutineStarter.AddComponent<OverdriveCoroutineStarter>();
        starter.Timer = Duration;
        starter.instantiatedOverdrive = instantiatedOverdrive.gameObject;
        starter.DamageMultiplier = DamageMultiplier;
        starter.StartRoutine();
    }
}
class OverdriveCoroutineStarter : MonoBehaviour
{
    public float Timer;
    public GameObject instantiatedOverdrive;
    public float DamageMultiplier;

    public void StartRoutine()
    {
        StartCoroutine(UsePowerCoroutine());

    }
    public IEnumerator UsePowerCoroutine()
    {
        CombatGameManager.Instance.PlayerCharacter.DamageMultiplier += DamageMultiplier;
        while (Timer > 0f)
        {
            Timer -= Time.deltaTime;
            if (CombatGameManager.Instance.isDead)
            {
                CombatGameManager.Instance.PlayerCharacter.DamageMultiplier -= DamageMultiplier;
                Destroy(instantiatedOverdrive);
                Destroy(gameObject);
                break;
            }
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        CombatGameManager.Instance.PlayerCharacter.DamageMultiplier -= DamageMultiplier;
        Destroy(instantiatedOverdrive);
        Destroy(gameObject);
    }
}