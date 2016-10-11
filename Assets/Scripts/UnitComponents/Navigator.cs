using UnityEngine;

/// <summary>
/// For moveable entities, controls their movement using aggro to determine where to move next.
/// </summary>
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(EntityAggro))]
[RequireComponent(typeof(NavMeshAgent))]
public class Navigator : MonoBehaviour 
{
    [SerializeField] private bool _isNavigating;
    [SerializeField] private Vector3 _destination;

    private NavMeshAgent _agent;
    private Entity _entity;
    private EntityAggro _aggro;

    void Awake()
    {
        _entity = GetComponent<Entity>();
        _agent = GetComponent<NavMeshAgent>();
        _aggro = GetComponent<EntityAggro>();

        // EARLY OUT! //
        if(_entity == null || _agent == null || _aggro == null)
        {
            Debug.LogWarning("Navigator requires entity, agent, and aggro");
            return;
        }

        _entity.InitializedEvent.AddListener(init);
    }

    private void init()
    {
        _agent.speed = _entity.Definition.MovementSpeed;
    }

    void Update()
    {
        if(_aggro.Target == null)
        {
            var closestEnemyBuilding = getClosestEnemyBuilding();
            if(closestEnemyBuilding != null)
            {
                moveTo(closestEnemyBuilding.transform.position);
            }
            else
            {
                cancelMove();
            }
        }
        else
        {
            // At this point, if we have an aggro target, move to attack it.
            var distance = Vector3.Distance(transform.position, _aggro.Target.transform.position);
            if(distance < _entity.Definition.AttackRange)
            {
                cancelMove();
                // Attack!
            }
            else
            {
                moveTo(_aggro.Target.transform.position);
            }
        }
    }

    void FixedUpdate()
    {
        // If we aren't navigating but we have an aggro target, we should still rotate towards the target.
        // This is needed for targeting.  Should it be part of it's own component?  We'll see when we get
        // to static building defenses where this should go.
        if(_aggro.Target != null && !_isNavigating)
        {
            rotateTowards(_aggro.Target.transform);
        }
    }

    private void rotateTowards(Transform target)
    {
        float rotationSpeed = 2f;

        Vector3 direction = (target.position - transform.position).normalized;

        // flatten the vector3
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed);
    }

    private Entity getClosestEnemyBuilding()
    {
        var enemy = GameModel.Instance.GetOppositePlayer(_entity.Owner);

        // EARLY OUT! //
        if(enemy == null) return null;

        Entity closestEnemyBuilding = null;
        float closestDist = float.MaxValue;

        // Find the closest structure.
        foreach(var building in enemy.Buildings)
        {
            var entity = building.Entity;
            if(entity != null && entity.HP > 0)
            {
                var dist = Vector3.Distance(transform.position, entity.transform.position);
                if (dist < closestDist)
                {
                    closestEnemyBuilding = entity;
                    closestDist = dist;
                }
            }
        }

        return closestEnemyBuilding;
    }

    /// <summary>
    /// Set the nav agent's destination.
    /// </summary>
    private void moveTo(Vector3 destination)
    {
        _isNavigating = true;
        _destination = destination;

        _agent.SetDestination(_destination);
        _agent.Resume();
    }

    /// <summary>
    /// Cancel the nav agent's movement.
    /// </summary>
    private void cancelMove()
    {
        _isNavigating = false;
        _agent.Stop();
    }
}