using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class Castle : MonoBehaviour, IHasCollisionRadius, IPathingTarget
{
    public const int EXEC_ORDER = Invader.EXEC_ORDER - 1;

#pragma warning disable CS0649
    [SerializeField] float _entranceCollisionRadius;
#pragma warning restore CS0649

    [HideInInspector] public Invader carriedBy;

    public float CollisionRadius => _entranceCollisionRadius;
    public Vector3 PathingTargetPoint => transform.position;

    void OnEnable() => Refs.I.Castles.Add(this);
    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.Castles.Remove(this);
    }
}
