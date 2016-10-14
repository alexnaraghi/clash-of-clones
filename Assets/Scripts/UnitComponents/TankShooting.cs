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

    /// <summary>
    /// The force given to the shell if the fire button is not held.
    /// </summary>
    public float _minLaunchForce = 15f;   
    
    private float _cooldownSeconds;
    private Entity _entity;
    private EntityAggro _aggro;
    private Rigidbody _rigidbody;      

    // OLD STUFF FROM TANKS GAME //

    /// <summary>
    /// A child of the tank that displays the current launch force.
    /// </summary>
    public Slider _aimSlider;                  

    /// <summary>
    /// Audio that plays when each shot is charging up.
    /// </summary>
    public AudioClip _chargingClip;            

    /// <summary>
    /// The force given to the shell if the fire button is held for the max charge time.
    /// </summary>
    public float _maxLaunchForce = 30f;        

    /// <summary>
    /// How long the shell can charge for before it is fired at max force.
    /// </summary>
    public float _maxChargeTime = 0.75f;       

    /// <summary>
    /// The input axis that is used for launching shells.
    /// </summary>
    private string _fireButton;                

    /// <summary>
    /// The force that will be given to the shell when the fire button is released.
    /// </summary>
    private float _currentLaunchForce;         

    /// <summary>
    /// How fast the launch force increases, based on the max charge time.
    /// </summary>
    private float _chargeSpeed;                

    /// <summary>
    /// Whether or not the shell has been launched with this button press.
    /// </summary>
    private bool _fired;                      
    ///////////////////////////////

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
    }

    private void Start ()
    {
        // When the tank is turned on, reset the launch force and the UI
        _currentLaunchForce = _minLaunchForce;
        _aimSlider.value = _minLaunchForce;

        // The fire axis is based on the player number.
        _fireButton = "Fire" + 1;

        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        _chargeSpeed = (_maxLaunchForce - _minLaunchForce) / _maxChargeTime;
    }


    private void Update ()
    {
        TEST_updateShootFromInput();

        // Our code which does simple shooting AI on cooldown if we have a target.
        _cooldownSeconds -= Time.deltaTime;

        var aggroTarget = _aggro.Target;
        if(aggroTarget != null && _cooldownSeconds <= 0f)
        {
            float distance = Vector3.Distance(transform.position, aggroTarget.transform.position);

            if (distance < _entity.Definition.AttackRange)
            {
                if (_aggro.IsInSights(aggroTarget.transform, _isDirectional))
                {
                    _cooldownSeconds = _entity.Definition.AttackSpeed;
                    fireProjectile();
                }
            }
        }
    }

    private void fireProjectile()
    {
        // Set the fired flag so only Fire is only called once.
        _fired = true;

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
            Instantiate (_shell, _fireTransform.position, _fireTransform.rotation) as Rigidbody;

        // TODO: Make the launch force correspond to where we want the projectile to hit!!!!
        // Set the shell's velocity to the launch force in the fire position's forward direction.
        shellInstance.velocity = _currentLaunchForce * _fireTransform.forward;

        var explosion = shellInstance.GetComponent<ShellExplosion>();
        var entity = GetComponent<Entity>();

        if(explosion != null && entity != null)
        {
            explosion.Init(entity.Owner, _entity.Definition.AreaAttackDamage);

            // Apply recoil to this entity for firing.
            if(_projectileRecoil > 0f)
            {
                _rigidbody.AddForce(-transform.forward * _projectileRecoil, ForceMode.VelocityChange);
            }
        }
        else
        {
            Debug.LogWarning("Problem with the shell setup.");
        }

        // Change the clip to the firing clip and play it.
        _shootingAudio.clip = _fireClip;
        _shootingAudio.Play ();

        // Reset the launch force.  This is a precaution in case of missing button events.
        _currentLaunchForce = _minLaunchForce;
    }

    /// <summary>
    /// TODO: Remove this, it's from the old game (but helpful for debugging so maybe keep?)
    /// </summary>
    void TEST_updateShootFromInput()
    {
        // The slider should have a default value of the minimum launch force.
        _aimSlider.value = _minLaunchForce;

        // If the max force has been exceeded and the shell hasn't yet been launched...
        if (_currentLaunchForce >= _maxLaunchForce && !_fired)
        {
            // ... use the max force and launch the shell.
            _currentLaunchForce = _maxLaunchForce;
            fireProjectile ();
        }
        // Otherwise, if the fire button has just started being pressed...
        else if (Input.GetButtonDown (_fireButton))
        {
            // ... reset the fired flag and reset the launch force.
            _fired = false;
            _currentLaunchForce = _minLaunchForce;

            // Change the clip to the charging clip and start it playing.
            _shootingAudio.clip = _chargingClip;
            _shootingAudio.Play ();
        }
        // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
        else if (Input.GetButton (_fireButton) && !_fired)
        {
            // Increment the launch force and update the slider.
            _currentLaunchForce += _chargeSpeed * Time.deltaTime;

            _aimSlider.value = _currentLaunchForce;
        }
        // Otherwise, if the fire button is released and the shell hasn't been launched yet...
        else if (Input.GetButtonUp (_fireButton) && !_fired)
        {
            // ... launch the shell.
            fireProjectile ();
        }
    }
}