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
        if(_entity == null || _agent == null || _aggro == null) return;

        _entity.InitializedEvent.AddListener(init);
    }

    private void init()
    {
        _agent.speed = _entity.Definition.MovementSpeed;
    }

    public void Init(float speed)
    {
        _agent.speed = speed;
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
        var enemy = GameState.Instance.GetOppositePlayer(_entity.Owner);

        Entity closestEnemyBuilding = null;
        float closestDist = float.MaxValue;

        // Check structures
        PickIfCloser(enemy.TopOutpost,    ref closestEnemyBuilding, ref closestDist);
        PickIfCloser(enemy.HQ,            ref closestEnemyBuilding, ref closestDist);
        PickIfCloser(enemy.BottomOutpost, ref closestEnemyBuilding, ref closestDist);

        return closestEnemyBuilding;
    }

    /// <summary>
    /// Helper to find the closest enemy building.
    /// </summary>
    /// <param name="entityToCheck">The transform to check.  If it's closer, sets the closestEnemyTransform to it.</param>
    /// <param name="closestEntity">The closest transform found so far.</param>
    /// <param name="nearestDist">The distance of the closest transform so far.</param>
    private void PickIfCloser(Entity entityToCheck, ref Entity closestEntity, ref float nearestDist)
    {
        if(entityToCheck.HP > 0)
        {
            var dist = Vector3.Distance(transform.position, entityToCheck.transform.position);
            if (dist < nearestDist)
            {
                closestEntity = entityToCheck;
                nearestDist = dist;
            }
        }
    }

    private void moveTo(Vector3 destination)
    {
        _isNavigating = true;
        _destination = destination;

        _agent.SetDestination(_destination);
        _agent.Resume();
    }

    private void cancelMove()
    {
        _isNavigating = false;
        _agent.Stop();
    }
}