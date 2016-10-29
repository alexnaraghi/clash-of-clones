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
    }

    void Update()
    {
        if (_entity.HP > 0)
        {
            _velocity = transform.position - _lastPosition;
            _lastPosition = transform.position;

            SetFloat("Velocity", _velocity.magnitude);
        }
    }

    public void Attack()
    {
        if(_entity.HP > 0)
        {
            SetTrigger("Attack");
            if(_attackAudio != null)
            {
                _attackAudio.Play();
            }
        }
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