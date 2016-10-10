using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Component that manages aggro on entities. 
/// </summary>
[RequireComponent(typeof(Entity))]
public class EntityAggro : MonoBehaviour 
{
    public Entity Target;

    private Entity _entity;

    void Awake()
    {
        _entity = GetComponent<Entity>();

        // EARLY OUT! //
        if(_entity == null) return;

        _entity.InitializedEvent.AddListener(init);
    }

    private void init()
    {
        
    }

    void Update()
    {
        // Clear out dead or invalid aggro target.
        if(Target != null && Target.HP <= 0)
        {
            Target = null;
        }

        // If no aggro target.
        if (Target == null)
        {
            var enemies = getAllEnemiesInRange(_entity.Definition.AggroRange);
            if (enemies.Length > 0)
            {
                Entity closestEnemy = null;
                float closestDistance = float.MaxValue;
                foreach (var enemy in enemies)
                {
                    if (enemy.HP > 0)
                    {
                        float distance = Vector3.Distance(transform.position, enemy.transform.position);
                        if (distance < closestDistance)
                        {
                            closestEnemy = enemy;
                            closestDistance = distance;
                        }
                    }
                }

                // Enemy logically can't be null since the array was greater than 0 in size.
                Target = closestEnemy;
            }
        }
    }

    private Entity[] getAllEnemiesInRange(float radius)
    {
        Collider[] allColliders = Physics.OverlapSphere(transform.position, radius);
        List<Entity> enemies = new List<Entity>();
        foreach(var collider in allColliders)
        {
            var entity = collider.GetComponent<Entity>();

            // If it's an enemy entity
            if(entity != null && entity.Owner != _entity.Owner)
            {
                enemies.Add(entity);
            }
        }
        return enemies.ToArray();
    }
}