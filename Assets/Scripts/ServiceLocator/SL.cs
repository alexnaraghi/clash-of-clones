using System;
using UnityEngine;

/// <summary>
/// Singleton global service locator, lazy instantiated.
/// </summary>
public static class SL
{
    private static ServiceLocator _instance;

    /// <summary>
    /// Singleton access w/lazy instantiation.  Creates a game object for the dependency services if one does 
    /// not exist.
    /// </summary>
    public static ServiceLocator Instance
    { 
        get
        {
            if(_instance == null)
            {
                var go = new GameObject("_SL");
                GameObject.DontDestroyOnLoad(go);
                _instance = go.AddComponent<ServiceLocator>();

                addGlobalBindings(_instance);
            }
            return _instance;
        }
    }

    /// <summary>
    /// Check for when a service is requested during OnDisable or OnDestroy.
    /// Not needed for standard use, just when calling a service while cleaning up a MonoBehaviour, since the 
    /// service locator itself could be destroyed (Unity destroys objects in a non-deterministic order).
    /// </summary>
    public static bool Exists
    { 
        get
        {
            return _instance != null;
        }
    }

    // Syntactic sugar so we don't have to qualify our static access with "Instance".
    public static T     Get<T>()        where T : class            { return Instance.Get<T>();        }
    public static T     Add<T>()        where T : MonoBehaviour    { return Instance.Add<T>();        }
    public static T     Add<T, U>()     where U : MonoBehaviour, T { return Instance.Add<T, U>();     }
    public static void  AddLazy<T>()    where T : MonoBehaviour    {        Instance.AddLazy<T>();    }
    public static void  AddLazy<T, U>() where U : MonoBehaviour, T {        Instance.AddLazy<T, U>(); }
    public static void  Remove<T>()                                {        Instance.Remove<T>();     }
    public static void  Remove(Type type)                          {        Instance.Remove(type);     }
    public static void  AddExisting<T>(T existing) where T : MonoBehaviour { Instance.AddExisting<T>(existing); }

    // Adds game-specific global bindings.
    private static void addGlobalBindings(ServiceLocator serviceLocator)
    {
        serviceLocator.Add<ResourceManager>();
        serviceLocator.Add<Config>();
        serviceLocator.Add<GameSessionData>();
    }
}