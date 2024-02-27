using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSightController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public LineRenderer LineRenderer { get { return (lineRenderer == null) ? lineRenderer = GetComponent<LineRenderer>() : lineRenderer; } }

    private CharacterShootingController characterShootingController;
    public CharacterShootingController CharacterShootingController { get { return (characterShootingController == null) ? characterShootingController = GetComponentInParent<CharacterShootingController>() : characterShootingController; } }

    public float lerpSpeed = 5.0f; // Adjust the speed of interpolation

    void Update()
    {
        RaycastHit hit;

        // Convert quaternion to forward vector
        Vector3 direction = CharacterShootingController.GetDirection() * Vector3.forward;

        // Define the layer mask
        int layerMask = LayerMask.GetMask("Wall");

        if (Physics.Raycast(transform.position, direction, out hit, 100.0f, layerMask))
        {
            // Smoothly interpolate the positions
            Vector3 targetPosition = hit.point;
            Vector3 currentPosition = LineRenderer.GetPosition(1);
            Vector3 newPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * lerpSpeed);

            LineRenderer.SetPosition(1, newPosition);
            LineRenderer.SetPosition(0, transform.position);
        }
        else
        {
            // If the ray doesn't hit anything, set a default endpoint
            Vector3 targetPosition = transform.position + direction * 100.0f;
            Vector3 currentPosition = LineRenderer.GetPosition(1);
            Vector3 newPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * lerpSpeed);

            LineRenderer.SetPosition(1, newPosition);
            LineRenderer.SetPosition(0, transform.position);
        }
    }
}
