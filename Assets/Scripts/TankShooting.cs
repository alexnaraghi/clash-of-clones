using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(EntityAggro))]
[RequireComponent(typeof(Rigidbody))]
public class TankShooting : MonoBehaviour
{
    public Rigidbody _shell;                   // Prefab of the shell.
    public Transform _fireTransform;           // A child of the tank where the shells are spawned.
    public AudioSource _shootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip _fireClip;                // Audio that plays when each shot is fired.
    public float _minLaunchForce = 15f;        // The force given to the shell if the fire button is not held.

    private float _cooldownSeconds;

    private Entity _entity;
    private EntityAggro _aggro;
    private Rigidbody _rigidbody;

    // OLD STUFF FROM TANKS GAME //
    public Slider _aimSlider;                  // A child of the tank that displays the current launch force.
    public AudioClip _chargingClip;            // Audio that plays when each shot is charging up.
    public float _maxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
    public float _maxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.
    private string _fireButton;                // The input axis that is used for launching shells.
    private float _currentLaunchForce;         // The force that will be given to the shell when the fire button is released.
    private float _chargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool _fired;                       // Whether or not the shell has been launched with this button press.
    ///////////////////////////////

    void Awake()
    {
        _entity = GetComponent<Entity>();
        _aggro = GetComponent<EntityAggro>();
        _rigidbody = GetComponent<Rigidbody>();

        // EARLY OUT! //
        if(_entity == null || _aggro == null || _rigidbody == null) return;

        _entity.InitializedEvent.AddListener(init);
    }

    private void init()
    {
        // When the tank is turned on, reset the launch force and the UI
        _currentLaunchForce = _minLaunchForce;
        _aimSlider.value = _minLaunchForce;
    }


    private void Start ()
    {
        // The fire axis is based on the player number.
        _fireButton = "Fire" + 1;

        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        _chargeSpeed = (_maxLaunchForce - _minLaunchForce) / _maxChargeTime;
    }


    private void Update ()
    {
        TEST_updateShootFromInput();

        _cooldownSeconds -= Time.deltaTime;

        var aggroTarget = _aggro.Target;
        if(aggroTarget != null && _cooldownSeconds <= 0f)
        {
            // Fire a shot if we are close enough and pointing approximately at the target.
            var distance = Vector3.Distance(transform.position, aggroTarget.transform.position);

            var ourDirection = transform.forward.normalized;
            var targetDirection = (aggroTarget.transform.position - transform.position).normalized;
            var dot = Vector3.Dot(ourDirection, targetDirection);

            bool isInSights = Mathf.Abs(dot) >  (1f - Consts.directionThreshholdForProjectileShot);

            if (distance < _entity.Definition.AttackRange && isInSights)
            {
                _cooldownSeconds = _entity.Definition.AttackSpeed;
                fire();
            }
        }
    }

    private void fire()
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
            _rigidbody.AddForce(-transform.forward * 10f, ForceMode.VelocityChange);
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
            fire ();
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
            fire ();
        }
    }
}