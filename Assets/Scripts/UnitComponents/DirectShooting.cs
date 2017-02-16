using UnityEngine;

/// <summary>
/// Represents an attack on a single target.  Projectiles ignore other enemies and always do direct damage.
/// For example, a bullet or an arrow.
/// </summary>
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(EntityAggro))]
public class DirectShooting : MonoBehaviour 
{
    [SerializeField] private Bullet _bulletPrefab;

    /// <summary>
    /// Does this entity need to be facing the other entity to attack?
    /// </summary>
    [SerializeField] private bool _isDirectional;

    /// <summary>
    /// A child of the entity where the bullets are spawned.
    /// </summary>
    [SerializeField] private Transform _fireTransform;

    /// <summary>
    /// Reference to the audio source.
    /// </summary>
    [SerializeField] private AudioSource _shootingAudio;

    [SerializeField] private float _speedMetersPerSecond = 100f;

    private float _cooldownSeconds;
    private Entity _entity;
    private EntityAggro _aggro;

    void Awake()
    {
        _entity = GetComponent<Entity>();
        _aggro = GetComponent<EntityAggro>();

        // EARLY OUT! //
        if(this.DisabledFromMissingObject(_entity, _aggro, _bulletPrefab, _fireTransform)) return;

        _entity.SpawnedEvent.AddListener(onSpawned);
    }

    private void onSpawned()
    {
        this.enabled = true;
    }

    // This looks a whole lot like tank shooting.
    // TODO: Reduce code duplication.
    private void Update()
    {
        // Our code which does simple shooting AI on cooldown if we have a target.
        _cooldownSeconds -= Time.deltaTime;

        var aggroTarget = _aggro.Target;
        if(aggroTarget != null && _cooldownSeconds <= 0f)
        {
            var targetPositionIgnoreY = aggroTarget.transform.position;
            targetPositionIgnoreY.y = transform.position.y;

            // Fire a shot if we are close enough and pointing approximately at the target.
            var distance = Vector3.Distance(transform.position, targetPositionIgnoreY);

            bool isInSights = _aggro.IsInSights(aggroTarget.transform, _isDirectional);
            if (distance < _entity.AttackRange && isInSights)
            {
                _cooldownSeconds = _entity.AttackSpeed;
                fire(aggroTarget);
            }
        }
    }

    private void fire(Entity target)
    {
        if(target != null)
        {
            var bullet = Utils.Instantiate(_bulletPrefab, SL.Get<GameModel>().BoardRoot.transform);
            bullet.transform.position = _fireTransform.position;

            if(bullet != null)
            {
                bullet.Init(target, _speedMetersPerSecond, _entity.DirectAttackDamage);

                // Play audio, if any.
                if(_shootingAudio != null)
                {
                    _shootingAudio.Play();
                }
            }
            else
            {
                Debug.LogWarning("Bullet prefab couldn't be created");
            }
        }
    }
}