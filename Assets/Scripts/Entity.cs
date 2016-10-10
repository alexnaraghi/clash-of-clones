using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour 
{
    [SerializeField] private int _hp;

    public Player Owner;

    public UnityEvent DamageTakenEvent;
    public UnityEvent InitializedEvent;

    // TODO: Hide the definition, everything should go through methods so an entity can be modified at play
    // time with passives or whatever else the game does to it.
    public CardDefinition Definition;

    public int HP
    {
        get
        {
            return _hp;
        }
    }

    public int MaxHP
    {
        get
        {
            return Definition.StartHP;
        }
    }

    public void Init(Player owner, CardDefinition definition)
    {
        Owner = owner;
        Definition = definition;
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
        }
    }

    public static Entity SpawnFromDefinition(Player owner, CardDefinition definition, Vector3 position)
    {
        var prefab = Resources.Load<GameObject>(Consts.UnitsPath + definition.PrefabName);

        if(prefab == null)
        {
            Debug.LogWarning("Prefab not found: " + definition);
            return null;
        }

        var go = (GameObject)Instantiate(prefab, position, Quaternion.identity);
        var entity = go.GetComponent<Entity>();

        if(entity == null)
        {
            Debug.LogWarning("Unit component not found on prefab: " + definition);
            return null;
        }

        entity.Init(owner, definition);
        return entity;
    }
}