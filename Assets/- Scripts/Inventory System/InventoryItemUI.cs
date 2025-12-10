using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventoryUI ui;
    public InventoryCore.GridItem item;
    public RectTransform rect;

    Vector2 originalPos;

    public void Initialize(InventoryUI uiController, InventoryCore.GridItem data)
    {
        ui = uiController;
        item = data;
        rect = GetComponent<RectTransform>();

        GetComponent<Image>().sprite = item.data.icon;
        UpdateSize();
        SnapToGrid(item.posX, item.posY);
    }

    public void UpdateSize()
    {
        rect.sizeDelta = new Vector2(
            item.Width * ui.CellWidth,
            item.Height * ui.CellHeight
        );

        rect.localRotation = Quaternion.Euler(0, 0, item.rotation);
    }

    public void SnapToGrid(int x, int y)
    {
        rect.anchoredPosition = new Vector2(
            x * ui.CellWidth,
            y * ui.CellHeight
        );
    }


    public void ReturnToOrigin()
    {
        rect.anchoredPosition = originalPos;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        originalPos = rect.anchoredPosition;
        ui.BeginDrag(this, item);
    }

    public void OnDrag(PointerEventData e)
    {
        ui.Drag(e);
    }

    public void OnEndDrag(PointerEventData e)
    {
        ui.EndDrag();
    }
}
