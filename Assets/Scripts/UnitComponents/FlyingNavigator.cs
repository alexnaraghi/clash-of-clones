using UnityEngine;

/// <summary>
/// For flying entities, controls their movement using aggro to determine where to move next.
/// </summary>
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(EntityAggro))]
public class FlyingNavigator : MonoBehaviour, INavigable 
{
    [SerializeField] private bool _isNavigating;
    [SerializeField] private Vector3 _destination;

    private Entity _entity;
    private EntityAggro _aggro;

    void Awake()
    {
        _entity = GetComponent<Entity>();
        _aggro = GetComponent<EntityAggro>();

        // EARLY OUT! //
        if(_entity == null || _aggro == null)
        {
            Debug.LogWarning("Navigator requires entity, agent, and aggro");
            return;
        }

        _entity.SpawnedEvent.AddListener(onSpawned);
    }

    private void onSpawned()
    {
        this.enabled = true;
    }

    void Update()
    {
        CombatUtils.MoveToAggroTarget(_entity, _aggro.Target, this);

        if(_isNavigating)
        {
            Vector3 dir = (_destination - transform.position).normalized;
            Vector3 velocity = dir * _entity.Definition.MovementSpeed * Time.deltaTime;

            // Ignore Y position for flying units.
            velocity.y = 0f;
            
            transform.position += velocity;
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
    }

    /// <summary>
    /// Cancel the nav agent's movement.
    /// </summary>
    public void CancelMove()
    {
        _isNavigating = false;
    }
}