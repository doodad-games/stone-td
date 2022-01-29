using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IHasCollisionRadius
{
    public static event Action onAnyDied;
    public event Action onDie;
    public event Action onLifeChanged;

    public ProjectileTargetParams projectileTargetParams;
    public int startingLife;

#pragma warning disable CS0649
    [SerializeField] float _collisionRadius;
#pragma warning restore CS0649

    int _life;
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

    public float CollisionRadius
    {
        get => _collisionRadius;
        set => _collisionRadius = value;
    }

    void Awake() => _life = startingLife;
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
