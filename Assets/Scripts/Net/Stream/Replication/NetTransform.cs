using System;
using UnityEngine;

[Serializable]
public struct NetTransformData
{
    public float X;
    public float Y;
    public float Z;

    public NetTransformData(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(X, Y, Z);
    }

    public static NetTransformData FromVector3(Vector3 v)
    {
        return new NetTransformData(v.x, v.y, v.z);
    }
}

public class NetTransform : NetBehaviourBase
{
    public override bool HasDelta()
    {
        var data = (NetTransformData)LatestState;
        return data.ToVector3() == transform.position;
    }
    
    public override void Rollback()
    {
        var data = (NetTransformData)LatestState;
        transform.position = data.ToVector3();
    }

    public override object Sample()
    {
        var data = NetTransformData.FromVector3(transform.position);
        return data;
    }
}
