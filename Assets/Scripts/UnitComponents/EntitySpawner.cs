using UnityEngine;

/// <summary>
/// Spawner for cards that have multiple characters.
/// </summary>
public class EntitySpawner : MonoBehaviour 
{
    [SerializeField] private int _numEntities;
    [SerializeField] private float _spawnRadius;


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
        for(int i = 0; i < _numEntities; i++)
        {
            float angle = (((float)i) / _numEntities) * 360f;
            Vector3 pos = Utils.GetPointOnCircle(transform.position, _spawnRadius, angle);

            Entity.SpawnFromDefinition(_entity.Owner, TestFactory.CreateSkeleton(), pos);

            // TODO: Destroy this.
        }
    }

    
}
