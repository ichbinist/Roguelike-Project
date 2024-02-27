using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.Rendering.DebugUI;

public class Power_SlowMotion : BasePower
{
    public float Duration = 10f;
    [MaxValue(1f)]
    public float SlowMotionValue = 0.2f;

    public override void UsePower(Vector3 position)
    {
        base.UsePower(position);
        GameObject SlowMotionObject = new GameObject("Slow Motion Object");
        EffectObject effectObject = SlowMotionObject.AddComponent<EffectObject>();
        effectObject.Duration = Duration;
        effectObject.SlowMotionValue = SlowMotionValue;
        effectObject.Effect();
    }
}

partial class EffectObject : MonoBehaviour
{
    private Volume volume;
    private VolumeProfile volumeProfile;

    public float Duration = 10f;
    [MaxValue(1f)]
    public float SlowMotionValue = 0.2f;

    void Start()
    {
        volume = FindObjectOfType<Volume>();

        if (volume.profile == null)
        {
            Debug.LogError("No VolumeProfile assigned to the Volume component.");
            return;
        }

        volumeProfile = volume.profile;
    }
    public void Effect()
    {
        StartCoroutine(SlowMotionEffect(SlowMotionValue, Duration));
    }

    void ModifyPostProcessEffect(float value)
    {
        if (volume && volumeProfile && volumeProfile.TryGet(out ChromaticAberration chromaticAberration))
        {
            chromaticAberration.intensity.Override(value);
            //chromaticAberration.intensity.value = value;
        }
    }

    IEnumerator SlowMotionEffect(float value, float Duration)
    {
        float postValue = 0f;

        Time.timeScale = value;
        Time.fixedDeltaTime = value * 0.02f;

        while (postValue < 1f)
        {
            if (CombatGameManager.Instance.isPaused)
            {
                yield return null;
            }
            else
            {
                ModifyPostProcessEffect(postValue);
                postValue += Time.unscaledDeltaTime *2f;
                yield return null;
            }
        }

    Paused:
        if (!CombatGameManager.Instance.isPaused)
            yield return new WaitForSecondsRealtime(Duration);
        else
            goto Paused;
        while (postValue > 0f)
        {
            if (CombatGameManager.Instance.isPaused)
            {
                yield return null;
            }
            else
            {
                ModifyPostProcessEffect(postValue);
                postValue -= Time.unscaledDeltaTime * 2f;
                yield return null;
            }
        }

        ModifyPostProcessEffect(0f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        ModifyPostProcessEffect(0f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
}