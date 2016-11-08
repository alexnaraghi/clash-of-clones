using System.Collections;
using UnityEngine;

/// <summary>
/// A bomb that explodes after a certain amount of time.
/// </summary>
public class TimedBomb : MonoBehaviour, IProjectile
{
    /// <summary>
    /// The time in seconds before the bomb goes off.
    /// </summary>
    [SerializeField] private float _lifetimeSeconds = 10f;

    /// <summary>
    /// The maximum distance away from the explosion tanks can be and are still affected.
    /// </summary>
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private int _areaDamage = 300;
    [SerializeField] private bool _hasSpawnClock;
    
    // Optional.
    [SerializeField] private ParticleSystem _explosionParticles;
    [SerializeField] private AudioSource _explosionAudio;            

    private PlayerModel _owner;

    void Start()
    {
        if(_hasSpawnClock)
        {
            var clockPrefab = Resources.Load<GameObject>(Consts.SpawnClockPrefabPath);
            if(clockPrefab != null)
            {
                var go = Instantiate(clockPrefab);
                if(go != null)
                {
                    var clock = go.GetComponent<SpawnClock>();
                    if(clock != null)
                    {
                        clock.Init(transform.position + Consts.SpawnClockOffset, _lifetimeSeconds);
                    }
                }
            }
        }
        StartCoroutine(loop());
    }

    private IEnumerator loop()
    {
        yield return new WaitForSeconds(_lifetimeSeconds);

        triggerBomb();
        Destroy (gameObject);
    }

    public void Init(Entity creator)
    {
        // EARLY OUT ! //
        if(creator == null) return;

        _owner = creator.Owner;
    }

    private void triggerBomb()
    {
        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        Collider[] colliders = Physics.OverlapSphere (transform.position, _explosionRadius, CombatUtils.EntityMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Entity targetEntity = colliders[i].GetComponent<Entity> ();
            
            // If it's an enemy unit, do damage to it.
            if(CombatUtils.IsEnemy(_owner, targetEntity))
            {
                targetEntity.TakeDamage(_areaDamage);
            }
        }

        if(_explosionParticles != null)
        {
            // Unparent the particles from the shell.
            _explosionParticles.transform.parent = null;

            // Play the particle system.
            _explosionParticles.Play();
            
            // Once the particles have finished, destroy the gameobject they are on.
            Destroy (_explosionParticles.gameObject, _explosionParticles.duration);
        }

        if(_explosionAudio != null)
        {
            // Play the explosion sound effect.
            _explosionAudio.Play();
        }

        // Destroy the shell.
        Destroy (gameObject);
    }
}