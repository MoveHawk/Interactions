using UnityEngine;
using System;

public class CubeRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float rotationAngle = 90f;
    [SerializeField] float smoothRotationSpeed = 1f;

    [Header("Player Settings")]
    [SerializeField] Transform player;
    [SerializeField] float activationRange = 5f;
    [SerializeField] Camera playerCamera;

    [Header("Puzzle Settings")]
    [SerializeField] float correctRotationZ = 0f;

    private bool canRotate = false;
    private bool isLookingAtCube = false;
    private bool isRotating = false;
    private float currentRotationZ = 0f;

    public static event Action OnCubeRotated;

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        canRotate = distanceToPlayer <= activationRange;

        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            isLookingAtCube = hit.transform == transform;
        else
            isLookingAtCube = false;

        if (canRotate && isLookingAtCube && Input.GetKeyDown(KeyCode.E) && !isRotating)
            StartCoroutine(RotateCubeSmoothly());
    }

    System.Collections.IEnumerator RotateCubeSmoothly()
    {
        isRotating = true;

        float targetRotation = currentRotationZ + rotationAngle;

        while (Mathf.Abs(currentRotationZ - targetRotation) > 0.1f)
        {
            currentRotationZ = Mathf.MoveTowards(currentRotationZ, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, currentRotationZ);
            yield return null;
        }

        // FINALIZE ROTATION
        currentRotationZ = targetRotation;

        // 🔥 CRITICAL FIX: Normalize angle so it stays between 0–360
        currentRotationZ %= 360f;

        transform.rotation = Quaternion.Euler(0f, 0f, currentRotationZ);

        isRotating = false;

        OnCubeRotated?.Invoke();
    }

    public bool IsCorrectlyAligned()
    {
        return Mathf.Abs(Mathf.DeltaAngle(currentRotationZ, correctRotationZ)) < 1f;
    }
}
