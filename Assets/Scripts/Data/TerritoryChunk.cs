using System;
using UnityEngine;

[Serializable]
public class TerritoryChunk
{
    public Territory[] Territories;

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