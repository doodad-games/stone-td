using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(EnemyPathToTarget))]
[RequireComponent(typeof(PathingBlocker))]
public class Stone : MonoBehaviour
{
    const float TARGET_REFRESH_INTERVAL = 1f;
    const float MERGE_CHECK_INTERVAL_MIN = 0.5f;
    const float MERGE_CHECK_INTERVAL_MAX = 1.25f;
    const float MERGE_HP_FACTOR = 1.25f;
    const float MERGE_RADIUS_FACTOR = 4f;

    public static event Action<Stone> onAnyTappedChanged;
    public static event Action<Stone.Type> onFailedToUntap;
    public static event Action onAnyMerged;

    public event Action onTappedChanged;

    public Type type;

#pragma warning disable CS0649
    [SerializeField] GameObject _hpBar;
#pragma warning restore CS0649

    [HideInInspector] public bool isTapped;
    [HideInInspector] public bool isAwakened;
    [HideInInspector] public int mergeCount;

    Enemy _thisEnemy;
    EnemyPathToTarget _movement;
    PathingBlocker _blocker;

    Crystal _targetCrystal;
    float _nextTargetTime;
    float _nextMergeCheckTime;

    void OnEnable()
    {
        _thisEnemy = GetComponent<Enemy>();
        _movement = GetComponent<EnemyPathToTarget>();
        _blocker = GetComponent<PathingBlocker>();
        _hpBar.SetActive(false);

        GameController.onTick += HandleTick;
    }

    void Msg_OnDied()
    {
        if (isTapped)
            Refs.I.tappedStones[type].Remove(this);
    }

    void OnDisable() => GameController.onTick -= HandleTick;

    public void Insp_SetTapped(bool to)
    {
        if (isTapped == to)
            return;
        
        if (to == false && !Refs.I.gc.CanUntapStone(this))
        {
            onFailedToUntap?.Invoke(type);
            UIController.onError?.Invoke();
            return;
        }

        isTapped = to;

        if (isTapped)
            Refs.I.tappedStones[type].Add(this);
        else Refs.I.tappedStones[type].Remove(this);

        onTappedChanged?.Invoke();
        onAnyTappedChanged?.Invoke(this);
    }
    
    public void Awaken()
    {
        if (!isTapped)
        {
            Debug.LogError("Tried to awaken a non-tapped Stone ???? Ignoring", gameObject);
            return;
        }

        _thisEnemy.enabled = true;
        _movement.enabled = true;
        _blocker.enabled = false;
        _hpBar.SetActive(true);
        isAwakened = true;

        onTappedChanged?.Invoke();

        SetNextMergeTime();
    }

    void HandleTick()
    {
        if (_thisEnemy.isDead)
            return;

        if (isAwakened)
        {
            MaybeFindNearestCrystal();

            if (HasReachedCrystal())
            {
                _targetCrystal.Break();
                return;
            }

            MaybeCheckForMerges();
        }
    }

    void MaybeFindNearestCrystal()
    {
        if (Refs.I.gc.time < _nextTargetTime)
            return;
        _nextTargetTime = Refs.I.gc.time + TARGET_REFRESH_INTERVAL;

        _movement.Target = _targetCrystal = Refs.NearestCrystal(transform.position);
    }
    
    void MaybeCheckForMerges()
    {
        if (Refs.I.gc.time < _nextMergeCheckTime)
            return;
        SetNextMergeTime();

        var otherStones = Refs.I.tappedStones[type].ToArray(); // This set will be modified during the loop
        foreach (var otherStone in otherStones)
        {
            if (
                // I don't understand why this is needed and I'm losing my mind, game jams for ya
                otherStone != null &&

                otherStone != this &&
                _thisEnemy.IsInRadiusOf(otherStone._thisEnemy, MERGE_RADIUS_FACTOR)
            )
            {
                if (_thisEnemy.Life >= otherStone._thisEnemy.Life)
                    otherStone.MergeInto(this);
                else
                {
                    MergeInto(otherStone);
                    return;
                }
            }
        }
    }

    void MergeInto(Stone otherStone)
    {
        otherStone._thisEnemy.Life += Mathf.CeilToInt((_thisEnemy.Life + mergeCount) * MERGE_HP_FACTOR);
        ++otherStone.mergeCount;
        _thisEnemy.Life = 0;
        onAnyMerged?.Invoke();
    }

    bool HasReachedCrystal() =>
        _targetCrystal != null &&
        _targetCrystal.IsInRadiusOf(_thisEnemy) == true;
    
    void SetNextMergeTime() =>
        _nextMergeCheckTime = Refs.I.gc.time + UnityEngine.Random.Range(
            MERGE_CHECK_INTERVAL_MIN,
            MERGE_CHECK_INTERVAL_MAX
        );
    
    [Serializable]
    public enum Type
    {
        None = 0,
        Wall = 1,
        Spike = 2
    }
}
