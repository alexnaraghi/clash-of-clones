using UnityEngine;

/// <summary>
/// For moveable entities, controls their movement using aggro to determine where to move next.
/// </summary>
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(EntityAggro))]
[RequireComponent(typeof(NavMeshAgent))]
public class Navigator : MonoBehaviour, INavigable
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
        _entity.SpawnedEvent.AddListener(onSpawned);
    }

    private void onSpawned()
    {
        this.enabled = true;
    }

    private void init()
    {
        _agent.speed = _entity.MovementSpeed;
    }

    void Update()
    {
        if(_entity.IsSpawned)
        {
            _agent.speed = _entity.MovementSpeed;
            CombatUtils.MoveToAggroTarget(_entity, _aggro.Target, this);
        }
    }

    void FixedUpdate()
    {
        // If we aren't navigating but we have an aggro target, we should still rotate towards the target.
        // This is needed for targeting.  Should it be part of it's own component?  We'll see when we get
        // to static building defenses where this should go.
        if(_aggro.Target != null && !_isNavigating)
        {
            CombatUtils.RotateTowards(transform, _aggro.Target.transform);
        }
    }

    /// <summary>
    /// Set the nav agent's destination.
    /// </summary>
    public void MoveTo(Vector3 destination)
    {
        _isNavigating = true;
        _destination = destination;

        _agent.SetDestination(_destination);
        _agent.Resume();
    }

    /// <summary>
    /// Cancel the nav agent's movement.
    /// </summary>
    public void CancelMove()
    {
        _isNavigating = false;
        _agent.Stop();
    }
}