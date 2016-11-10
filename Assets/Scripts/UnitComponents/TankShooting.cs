using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Projectile shooting code.  Largely untouched from Tanks!  codebase.
/// </summary>
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(EntityAggro))]
[RequireComponent(typeof(Rigidbody))]
public class TankShooting : MonoBehaviour
{
    /// <summary>
    /// Does this entity need to be facing the other entity to attack?
    /// </summary>
    [SerializeField] private bool _isDirectional;

    /// <summary>
    /// The amount of recoil to give to this entity after firing a shot.
    /// </summary>
    [SerializeField] private float _projectileRecoil;

    /// <summary>
    /// The speed at which the projectile should be fired (in terms of the horizontal distance it will travel).
    /// </summary>
    [SerializeField] private float _secondsToFlyHorizontalMeter = 1 / 15f;

    /// <summary>
    /// Prefab of the shell.
    /// </summary>
    public Rigidbody _shell;                   

    /// <summary>
    /// A child of the tank where the shells are spawned.
    /// </summary>
    public Transform _fireTransform;

    /// <summary>
    /// Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    /// </summary>
    public AudioSource _shootingAudio;

    /// <summary>
    /// Audio that plays when each shot is fired.
    /// </summary>
    public AudioClip _fireClip;                
    
    private float _cooldownSeconds;
    private Entity _entity;
    private EntityAggro _aggro;
    private Rigidbody _rigidbody;

    void Awake()
    {
        _entity = GetComponent<Entity>();
        _aggro = GetComponent<EntityAggro>();
        _rigidbody = GetComponent<Rigidbody>();

        // EARLY OUT! //
        if(_entity == null || _aggro == null || _rigidbody == null)
        {
            Debug.LogWarning("TankShooting requires entity, aggro, and rigidbody.");
            return;
        }

        _entity.SpawnedEvent.AddListener(onSpawned);
    }

    private void onSpawned()
    {
        this.enabled = true;
    }

    private void Update ()
    {
        // Our code which does simple shooting AI on cooldown if we have a target.
        _cooldownSeconds -= Time.deltaTime;

        var aggroTarget = _aggro.Target;
        if(aggroTarget != null && _cooldownSeconds <= 0f)
        {
            float distance = Vector3.Distance(transform.position, aggroTarget.transform.position);

            if (distance < _entity.AttackRange)
            {
                if (_aggro.IsInSights(aggroTarget.transform, _isDirectional))
                {
                    _cooldownSeconds = _entity.AttackSpeed;

                    CombatUtils.FireProjectile(_entity, 
                        _shell,
                        _fireTransform, 
                        aggroTarget.transform.position,
                        _secondsToFlyHorizontalMeter);

                    // Apply recoil to this entity for firing.
                    if(_projectileRecoil > 0f)
                    {
                        _rigidbody.AddForce(-transform.forward * _projectileRecoil, ForceMode.VelocityChange);
                    }

                    if(_shootingAudio != null)
                    {
                        // Change the clip to the firing clip and play it.
                        _shootingAudio.clip = _fireClip;
                        _shootingAudio.Play ();
                    }
                }
            }
        }
    }
}