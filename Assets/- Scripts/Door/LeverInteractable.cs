using UnityEngine;

public class LeverInteractable : MonoBehaviour, IInteractable
{
    [Header("Lever Settings")]
    [SerializeField] float leverRotateAngle = -45f;  // Rotate on Z
    [SerializeField] float leverSpeed = 3f;
    [SerializeField] DoorOpener connectedDoor;

    private bool isActivated = false;
    private bool isAnimating = false;
    private float startZ;
    private float targetZ;

    void Start()
    {
        startZ = transform.localEulerAngles.z;
        targetZ = startZ + leverRotateAngle;
    }

    public void PlayerInteracted()
    {
        if (!isAnimating && !isActivated)
            StartCoroutine(RotateLever());
    }

    System.Collections.IEnumerator RotateLever()
    {
        isAnimating = true;

        float start = transform.localEulerAngles.z;
        float end = targetZ;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * leverSpeed;

            float z = Mathf.LerpAngle(start, end, t);
            transform.localEulerAngles = new Vector3(
                transform.localEulerAngles.x,
                transform.localEulerAngles.y,
                z
            );

            yield return null;
        }

        isActivated = true;
        isAnimating = false;

        // Unlock the door!
        if (connectedDoor != null)
            connectedDoor.UnlockDoor();
    }
}
