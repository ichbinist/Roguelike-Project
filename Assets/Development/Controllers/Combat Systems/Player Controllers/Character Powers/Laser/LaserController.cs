using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    //public ParticleSystem LazerNoozle;
    public GameObject ColliderHolder;
    [HideInInspector]
    public CharacterShootingController CharacterShootingController;
    [HideInInspector]
    public float LaserDamage;
    [HideInInspector] 
    public float LaserAttackSpeed;

    private List<IDamageable> damageables = new List<IDamageable>();

    private float timer = 0;
    private float selfDestroyTimer = 0f;

    private void Update()
    {
        if(selfDestroyTimer > 0 && !CombatGameManager.Instance.isPaused)
        {
            DeathTimer();

            transform.localPosition = Vector3.up;
            transform.rotation = Quaternion.Lerp(transform.rotation, CharacterShootingController.GetDirection(), Time.deltaTime * 6f);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 45f, LayerMask.GetMask("Wall")))
            {
                ColliderHolder.transform.localScale = new Vector3(1f, 1f, 1f * hit.distance);
            }

            DealDamage();
        }
        else
        {
            if(selfDestroyTimer <= 0)
            {
                CombatGameManager.Instance.PlayerCharacter.IsShootingAllowed = true;
                Destroy(gameObject);
            }
        }
    }

    private void DealDamage()
    {
        timer += Time.deltaTime * LaserAttackSpeed;

        if (timer > 1f) 
        {
            timer = 0f;

            foreach (var item in damageables)
            {
                if (!item.Equals(null) && item.IsDamageable && item.CharacterType != CharacterType.Player)
                {
                    item.TakeDamage(LaserDamage, (transform.position - item.GetPosition()).normalized);
                }
                else if (item == null)
                {
                    damageables.Remove(item);
                    break;
                }
            }
        }
    }

    public void StartCloak(float duration)
    {
        selfDestroyTimer = duration;
    }

    private void DeathTimer()
    {
        if (CombatGameManager.Instance.isPaused)
            return;

        if (CombatGameManager.Instance.isDead)
        {
            CombatGameManager.Instance.PlayerCharacter.IsShootingAllowed = true;
            Destroy(gameObject);
        }
        else
        {
            if(selfDestroyTimer <= 0)
            {
                CombatGameManager.Instance.PlayerCharacter.IsShootingAllowed = true;
                Destroy(gameObject);
            }
            else if(selfDestroyTimer > 0)
            {
                CombatGameManager.Instance.PlayerCharacter.IsShootingAllowed = false;
                selfDestroyTimer -= Time.deltaTime;
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable != null && damageables.Contains(damageable) == false)
        {
            damageables.Add(damageable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && damageables.Contains(damageable) == true)
        {
            damageables.Remove(damageable);
        }
    }
}