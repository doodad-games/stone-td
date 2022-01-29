using UnityEngine;

public interface IEnemy : IHasCollisionRadius
{
    Transform[] HitLocations { get; }
    float HitLocationRadius { get; }
}
