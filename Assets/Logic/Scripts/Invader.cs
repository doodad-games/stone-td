using System;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(EnemyPathToTarget))]
[DefaultExecutionOrder(EXEC_ORDER)]
public class Invader : MonoBehaviour
{
    public const int EXEC_ORDER = 10; // Non-zero to give things space to execute before it

    const float TARGET_REFRESH_INTERVAL = 1f;

    public static event Action onBroughtCrystalToCastle;

    public Transform grabCrystalPoint;

    [HideInInspector] public Crystal targetCrystal;
    [HideInInspector] public bool wasSpawned;
    [HideInInspector] public bool isHoldingCrystal;

    Enemy _thisEnemy;
    EnemyPathToTarget _movement;

    Castle _targetCastle;
    float _nextTargetTime;

    void OnEnable()
    {
        Refs.I.Invaders.Add(this);
        _thisEnemy = GetComponent<Enemy>();
        _movement = GetComponent<EnemyPathToTarget>();
        FindMovementTarget();

        GameController.onTick += HandleTick;
    }

    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.Invaders.Remove(this);

        GameController.onTick -= HandleTick;

        if (isHoldingCrystal && targetCrystal != null)
            DropCrystal();
    }

    public void SetCrystalFollowDistance(int followerI) =>
        _movement.followDistance = Calcs.CrystalCarrierFollowDistance(followerI);
    
    public void ClearCrystalFollowerDistance() =>
        _movement.followDistance = 0f;

    void HandleTick()
    {
        MaybeRefreshMovementTarget();

        if (isHoldingCrystal)
        {
            if (HasReachedCastle())
                HandleReachedCastle();
        }
        else
        {
            if (HasReachedCrystal())
                HandleReachedCrystal();
        }
    }

    void MaybeRefreshMovementTarget()
    {
        if (Refs.I.gc.time > _nextTargetTime)
            FindMovementTarget();
    }

    void FindMovementTarget()
    {
        _nextTargetTime = Refs.I.gc.time + TARGET_REFRESH_INTERVAL;

        if (isHoldingCrystal)
            FindNearestCastle();
        else FindNearestCrystal();
    }

    void FindNearestCastle()
    {
        _targetCastle = Refs.NearestCastle(transform.position);
        _movement.SetTarget(_targetCastle);
        _movement.followDistance = 0f;
    }
    
    void FindNearestCrystal()
    {
        var newTargetCrystal = Refs.NearestCrystal(transform.position);
        if (targetCrystal == newTargetCrystal)
            return;

        _movement.SetTarget(newTargetCrystal);
        _movement.followDistance = 0f;
        
        targetCrystal = newTargetCrystal;
    }

    bool HasReachedCastle() =>
        isHoldingCrystal &&
        _targetCastle != null &&
        _targetCastle.IsInRadiusOf(_thisEnemy) == true;
    
    void HandleReachedCastle()
    {
        BroadcastMessage("Msg_OnBroughtCrystalToCastle");
        onBroughtCrystalToCastle?.Invoke();
    }
    
    bool HasReachedCrystal() =>
        targetCrystal != null &&
        targetCrystal.carriedBy == null &&
        targetCrystal.IsInRadiusOf(_thisEnemy) == true;
    
    void HandleReachedCrystal()
    {
        isHoldingCrystal = true;
        targetCrystal.HandleGrabbed(this);
        FindNearestCastle();
    }
    
    void DropCrystal() => targetCrystal.Drop();
}
