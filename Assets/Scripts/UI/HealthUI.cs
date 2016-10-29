using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Gui for a slider based health display.
/// </summary> 
[RequireComponent(typeof(Entity))]
public class HealthUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _fillImage;

    // A lot of this explosion/death stuff is part of the original Tanks! codebase.
    // I'd like to see it moved to its own component, it doesn't have much to do with the health display.

    /// <summary>
    /// Optional.  A prefab that will be instantiated in Awake, then used whenever the tank dies.
    /// </summary>
    [SerializeField] private GameObject _explosionPrefab;

    private AudioSource _explosionAudio;
    private ParticleSystem _explosionParticles;

    /// <summary>
    /// Has the tank been reduced beyond zero health yet?
    /// </summary>
    private bool _dead;                                

    private Entity _entity;

    private void Awake()
    {
        if(_explosionPrefab != null)
        {
            // Instantiate the explosion prefab and get a reference to the particle system on it.
            _explosionParticles = Instantiate(_explosionPrefab).GetComponent<ParticleSystem>();
        
            // Get a reference to the audio source on the instantiated prefab.
            _explosionAudio = _explosionParticles.GetComponent<AudioSource>();

            // Disable the prefab so it can be activated when it's required.
            _explosionParticles.gameObject.SetActive(false);
        }

        _entity = GetComponent<Entity>();
        Assert.IsNotNull(_entity);

        _entity.InitializedEvent.AddListener(init);
    }

    public void init()
    {
        _entity.DamageTakenEvent.AddListener(onDamageTaken);
        _slider.maxValue = _entity.MaxHP;
        _fillImage.color = _entity.Owner.PlayerColor;

        SetHealthUI();
    }

    private void OnEnable()
    {
        // When the tank is enabled, reset the tank's health and whether or not it's dead.
        _dead = false;
    }


    private void onDamageTaken()
    {
        if (_entity != null)
        {
            // Change the UI elements appropriately.
            SetHealthUI();

            // If the current health is at or below zero and it has not yet been registered, call OnDeath.
            if (_entity.HP <= 0 && !_dead)
            {
                OnDeath();
            }
        }
    }


    private void SetHealthUI()
    {
        if (_entity != null)
        {
            // Set the slider's value appropriately.
            _slider.value = _entity.HP;
        }
    }


    private void OnDeath()
    {
        // Set the flag so that this function is only called once.
        _dead = true;

        if(_explosionParticles != null)
        {
            // Move the instantiated explosion prefab to the tank's position and turn it on.
            _explosionParticles.transform.position = transform.position;
            _explosionParticles.gameObject.SetActive(true);

            // Play the particle system of the tank exploding.
            _explosionParticles.Play();
        }

        if(_explosionAudio)
        {
            // Play the tank explosion sound effect.
            _explosionAudio.Play();
        }

        // Turn the tank off.
        gameObject.SetActive(false);
    }
}