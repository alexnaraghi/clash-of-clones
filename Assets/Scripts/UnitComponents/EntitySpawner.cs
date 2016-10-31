using UnityEngine;

/// <summary>
/// Spawner for cards that have multiple characters.
/// </summary>
public class EntitySpawner : MonoBehaviour 
{
    [SerializeField] private int _numEntities;
    [SerializeField] private float _spawnRadius;

    private float _cooldownSeconds;
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
    }

    private void onInit()
    {
        _cooldownSeconds = _entity.Definition.SpawnSeconds;
        
        // If the cooldown is 0, immediately spawn.
        if(Mathf.Approximately(_entity.Definition.SpawnSeconds, 0f))
        {
            spawnUnits();
        }
    }

    void Update()
    {
        // EARLY OUT! //
        // If this spawner doesn't have a cooldown, it's a one time spawner so we shouldnt continue
        // to spawn.
        if(_entity == null || Mathf.Approximately(_entity.Definition.SpawnSeconds, 0f)) return;

        // Our code which does simple shooting AI on cooldown if we have a target.
        _cooldownSeconds -= Time.deltaTime;

        if(_cooldownSeconds <= 0f)
        {
            spawnUnits();
            _cooldownSeconds = _entity.Definition.SpawnSeconds;
        }
    }

    private void spawnUnits()
    {
        for(int i = 0; i < _numEntities; i++)
        {
            float angle = (((float)i) / _numEntities) * 360f;
            Vector3 pos = Utils.GetPointOnCircle(transform.position, _spawnRadius, angle);

            var unit = Entity.SpawnFromDefinition(_entity.Owner, Config.Instance.GetCardByName("Skelly"), pos);
            _entity.Owner.RotateForPlayer(unit.gameObject);

            // TODO: Destroy this.
        }
    }

    
}
