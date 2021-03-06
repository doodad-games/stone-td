using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IHasCollisionRadius
{
    public static event Action<Enemy> onAnyDied;
    public static event Action<Enemy> onAnyLostLife;
    public event Action onLifeChanged;

    public ProjectileTargetParams projectileTargetParams;
    public int startingLife;

#pragma warning disable CS0649
    [SerializeField] float _collisionRadius;
#pragma warning restore CS0649

    [HideInInspector] public bool isDead;

    int _life;
    public int Life
    {
        get => _life;
        set
        {
            if (value <= 0)
                value = 0;
            
            if (_life == value)
                return;
            
            var isLifeLoss = value < _life;
            _life = value;

            if (value == 0 && !isDead)
                if (!isDead)
                    Die();

            onLifeChanged?.Invoke();
            if (isLifeLoss)
                onAnyLostLife?.Invoke(this);
        }
    }

    public float CollisionRadius
    {
        get => _collisionRadius;
        set => _collisionRadius = value;
    }

    void Awake() => _life = startingLife;
    void OnEnable()
    {
        if (!isDead)
            Refs.I.Enemies.Add(this);
    }
    void OnDisable()
    {
        if (!isDead && Refs.I != null)
            Refs.I.Enemies.Remove(this);
    }

    void Die()
    {
        Refs.I.Enemies.Remove(this);
        isDead = true;
        BroadcastMessage("Msg_OnDied");
        onAnyDied?.Invoke(this);
    }
}
