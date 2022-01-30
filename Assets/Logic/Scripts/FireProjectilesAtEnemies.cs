using System;
using UnityEngine;

public class FireProjectilesAtEnemies : MonoBehaviour, IHasCollisionRadius
{
    public static event Action onAnyDidFire;
    public event Action onDidFire;

    public Projectile projectile;
    public float radius;
    public float cooldown;
    public Transform spawnPoint;

    public float CollisionRadius => radius;

    Enemy _target;
    float _nextShotAfter;

    void OnEnable() => GameController.onTick += HandleTick;
    void OnDisable() => GameController.onTick -= HandleTick;

    void HandleTick()
    {
        if (_target != null)
            EnsureTargetIsValid();

        if (_target == null)
            TryAcquireTarget();
        
        if (_target != null)
            TryFire();
    }

    void EnsureTargetIsValid()
    {
        if (
            _target == null ||
            _target.isDead ||
            !this.IsInRadiusOf(_target)
        ) _target = null;
    }

    void TryAcquireTarget()
    {
        var thisPos = transform.position;
        float closestDistSq = Mathf.Infinity;
        foreach (var enemy in Refs.I.Enemies)
        {
            var distSq = (thisPos - enemy.transform.position).sqrMagnitude;
            if (distSq < closestDistSq)
            {
                _target = enemy;
                closestDistSq = distSq;
            }
        }

        if (closestDistSq > Mathf.Pow(radius, 2))
            _target = null;
    }

    void TryFire()
    {
        if (Refs.I.gc.time < _nextShotAfter)
            return;
        _nextShotAfter = Refs.I.gc.time + cooldown;

        Instantiate(
            projectile,
            spawnPoint.position,
            Quaternion.identity
        ).Init(_target);

        onDidFire?.Invoke();
        onAnyDidFire?.Invoke();
    }
}
