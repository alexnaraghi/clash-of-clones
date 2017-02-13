using System;
using System.Collections;
using System.Collections.Generic;
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

    /// <summary>
    /// The entity has been spawned.
    /// </summary>
    public UnityEvent SpawnedEvent;

    [SerializeField] private GameObject _deathEffectPrefab;

    private bool _isDead;

    private List<Effect> _activeEffects = new List<Effect>();

    /// <summary>
    /// The seconds we have been spawning for.
    /// </summary>
    private float _spawnSeconds;

    public bool IsSpawned
    {
        get;
        private set;
    }

    // TODO: Hide the definition!
    private CardData Definition 
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

    public float MovementSpeed
    {
        get
        {
            float baseSpeed = _definition.MovementSpeed;
            float accumulatedSpeed = baseSpeed;
            foreach(var effect in _activeEffects)
            {
                if(effect.Attribute == AttributeModifier.MovementSpeed)
                {
                    var modifier = baseSpeed * effect.Multiplier;
                    accumulatedSpeed += modifier;
                }
            }

            return accumulatedSpeed;
        }
    }

    public float AttackSpeed
    {
        get
        {
            float baseSpeed = _definition.AttackSpeed;
            float accumulatedSpeed = baseSpeed;
            foreach(var effect in _activeEffects)
            {
                if(effect.Attribute == AttributeModifier.AttackSpeed)
                {
                    var modifier = baseSpeed * effect.Multiplier;
                    accumulatedSpeed += modifier;
                }
            }

            return accumulatedSpeed;
        }
    }

    public bool AttacksGroundUnits      { get { return _definition.AttacksGroundUnits; } }
    public bool AttacksAirUnits         { get { return _definition.AttacksAirUnits; } }
    public bool IsAirUnit               { get { return _definition.IsAirUnit; } }
    public bool IsBuilding              { get { return _definition.IsBuilding; } }
    public bool IsProjectile            { get { return _definition.IsProjectile; } }
    public int  ChildEntitySpawnSeconds  { get { return _definition.ChildEntitySpawnSeconds; } }

    public int AggroRange
    {
        get
        {
            return _definition.AggroRange;
        }
    }

    public int DirectAttackDamage
    {
        get
        {
            return _definition.DirectAttackDamage;
        }
    }

    public int AreaAttackDamage
    {
        get
        {
            return _definition.AreaAttackDamage;
        }
    }

    public int AttackRange
    {
        get
        {
            return _definition.AttackRange;
        }
    }

    public void Init(PlayerModel owner, CardData definition, bool isFromPlayersHand)
    {
        // EARLY OUT! //
        if(owner == null || definition == null)
        {
            Debug.LogWarning("Need an owner and card to initialize an entity.");
            return;
        }

        _spawnSeconds = 0f;
        _owner = owner;
        _definition = definition;
        _hp = definition.StartHP;

        if(isFromPlayersHand && definition.SpawnChargeSeconds > 0)
        {
            var clockPrefab = Resources.Load<GameObject>(Consts.SpawnClockPrefabPath);
            if(clockPrefab != null)
            {
                var go = Utils.Instantiate(clockPrefab, SL.Get<GameModel>().BoardRoot.transform);
                if(go != null)
                {
                    var clock = go.GetComponent<SpawnClock>();
                    if(clock != null)
                    {
                        clock.Init(transform.position + Consts.SpawnClockOffset, _definition.SpawnChargeSeconds);
                    }
                }
            }
        }

        InitializedEvent.Invoke();
    }

    void Update()
    {
        if(!IsSpawned)
        {
            _spawnSeconds += Time.deltaTime;
            if(!_definition.WillWaitForSpawnCharge || _spawnSeconds > Definition.SpawnChargeSeconds)
            {
                IsSpawned = true;
                SpawnedEvent.Invoke();
            }
        }
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

    /// <summary>
    /// If the given effect is not applied to this entity already, apply it.
    /// </summary>
    public void ApplyEffect(Effect effect)
    {
        if(!_activeEffects.Contains(effect))
        {
            _activeEffects.Add(effect);
        }
    }

    public void RemoveEffect(Effect effect)
    {
        _activeEffects.Remove(effect);
    }

    private void die()
    {
        DiedEvent.Invoke();
        _owner.Units.Remove(this);

        if(_deathEffectPrefab != null)
        {
            var go = Utils.Instantiate(_deathEffectPrefab, SL.Get<GameModel>().BoardRoot.transform);
            go.transform.position = transform.position;

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
    /// <param name="isFromPlayersHand">Was the card played from the player's hand?  If not, it was spawned
    /// by some other entity/effect in the game.</param>
    /// <returns>The created entity.</returns>
    public static Entity SpawnFromDefinition(PlayerModel owner, CardData definition, Vector3 position, bool isFromPlayersHand)
    {
         var prefab = SL.Get<ResourceManager>().Load<GameObject>(Consts.UnitsPath + definition.PrefabName);
                             
        // EARLY OUT! //
        if(prefab == null)
        {
            Debug.LogWarning("Prefab not found: " + definition.Name);
            return null;
        }

        var spawnPosition = position;
        if(definition.IsAirUnit)
        {
            spawnPosition.y = Consts.AirUnitHeight;
        }
        var go = Utils.Instantiate(prefab, SL.Get<GameModel>().BoardRoot.transform);
        go.transform.position = spawnPosition;

        var entity = go.GetComponent<Entity>();

        // EARLY OUT! //
        if(entity == null)
        {
            Debug.LogWarning("Unit component not found on prefab: " + definition.Name);
            return null;
        }

        entity.Init(owner, definition, isFromPlayersHand);
        return entity;
    }
}