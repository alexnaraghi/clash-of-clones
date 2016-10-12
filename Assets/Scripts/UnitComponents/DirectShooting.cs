using UnityEngine;

/// <summary>
/// Represents an attack on a single target.  Projectiles ignore other enemies and always do direct damage.
/// For example, a bullet or an arrow.
/// </summary>
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(EntityAggro))]
public class DirectShooting : MonoBehaviour 
{
    private float _cooldownSeconds;
    private Entity _entity;
    private EntityAggro _aggro;

    public Bullet _bulletPrefab;

    /// <summary>
    /// A child of the entity where the bullets are spawned.
    /// </summary>
    public Transform _fireTransform;

    /// <summary>
    /// Reference to the audio source.
    /// </summary>
    public AudioSource _shootingAudio;

    void Awake()
    {
        _entity = GetComponent<Entity>();
        _aggro = GetComponent<EntityAggro>();

        // EARLY OUT! //
        if(_entity == null || _aggro == null || _bulletPrefab == null)
        {
            Debug.LogWarning("DirectShooting requires entity, aggro, and bulletPrefab.");
            return;
        }
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
            // Fire a shot if we are close enough and pointing approximately at the target.
            var distance = Vector3.Distance(transform.position, aggroTarget.transform.position);

            // TODO: How do we resolve static defenses with no direction versus targets that need to aim to
            // shoot?  Buildings (well, at least some of them) don't need to turn, they just fire in any
            // direction instantly.
            /*
            var ourDirection = transform.forward.normalized;
            var targetDirection = (aggroTarget.transform.position - transform.position).normalized;
            var dot = Vector3.Dot(ourDirection, targetDirection);

            bool isInSights = Mathf.Abs(dot) >  (1f - Consts.directionThreshholdForProjectileShot);
            */

            if (distance < _entity.Definition.AttackRange )//&& isInSights)
            {
                _cooldownSeconds = _entity.Definition.AttackSpeed;
                fire(aggroTarget);
            }
        }
    }

    private void fire(Entity target)
    {
        if(target != null)
        {
            var bullet = Instantiate(_bulletPrefab);
            bullet.transform.position = _fireTransform.position;

            if(bullet != null)
            {
                _shootingAudio.Play();
                bullet.Init(target, 100, _entity.Definition.DirectAttackDamage);
            }
            else
            {
                Debug.LogWarning("Bullet prefab couldn't be created");
            }
        }
    }
}