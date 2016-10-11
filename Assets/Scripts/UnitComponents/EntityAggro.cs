using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

/// <summary>
/// Component that manages aggro on entities.  Who is this entity targeting?
/// </summary>
[RequireComponent(typeof(Entity))]
public class EntityAggro : MonoBehaviour 
{
    public Entity Target
    {
        get { return _target; }
    }
    [SerializeField]private Entity _target;

    private Entity _entity;

    void Awake()
    {
        _entity = GetComponent<Entity>();
        Assert.IsNotNull(_entity);
    }

    void Update()
    {
        // Clear out dead or invalid aggro target.
        if(_target != null && _target.HP <= 0)
        {
            _target = null;
        }

        // If no aggro target, find one.
        if (_target == null)
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
                Assert.IsNotNull(closestEnemy);

                _target = closestEnemy;
            }
        }
    }

    // Question, will this be sufficient for flying enemies?  Or do we want a box check?
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