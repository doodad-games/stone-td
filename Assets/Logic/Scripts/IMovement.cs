using UnityEngine;

public interface IMovement
{
    Vector3 LastMovementDir { get; }
    bool DidJustMove { get; }
}
