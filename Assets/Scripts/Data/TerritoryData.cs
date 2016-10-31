using System;
using UnityEngine;

// TODO: Refactor all grid calculations and such to somewhere more sensible.
public struct GridPoint
{
    public int X;
    public int Y;

    public GridPoint(int x, int y)
    {
        X = x;
        Y = y;
    }
}

/// <summary>
/// Definition of a territory rectangle in grid space.  Territory is area that a player controls and
/// therefore the other player cannot spawn units on.  It may also have other uses.
/// Area that is not convered by territory is neutral and both players may spawn units there.
/// </summary>
[Serializable]
public class TerritoryData 
{
    public int StartX;
    public int StartY;
    public int Width;
    public int Height;

    private static readonly int START_X = Mathf.FloorToInt(Consts.GridWidth / 2f);
    private static readonly int START_Y = Mathf.FloorToInt(Consts.GridHeight / 2f);
    private static readonly float START_WORLD_X = (0.5f + START_X) * Consts.GridCellWidth;
    private static readonly float START_WORLD_Y = (0.5f + START_Y) * Consts.GridCellHeight;

    public bool IsInTerritory(Vector3 position)
    {
        var GridPoint = GetGridPosition(position);

        return StartX <= GridPoint.X && GridPoint.X < StartX + Width
            && StartY <= GridPoint.Y && GridPoint.Y < StartY + Height;
    }

    public static GridPoint GetGridPosition(Vector3 position)
    {

        // The 3D position's z is the 2D position's Y.
        int gridX = START_X - Mathf.FloorToInt(0.5f + position.x / Consts.GridCellWidth);
        int gridZ = START_Y - Mathf.FloorToInt(0.5f + position.z / Consts.GridCellHeight);

        return new GridPoint(gridX, gridZ);
    }

    public static GridPoint GetGridPositionFromMouse()
    {
        GridPoint point = new GridPoint();

        var inputPosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        // If the position is on the ground
        if (Physics.Raycast(ray, out hit, int.MaxValue))
        {
            point = GetGridPosition(hit.point);
        }

        return point;
    }

    public static Vector3 GetCenter(int x, int y)
    {
        // 2D (x,y) coordinates get converted into 3d (x,z)
        float worldX = -(Consts.GridCellWidth * 0.5f) + START_WORLD_X - x * Consts.GridCellWidth;
        float worldZ = -(Consts.GridCellHeight) * 0.5f + START_WORLD_Y - y * Consts.GridCellHeight;

        
        Vector3 worldPosition = new Vector3(worldX, 0f, worldZ);
        return worldPosition;
    }

    /// <summary>
    /// Converts a territory definition into world space.
    /// </summary>
    /// <param name="territory">The territory to convert.</param>
    /// <returns>A rectangle in world space on the Y plane.</returns>
    public static Rect ToWorldRect(TerritoryData territory)
    {
        Rect rect = new Rect(
            territory.StartX * Consts.GridCellWidth,
            territory.StartY * Consts.GridCellHeight,
            territory.Width * Consts.GridCellWidth,
            territory.Height * Consts.GridCellHeight);

        return rect;
    }
}