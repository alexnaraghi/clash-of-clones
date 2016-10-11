using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Handles the tank movement.  Largely untouched from Tanks! codebase.
/// TODO: Looks like we need to pull out the audio/effects and put it into the navigator component, this
/// code does nothing useful anymore.
/// </summary>
public class TankMovement : MonoBehaviour
{       
    /// <summary>
    /// How fast the tank moves forward and back.
    /// </summary>
    [SerializeField] private float _speed = 12f;                 

    /// <summary>
    /// How fast the tank turns in degrees per second.
    /// </summary>
    [SerializeField] private float _turnSpeed = 180f;            

    /// <summary>
    /// Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    /// </summary>
    [SerializeField] private AudioSource _movementAudio;         

    /// <summary>
    /// Audio to play when the tank isn't moving.
    /// </summary>
    [SerializeField] private AudioClip _engineIdling;            

    /// <summary>
    /// Audio to play when the tank is moving.
    /// </summary>
    [SerializeField] private AudioClip _engineDriving;           

    /// <summary>
    /// The amount by which the pitch of the engine noises can vary.
    /// </summary>
    [SerializeField] private float _pitchRange = 0.2f;           

    /// <summary>
    /// Reference used to move the tank.
    /// </summary>
    private Rigidbody _rigidbody;              

    /// <summary>
    /// The current value of the movement input.
    /// </summary>
    private float _movementInputValue;         

    /// <summary>
    /// The current value of the turn input.
    /// </summary>
    private float _turnInputValue;             

    /// <summary>
    /// The pitch of the audio source at the start of the scene.
    /// </summary>
    private float _originalPitch;              

    private void Awake ()
    {
        _rigidbody = GetComponent<Rigidbody> ();
        Assert.IsNotNull(_rigidbody);
    }


    private void OnEnable ()
    {
        // When the tank is turned on, make sure it's not kinematic.
        _rigidbody.isKinematic = false;

        // Also reset the input values.
        _movementInputValue = 0f;
        _turnInputValue = 0f;
    }


    private void OnDisable ()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        _rigidbody.isKinematic = true;
    }


    private void Start ()
    {
        // Store the original pitch of the audio source.
        _originalPitch = _movementAudio.pitch;
    }


    private void Update ()
    {
        EngineAudio ();
    }


    private void EngineAudio ()
    {
        // If there is no input (the tank is stationary)...
        if (Mathf.Abs (_movementInputValue) < 0.1f && Mathf.Abs (_turnInputValue) < 0.1f)
        {
            // ... and if the audio source is currently playing the driving clip...
            if (_movementAudio.clip == _engineDriving)
            {
                // ... change the clip to idling and play it.
                _movementAudio.clip = _engineIdling;
                _movementAudio.pitch = Random.Range (_originalPitch - _pitchRange, _originalPitch + _pitchRange);
                _movementAudio.Play ();
            }
        }
        else
        {
            // Otherwise if the tank is moving and if the idling clip is currently playing...
            if (_movementAudio.clip == _engineIdling)
            {
                // ... change the clip to driving and play.
                _movementAudio.clip = _engineDriving;
                _movementAudio.pitch = Random.Range(_originalPitch - _pitchRange, _originalPitch + _pitchRange);
                _movementAudio.Play();
            }
        }
    }


    private void FixedUpdate ()
    {
        // Adjust the rigidbodies position and orientation in FixedUpdate.
        Move ();
        Turn ();
    }


    private void Move ()
    {
        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        Vector3 movement = transform.forward * _movementInputValue * _speed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        _rigidbody.MovePosition(_rigidbody.position + movement);
    }


    private void Turn ()
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = _turnInputValue * _turnSpeed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        _rigidbody.MoveRotation (_rigidbody.rotation * turnRotation);
    }
}