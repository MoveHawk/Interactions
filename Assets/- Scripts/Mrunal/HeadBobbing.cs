using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobSpeed = 7f;
    public float verticalBobAmount = 0.1f;
    public float horizontalBobAmount = 0.05f;
    public CharacterController playerController;

    [Header("Stabilization")]
    public float stabilizationStrength = 12f; // Higher = more perfect center

    private Vector3 originalLocalPos;
    private float timer;
    private Vector3 bobOffset; // <- store offset separately

    private void Start()
    {
        originalLocalPos = transform.localPosition;
    }

    private void Update()
    {
        if (playerController == null) return;

        if (IsPlayerMoving())
        {
            timer += Time.deltaTime * bobSpeed;

            bobOffset.x = Mathf.Sin(timer * 0.5f) * horizontalBobAmount;
            bobOffset.y = Mathf.Sin(timer) * verticalBobAmount;
        }
        else
        {
            timer = 0;
            bobOffset = Vector3.zero; // Stop bob
        }

        // Smoothly apply bob on top of original position without drifting aim
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            originalLocalPos + bobOffset,
            Time.deltaTime * stabilizationStrength
        );
    }

    private bool IsPlayerMoving()
    {
        return playerController.isGrounded &&
               (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f ||
                Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f);
    }
}
