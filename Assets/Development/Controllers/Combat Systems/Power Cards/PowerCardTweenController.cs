using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PowerCardTweenController : MonoBehaviour
{
    public float rotationAngleLimit = 45f;
    CharacterMovementController characterMovementController;
    private void Update()
    {
        if (characterMovementController != null)
        {
            // Get the horizontal movement input from the player
            float horizontalInputx = characterMovementController.GetMovementDirection().x;
            float horizontalInputz = characterMovementController.GetMovementDirection().z;

            // Calculate the rotation angle based on the player's movement
            float rotationAnglex = Mathf.Clamp(horizontalInputx * rotationAngleLimit, -rotationAngleLimit, rotationAngleLimit);
            float rotationAnglez = Mathf.Clamp(horizontalInputz * rotationAngleLimit, -rotationAngleLimit, rotationAngleLimit);

            // Apply the rotation to the card on the X axis
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotationAnglex, 0f, rotationAnglez), Time.deltaTime * 3f);
        }
        else
        {
            characterMovementController = CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterMovementController>();
        }

    }
}
