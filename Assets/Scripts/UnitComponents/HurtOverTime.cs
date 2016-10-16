using UnityEngine;

/// <summary>
/// Hurts an entity over time.
/// </summary>
public class HurtOverTime : MonoBehaviour 
{
    /// <summary>
    /// The number of seconds the entity it should take for it to die from its own damage.
    /// </summary>
    [SerializeField] int lifeSpanSeconds;


    // Need to maintain remainder for fractional damage (health is an integer value).
    private float _damageRemainder;
    private Entity _entity;
    private float _damagePerSecond;

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
    }

    private void onInit()
    {
        _damagePerSecond = ((float)_entity.MaxHP) / lifeSpanSeconds;
    }

    void Update()
    {
        if(!Mathf.Approximately(_damagePerSecond, 0f))
        {
            _damageRemainder += _damagePerSecond * Time.deltaTime;
            int damageAsInt = Mathf.FloorToInt(_damageRemainder);
            _damageRemainder -= damageAsInt;

            _entity.TakeDamage(damageAsInt);
        }
    }
}
