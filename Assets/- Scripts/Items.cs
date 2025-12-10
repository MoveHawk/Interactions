using UnityEngine;

public class Items : MonoBehaviour, IInteractable
{
    public bool canPickUp = true;
    public AudioSource itemAudioSource;
    public AudioClip itemPickupSound;

    [SerializeField][HideInInspector] public Vector3 itemPositionDeviation = new Vector3(0, 0, 0);
    [SerializeField][HideInInspector] public Vector3 itemRotationDeviation = new Vector3(0, 0, 0);

    [Header("Item Info")]
    public string itemName;
    [TextArea] public string itemDescription;
    public ItemData itemData2D;

    private Rigidbody rb;
    private Collider coll;
    private Player player;

    private Renderer itemRenderer;
    private Material[] originalMaterials;
    private InventoryUI inventoryUI;

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

        inventoryUI = FindFirstObjectByType<InventoryUI>();
    }

    public void PlayerInteracted()
    {
        //if (player.isHandsFree && canPickUp)
        //{
        //    rb.isKinematic = true;
        //    coll.isTrigger = true;
        //    transform.SetParent(player.itemContainer);
        //    transform.localPosition = itemPositionDeviation;
        //    transform.localRotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(itemRotationDeviation);
        //    player.isHandsFree = false;

        //    itemAudioSource.PlayOneShot(itemPickupSound);

        //    // Remove glow material (assumed to be the second one)
        //    if (itemRenderer != null && itemRenderer.materials.Length > 1)
        //    {
        //        Material[] mats = itemRenderer.materials;
        //        Material[] trimmed = new Material[1];
        //        trimmed[0] = mats[0]; // Keep only the default material
        //        itemRenderer.materials = trimmed;
        //    }
        //}
   
        if (player.isHandsFree && canPickUp)
        {
            // 1. Add item to inventory (auto-place, stack, rotate)
            inventoryUI.AddItemFromPickup(itemData2D);

            // 2. Destroy the 3D object (best performance)
            Destroy(gameObject);
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
