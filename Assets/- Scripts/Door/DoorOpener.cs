using UnityEngine;

public class DoorOpener : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] bool isLocked = false;           // For lever-based or key-based doors
    [SerializeField] float openAngle = 90f;           // How far the door rotates
    [SerializeField] float openSpeed = 2f;            // How fast the door opens
    [SerializeField] bool openInPositiveDirection = true; // For left/right doors

    private bool isOpen = false;
    private bool isAnimating = false;
    private float startAngle;
    private float targetAngle;

    void Start()
    {
        startAngle = transform.localEulerAngles.y;
        targetAngle = startAngle + (openInPositiveDirection ? openAngle : -openAngle);
    }

    public void PlayerInteracted()
    {
        if (isLocked)
        {
            Debug.Log("Door is locked!");
            return;
        }

        if (!isAnimating)
            StartCoroutine(AnimateDoor());
    }

    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("Door unlocked!");
    }

    System.Collections.IEnumerator AnimateDoor()
    {
        isAnimating = true;

        float start = transform.localEulerAngles.y;
        float end = isOpen ? startAngle : targetAngle;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;

            float y = Mathf.LerpAngle(start, end, t);
            transform.localEulerAngles = new Vector3(0, y, 0);

            yield return null;
        }

        isOpen = !isOpen;
        isAnimating = false;
    }
}
