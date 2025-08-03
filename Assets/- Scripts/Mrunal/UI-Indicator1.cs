using UnityEngine;

public class UIIndicator1 : MonoBehaviour
{
    public float bobbingSpeed = 2.0f;  // Speed of the up and down movement
    public float bobbingHeight = 0.5f;  // Height of the vertical movement
    public SpriteRenderer arrowRenderer;  // Reference to the arrow's SpriteRenderer
    private Vector3 initialPosition;  // Store the initial position of the arrow

    void Start()
    {
        // Store the initial position of the arrow
        initialPosition = transform.position;
    }

    void Update()
    {
        // Vertical Bobbing Movement
        float newY = initialPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);

       
        Color arrowColor = arrowRenderer.color;
        arrowRenderer.color = arrowColor;
    }
}
