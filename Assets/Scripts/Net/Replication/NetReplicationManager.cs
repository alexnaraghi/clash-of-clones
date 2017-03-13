using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NetSample
{
    public int Id;
    public object Content;

    public NetSample(int id, object content)
    {
        Id = id;
        Content = content;
    }
}

public class NetReplicationManager : MonoBehaviour 
{
    private List<NetBehaviourBase> _replicants = new List<NetBehaviourBase>();
    private int _latestId = 0;

    // For testing, sample and apply everything in the game.
    private List<NetSample> _snapshot = new List<NetSample>();

    public bool IsHost 
    { 
        get
        {
            // TODO: Use some lower network layer to determine if this is the host.
            return true;
        }
    }

    public void Spawn(GameObject go)
    {
        var components = go.GetInterfaces<NetBehaviourBase>();
        foreach(var component in components)
        {
            assignId(component);
            _replicants.Add(component);
        }

        // TODO: spawn this to on all clients.
    }

    public void Despawn(GameObject go)
    {
        var components = go.GetInterfaces<NetBehaviourBase>();
        foreach(var component in components)
        {
            _replicants.Remove(component);
        }
    }

    public void Tick()
    {
        if(IsHost)
        {
            
        }
    }

    private void assignId(NetBehaviourBase component)
    {
        component.Init(_latestId++);
    }

    public void TakeSnapshot()
    {
        _snapshot.Clear();
        foreach(var component in _replicants)
        {
            var content = component.Sample();
            var sample = new NetSample(component.Id, content);
            _snapshot.Add(sample);
        }
    }

    public void ApplySnapshot()
    {
        foreach(var sample in _snapshot)
        {
            var component = _replicants.Find(f => f.Id == sample.Id);
            if(component != null)
            {
                component.Apply(sample.Content);
            }
        }
    }
}