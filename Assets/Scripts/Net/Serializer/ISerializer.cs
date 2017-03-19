
public interface ISerializer
{
    byte[] Serialize(object data);

    /// <summary>
    /// Serializes the object into a buffer.
    /// PRECONDITION: Size is less than max int.
    /// </summary>
    int Serialize(object data, byte[] buffer);
    object Deserialize(byte[] bytes);
    object Deserialize(byte[] bytes, int offset, int size);
    T Deserialize<T>(byte[] bytes) where T : class;
}