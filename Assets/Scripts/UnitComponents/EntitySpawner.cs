using UnityEngine;

/// <summary>
/// Spawner for cards that have multiple characters.
/// </summary>
public class EntitySpawner : MonoBehaviour 
{
    [SerializeField] private int _numEntities;
    [SerializeField] private float _spawnRadius;
    [SerializeField] private string _entityToSpawn;

    // do we destroy this entity after the first spawn activates?
    [SerializeField] private bool _destroyAfterFirstSpawn;

    // Should a spawned object spawn instantly?  If false it uses its default spawn timer.
    [SerializeField] private bool _shouldInstaSpawnChildren;

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
        _entity.SpawnedEvent.AddListener(onSpawned);
    }

    private void onInit()
    {
        _cooldownSeconds = _entity.ChildEntitySpawnSeconds;
        
        // If the cooldown is 0, immediately spawn.
        if(Mathf.Approximately(_entity.ChildEntitySpawnSeconds, 0f))
        {
            spawnUnits();
        }
    }

    private void onSpawned()
    {
        this.enabled = true;
    }

    void Update()
    {
        // EARLY OUT! //
        // If this spawner doesn't have a cooldown, it's a one time spawner so we shouldnt continue
        // to spawn.
        if(_entity == null || Mathf.Approximately(_entity.ChildEntitySpawnSeconds, 0f)) return;

        // Our code which does simple shooting AI on cooldown if we have a target.
        _cooldownSeconds -= Time.deltaTime;

        if(_cooldownSeconds <= 0f)
        {
            spawnUnits();
            _cooldownSeconds = _entity.ChildEntitySpawnSeconds;
        }
    }

    private void spawnUnits()
    {
        for(int i = 0; i < _numEntities; i++)
        {
            float angle = (((float)i) / _numEntities) * 360f;
            Vector3 pos = Utils.GetPointOnCircle(transform.position, _spawnRadius, angle);
            spawnUnitAtPosition(pos);
        }

        if(_destroyAfterFirstSpawn)
        {
            Destroy(gameObject);
        }
    }

    private void spawnUnitAtPosition(Vector3 position)
    {
        var card = Config.Instance.GetCardByName(_entityToSpawn);
        if(_shouldInstaSpawnChildren)
        {
            card.SpawnChargeSeconds = 0;
        }

        if(card != null)
        {
            var unit = Entity.SpawnFromDefinition(_entity.Owner, card, position, isFromPlayersHand: false);
            _entity.Owner.RotateForPlayer(unit.gameObject);
        }
        else
        {
            Debug.LogWarning("Can't find card: " + card.Name);
        }
    }
}
