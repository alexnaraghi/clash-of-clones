using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetObject : MonoBehaviour
{
    public int Id;
    public bool HasAuthority;

    [HideInInspector] 
    public IReplicable[] Components;

    void Awake()
    {
        Components = this.GetInterfaces<IReplicable>();
    }
}