using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealthController : CharacterHealthController
{
    [SerializeField, FoldoutGroup("Enemy Character References")] private Renderer characterRenderer;

    private void OnEnable()
    {
        onDamageTaken.AddListener(GlimpseBody);
    }

    private void OnDisable()
    {
        onDamageTaken.RemoveListener(GlimpseBody);
    }

    private void GlimpseBody()
    {
        StartCoroutine(GlimpsCoroutine());
    }

    IEnumerator GlimpsCoroutine()
    {
        materialPropertyBlock.SetColor("_BaseColor", Color.white);
        characterRenderer.SetPropertyBlock(materialPropertyBlock);
        yield return new WaitForSeconds(0.085f);
        materialPropertyBlock.SetColor("_BaseColor", Color.red);
        characterRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}