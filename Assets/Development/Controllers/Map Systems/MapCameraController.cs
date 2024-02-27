using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineVirtualCamera CinemachineVirtualCamera { get { return (cinemachineVirtualCamera == null) ? cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>() : cinemachineVirtualCamera; } }

    private CinemachineTransposer cinemachineTransposer;
    private CinemachineTransposer CinemachineTransposer { get { return (cinemachineTransposer == null) ? cinemachineTransposer = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>() : cinemachineTransposer; } }

    private Vector3 startingFollowVector;
    private Vector3 StartingFollowVector { get { return (startingFollowVector == Vector3.zero) ? startingFollowVector = CinemachineTransposer.m_FollowOffset : startingFollowVector; } }

    public Vector3 MaximumFollowOffset;
    public float SubmarineViewOffset = 0.2f;
    public GameObject ShipSubmarine_Body_Cut;
    private float lerpValue;
    private float target;
    private void Update()
    {
        lerpValue = Mathf.Clamp01(lerpValue - (Input.GetAxis("Mouse ScrollWheel") * 0.85f));

        target = Mathf.Lerp(target, lerpValue, Time.deltaTime * 2.5f);

        if (lerpValue < SubmarineViewOffset)
        {
            //Denizaltýnýn içi
            if(CombatGameManager.Instance.PlayerCharacter.MapState != MapState.Base)
            {
                CombatGameManager.Instance.PlayerCharacter.MapState = MapState.Base;
                ShipSubmarine_Body_Cut.SetActive(false);
            }
        }
        else
        {
            //Denizaltýnýn dýþý
            if (CombatGameManager.Instance.PlayerCharacter.MapState != MapState.Overview)
            {
                CombatGameManager.Instance.PlayerCharacter.MapState = MapState.Overview;
                InventoryPanelController.Instance.ClosePanel();
                ShipSubmarine_Body_Cut.SetActive(true);
            }
        }

        CinemachineTransposer.m_FollowOffset = Vector3.Lerp(StartingFollowVector, MaximumFollowOffset, target);
    }
}