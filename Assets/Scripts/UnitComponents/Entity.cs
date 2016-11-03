using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents an in-game entity.
/// </summary>
/// <remarks>
/// TODO: Don't directly expose the card's definition, provide an access layer so we can pile on active effects
/// and modify traits.  For example, a freeze spell might reduce a unit's movement to 0, or a nearby unit
/// might buff our attack power.  Once a card is in-game, we don't care about its definition, we care about
/// its active traits post-buffs/debuffs.
/// </remarks>
public class Entity : MonoBehaviour 
{
    /// <summary>
    /// Event when damage is taken.
    /// </summary>
    public UnityEvent DamageTakenEvent;

    /// <summary>
    /// Event when unit dies.
    /// </summary>
    public UnityEvent DiedEvent;

    /// <summary>
    /// Event when entity is initialized.  All components relying on entity should initialize based off
    /// this event!
    /// </summary>
    public UnityEvent InitializedEvent;

    [SerializeField]private GameObject _deathEffectPrefab;

    private bool _isDead;

    // TODO: Hide the definition!
    public CardData Definition 
    { 
        get { return _definition; } 
    }
    [SerializeField] private CardData _definition;

    public PlayerModel Owner 
    { 
        get { return _owner; } 
    }
    [SerializeField] private PlayerModel _owner;

    public int HP 
    { 
        get { return _hp; } 
    }
    [SerializeField] private int _hp;

    public int MaxHP 
    { 
        get { return Definition.StartHP; } 
    }

    public void Init(PlayerModel owner, CardData definition)
    {
        // EARLY OUT! //
        if(owner == null || definition == null)
        {
            Debug.LogWarning("Need an owner and card to initialize an entity.");
            return;
        }
        
        _owner = owner;
        _definition = definition;
        _hp = definition.StartHP;

        InitializedEvent.Invoke();
    }

    public void TakeDamage(int damage)
    {
        if(Definition != null)
        {
            int oldHp = _hp;
            _hp = Mathf.Clamp(_hp - damage, 0, MaxHP);

            if(oldHp != _hp)
            {
                DamageTakenEvent.Invoke();
            }

            if(!_isDead && _hp == 0)
            {
                _isDead = true;
                die();
            }
        }
    }

    private void die()
    {
        DiedEvent.Invoke();
        _owner.Units.Remove(this);

        if(_deathEffectPrefab != null)
        {
            var go = (GameObject)Instantiate(_deathEffectPrefab, transform.position, Quaternion.identity);
            if(go != null)
            {
                Destroy(go, 1.5f);
            }
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Static initialization of an entity.
    /// </summary>
    /// <param name="owner">The owning player.  Recommend not writing, just reading from it
    /// to let <see cref="GameModel"/> be authoritative.</param>
    /// <param name="definition">The card's definition.</param>
    /// <param name="position">The spawn position.</param>
    /// <returns>The created entity.</returns>
    public static Entity SpawnFromDefinition(PlayerModel owner, CardData definition, Vector3 position)
    {
         var prefab = Resources.Load<GameObject>(Consts.UnitsPath + definition.PrefabName);
                             
        // EARLY OUT! //
        if(prefab == null)
        {
            Debug.LogWarning("Prefab not found: " + definition);
            return null;
        }

        var spawnPosition = position;
        if(definition.IsAirUnit)
        {
            spawnPosition.y = Consts.AirUnitHeight;
        }
        var go = (GameObject)Instantiate(prefab, spawnPosition, Quaternion.identity);
        var entity = go.GetComponent<Entity>();

        // EARLY OUT! //
        if(entity == null)
        {
            Debug.LogWarning("Unit component not found on prefab: " + definition);
            return null;
        }

        entity.Init(owner, definition);
        return entity;
    }
}