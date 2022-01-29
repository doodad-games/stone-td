using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IHasCollisionRadius
{
    public static event Action onAnyDied;
    public event Action onDie;
    public event Action onLifeChanged;

    public Transform[] hitLocations;
    public float hitLocationRadius;

#pragma warning disable CS0649
    [SerializeField] float _collisionRadius;
    [SerializeField] int _life;
#pragma warning restore CS0649

    public float CollisionRadius => _collisionRadius;

    public int Life
    {
        get => _life;
        set
        {
            if (value <= 0)
            {
                Die();
                return;
            }

            _life = value;
            onLifeChanged?.Invoke();
        }
    }

    void OnEnable() => Refs.I.Enemies.Add(this);

    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.Enemies.Remove(this);
    }

    void Die()
    {
        Destroy(gameObject);
        onDie?.Invoke();
        onAnyDied?.Invoke();
    }
}
