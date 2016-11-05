using UnityEngine;

/// <summary>
/// Shoots a projectile from the owner's HQ to the position of this spell.
/// </summary>
public class CastProjectileSpell : MonoBehaviour 
{
    /// <summary>
    /// The speed at which the projectile should be fired (in terms of the horizontal distance it will travel).
    /// </summary>
    [SerializeField] private float _secondsToFlyHorizontalMeter = 1 / 15f;

    /// <summary>
    /// Prefab of the shell.
    /// </summary>
    [SerializeField] private Rigidbody _projectile;

    /// <summary>
    /// Audio that plays when each shot is fired.
    /// </summary>
    [SerializeField] private AudioClip _fireClip;

    /// <summary>
    /// Reference to the audio source used to play the shooting audio.
    /// </summary>
    [SerializeField] private AudioSource _shootingAudio;

    private Entity _entity;

    void Awake()
    {
        _entity = GetComponent<Entity>();

        // EARLY OUT! //
        if(_entity == null)
        {
            Debug.LogWarning("Requires entity.");
            return;
        }

        _entity.InitializedEvent.AddListener(onInit);
        _entity.SpawnedEvent.AddListener(onSpawned);
    }

    private void onSpawned()
    {
        this.enabled = true;
    }

    private void onInit()
    {
        // EARLY OUT! //
        if(_entity == null) return;
        
        var owner = _entity.Owner;
        
        if(owner != null && owner.HQ != null) 
        {
            var fireTransform = owner.HQ.transform.Find("FireTransform");

            if(fireTransform != null)
            {
                // Fire a projectile to this position.
                CombatUtils.FireProjectile(_entity, 
                    _projectile, 
                    fireTransform, 
                    transform.position, 
                    _secondsToFlyHorizontalMeter);

                // Change the clip to the firing clip and play it.
                _shootingAudio.clip = _fireClip;
                _shootingAudio.Play ();

                // The spell is complete.
                // TODO: Destroy.
            }
        }
    }
}