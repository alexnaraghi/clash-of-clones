
/// <summary>
/// T must be serializable.
/// </summary>
public interface IReplicable
{
    object Sample();
    void Apply(object state);
}