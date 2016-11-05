using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// A projectile that spawns another game object on collision.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile: MonoBehaviour, IProjectile
{
    /// <summary>
    /// The time in seconds before the projectile is removed.
    /// </summary>
    [SerializeField] private float _maxLifeTime = 10f;
    [SerializeField] private string _entityToSpawn;

    private Entity _creator;
    private CardData _spawnDefinition;
    private Rigidbody _rigidbody;
    private bool _isCollided;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(_rigidbody);

        // If it isn't destroyed by then, destroy the shell after it's lifetime.
        Destroy (gameObject, _maxLifeTime);
    }

    public void Init(Entity creator)
    {
        _creator = creator;
    }

    void Update()
    {
        // Point the projectile to its direction of movement.
        if(_rigidbody != null && !Mathf.Approximately(_rigidbody.velocity.sqrMagnitude, 0f))
        {
            transform.forward = _rigidbody.velocity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // EARLY OUT! //
        // If the trigger isn't on an impact layer, ignore it.
        if(_isCollided || !CombatUtils.IsProjectileCollider(other.gameObject.layer)) return;

        // EARLY OUT! //
        // If the collider is a friendly entity, early out.
        if(CombatUtils.IsEntity(other.gameObject.layer) && !CombatUtils.IsEnemy(_creator.Owner, other)) return;
        
        _isCollided = true;

        var card = Config.Instance.GetCardByName(_entityToSpawn);
        card.SpawnChargeSeconds = 0;

        if(card != null)
        {
            var unit = Entity.SpawnFromDefinition(_creator.Owner, card, transform.position, isFromPlayersHand: false);
            _creator.Owner.RotateForPlayer(unit.gameObject);
        }
        else
        {
            Debug.LogWarning("Can't find card: " + card.Name);
        }

        // Destroy the shell.
        Destroy (gameObject);
    }
}