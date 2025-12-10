using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public RectTransform gridRoot;
    public GameObject itemUIPrefab;
    [HideInInspector] public InventoryCore core;

    bool isOpen;
    InventoryItemUI draggedUI;
    InventoryCore.GridItem draggedCoreItem;

    // ----------------------------
    // Grid Size (INSPECTOR CONTROLLED)
    // ----------------------------
    [Header("Grid Size")]
    public int gridWidth = 10;
    public int gridHeight = 6;

    // ----------------------------
    // Grid Renderer Settings
    // ----------------------------
    [Header("Grid Visuals")]
    public Color gridLineColor = Color.white;
    [Range(1f, 4f)] public float gridLineThickness = 1.5f;

    public float CellWidth => gridRoot.rect.width / core.width;
    public float CellHeight => gridRoot.rect.height / core.height;

    Texture2D gridTexture;
    Material gridMaterial;

    void Start()
    {
        // Use inspector values to define grid
        core = new InventoryCore(gridWidth, gridHeight);

        CreateGridMaterial();
        DrawGridLines();
    }

    void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
            Toggle();

        if (draggedCoreItem != null && Keyboard.current.rKey.wasPressedThisFrame)
            RotateDraggedItem();
    }

    void Toggle()
    {
        isOpen = !isOpen;
        panel.SetActive(isOpen);

        // Inform Player to lock/unlock mouse
        FindFirstObjectByType<Player>().SetInventoryState(isOpen);
        FindFirstObjectByType<CameraEffects>().SetInventoryState(isOpen);
    }


    // ---------------------------
    //  Public pickup API
    // ---------------------------
    public void AddItemFromPickup(ItemData data)
    {
        if (core.TryStack(data))
            return;

        var newItem = core.Create(data);

        if (core.TryAutoPlace(newItem))
            CreateUIItem(newItem);
        else
            Debug.Log("Inventory is full!");
    }

    void CreateUIItem(InventoryCore.GridItem item)
    {
        var ui = Instantiate(itemUIPrefab, gridRoot)
            .GetComponent<InventoryItemUI>();

        ui.Initialize(this, item);
    }

    // ---------------------------
    //  Dragging
    // ---------------------------
    public void BeginDrag(InventoryItemUI ui, InventoryCore.GridItem coreItem)
    {
        draggedUI = ui;
        draggedCoreItem = coreItem;
        core.Remove(coreItem);
    }

    public void Drag(PointerEventData e)
    {
        if (draggedUI == null) return;

        draggedUI.rect.anchoredPosition += e.delta / GetCanvasScale();
    }

    public void EndDrag()
    {
        if (draggedUI == null) return;

        Vector2 localPos = ScreenToGridPosition(Mouse.current.position.ReadValue());

        float cellW = gridRoot.rect.width / core.width;
        float cellH = gridRoot.rect.height / core.height;

        int cellX = Mathf.FloorToInt(localPos.x / cellW);
        int cellY = Mathf.FloorToInt(localPos.y / cellH);

        if (core.Place(draggedCoreItem, cellX, cellY))
        {
            draggedUI.SnapToGrid(cellX, cellY);
        }
        else
        {
            draggedUI.ReturnToOrigin();
        }

        draggedUI = null;
        draggedCoreItem = null;
    }

    void RotateDraggedItem()
    {
        core.Remove(draggedCoreItem);

        draggedCoreItem.rotation = draggedCoreItem.rotation == 0 ? 90 : 0;

        draggedUI.UpdateSize();

        float cw = CellWidth;
        float ch = CellHeight;

        draggedUI.rect.anchoredPosition = new Vector2(
            draggedCoreItem.posX * cw,
            draggedCoreItem.posY * ch
        );
    }


    float GetCanvasScale()
    {
        return gridRoot.GetComponentInParent<Canvas>().scaleFactor;
    }

    Vector2 ScreenToGridPosition(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridRoot, screenPos, null, out Vector2 localPos
        );

        return localPos;  // FIX → REMOVE offset
    }

    // ============================================================
    //  EMBEDDED GRID RENDERER (Auto cell-size, no manual pixels)
    // ============================================================

    void CreateGridMaterial()
    {
        gridTexture = new Texture2D(1, 1);
        gridTexture.SetPixel(0, 0, gridLineColor);
        gridTexture.Apply();

        gridMaterial = new Material(Shader.Find("UI/Default"));
        gridMaterial.mainTexture = gridTexture;
        gridMaterial.color = gridLineColor;
    }

    void DrawGridLines()
    {
        // Remove previous lines
        for (int i = gridRoot.childCount - 1; i >= 0; i--)
        {
            Transform child = gridRoot.GetChild(i);
            if (child.name.StartsWith("[GridLine]"))
                Destroy(child.gameObject);
        }

        if (core == null) return;

        // Compute actual cell sizes from the UI panel
        float cellWidth = gridRoot.rect.width / core.width;
        float cellHeight = gridRoot.rect.height / core.height;

        // Draw vertical lines
        for (int x = 0; x <= core.width; x++)
        {
            float xPos = x * cellWidth;

            DrawLine(
                new Vector2(xPos, 0),
                new Vector2(xPos, gridRoot.rect.height)
            );
        }

        // Draw horizontal lines
        for (int y = 0; y <= core.height; y++)
        {
            float yPos = y * cellHeight;

            DrawLine(
                new Vector2(0, yPos),
                new Vector2(gridRoot.rect.width, yPos)
            );
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        GameObject go = new GameObject("[GridLine]");
        go.transform.SetParent(gridRoot, false);

        var img = go.AddComponent<Image>();
        img.material = gridMaterial;

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;

        Vector2 direction = end - start;
        float length = direction.magnitude;

        rect.sizeDelta = new Vector2(length, gridLineThickness);
        rect.anchoredPosition = start + direction / 2f;
        rect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    // Redraw on UI resize
    private void OnRectTransformDimensionsChange()
    {
        if (isActiveAndEnabled)
            DrawGridLines();
    }
}
