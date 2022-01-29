using System;
using UnityEngine;
using UnityEngine.Events;

public class OnLifeChanged : MonoBehaviour
{
    public Threshold[] thresholds;

    Enemy _enemy;

    Threshold _lastThreshold;

    void OnEnable()
    {
        _enemy = GetComponentInParent<Enemy>();
        _enemy.onLifeChanged += Refresh;
        Refresh();
    }
    void OnDisable()
    {
        if (_enemy != null)
            _enemy.onLifeChanged -= Refresh;
    }

    void Refresh()
    {
        var lifeMult = (float)_enemy.Life / _enemy.startingLife;

        Threshold highestThreshold = null;
        foreach (var threshold in thresholds)
        {
            if (
                highestThreshold == null ||
                (
                    lifeMult >= threshold.fromLifeMultiplier &&
                    threshold.fromLifeMultiplier > highestThreshold.fromLifeMultiplier
                )
            ) highestThreshold = threshold;
        }

        if (
            highestThreshold != null &&
            highestThreshold != _lastThreshold
        )
        {
            highestThreshold.actions?.Invoke();
            _lastThreshold = highestThreshold;
        }
    }

    [Serializable]
    public class Threshold
    {
        [Tooltip("Life multiplier is their current life divided by their starting life")]
        public int fromLifeMultiplier;
        public UnityEvent actions;
    }
}
