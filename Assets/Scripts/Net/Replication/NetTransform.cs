using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NetTransformData
{
    public Vector3 Position;

    public NetTransformData(Vector3 position)
    {
        Position = position;
    }
}

public class NetTransform : NetBehaviourBase
{
    public override void Apply(object state)
    {
        var data = (NetTransformData)state;
        transform.position = data.Position;
    }

    public override object Sample()
    {
        var data = new NetTransformData(transform.position);
        return data;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
