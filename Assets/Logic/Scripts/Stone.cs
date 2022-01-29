using System;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(EnemyPathToTarget))]
[RequireComponent(typeof(PathingBlocker))]
public class Stone : MonoBehaviour
{
    const float TARGET_REFRESH_INTERVAL = 1f;

    public static event Action<Stone> onAnyTappedChanged;
    public static event Action<Stone.Type> onFailedToUntap;

    public event Action onTappedChanged;

    [HideInInspector] public bool tapped;
    [HideInInspector] public bool isAwakened;
    public Type type;

    Enemy _thisEnemy;
    EnemyPathToTarget _movement;
    PathingBlocker _blocker;

    Crystal _targetCrystal;
    float _nextTargetTime;

    void OnEnable()
    {
        _thisEnemy = GetComponent<Enemy>();
        _movement = GetComponent<EnemyPathToTarget>();
        _blocker = GetComponent<PathingBlocker>();

        GameController.onTick += HandleTick;
    }

    void OnDisable() => GameController.onTick -= HandleTick;

    public void Insp_SetTapped(bool to)
    {
        if (tapped == to)
            return;
        
        if (to == false && !Refs.I.gc.CanConstructMore(type))
        {
            onFailedToUntap?.Invoke(type);
            return;
        }

        tapped = to;

        if (tapped)
            Refs.I.tappedStones[type].Add(this);
        else Refs.I.tappedStones[type].Remove(this);

        onTappedChanged?.Invoke();
        onAnyTappedChanged?.Invoke(this);
    }
    
    public void Awaken()
    {
        if (!tapped)
        {
            Debug.LogError("Tried to awaken a non-tapped Stone 🤔 Ignoring", gameObject);
            return;
        }

        _thisEnemy.enabled = true;
        _movement.enabled = true;
        _blocker.enabled = false;
        isAwakened = true;
    }

    void HandleTick()
    {
        if (isAwakened)
        {
            MaybeRefreshMovementTarget();

            if (HasReachedCrystal())
                _targetCrystal.Break();
        }
    }

    void MaybeRefreshMovementTarget()
    {
        if (Refs.I.gc.time > _nextTargetTime)
            FindNearestCrystal();
    }
    
    void FindNearestCrystal()
    {
        _nextTargetTime = Refs.I.gc.time + TARGET_REFRESH_INTERVAL;

        _movement.Target = _targetCrystal = Refs.NearestCrystal(transform.position);
    }

    bool HasReachedCrystal() =>
        _targetCrystal != null &&
        _targetCrystal.IsInRadiusOf(_thisEnemy) == true;
    
    [Serializable]
    public enum Type
    {
        None = 0,
        Wall = 1,
        Spike = 2
    }
}
