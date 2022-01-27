using System;
using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class Invader : MonoBehaviour, IHasCollisionRadius
{
    public const int EXEC_ORDER = 10; // Non-zero to give things space to execute before it

    const float TARGET_REFRESH_INTERVAL = 1f;

    public static event Action onBroughtCrystalToCastle;

    public Transform grabCrystalPoint;

#pragma warning disable CS0649
    [SerializeField] float _collisionRadius;
#pragma warning restore CS0649

    [HideInInspector] public Crystal targetCrystal;
    [HideInInspector] public bool wasSpawned;

    MoveToTarget _movement;

    Castle _targetCastle;
    float _nextTargetTime;
    bool _isHoldingCrystal;

    public float CollisionRadius => _collisionRadius;

    void OnEnable()
    {
        Refs.I.Invaders.Add(this);
        _movement = GetComponent<MoveToTarget>();
        FindMovementTarget();

        GameController.onTick += HandleTick;
    }

    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.Invaders.Remove(this);

        GameController.onTick -= HandleTick;
    }

    public void SetCrystalFollowDistance(int followerI) =>
        _movement.followDistance = Calcs.CrystalCarrierFollowDistance(followerI);

    void HandleTick()
    {
        MaybeRefreshMovementTarget();

        if (_isHoldingCrystal)
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

        if (_isHoldingCrystal)
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
        _isHoldingCrystal &&
        _targetCastle != null &&
        _targetCastle.IsInRadiusOf(this) == true;
    
    void HandleReachedCastle()
    {
        BroadcastMessage("Msg_OnBroughtCrystalToCastle");
        onBroughtCrystalToCastle?.Invoke();
    }
    
    bool HasReachedCrystal() =>
        targetCrystal != null &&
        targetCrystal.carriedBy == null &&
        targetCrystal.IsInRadiusOf(this) == true;
    
    void HandleReachedCrystal()
    {
        _isHoldingCrystal = true;
        targetCrystal.HandleGrabbed(this);
        FindNearestCastle();
    }
}
