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

        if (targetCrystal != null)
            targetCrystal.onGrabbed -= HandleSomeoneGrabbedTargetCrystal;
        targetCrystal = null;

        GameController.onTick -= HandleTick;
    }

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
        if (Time.time > _nextTargetTime)
            FindMovementTarget();
    }

    void FindMovementTarget()
    {
        _nextTargetTime = Time.time + TARGET_REFRESH_INTERVAL;

        if (_isHoldingCrystal)
            FindNearestCastle();
        else FindNearestCrystal();
    }

    void FindNearestCastle()
    {
        _targetCastle = Refs.NearestCastle(transform.position);
        _movement.SetTarget(_targetCastle?.transform);
    }
    
    void FindNearestCrystal()
    {
        if (targetCrystal != null)
            targetCrystal.onGrabbed -= HandleSomeoneGrabbedTargetCrystal;

        targetCrystal = Refs.NearestCrystal(transform.position);

        if (targetCrystal?.carriedBy != null)
            _movement.SetTarget(targetCrystal.carriedBy.transform);
        else _movement.SetTarget(targetCrystal?.transform);

        if (targetCrystal != null)
            targetCrystal.onGrabbed += HandleSomeoneGrabbedTargetCrystal;
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

    void HandleSomeoneGrabbedTargetCrystal()
    {
        if (targetCrystal.carriedBy == this)
            return;
        
        _movement.SetTarget(targetCrystal.carriedBy.transform);
    }
}
