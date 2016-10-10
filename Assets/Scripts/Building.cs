using System;
using UnityEngine;

[Serializable]
public class Building 
{
    public Entity Entity;
    public TerritoryChunk Territory;

    public Building()
    {

    }

    public Building(Entity entity, TerritoryChunk territory)
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