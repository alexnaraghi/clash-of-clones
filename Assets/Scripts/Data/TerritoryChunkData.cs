using System;
using UnityEngine;

/// <summary>
/// Represents a chunk of one or more territory rectangles.
/// </summary>
[Serializable]
public class TerritoryChunkData
{
    public TerritoryData[] Territories;

    public bool IsInTerritoryChunk(Vector3 position)
    {
        bool isInChunk = false;
        if (Territories != null)
        { 
            foreach(var territory in Territories)
            {
                if(territory.IsInTerritory(position))
                {
                    isInChunk = true;
                    break;
                }
            }
        }
        return isInChunk;
    }
}