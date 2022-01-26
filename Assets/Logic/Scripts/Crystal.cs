using System;
using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class Crystal : MonoBehaviour, IHasCollisionRadius
{
    public const int EXEC_ORDER = Invader.EXEC_ORDER - 1;

    public event Action onGrabbed;

#pragma warning disable CS0649
    [SerializeField] GameObject _collision;
    [SerializeField] float _collisionRadius;
#pragma warning restore CS0649

    [HideInInspector] public Invader carriedBy;

    public float CollisionRadius => _collisionRadius;

    void OnEnable() => Refs.I.Crystals.Add(this);
    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.Crystals.Remove(this);
    }

    public void HandleGrabbed(Invader invader)
    {
        _collision.SetActive(false);
        transform.SetParent(invader.grabCrystalPoint, false);
        transform.localPosition = Vector3.zero;
        carriedBy = invader;
        onGrabbed?.Invoke();
    }
}
