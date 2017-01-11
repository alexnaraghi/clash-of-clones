using UnityEngine;

public enum EventTrigger
{
    Spawn,
    DamageTaken,
    Death
}


/// <summary>
/// Spawns a game object on a trigger event.
/// </summary>
[RequireComponent(typeof(Entity))]
public class SpawnOnEvent : MonoBehaviour 
{
    [SerializeField] private TimedBomb _prefabToSpawn;
    [SerializeField] private EventTrigger _triggerReason;

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

        switch(_triggerReason)
        {
            case EventTrigger.Spawn:
                _entity.SpawnedEvent.AddListener(onEventTriggered);
                break;
            case EventTrigger.DamageTaken:
                _entity.DamageTakenEvent.AddListener(onEventTriggered);
            break;
            case EventTrigger.Death:
                _entity.DiedEvent.AddListener(onEventTriggered);
            break;
            default:
                Debug.LogWarning("Unknown event trigger: " + _triggerReason);
                break;
        }
    }

    private void onEventTriggered()
    {
        if(_prefabToSpawn != null)
        {
            var go = Utils.Instantiate(_prefabToSpawn, SL.Get<GameModel>().BoardRoot.transform);
            go.transform.position = transform.position;
            go.Init(_entity);
        }
    }
}