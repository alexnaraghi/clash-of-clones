using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a single target, direct damage, always-hit projectile.
/// </summary>
public class Bullet : MonoBehaviour 
{
    [SerializeField] private Entity _target;
    [SerializeField] private float _speedMetersPerSecond;
    [SerializeField] private int _directDamage;

    /// <summary>
    /// The time in seconds before the bullet is removed if it misses.
    /// </summary>
    [SerializeField] private float _maxLifeSeconds = 1f;

    void Start()
    {
        this.Invoke(hit, _maxLifeSeconds);
    }

    public void Init(Entity target, float speedMetersPerSecond, int directDamage)
    {
        Assert.IsNotNull(target);
        
        _target = target;
        _speedMetersPerSecond = speedMetersPerSecond;
        _directDamage = directDamage;
    }

    void Update()
    {
        if(_target != null)
        {
            Vector3 dir = _target.transform.position - transform.position;
            dir.Normalize();

            // Do a non-physics lock on movement.  If the projectile is fast/small enough it shouldn't
            // appear to be too "homing".
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.position += dir * _speedMetersPerSecond * Time.deltaTime;
        }
    }

    void OnDestroy()
    {
        CancelInvoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_target != null && other.gameObject == _target.gameObject)
        {
            hit();
        }
    }

    private void hit()
    {
        _target.TakeDamage(_directDamage);
        Destroy(gameObject);
    }
}