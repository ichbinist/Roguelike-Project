using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public Vector3 GetPosition();
    public bool IsDamageable { get; }

    public CharacterType CharacterType { get; }

    public void TakeDamage(float damage);
    public void TakeDamage(float damage, Vector3 direction);
    public void TakeDamage(float damage, Vector3 direction, bool pushBack);
}
