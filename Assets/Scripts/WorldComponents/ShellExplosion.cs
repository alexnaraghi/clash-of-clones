using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a projectile explosion.  Damages enemy entities with an area effect.
/// </summary>
public class ShellExplosion : MonoBehaviour
{
    /// <summary>
    /// Used to filter what the projectile can collide with.
    /// </summary>
    [SerializeField] private LayerMask _impactMask;

    /// <summary>
    /// Used to filter what the explosion does damage to. 
    /// </summary>
    [SerializeField] private LayerMask _damageMask;

    /// <summary>
    /// The time in seconds before the shell is removed.
    /// </summary>
    [SerializeField] private float _maxLifeTime = 2f;

    /// <summary>
    /// The maximum distance away from the explosion tanks can be and are still affected.
    /// </summary>
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private ParticleSystem _explosionParticles;
    [SerializeField] private AudioSource _explosionAudio;              

    private PlayerModel _owner;
    private int _areaDamage;
    
    private void Start ()
    {
        Assert.IsNotNull(_explosionParticles);
        Assert.IsNotNull(_explosionAudio);

        // If it isn't destroyed by then, destroy the shell after it's lifetime.
        Destroy (gameObject, _maxLifeTime);
    }

    public void Init(PlayerModel owner, int damage)
    {
        _owner = owner;
        _areaDamage = damage;
    }

    private void OnTriggerEnter (Collider other)
    {
        bool isImpactLayer = ((1 << other.gameObject.layer) & _impactMask) == 0;
        
        // EARLY OUT! //
        // If the trigger isn't on an impact layer, ignore it.
        if(isImpactLayer) return;

        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        Collider[] colliders = Physics.OverlapSphere (transform.position, _explosionRadius, _damageMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Entity targetEntity = colliders[i].GetComponent<Entity> ();

            // If there is no entity script attached to the gameobject, go on to the next collider.
            if (targetEntity != null)
            {
                // If it's an enemy unit, do damage to it.
                if(targetEntity.Owner != _owner)
                {
                    targetEntity.TakeDamage(_areaDamage);
                }
            }
        }

        // Unparent the particles from the shell.
        _explosionParticles.transform.parent = null;

        // Play the particle system.
        _explosionParticles.Play();

        // Play the explosion sound effect.
        _explosionAudio.Play();

        // Once the particles have finished, destroy the gameobject they are on.
        Destroy (_explosionParticles.gameObject, _explosionParticles.duration);

        // Destroy the shell.
        Destroy (gameObject);
    }
}