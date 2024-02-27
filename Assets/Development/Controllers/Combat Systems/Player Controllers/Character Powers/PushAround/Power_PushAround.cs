using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_PushAround : BasePower
{
    public float PushValue = 10f;
    public float PushDamage = 4f;
    public float PushDistance = 8.5f;

    public ParticleSystem PushParticle;

    private ParticleSystem instantiatedPushParticle;
    public override void UsePower(Vector3 position)
    {
        base.UsePower(position);
        instantiatedPushParticle = Instantiate(PushParticle, CombatGameManager.Instance.PlayerCharacter.transform);
        instantiatedPushParticle.transform.localPosition = Vector3.up * 1.25f;
        List<CharacterSettings> enemies = new List<CharacterSettings>();

        foreach (CharacterSettings enemy in CombatGameManager.Instance.CurrentRoom.Enemies)
        {
            enemies.Add(enemy);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null && Vector3.Distance(CombatGameManager.Instance.PlayerCharacter.transform.position, enemies[i].transform.position) < PushDistance)
            {
                Rigidbody enemyRigidbody = enemies[i].GetComponent<Rigidbody>();
                enemies[i].IsControlAllowed = false;
                Transform target = enemies[i].transform.GetChild(0);
                CharacterSettings enemy = enemies[i];
                target.DOLocalMoveY(3f,0.25f).SetEase(Ease.Linear).OnComplete(()=> target.DOLocalMoveY(0f, 0.25f).SetEase(Ease.Linear).OnComplete(() => enemy.IsControlAllowed = true));
                enemyRigidbody.velocity = Vector3.zero;
                enemyRigidbody.AddForce((enemies[i].transform.position - CombatGameManager.Instance.PlayerCharacter.transform.position).normalized * PushValue, ForceMode.Acceleration);
                enemies[i].GetComponent<IDamageable>().TakeDamage(PushDamage);
            }else if(enemies[i] == null)
            {
                continue;
            }
        }
    }
}
