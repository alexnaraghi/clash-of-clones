using UnityEngine;

public static class CombatUtils 
{
    /// <summary>
    /// Gets the velocity vector to hit target from source, using a time scaled by the distance so further
    /// objects take a proportional amount of time for the projectile to reach them.
    /// </summary>
    /// <param name="source">The position of the source object.</param>
    /// <param name="target">The position of the target object.</param>
    /// <param name="secondsPerHorizonalMeter">The number of seconds it should take the projectile to travel 1
    /// meter on the horizontal plane.</param>
    /// <returns>The velocity to apply to the object (before applying mass).</returns>
    public static Vector3 CalculateVelocityToHit(Vector3 source, Vector3 target, float secondsPerHorizonalMeter)
    {
        // NOTE: This calculation is to hit the target at the same y level as the source, will not work
        // for slopes.  Improve it to take into account y displacement betweeen objects. 

        // Reduce the equation to two dimensions, we can solve for vertical and horizontal velocities.
        // Horizontal velocity can then be reapplied to the direction on the Y plane to expand back to 3 
        // dimensions.
        Vector3 displacement = target - source;
        Vector3 displacementOnYPlane = new Vector3(displacement.x, 0f, displacement.z);
        float distOnYPlane = displacementOnYPlane.magnitude;
        Vector3 dirOnYPlane = displacementOnYPlane.normalized;

        // Solve for duration.
        float durationSeconds = secondsPerHorizonalMeter * distOnYPlane;

        // Solve for the horizontal and vertical velocities.
        float g = -Physics.gravity.y;
        float vH = distOnYPlane / durationSeconds;
        float vV = (target.y + (0.5f * g * durationSeconds * durationSeconds) - source.y) / durationSeconds;

        // Apply the velocities to their direction vectors.
        Vector3 vX = dirOnYPlane * vH;
        Vector3 vY = Vector3.up * vV;

        // Sum the velocity.
        Vector3 velocity = vX + vY;
        return velocity;
    }

    public static void FireProjectile(Entity creator, Rigidbody projectilePrefab, Transform fireTransform, 
        Vector3 destination, float secondsToFlyHorizontalMeter)
    {
        // EARLY OUT! //
        if(creator == null || projectilePrefab == null || fireTransform == null)
        {
            Debug.LogWarning("Requires creator, projectilePrefab, fireTransform");
            return;
        }

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody projectileRigidbody =
            GameObject.Instantiate (projectilePrefab, fireTransform.position, fireTransform.rotation) as Rigidbody;

        Vector3 velocity = CombatUtils.CalculateVelocityToHit(
            fireTransform.position, 
            destination, 
            secondsToFlyHorizontalMeter);

        projectileRigidbody.AddForce(velocity, ForceMode.VelocityChange);

        var projectile = projectileRigidbody.gameObject.GetInterface<IProjectile>();
        if(projectile != null)
        {
            projectile.Init(creator);
        }
        else
        {
            Debug.LogWarning("Problem with the projectile setup.");
        }
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

    /// <summary>
    /// Gets the layer mask that projectiles should explode when colliding with.
    /// </summary>
    public static LayerMask ProjectileMask
    {
        get
        {
            return (1 << 8) | (1 << 9);
        }
    }

    public static bool IsEntity(LayerMask targetLayer)
    {
        return ((1 << targetLayer) & EntityMask) > 0;
    }

    public static bool IsProjectileCollider(LayerMask targetLayer)
    {
        return ((1 << targetLayer) & ProjectileMask) > 0;
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
    
    public static void MoveToAggroTarget(Entity entity, Entity target, INavigable navigator)
    {
        if(target == null)
        {
            var closestEnemyBuilding = GetClosestEnemyBuilding(entity);
            if(closestEnemyBuilding != null)
            {
                navigator.MoveTo(closestEnemyBuilding.transform.position);
            }
            else
            {
                navigator.CancelMove();
            }
        }
        else
        {
            var targetPositionIgnoreY = target.transform.position;
            targetPositionIgnoreY.y = entity.transform.position.y;

            // At this point, if we have an aggro target, move to attack it.
            var distance = Vector3.Distance(entity.transform.position, targetPositionIgnoreY);
            if(distance < entity.Definition.AttackRange)
            {
                navigator.CancelMove();
                // Attack!
            }
            else
            {
                navigator.MoveTo(target.transform.position);
            }
        }
    }

    public static Entity GetClosestEnemyBuilding(Entity entity)
    {
        var enemy = GameModel.Instance.GetOppositePlayer(entity.Owner);

        // EARLY OUT! //
        if(enemy == null) return null;

        Entity closestEnemyBuilding = null;
        float closestDist = float.MaxValue;

        // Find the closest structure.
        foreach(var building in enemy.Buildings)
        {
            var buildingEntity = building.Entity;
            if(buildingEntity != null && buildingEntity.HP > 0)
            {
                var dist = Vector3.Distance(entity.transform.position, buildingEntity.transform.position);
                if (dist < closestDist)
                {
                    closestEnemyBuilding = buildingEntity;
                    closestDist = dist;
                }
            }
        }

        return closestEnemyBuilding;
    }

    public static void RotateTowards(Transform transform, Transform target)
    {
        float rotationSpeed = 2f;

        Vector3 direction = (target.position - transform.position).normalized;

        // flatten the vector3
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed);
    }
}