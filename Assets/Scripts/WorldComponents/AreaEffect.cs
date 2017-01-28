using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A spell that has an area of effect and disappears at the end of its lifetime.
/// </summary>
[RequireComponent(typeof(Entity))]
public class AreaEffect : MonoBehaviour
{
    /// <summary>
    /// The time in seconds before the area of effect disappears.
    /// </summary>
    [SerializeField] private float _lifetimeSeconds = 12f;

    /// <summary>
    /// The maximum distance away from the explosion tanks can be and are still affected.
    /// </summary>
    [SerializeField] private float _effectRadius = 5f;

    // Do we need this?
    [SerializeField] private bool _hasSpawnClock;

    /// <summary>
    /// If true, this effect works on friendlies.  If false, it affects enemies.
    /// </summary>
    [SerializeField] private bool _isAffectingFriendlies;

    [SerializeField] private Effect[] _effects;

    private List<Entity> _affectedEntities = new List<Entity>();
    private HashSet<Entity> _workingSet = new HashSet<Entity>();

    private Entity _entity;

    void Awake()
    {
        _entity = GetComponent<Entity>();

        // EARLY OUT! //        
        if(Utils.DisabledFromMissingObject(_entity)) return;

        _entity.SpawnedEvent.AddListener(onSpawned);
    }

    private void onSpawned()
    {
        this.enabled = true;

        Destroy(gameObject, _lifetimeSeconds);
        StartCoroutine(loop());
    }

    void Start()
    {
        if(_hasSpawnClock)
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
                        clock.Init(transform.position + Consts.SpawnClockOffset, _lifetimeSeconds);
                    }
                }
            }
        }
    }

    private IEnumerator loop()
    {
        while(true)
        {
            yield return new WaitForSeconds(_entity.AttackSpeed);
            triggerEffect();
        }
    }

    private void triggerEffect()
    {
        Vector3 bottom, top;
        CombatUtils.GetCapsulePointsFromPosition(transform.position, out bottom, out top);

        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        Collider[] colliders = Physics.OverlapCapsule (bottom, top, _effectRadius, CombatUtils.EntityMask);
        _workingSet.Clear();

        for (int i = 0; i < colliders.Length; i++)
        {
            Entity targetEntity = colliders[i].GetComponent<Entity> ();
            
            // If it's an enemy unit, do damage to it.
            if(isAffectedEntity(_entity.Owner, targetEntity))
            {
                targetEntity.TakeDamage(_entity.AreaAttackDamage);

                if(!_affectedEntities.Contains(targetEntity))
                {
                    foreach(var effect in _effects)
                    {
                        _affectedEntities.Add(targetEntity);
                        targetEntity.ApplyEffect(effect);
                    }
                }

                _workingSet.Add(targetEntity);
            }
        }

        var oldList = _affectedEntities;
        _affectedEntities = new List<Entity>();
        foreach(var entity in oldList)
        {
            if(!_workingSet.Contains(entity))
            {
                foreach(var effect in _effects)
                {
                    entity.RemoveEffect(effect);
                }
            }
            else if(entity != null)
            {
                _affectedEntities.Add(entity);
            }
        }
    }

    /// <summary>
    /// Should this unit be affected by this spell?
    /// </summary>
    private bool isAffectedEntity(PlayerModel friendly, Entity target)
    {
        if(_isAffectingFriendlies)
        {
            return !CombatUtils.IsEnemy(friendly, target);
        }
        else
        {
            return CombatUtils.IsEnemy(friendly, target);
        }
    }

    void OnDestroy()
    {
        foreach(var entity in _affectedEntities)
        {
            if(entity != null)
            {
                foreach(var effect in _effects)
                {
                    entity.RemoveEffect(effect);
                }
            }
        }
        _affectedEntities.Clear();
    }
}