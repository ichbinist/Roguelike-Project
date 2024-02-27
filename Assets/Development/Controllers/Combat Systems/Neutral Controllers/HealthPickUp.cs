using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : PickUpController
{
    public float HealAmount = 3f;
    public override void PickUpEffect(CharacterSettings characterSettings)
    {
        characterSettings.CurrentHealth = Mathf.Clamp(characterSettings.CurrentHealth + HealAmount, 0f, characterSettings.MaximumHealth);
        base.PickUpEffect(characterSettings);
        Destroy(gameObject);
    }
}
