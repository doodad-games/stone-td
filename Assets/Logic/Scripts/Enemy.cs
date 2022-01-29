using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IHasCollisionRadius
{
    public static event Action onAnyDied;
    public event Action onDie;
    public event Action onLifeChanged;

    public Transform[] hitLocations;
    public float hitLocationRadius;
    public int life;

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

    public void TakeDamage(int amount)
    {
        if (life <= amount)
            Die();
        life -= amount;
        onLifeChanged?.Invoke();
    }

    void Die()
    {
        Destroy(gameObject);
        onDie?.Invoke();
        onAnyDied?.Invoke();
    }
}
