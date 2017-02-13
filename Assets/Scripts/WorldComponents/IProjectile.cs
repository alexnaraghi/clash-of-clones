using UnityEngine.Events;

public interface IProjectile 
{
    UnityEvent DestroyedEvent { get; }
    void Init(Entity creator);
}