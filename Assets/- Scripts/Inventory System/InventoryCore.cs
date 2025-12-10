using NUnit.Framework.Interfaces;
using System;
using UnityEngine;

[Serializable]
public class InventoryCore
{
    [Serializable]
    public class GridItem
    {
        public ItemData data;
        public int posX;
        public int posY;
        public int rotation; // 0 or 90
        public int currentStack = 1;

        public int Width => rotation == 0 ? data.sizeX : data.sizeY;
        public int Height => rotation == 0 ? data.sizeY : data.sizeX;
    }

    public int width;
    public int height;
    public GridItem[,] cells;

    public InventoryCore(int w, int h)
    {
        width = w;
        height = h;
        cells = new GridItem[w, h];
    }

    // --------------------------
    //  Placement / Collision
    // --------------------------
    public bool CanPlace(GridItem item, int startX, int startY)
    {
        for (int x = 0; x < item.Width; x++)
            for (int y = 0; y < item.Height; y++)
            {
                int gx = startX + x;
                int gy = startY + y;

                if (gx < 0 || gy < 0 || gx >= width || gy >= height)
                    return false;

                if (cells[gx, gy] != null)
                    return false;
            }

        return true;
    }

    public bool Place(GridItem item, int x, int y)
    {
        if (!CanPlace(item, x, y))
            return false;

        item.posX = x;
        item.posY = y;

        for (int i = 0; i < item.Width; i++)
            for (int j = 0; j < item.Height; j++)
                cells[x + i, y + j] = item;

        return true;
    }

    public void Remove(GridItem item)
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (cells[x, y] == item)
                    cells[x, y] = null;
    }

    // --------------------------
    //  Auto Placement
    // --------------------------
    public bool TryAutoPlace(GridItem item)
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (CanPlace(item, x, y))
                {
                    Place(item, x, y);
                    return true;
                }

        return false;
    }

    // --------------------------
    //  Stacking
    // --------------------------
    public bool TryStack(ItemData data)
    {
        if (!data.stackable)
            return false;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                GridItem i = cells[x, y];

                if (i != null && i.data == data)
                {
                    if (i.currentStack < data.maxStack)
                    {
                        i.currentStack++;
                        return true;
                    }
                }
            }
        return false;
    }

    // --------------------------
    //  Create New Grid Item
    // --------------------------
    public GridItem Create(ItemData data)
    {
        return new GridItem
        {
            data = data,
            currentStack = 1,
            rotation = 0
        };
    }
}
