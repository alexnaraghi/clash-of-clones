using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Really quick and dirty way to cache prefabs during a game session.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    private Dictionary<string, UnityEngine.Object> _prefabs = new Dictionary<string, UnityEngine.Object>();

    void Awake()
    {
        // There can be only one.
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public T Load<T>(string prefabPath, bool ignoreCache = false) where T : UnityEngine.Object
    {
        T prefab = null;
        if(!ignoreCache && _prefabs.ContainsKey(prefabPath))
        {
            prefab = (T)_prefabs[prefabPath];
        }
        else
        {
            prefab = Resources.Load<T>(prefabPath);
            if(prefab != null)
            {
                _prefabs.Add(prefabPath, prefab);
            }
        }

        return prefab;
    }
}
