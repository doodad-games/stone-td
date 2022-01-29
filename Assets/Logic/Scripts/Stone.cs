using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(EnemyPathToTarget))]
[RequireComponent(typeof(PathingBlocker))]
public class Stone : MonoBehaviour
{
    const float TARGET_REFRESH_INTERVAL = 1f;
    const float MERGE_CHECK_INTERVAL_MIN = 0.75f;
    const float MERGE_CHECK_INTERVAL_MAX = 2f;

    public static event Action<Stone> onAnyTappedChanged;
    public static event Action<Stone.Type> onFailedToUntap;

    public event Action onTappedChanged;

    public Type type;

#pragma warning disable CS0649
    [SerializeField] GameObject _hpBar;
#pragma warning restore CS0649

    [HideInInspector] public bool tapped;
    [HideInInspector] public bool isAwakened;

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

    void OnDisable() => GameController.onTick -= HandleTick;

    public void Insp_SetTapped(bool to)
    {
        if (tapped == to)
            return;
        
        if (to == false && !Refs.I.gc.CanUntapStone(this))
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
        _hpBar.SetActive(true);
        isAwakened = true;
        SetNextMergeTime();
    }

    void HandleTick()
    {
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
                _thisEnemy.IsInRadiusOf(otherStone._thisEnemy)
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
        otherStone._thisEnemy.Life += Mathf.CeilToInt(_thisEnemy.Life * 1.5f);
        _thisEnemy.Life = 0;
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
