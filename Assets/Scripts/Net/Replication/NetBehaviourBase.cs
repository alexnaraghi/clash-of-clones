using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetBehaviourBase : MonoBehaviour 
{
    [HideInInspector]
    public NetObject NetObject;

    public int Id
    {
        get;
        private set;
    }

    public virtual void Init(int id)
    {
        Id = id;
    }

    public abstract void Apply(object state);
    public abstract object Sample();

    private void Start () 
    {
        NetObject = GetComponent<NetObject>();
    }
}
