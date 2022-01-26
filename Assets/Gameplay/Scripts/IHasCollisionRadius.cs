using UnityEngine;

public interface IHasCollisionRadius
{
    Transform transform { get; }
    public float CollisionRadius { get; }
}

public static class IHasCollisionRadiusExtensions
{
    public static bool IsInRadiusOf(this IHasCollisionRadius @this, IHasCollisionRadius that)
    {
        var combinedRadiusSq = Mathf.Pow(@this.CollisionRadius + that.CollisionRadius, 2);
        var distSq = (@this.transform.position - that.transform.position).sqrMagnitude;
        return distSq < combinedRadiusSq;
    }
}
