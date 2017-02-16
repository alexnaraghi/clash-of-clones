using UnityEngine;

public class Instant : MonoBehaviour 
{
    [SerializeField] private string _entityToSpawn;

    private Entity _entity;
    private CardData _spawnDefinition;
    private Rigidbody _rigidbody;
    private bool _isCollided;
    
    private void Start()
    {
        _entity = GetComponent<Entity>();

        // EARLY OUT! //        
        if(this.DisabledFromMissingObject(_entity)) return;

        _entity.InitializedEvent.AddListener(onInit);
        _entity.SpawnedEvent.AddListener(onSpawned);

    }

    private void onSpawned()
    {
        this.enabled = true;
    }

    private void onInit()
    {
        var card = SL.Get<Config>().GetCardByName(_entityToSpawn);
        if(card != null)
        {
            card.SpawnChargeSeconds = 0;
            var unit = Entity.SpawnFromDefinition(_entity.Owner, card, transform.position.ZeroY(), isFromPlayersHand: false);
            _entity.Owner.RotateForPlayer(unit.gameObject);
        }

        Destroy (gameObject);
    }
}