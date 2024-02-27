using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyCharacterMovementController : MonoBehaviour
{
    private EnemyCharacterSettings enemyCharacterSettings;
    public EnemyCharacterSettings EnemyCharacterSettings { get { return (enemyCharacterSettings == null) ? enemyCharacterSettings = GetComponent<EnemyCharacterSettings>() : enemyCharacterSettings; } }

    [FoldoutGroup("Movement Settings")]
    public float DestinationDetectionDamping = 0.1f;

    private Rigidbody _rigidbody;
    public Rigidbody _Rigidbody { get { return (_rigidbody == null) ? _rigidbody = GetComponent<Rigidbody>() : _rigidbody; } }

    private Transform graphics;
    public Transform Graphics { get { return (graphics == null) ? graphics = transform.GetChild(0) : graphics; } }

    protected Vector3 destination = Vector3.zero;

    protected virtual void OnEnable()
    {
        EnemyCharacterSettings.OnControlStateChanged.AddListener(ControlStateUpdate);
    }

    protected virtual void OnDisable()
    {
        EnemyCharacterSettings.OnControlStateChanged.RemoveListener(ControlStateUpdate);
    }

    protected virtual void ControlStateUpdate(bool state)
    {
        if (state)
        {
            ControlStarted();
        }
        else
        {
            ControlStopped();
        }
    }

    protected abstract void ControlStarted();
    protected abstract void ControlStopped();

    protected virtual Vector3 GetMovementDirection()
    {
        return (destination - transform.position).normalized;
    }

    protected virtual Quaternion GetDirection()
    {

        Vector3 direction = GetMovementDirection() - transform.position;
        direction.y = 0f;
        return Quaternion.LookRotation(direction);
    }

    protected void Movement()
    {
        _Rigidbody.velocity = GetMovementDirection() * EnemyCharacterSettings.MovementSpeed;
    }

    protected void Rotation()
    {
        if(!EnemyCharacterSettings.LockMovementRotation)
            Graphics.rotation = Quaternion.Lerp(Graphics.rotation, GetDirection(), Time.deltaTime * EnemyCharacterSettings.RotationSpeed);
    }

    protected virtual bool IsDestinationReached()
    {
        return Vector3.Distance(transform.position, destination) < DestinationDetectionDamping;
    }

    protected virtual Vector4 GetRoomDimensionsAsWorldPosition()
    {
        return EnemyCharacterSettings.RoomSettings.GetRoomDimensionsAsWorldPosition();
    }
    protected virtual Vector4 AdjustedRoomDimensions()
    {
        Vector4 adjustingValue = Vector4.one * 1.25f;
        return GetRoomDimensionsAsWorldPosition() - adjustingValue;
    }

    protected virtual Vector3 GetRandomTargetDestination()
    {
        Vector3 destination = Vector3.zero;
        destination = new Vector3(Random.Range(AdjustedRoomDimensions().x, AdjustedRoomDimensions().z), 0f, Random.Range(AdjustedRoomDimensions().y, AdjustedRoomDimensions().w));
        return destination;
    }

    protected virtual Vector3 GetPlayerTargetDestination()
    {
        return EnemyCharacterSettings.RoomSettings.PlayerCharacter.transform.position;
    }
}