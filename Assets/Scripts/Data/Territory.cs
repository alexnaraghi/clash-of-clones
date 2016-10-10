using System;
using UnityEngine;

/// <summary>
/// Definition of a territory square in cell coordinates.
/// </summary>
[Serializable]
public class Territory 
{
    public int StartX;
    public int StartY;
    public int Width;
    public int Height;

    public bool IsInTerritory(Vector3 position)
    {
        // The 3D position's z is the 2D position's Y.
        int gridX = (int)(position.x / Consts.GridCellWidth);
        int gridZ = (int)(position.z / Consts.GridCellHeight);

        return StartX <= gridX && gridX <= StartX + Width
            && StartY <= gridZ && gridZ <= StartY + Height;
    }
}