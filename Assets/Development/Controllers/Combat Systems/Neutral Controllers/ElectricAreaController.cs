using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricAreaController : MonoBehaviour
{
    public float ElectricDamage;
    public float ElectricAttackSpeed;
    public AudioClip ElectricZap;
    private List<IDamageable> damageables = new List<IDamageable>();

    private float timer = 0;

    private RoomSettings roomSettings;

    private void Start()
    {
        roomSettings = GetComponentInParent<RoomSettings>();
    }

    private void Update()
    {
        if (CombatGameManager.Instance.isPaused || CombatGameManager.Instance.isDead)
            return;

        if(roomSettings != null && roomSettings == CombatGameManager.Instance.CurrentRoom)
            DealDamage();
    }
    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && damageables.Contains(damageable) == false)
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

    private void DealDamage()
    {
        timer += Time.deltaTime * ElectricAttackSpeed;

        if (timer > 1f)
        {
            timer = 0f;
            int i = 0;
            foreach (var item in damageables)
            {
                if (!item.Equals(null) && item.IsDamageable)
                {
                    item.TakeDamage(ElectricDamage, (transform.position - item.GetPosition()).normalized, false);
                    i++;
                }
                else if (item == null)
                {
                    damageables.Remove(item);
                    break;
                }
            }
            if(i > 0)
            {
                AudioPoolManager.Instance.PlaySound(ElectricZap, 0.20f);
            }
        }
    }

}
