using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class PowerCardViewRoomController : Singleton<PowerCardViewRoomController>
{
    public PowerCardController PowerCardController;
    public WeaponCardController WeaponCardController;

    public List<Camera> Cameras = new List<Camera>();

    [Button]
    public void CardEnter(CardType CardType)
    {
        switch (CardType)
        {
            case CardType.Power:
                OpenCameras(true);
                DOTween.Kill(PowerCardController.transform);
                PowerCardController.transform.DOLocalJump(Vector3.zero, 0f, 1, 0.45f).SetEase(Ease.Linear);
                //PowerCardController.transform.DOLocalRotate(new Vector3(0f, 0f, 720f), 0.85f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
                break;
            case CardType.Weapon:
                OpenCameras(false);
                DOTween.Kill(WeaponCardController.transform);
                WeaponCardController.transform.DOLocalJump(Vector3.zero, 0f, 1, 0.45f).SetEase(Ease.Linear);
                //WeaponCardController.transform.DOLocalRotate(new Vector3(0f, 0f, 720f), 0.85f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
                break;
            default:
                break;
        }

    }

    private void OpenCameras(bool power)
    {
        if (power)
        {
            Cameras[0].enabled = true;
        }
        else
        {
            Cameras[1].enabled = true;
        }
    }

    private void CloseCameras(bool power)
    {
        if (power)
        {
            Cameras[0].enabled = false;
        }
        else
        {
            Cameras[1].enabled = false;
        }
    }

    [Button]
    public void CardExit(CardType CardType) 
    {
        switch (CardType)
        {
            case CardType.Power:
                DOTween.Kill(PowerCardController.transform);
                PowerCardController.transform.DOLocalJump(new Vector3(10f, 0f, 0f), 1.5f, 1, 0.45f).SetEase(Ease.Linear).OnComplete(()=> CloseCameras(true));
                //PowerCardController.transform.DOLocalRotate(new Vector3(0f, 0f, -720f), 0.85f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
                break;
            case CardType.Weapon:
                DOTween.Kill(WeaponCardController.transform);
                WeaponCardController.transform.DOLocalJump(new Vector3(10f, 0f, 0f), 1.5f, 1, 0.45f).SetEase(Ease.Linear).OnComplete(() => CloseCameras(false));
                //WeaponCardController.transform.DOLocalRotate(new Vector3(0f, 0f, -720f), 0.85f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
                break;
            default:
                break;
        }
    }
}

public enum CardType
{
    Power,
    Weapon
}