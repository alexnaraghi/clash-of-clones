using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Slider Slider;                             // The slider to represent how much health the tank currently has.
    public Image FillImage;                           // The image component of the slider.
    //public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
    //public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
    public GameObject ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.

    private AudioSource _explosionAudio;               // The audio source to play when the tank explodes.
    private ParticleSystem _explosionParticles;        // The particle system the will play when the tank is destroyed.
    private bool _dead;                                // Has the tank been reduced beyond zero health yet?

    private Entity _entity;

    private void Awake()
    {
        // Instantiate the explosion prefab and get a reference to the particle system on it.
        _explosionParticles = Instantiate(ExplosionPrefab).GetComponent<ParticleSystem>();

        // Get a reference to the audio source on the instantiated prefab.
        _explosionAudio = _explosionParticles.GetComponent<AudioSource>();

        // Disable the prefab so it can be activated when it's required.
        _explosionParticles.gameObject.SetActive(false);

        if (_entity == null)
        {
            _entity = GetComponent<Entity>();
        }
    }

    public void Init(Entity entity)
    {
        _entity = entity;
        _entity.DamageTakenEvent.AddListener(onDamageTaken);
        Slider.maxValue = entity.MaxHP;
        FillImage.color = entity.Owner.PlayerColor;

        SetHealthUI();
    }

    void Start()
    {
        var entity = GetComponent<Entity>();
        if (entity != null)
        {
            Init(entity);
        }
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
            Slider.value = _entity.HP;

            /*
            if(_entity.MaxHP != 0)
            {
                // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
                m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, _entity.HP / _entity.MaxHP);
            }
            */
        }
    }


    private void OnDeath()
    {
        // Set the flag so that this function is only called once.
        _dead = true;

        // Move the instantiated explosion prefab to the tank's position and turn it on.
        _explosionParticles.transform.position = transform.position;
        _explosionParticles.gameObject.SetActive(true);

        // Play the particle system of the tank exploding.
        _explosionParticles.Play();

        // Play the tank explosion sound effect.
        _explosionAudio.Play();

        // Turn the tank off.
        gameObject.SetActive(false);
    }
}