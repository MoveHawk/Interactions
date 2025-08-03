using UnityEngine;

public class UIIndicator2 : MonoBehaviour
{
    public Transform player;                  // Assign your player here
    public Camera mainCamera;                // Assign main camera if needed

    [Header("Fade Settings")]
    public float fadeStartDistance = 3f;
    public float fadeEndDistance = 10f;
    public float fadeSpeed = 2f;

    private SpriteRenderer spriteRenderer;
    private float originalXRotation;
    private float originalZRotation;

    private void Start()
    {
        if (!mainCamera) mainCamera = Camera.main;
        if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;

        spriteRenderer = GetComponent<SpriteRenderer>();

        // Store the original X and Z rotation angles
        Vector3 euler = transform.eulerAngles;
        originalXRotation = euler.x;
        originalZRotation = euler.z;
    }

    private void Update()
    {
        if (!player || !spriteRenderer) return;

        // ---- 1. Rotate only around Y axis to face the camera ----
        Vector3 toCamera = mainCamera.transform.position - transform.position;
        toCamera.y = 0f; // Ignore vertical difference
        if (toCamera.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(toCamera);
            Vector3 euler = lookRotation.eulerAngles;

            // Apply Y rotation from lookRotation, keep original X and Z
            transform.rotation = Quaternion.Euler(originalXRotation, euler.y, originalZRotation);
        }

        // ---- 2. Fade based on player distance ----
        float distance = Vector3.Distance(transform.position, player.position);
        float targetAlpha = Mathf.InverseLerp(fadeStartDistance, fadeEndDistance, distance);

        Color color = spriteRenderer.color;
        color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * fadeSpeed);
        spriteRenderer.color = color;
    }
}
