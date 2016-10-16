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
}