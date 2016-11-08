using UnityEngine;

/// <summary>
/// Spawner for cards that have multiple characters.
/// </summary>
public class DistributedObjects : MonoBehaviour 
{
    [SerializeField] private int _numEntities;
    [SerializeField] private float _spawnRadius;
    [SerializeField] private GameObject _prefabToSpawn;

    void Start()
    {
        spawnObjects();
    }

    private void spawnObjects()
    {
        for(int i = 0; i < _numEntities; i++)
        {
            float angle = (((float)i) / _numEntities) * 360f;
            Vector3 pos = Utils.GetPointOnCircle(transform.position, _spawnRadius, angle);

            if(_prefabToSpawn != null)
            {
                var go = Instantiate(_prefabToSpawn);
                go.transform.position = pos;
                go.transform.SetParent(transform);
                GameModel.Instance.MyPlayer.RotateForPlayer(go);
            }
        }
    }
}
