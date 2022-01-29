using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IHasCollisionRadius
{
    public static event Action onAnyDied;
    public event Action onDie;

    public Transform[] hitLocations;
    public float hitLocationRadius;

#pragma warning disable CS0649
    [SerializeField] float _collisionRadius;
#pragma warning restore CS0649

    public float CollisionRadius => _collisionRadius;

    void OnEnable() => Refs.I.Enemies.Add(this);

    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.Enemies.Remove(this);
    }

    public void Die()
    {
        Destroy(gameObject);
        onDie?.Invoke();
        onAnyDied?.Invoke();
    }
}
