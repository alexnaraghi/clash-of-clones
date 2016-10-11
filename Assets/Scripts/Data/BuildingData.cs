using System;
using UnityEngine;

/// <summary>
/// Links a building entity with its territory.
/// </summary>
[Serializable]
public class BuildingData 
{
    public Entity Entity;
    public TerritoryChunkData Territory;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public BuildingData() { }

    public BuildingData(Entity entity, TerritoryChunkData territory)
    {
        Entity = entity;
        Territory = territory;
    }

    /// <summary>
    /// Returns true if the building is alive and the given position is in its territory.
    /// </summary>
    public bool IsInControlledTerritory(Vector3 position)
    {
        bool isControlled = false;

        // If the building is still alive, its territory is controlled.
        if(Entity != null && Entity.HP > 0)
        {
            if (Territory.IsInTerritoryChunk(position))
            {
                isControlled = true;
            }
        }

        return isControlled;
    }
}