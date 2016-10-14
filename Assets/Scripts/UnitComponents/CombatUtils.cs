using UnityEngine;

public static class CombatUtils 
{
    public static Vector3 CalculateForceToHit(Vector3 source, Vector3 target)
    {
        return default(Vector3);
    }

    /// <summary>
    /// Gets the layer mask that covers all entities.
    /// </summary>
    public static LayerMask EntityMask
    {
        get
        {
            return 1 << 9;
        }
    }

    public static bool IsEntity(LayerMask targetLayer)
    {
        return ((1 << targetLayer) & EntityMask) > 0;
    }

    /// <summary>
    /// Is the collider an enemy?
    /// </summary>
    /// <param name="friendlyPlayer">The friendly player.</param>
    /// <param name="collider">The potential enemy collider.</param>
    /// <returns>True if the collider belongs to the enemy player, false otherwise.</returns>
    public static bool IsEnemy(PlayerModel friendlyPlayer, Collider collider)
    {
        bool isEnemy = false;

        if (collider != null)
        {

            Entity targetEntity = collider.GetComponent<Entity>();

            isEnemy = IsEnemy(friendlyPlayer, targetEntity);
        }
        return isEnemy;
    }

    /// <summary>
    /// Is the entity an enemy?
    /// </summary>
    /// <param name="friendlyPlayer">The friendly player.</param>
    /// <param name="collider">The potential enemy entity.</param>
    /// <returns>True if the entity belongs to the enemy player, false otherwise.</returns>
    public static bool IsEnemy(PlayerModel friendly, Entity entity)
    {
        bool isEnemy = false;

        // If there is no entity script attached to the gameobject, go on to the next collider.
        if (entity != null)
        {
            // If it's an enemy unit, do damage to it.
            if (entity.Owner != friendly)
            {
                isEnemy = true;
            }
        }
        return isEnemy;
    }
}