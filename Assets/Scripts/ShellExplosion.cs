using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    // Used to filter what the projectile can collide with.
    public LayerMask _impactMask;

    // Used to filter what the explosion does damage to.
    public LayerMask _damageMask;                

    public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
    public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.
    public int AreaDamage = 30;                         // The amount of damage done if the explosion is centred on a tank.
    public float m_ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
    public float m_MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
    public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.

    private Player _owner;

    private void Start ()
    {
        // If it isn't destroyed by then, destroy the shell after it's lifetime.
        Destroy (gameObject, m_MaxLifeTime);
    }

    public void Init(Player owner, int damage)
    {
        _owner = owner;
        AreaDamage = damage;
    }

    private void OnTriggerEnter (Collider other)
    {
        bool isImpactLayer = ((1 << other.gameObject.layer) & _impactMask) == 0;
        
        // EARLY OUT! //
        // If the trigger isn't on an impact layer, ignore it.
        if(isImpactLayer) return;

        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, _damageMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Entity targetEntity = colliders[i].GetComponent<Entity> ();

            // If there is no entity script attached to the gameobject, go on to the next collider.
            if (targetEntity != null)
            {
                // If it's an enemy unit, do damage to it.
                if(targetEntity.Owner != _owner)
                {
                    targetEntity.TakeDamage(AreaDamage);
                }
            }
        }

        // Unparent the particles from the shell.
        m_ExplosionParticles.transform.parent = null;

        // Play the particle system.
        m_ExplosionParticles.Play();

        // Play the explosion sound effect.
        m_ExplosionAudio.Play();

        // Once the particles have finished, destroy the gameobject they are on.
        Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);

        // Destroy the shell.
        Destroy (gameObject);
    }
}