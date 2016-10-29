using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Entity))]
public class EntityAnimator : MonoBehaviour 
{
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _attackAudio;

    private Entity _entity;
    private Vector3 _velocity;
    private Vector3 _lastPosition;

    void Awake()
    {
        _entity = GetComponent<Entity>();

        // EARLY OUT! //
        if(_entity == null || _animator == null)
        {
            Debug.LogWarning("Requires entity, animator.");
            return;
        }

        _entity.DamageTakenEvent.AddListener(onTakenDamage);
    }

    void Update()
    {
        _velocity = transform.position - _lastPosition;
        _lastPosition = transform.position;

        SetFloat("Velocity", _velocity.magnitude);
    }

    public void Attack()
    {
        SetTrigger("Attack");
        if(_attackAudio != null)
        {
            _attackAudio.Play();
        }
    }

    private void onTakenDamage()
    {
        if(_entity.HP > 0)
        {
            TakeDamage();
        }
        else
        {
            Die();
        }
    }

    public void TakeDamage()
    {
        // Testing made it clear that the animations I have are far too exaggerated to make every time
        // we take damage.  In addition, melee units need to prioritize attack animations over taking damage.
        // Need to take a look at this again when the game is in a more polished state.
        //SetTrigger("GetHit");
    }

    public void Die()
    {
        SetTrigger("Die");
    }

    private void SetTrigger(string name)
    {
        if(_animator != null)
        {
            _animator.SetTrigger(name);
        }
    }

    private void SetFloat(string name, float value)
    {
        if(_animator != null)
        {
            _animator.SetFloat(name, value);
        }
    }
}