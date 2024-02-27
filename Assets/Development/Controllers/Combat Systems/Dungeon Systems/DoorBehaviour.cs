using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    private RoomSettings roomSettings;
    public RoomSettings RoomSettings { get { return (roomSettings == null) ? roomSettings = GetComponentInParent<RoomSettings>() : roomSettings; } }
    [FoldoutGroup("Sounds")]
    public AudioClip OpenDoorSound, CloseDoorSound;
    [FoldoutGroup("Statistics")]
    [ReadOnly]
    public bool IsDoorControlable = true; //If true, doors opens and closes automaticly when player gets close.

    private float doorMovementTarget = 0f;
    private bool isOpen = false;

    public void Start()
    {
        RoomSettings.Doors.Add(this);
    }

    private void Update()
    {
        if (IsDoorControlable)
        {
            AutomaticDoor();
        }
        else
        {
            CloseDoor();
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, doorMovementTarget, transform.localPosition.z), Time.deltaTime * RoomSettings.DoorMovementSpeed);
    }

    private void CloseDoor()
    {
        doorMovementTarget = 4f;
        if (isOpen)
        {
            isOpen = false;
            AudioPoolManager.Instance.PlaySound(CloseDoorSound);
        }
    }

    private void OpenDoor()
    {
        doorMovementTarget = -10f;
        if (!isOpen)
        {
            isOpen = true;
            AudioPoolManager.Instance.PlaySound(OpenDoorSound);
        }
    }

    private void AutomaticDoor()
    {
        if(GetDistance() <= RoomSettings.PlayerDetectionDistance && IsDoorControlable)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private float GetDistance()
    {
        float distance = 0f;
        distance = Vector3.Distance(new Vector3(transform.position.x,0f, transform.position.z), RoomSettings.PlayerCharacter.transform.position);
        return distance;
    }
}
