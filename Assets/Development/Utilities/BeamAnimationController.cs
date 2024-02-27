using System.Collections;
using UnityEngine;

public class BeamAnimationController : MonoBehaviour
{
    [HideInInspector]
    public LineRenderer LineRenderer;
    public float FadeOutSpeed = 2f;
    private void Start()
    {
        LineRenderer = GetComponent<LineRenderer>();
        Animation();
    }

    private void Animation()
    {
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        Color startColor = LineRenderer.material.color;

        float alpha = 1.0f;

        while (alpha > 0.0f)
        {
            alpha -= Time.deltaTime * FadeOutSpeed;
            materialPropertyBlock.SetColor("_Color", new Color(startColor.r, startColor.g, startColor.b, alpha));
            LineRenderer.SetPropertyBlock(materialPropertyBlock);

            yield return null;
        }

        Destroy(gameObject);
    }
}
