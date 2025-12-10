using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobSpeed = 7f;
    public float verticalBobAmount = 0.1f;
    public float horizontalBobAmount = 0.05f;
    public CharacterController playerController;

    [Header("Sway Settings")]
    public float swayAmount = 2f;
    public float swaySmoothing = 6f;

    private Vector3 originalLocalPos;
    private float timer;
    private float swayAngleZ;

    private Quaternion baseRotation; // stores camera rotation from mouse look
    private bool inventoryOpen = false;

    public void SetInventoryState(bool isOpen)
    {
        inventoryOpen = isOpen;
    }

    void Start()
    {
        originalLocalPos = transform.localPosition;
        baseRotation = transform.localRotation;
    }

    void Update()
    {
        if (inventoryOpen) return;   // ← disables bob + sway

        if (playerController == null) return;

        baseRotation = transform.localRotation;
        HandleHeadBob();
        HandleCameraSway();
    }


    private void HandleHeadBob()
    {
        Vector3 bobOffset = Vector3.zero;

        if (IsPlayerMoving())
        {
            timer += Time.deltaTime * bobSpeed;

            bobOffset.y = Mathf.Sin(timer) * verticalBobAmount;
            bobOffset.x = Mathf.Sin(timer * 0.5f) * horizontalBobAmount;
        }
        else
            timer = 0;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            originalLocalPos + bobOffset,
            Time.deltaTime * 10f
        );
    }

    private void HandleCameraSway()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");

        swayAngleZ = Mathf.Lerp(swayAngleZ, -mouseX * swayAmount, Time.deltaTime * swaySmoothing);

        // Reapply original X/Y rotation + add Z tilt only
        Quaternion tilt = Quaternion.Euler(0, 0, swayAngleZ);
        transform.localRotation = baseRotation * tilt;
    }

    private bool IsPlayerMoving()
    {
        return playerController.isGrounded &&
              (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f ||
               Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f);
    }
}
