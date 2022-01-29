using UnityEngine;
using UnityEngine.Events;

public class OnAnyStonesTappedChanged : MonoBehaviour
{
    [Tooltip("The number tapped from each type will be summed together")]
    public StoneTypeParams[] types;
    public int threshold = 1;
    public UnityEvent onCountDidntReachThreshold;
    public UnityEvent onCountReachedThreshold;
    public bool countNumberOfDifferentTypesUsed;

    void OnEnable()
    {
        Stone.onAnyTappedChanged += HandleAnyStoneTappedChanged;
        Refresh();
    }
    void OnDisable() => Stone.onAnyTappedChanged -= HandleAnyStoneTappedChanged;

    void HandleAnyStoneTappedChanged(Stone _) => Refresh();

    void Refresh()
    {
        var result = 0;
        foreach (var type in types)
        {
            var count = Refs.I.tappedStones[type.type].Count;
            if (count != 0)
                result += countNumberOfDifferentTypesUsed
                    ? 1
                    : count;
        }

        if (result < threshold)
            onCountDidntReachThreshold?.Invoke();
        else onCountReachedThreshold?.Invoke();
    }
}
