using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class BinarySerializer : ISerializer
{
    public byte[] Serialize(object data)
    {
        byte[] bytes;

        BinaryFormatter bf = new BinaryFormatter();
        using(var ms = new MemoryStream()) 
        {
            bf.Serialize(ms, data);
            bytes = ms.ToArray();
        }
        return bytes;
    }

    public int Serialize(object data, byte[] buffer)
    {
        int length;
        BinaryFormatter bf = new BinaryFormatter();
        using(var ms = new MemoryStream(buffer)) 
        {
            bf.Serialize(ms, data);
            length = Convert.ToInt32(ms.Position);
        }
        return length;
    }

    public object Deserialize(byte[] bytes)
    {
        return Deserialize(bytes, 0, bytes.Length);
    }

    public object Deserialize(byte[] bytes, int offset, int size)
    {
        object packet;
        using (var ms = new MemoryStream(bytes, offset, size))
        {
            BinaryFormatter bf = new BinaryFormatter();
            packet = bf.Deserialize(ms);
        }
        return packet;
    }

    public T Deserialize<T>(byte[] bytes) where T : class
    {
        return Deserialize(bytes) as T;
    }
}