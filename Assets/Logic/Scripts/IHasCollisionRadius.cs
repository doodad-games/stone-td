using UnityEngine;

public interface IHasCollisionRadius
{
    GameObject gameObject { get; }
    Transform transform { get; }
    public float CollisionRadius { get; }
}

public static class IHasCollisionRadiusExtensions
{
    public static bool IsInRadiusOf(this IHasCollisionRadius @this, IHasCollisionRadius that, float multiplier = 1f)
    {
        var combinedRadius = (@this.CollisionRadius + that.CollisionRadius) * multiplier;
        var combinedRadiusSq = Mathf.Pow(combinedRadius, 2);
        var paddedRadiusSq = combinedRadiusSq + 0.02f; // Lazy way of dodging annoying rounding errors
        var distSq = (@this.transform.position - that.transform.position).sqrMagnitude;
        return distSq < paddedRadiusSq;
    }
}
