using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBeamController : MonoBehaviour
{
    private CharacterShootingController characterShootingController;
    public CharacterShootingController CharacterShootingController { get { return (characterShootingController == null) ? characterShootingController = GetComponentInParent<CharacterShootingController>() : characterShootingController; } }

    private CharacterSettings characterSettings;
    public CharacterSettings CharacterSettings { get { return (characterSettings == null) ? characterSettings = GetComponentInParent<CharacterSettings>() : characterSettings; } }

    public LineRenderer LineRenderer;

    private LineRenderer instantiatedLineRenderer;

    public AudioClip AudioClip;

    private void OnEnable()
    {
        CharacterSettings.OnBulletFired += CreateBeam;
    }

    private void OnDisable()
    {
        CharacterSettings.OnBulletFired -= CreateBeam;
    }

    private void CreateBeam()
    {
        instantiatedLineRenderer = Instantiate(LineRenderer, transform.position, CharacterShootingController.GetDirection());
        AudioPoolManager.Instance.PlaySound(AudioClip, 0.25f);
        RaycastHit hit;
        Vector3 direction = CharacterShootingController.GetDirection() * Vector3.forward;
        int layerMask = LayerMask.GetMask("Wall");

        if (Physics.Raycast(transform.position, direction, out hit, 100.0f, layerMask))
        {
            Vector3 targetPosition = hit.point;

            instantiatedLineRenderer.SetPosition(1, targetPosition);
            instantiatedLineRenderer.SetPosition(0, transform.position);
        }

        int layerMask2 = LayerMask.GetMask("Enemy" , "Wall" , "Neutral");
        RaycastHit[] m_Results = Physics.RaycastAll(transform.position, direction, 200f, layerMask2);

        List<IDamageable> damageables = new List<IDamageable>();

        for (int i = 0; i < m_Results.Length; i++)
        {
            IDamageable damageable = m_Results[i].transform.gameObject.GetComponent<IDamageable>();
            if(damageable != null)
            {
                if(damageables.Contains(damageable) == false)
                {
                    damageables.Add(damageable);
                }
            }
        }

        foreach (IDamageable damageable in damageables)
        {
            if(CombatGameManager.Instance.CurrentRoom.BoxCollider.bounds.Contains(damageable.GetPosition()))
                damageable.TakeDamage(CharacterSettings.TemporaryDamage * CharacterSettings.DamageMultiplier);
        }
    }


}
