using UnityEngine;

public interface INavigable
{
    void MoveTo(Vector3 position);
    void CancelMove();
}