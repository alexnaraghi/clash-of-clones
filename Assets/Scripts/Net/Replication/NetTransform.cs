using System;
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
    public override bool HasDelta()
    {
        var data = (NetTransformData)LatestState;
        return data.Position == transform.position;
    }
    
    public override void Rollback()
    {
        var data = (NetTransformData)LatestState;
        transform.position = data.Position;
    }

    public override object Sample()
    {
        var data = new NetTransformData(transform.position);
        return data;
    }
}
