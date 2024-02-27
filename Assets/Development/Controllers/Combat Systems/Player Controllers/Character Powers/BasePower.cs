using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePower : MonoBehaviour
{
    public AudioClip PowerSound;
    public virtual void UsePower(Vector3 position)
    {
        AudioPoolManager.Instance.PlaySound(PowerSound);
    }
}
