using UnityEngine;

public class WaveEffectController : MonoBehaviour
{
    [SerializeField] private Renderer objectRenderer; // The renderer using the wave effect shader
    [SerializeField] private string fillPropertyName = "_Fill"; // Name of the float property in the shader

    [SerializeField] private float animationSpeed = 1f;

    private Material materialInstance;
    private float fillValue = 0f;
    private bool isAnimating = true;

    void Awake()
    {
        if (objectRenderer == null)
            objectRenderer = GetComponent<Renderer>();

        // Instantiate material to avoid changing shared material
        materialInstance = objectRenderer.material;
    }

    void Update()
    {
        if (!isAnimating) return;

        fillValue += animationSpeed * Time.deltaTime;
        if (fillValue > 1f)
            fillValue = 0f;

        materialInstance.SetFloat(fillPropertyName, fillValue);
    }

    public void EnableWaveEffect()
    {
        isAnimating = true;
        objectRenderer.enabled = true;
    }

    public void DisableWaveEffect()
    {
        isAnimating = false;
        objectRenderer.enabled = false;
    }
}
