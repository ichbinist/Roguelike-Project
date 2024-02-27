using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionProbeRefresh : MonoBehaviour
{

    private ReflectionProbe reflectionProbe;
    private ReflectionProbe ReflectionProbe { get { return (reflectionProbe == null) ? reflectionProbe = GetComponent<ReflectionProbe>() : reflectionProbe; } }

    void LateUpdate()
    {
        ReflectionProbe.RenderProbe();
    }
}
