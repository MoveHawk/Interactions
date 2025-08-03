using UnityEngine;

public class Items : MonoBehaviour, IInteractable
{
    public bool canPickUp = true;
    public AudioSource itemSound;

    [SerializeField][HideInInspector] public Vector3 itemPositionDeviation = new Vector3(0, 0, 0);
    [SerializeField][HideInInspector] public Vector3 itemRotationDeviation = new Vector3(0, 0, 0);

    [Header("Item Info")]
    public string itemName;
    [TextArea] public string itemDescription;

    private Rigidbody rb;
    private Collider coll;
    private Player player;

    private Renderer itemRenderer;
    private Material[] originalMaterials;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        player = FindFirstObjectByType<Player>();

        itemRenderer = GetComponent<Renderer>();
        if (itemRenderer != null)
        {
            originalMaterials = itemRenderer.materials;
        }
    }

    public void PlayerInteracted()
    {
        if (player.isHandsFree && canPickUp)
        {
            rb.isKinematic = true;
            coll.isTrigger = true;
            transform.SetParent(player.itemContainer);
            transform.localPosition = itemPositionDeviation;
            transform.localRotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(itemRotationDeviation);
            player.isHandsFree = false;

            if (itemSound != null)
                itemSound.Play();

            // Remove glow material (assumed to be the second one)
            if (itemRenderer != null && itemRenderer.materials.Length > 1)
            {
                Material[] mats = itemRenderer.materials;
                Material[] trimmed = new Material[1];
                trimmed[0] = mats[0]; // Keep only the default material
                itemRenderer.materials = trimmed;
            }
        }
    }

    void OnTransformParentChanged()
    {
        // Re-add the glow material when the item is dropped or unequipped
        if (transform.parent != player.itemContainer && originalMaterials != null && originalMaterials.Length > 1)
        {
            itemRenderer.materials = originalMaterials;
        }
    }
}
