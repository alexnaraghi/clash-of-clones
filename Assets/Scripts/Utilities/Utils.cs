using System;
using UnityEngine;

public static class Utils 
{
    /// <summary>
    /// Gets the index of an element in an array.
    /// </summary>
    public static int IndexOf<T>(this T[] arr, T element) where T : class
    {
        int index = -1;
        for(int i = 0; i < arr.Length; i++)
        {
            if(arr[i] == element)
            {
                index = i;
            }
        }
        return index;
    }

    public static void Invoke(this MonoBehaviour behaviour, Action action, float seconds)
    {
        behaviour.Invoke(action.Method.Name, seconds);
    }

    public static void InvokeRepeating(this MonoBehaviour behaviour, Action action, float seconds, float repeatSeconds)
    {
        behaviour.InvokeRepeating(action.Method.Name, seconds, repeatSeconds);
    }

    public static void CancelInvoke(this MonoBehaviour behaviour, Action action)
    {
        behaviour.CancelInvoke(action.Method.Name);
    }

    public static Vector3 GetPointOnCircle (Vector3 center, float radius, float angle)
    {
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        return pos;
    }

    public static T GetInterface<T>(this MonoBehaviour src) where T : class
    {
        return src.GetComponent(typeof(T)) as T;
    }

    public static T GetInterface<T>(this GameObject src) where T : class
    {
        return src.GetComponent(typeof(T)) as T;
    }

    public static T[] GetInterfaces<T>(this MonoBehaviour src) where T : class
    {
        var components = src.GetComponents(typeof(T));
        T[] toReturn = new T[components.Length];
        for(int i = 0; i < components.Length; i++)
        {
            toReturn[i] = components[i] as T;
        }
        return toReturn;
    }

    public static T[] GetInterfaces<T>(this GameObject src) where T : class
    {
        var components = src.GetComponents(typeof(T));
        T[] toReturn = new T[components.Length];
        for(int i = 0; i < components.Length; i++)
        {
            toReturn[i] = components[i] as T;
        }
        return toReturn;
    }

    /// <summary>
    /// Zeroes out the y component of the vector.
    /// </summary>
    public static Vector3 ZeroY(this Vector3 src)
    {
        return new Vector3(src.x, 0f, src.z);
    }

    public static T Instantiate<T>(T original, Transform parent) where T : UnityEngine.Object
    {
        return (T)UnityEngine.Object.Instantiate(original, parent);
    }

    public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : UnityEngine.Object
    {
        return (T)UnityEngine.Object.Instantiate(original, position, rotation);
    }

    /// <summary>
    /// Disables the MonoBehaviour if any of the given unity objects are null.
    /// </summary>
    /// <returns>True if one of the given objects was null, false if all had a valid reference.</returns>
    public static bool DisabledFromMissingObject(this MonoBehaviour mb, params UnityEngine.Object[] dependencies)
    {
        bool isNull = false;
        foreach(var dependency in dependencies)
        {
            if(dependency == null)
            {
                Debug.LogError(string.Format("{0}: Dependency {1} is null.", mb.name, dependency.GetType()));
                isNull = true;
                mb.enabled = false;
            }
        }

        return isNull;
    }
}