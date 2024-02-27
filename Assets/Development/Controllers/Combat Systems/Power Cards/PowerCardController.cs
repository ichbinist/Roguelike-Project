using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class PowerCardController : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public TextMeshProUGUI HeaderText, SpecialsText, DetailsText;

    private void Start()
    {
        transform.GetChild(0).DOLocalRotate(new Vector3(10f, 0f, 10f), 4)
            .SetLoops(-1, LoopType.Yoyo) // Yoyo for smooth back-and-forth rotation
            .SetEase(Ease.InOutQuad);
    }
}
